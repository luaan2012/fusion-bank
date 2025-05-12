using fusion.bank.core;
using fusion.bank.core.Enum;
using fusion.bank.core.Interfaces;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model;
using fusion.bank.creditcard.domain.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.creditCard.api.Controllers;

[ApiController]
[Route("[controller]")]
//[Authorize]
public class CreditCardController(ILogger<CreditCardController> logger, ICreditCardRepository creditCardRepository,
    IRequestClient<NewAccountRequestInformation> requestClient, IRequestClient<NewCreditCardCreatedRequest> requestClientCreated,
    IPublishEndpoint publishEndpoint, IBackgroundTaskQueue backgroundTaskQueue,
    IGenerateCreditCardService generateCreditCardService) : MainController
{
    [HttpGet("request-creditcard/{accountId:Guid}")]
    public async Task<IActionResult> RequestCreditCard(Guid accountId, decimal limit)
    {
        var creditCardExist = await creditCardRepository.ListCreditCardByAccountId(accountId);

        if (creditCardExist != null && creditCardExist.CreditCardTried)
        {
            if (DateTime.UtcNow < creditCardExist.CreditCardNextAttempt)
            {
                return CreateResponse(new DataContractMessage<string>(), $"Ainda estamos analisando sua conta, tente novamente em {creditCardExist.CreditCardNextAttempt}");
            }

            CheckTriedCard(creditCardExist);
        }

        var creditCardRequestResponse = await requestClient.GetResponse<DataContractMessage<AccountInformationResponse>>(new NewAccountRequestInformation(accountId));

        if (!creditCardRequestResponse.Message.Success)
        {
            return CreateResponse(new DataContractMessage<string>(), $"Nao foi possivel solicitar um cartao para voce agora, tente novamente mais tarde.");
        }

        await publishEndpoint.Publish(GenerateEvent.CreateCreditCardRequestedEvent(accountId.ToString()));

        await backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (cancellationToken) =>
        {
            // Simular atraso de processamento (ex.: 5 segundos)
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            var (cardType, success, suggestedLimit) = SuggestCreditCardAndLimit(
            creditCardRequestResponse.Message.Data.SalaryPerMonth,
            creditCardRequestResponse.Message.Data.AverageBudgetPerMonth,
            creditCardRequestResponse.Message.Data.AccountType, limit);

            if (!success)
            {
                await SaveTriedCard(accountId, creditCardExist?.Id ?? Guid.Empty);
                return;
            }

            var creditCard = GetCreditCard(cardType, accountId, suggestedLimit);

            if (creditCard is null)
            {
                await SaveTriedCard(accountId, creditCardExist?.Id ?? Guid.Empty);
                return;
            }

            if(creditCard.CreditCardType == creditCardExist?.CreditCardType && !string.IsNullOrEmpty(creditCardExist.CreditCardNumber))
            {
                await creditCardRepository.SaveTriedCard(creditCardExist);
                await publishEndpoint.Publish(GenerateEvent.CreateCreditCardFailedEvent(accountId.ToString(), creditCard.CreditCardNextAttempt));
                return;
            }

            var generateInformations = await generateCreditCardService.GenerateCreditCard(creditCard.CreditCardFlag);

            creditCard.CreditCardNumber = generateInformations.CreditCardNumber;
            creditCard.CreditCardCode = generateInformations.CreditCardCode;
            creditCard.CreditCardValidity = DateTime.ParseExact(generateInformations.CreditCardValidity, "dd/MM/yyyy", null);
            creditCard.CreditCardName = creditCardRequestResponse.Message.Data.Name;
            creditCard.CreditCardTried = true;
            creditCard.CreditCardTriedTimes = 1;

            var creditCardCreated = await requestClientCreated.GetResponse<DataContractMessage<string>>(new NewCreditCardCreatedRequest { CreditCard = creditCard, AccountId = creditCard.AccountId });

            if (!creditCardCreated.Message.Success)
            {
                await publishEndpoint.Publish(GenerateEvent.CreateCreditCardFailedEvent(accountId.ToString(), creditCard.CreditCardNextAttempt));
                return;
            }

            await publishEndpoint.Publish(GenerateEvent.CreateCreditCardResponsedEvent(accountId.ToString(), creditCard.CreditCardType.ToString(), creditCard.CreditCardNumber, creditCard.CreditCardLimit), cancellationToken);

            await creditCardRepository.SaveTriedCard(creditCard);

        });

        return Accepted(new DataContractMessage<string>
        {
            Success = true,
            Data = "Sua solicitação de cartão está em processamento. Você será notificado em breve."
        });
    }

    [HttpGet("request-virtual-creditcard/{id:Guid}")]
    public async Task<IActionResult> RequestVirtualCreditCard(Guid id)
    {
        var creditCard = await creditCardRepository.ListCreditCardById(id);

        if(creditCard == null || string.IsNullOrEmpty(creditCard.CreditCardNumber))
        {
            return CreateResponse(new DataContractMessage<string> { Success = false }, "Nenhum cartao encontrado para essa conta");
        }

        if(creditCard.VirtualCreditCards.Count == 1)
        {
            return CreateResponse(new DataContractMessage<string> { Success = false }, "Limite máximo de cartao virtual atingido.");
        }

        var generateInformations = await generateCreditCardService.GenerateCreditCard(creditCard.CreditCardFlag);

        var virtualCreditCard = new VirtualCreditCard { CreditCardNumber = generateInformations.CreditCardNumber, 
                                    CreditCardCode = generateInformations.CreditCardCode, CreditCardName = creditCard.CreditCardName, Id = Guid.NewGuid() };

        creditCard.VirtualCreditCards.Add(virtualCreditCard);

        await creditCardRepository.SaveTriedCard(creditCard);

        await publishEndpoint.Publish(GenerateEvent.CreateCreditCardResponsedEvent(creditCard.AccountId.ToString(), creditCard.CreditCardType.ToString(), creditCard.CreditCardNumber, creditCard.CreditCardLimit));

        return CreateResponse(new DataContractMessage<CreditCard> { Data = creditCard, Success = true });
    }

    [HttpPut("creditcard-toggle-blocked/{id:Guid}")]
    public async Task<IActionResult> RequestVirtualCreditCard(Guid id, bool isBlocked)
    {
        var creditCard = await creditCardRepository.ListCreditCardById(id);

        if (creditCard == null || string.IsNullOrEmpty(creditCard.CreditCardNumber))
        {
            return CreateResponse(new DataContractMessage<string> { Success = false }, "Nenhum cartao encontrado para essa conta");
        }

        var update = await creditCardRepository.ToggleBlockdCard(id, isBlocked);

        creditCard.CreditCardBlocked = update ? isBlocked : creditCard.CreditCardBlocked;

        return CreateResponse(new DataContractMessage<CreditCard> { Data = creditCard, Success = update });
    }

    [HttpDelete("virtual-creditcard-delete/{id:Guid}")]
    public async Task<IActionResult> VirtualCreditCardDelete(Guid id)
    {
        var creditCard = await creditCardRepository.ListCreditCardById(id);

        if (creditCard == null || string.IsNullOrEmpty(creditCard.CreditCardNumber))
        {
            return CreateResponse(new DataContractMessage<string> { Success = false }, "Nenhum cartao encontrado para essa conta");
        }

        var update = await creditCardRepository.VirtaulCreditCardDelete(id);

        creditCard.VirtualCreditCards = update ? [] : creditCard.VirtualCreditCards;

        return CreateResponse(new DataContractMessage<CreditCard> { Data = creditCard, Success = update });
    }


    [HttpGet("list-all-creditcards")]
    public async Task<IActionResult> ListAllCreditCards()
    {
        return CreateResponse(new DataContractMessage<IEnumerable<CreditCard>> { Data = await creditCardRepository.ListAllCreditCards(), Success = true });
    }

    [HttpGet("list-credicard-by-id/{accountId}")]
    public async Task<IActionResult> ListCreditCardById(Guid accountId)
    {
        var creditCard = await creditCardRepository.ListCreditCardByAccountId(accountId);

        if (creditCard == null)
        {
            return CreateResponse(new DataContractMessage<string> { Success = false }, "Nenhum cartao de credito encontrado");
        }

        return CreateResponse(new DataContractMessage<CreditCard> { Data = creditCard, Success = true });
    }

    private (CreditCardType, bool, decimal) SuggestCreditCardAndLimit(decimal salaryPerMonth, decimal averageBudgetPerMonth, AccountType accountType, decimal suggestedLimit)
    {
        CreditCardType cardType = CreditCardType.STANDARD;
        bool isLimitFullyAccepted = true;
        decimal approvedLimit;

        // Função auxiliar para calcular o limite ajustado
        decimal CalculateAdjustedLimit(decimal minLimit, decimal maxLimit, decimal influenceFactor)
        {
            // Garante que o limite sugerido esteja dentro da faixa mínima e máxima
            decimal boundedLimit = Math.Max(minLimit, Math.Min(suggestedLimit, maxLimit));
            // Combina o limite sugerido (com peso) e o limite máximo (com peso complementar)
            return boundedLimit * influenceFactor + maxLimit * (1 - influenceFactor);
        }

        // Verifica os critérios para cada tipo de cartão
        if (salaryPerMonth >= 15000m && averageBudgetPerMonth >= 10000m && accountType == AccountType.BusinessAccount)
        {
            cardType = CreditCardType.INFINITE;
            approvedLimit = CalculateAdjustedLimit(
                minLimit: 10000m,
                maxLimit: Math.Min(averageBudgetPerMonth * 0.5m, 50000m),
                influenceFactor: 0.9m
            );
        }
        else if (salaryPerMonth >= 15000m && averageBudgetPerMonth >= 6000m)
        {
            cardType = CreditCardType.BLACK;
            approvedLimit = CalculateAdjustedLimit(
                minLimit: 5000m,
                maxLimit: Math.Min(salaryPerMonth * 0.4m, 30000m),
                influenceFactor: 0.8m
            );
        }
        else if (salaryPerMonth >= 6000m && averageBudgetPerMonth >= 2000m)
        {
            cardType = CreditCardType.PLATINUM;
            approvedLimit = CalculateAdjustedLimit(
                minLimit: 3000m,
                maxLimit: Math.Min(salaryPerMonth * 0.3m, 15000m),
                influenceFactor: 0.7m
            );
        }
        else if (salaryPerMonth >= 3000m && averageBudgetPerMonth >= 1000m)
        {
            cardType = CreditCardType.GOLD;
            approvedLimit = CalculateAdjustedLimit(
                minLimit: 2000m,
                maxLimit: Math.Min(salaryPerMonth * 0.25m, 10000m),
                influenceFactor: 0.6m
            );
        }
        else if (salaryPerMonth >= 1600m)
        {
            cardType = CreditCardType.STANDARD;
            approvedLimit = CalculateAdjustedLimit(
                minLimit: 1000m,
                maxLimit: Math.Min(salaryPerMonth * 0.2m, 5000m),
                influenceFactor: 0.5m
            );
        }
        else
        {
            isLimitFullyAccepted = false;
            decimal maxLimit = accountType == AccountType.BusinessAccount
                ? Math.Min(averageBudgetPerMonth * 0.1m, 2000m)
                : Math.Min(salaryPerMonth * 0.15m, 2000m);
            approvedLimit = CalculateAdjustedLimit(
                minLimit: 500m,
                maxLimit: maxLimit,
                influenceFactor: 0.3m
            );
        }

        return (cardType, isLimitFullyAccepted, approvedLimit);
    }

    private CreditCard GetCreditCard(CreditCardType creditCardType, Guid accountId, decimal suggestedLimit)
    {
        var cardMappings = new Dictionary<CreditCardType, CreditCard>
            {
                { CreditCardType.STANDARD, new CreditCard { CreditCardType = CreditCardType.STANDARD, CreditCardLimit = suggestedLimit, AccountId = accountId, Id = Guid.NewGuid(), CreditCardTried = true, CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.GOLD, new CreditCard { CreditCardType = CreditCardType.GOLD, CreditCardLimit = suggestedLimit, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.PLATINUM, new CreditCard { CreditCardType = CreditCardType.PLATINUM, CreditCardLimit = suggestedLimit, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.BLACK, new CreditCard { CreditCardType = CreditCardType.BLACK, CreditCardLimit = suggestedLimit, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.INFINITE, new CreditCard { CreditCardType = CreditCardType.INFINITE, CreditCardLimit = suggestedLimit, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } }
            };

        return cardMappings.TryGetValue(creditCardType, out var creditCard) ? creditCard : null;
    }

    private void CheckTriedCard(CreditCard creditCard)
    {
        if (creditCard.CreditCardTriedTimes == 1)
        {
            creditCard.CreditCardNextAttempt = DateTime.Now.AddMinutes(5);
        }

        if (creditCard.CreditCardTriedTimes == 2)
        {
            creditCard.CreditCardNextAttempt = DateTime.Now.AddMinutes(10);
        }

        if (creditCard.CreditCardTriedTimes == 3)
        {
            creditCard.CreditCardNextAttempt = DateTime.Now.AddMinutes(15);
        }

        creditCard.CreditCardTried = true;
    }

    private DateTime CalculateNextAttempt(int triedTimes)
    {
        return triedTimes switch
        {
            1 => DateTime.UtcNow.AddMinutes(5),      // 5 minutos após a 1ª tentativa
            2 => DateTime.UtcNow.AddMinutes(15),     // 15 minutos após a 2ª tentativa
            _ => DateTime.UtcNow.AddMinutes(15)      // Máximo de 15 minutos para 3+ tentativas
        };
    }

    private async Task<IActionResult> SaveTriedCard(Guid accountId, Guid cardExist)
    {
        var saveCreditCard = new CreditCard { CreditCardTriedTimes = 1, CreditCardNextAttempt = CalculateNextAttempt(1), AccountId = accountId, Id = cardExist == Guid.Empty ? Guid.NewGuid() : cardExist, CreditCardTried = true };

        await creditCardRepository.SaveTriedCard(saveCreditCard);

        await publishEndpoint.Publish(GenerateEvent.CreateCreditCardFailedEvent(accountId.ToString(), saveCreditCard.CreditCardNextAttempt));

        return CreateResponse(new DataContractMessage<string>(), $"Nao foi possivel concender um cartao de credito para voce agora, tente novamente em {saveCreditCard.CreditCardNextAttempt}");
    }
}
