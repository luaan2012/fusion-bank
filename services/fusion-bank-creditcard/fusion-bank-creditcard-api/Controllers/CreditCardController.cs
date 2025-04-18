using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model;
using fusion.bank.creditcard.domain.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.creditCard.api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CentralBankController(ILogger<CentralBankController> logger, ICreditCartRepository creditCartRepository, 
    IRequestClient<NewAccountRequestInformation> requestClient, IRequestClient<NewCreditCardCreatedRequest> requestClientCreated,
    IGenerateCreditCardService generateCreditCardService)  : MainController
{
    [HttpGet("request-creditcard/{accountId:Guid}")]
    public async Task<IActionResult> RequestCreditCard(Guid accountId)
    {
        var creditCardExist = await creditCartRepository.GetTriedCard(accountId);

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

        var (cardType, success) = DetermineCreditCardType(
            creditCardRequestResponse.Message.Data.SalaryPerMonth,
            creditCardRequestResponse.Message.Data.AverageBudgetPerMonth,
            creditCardRequestResponse.Message.Data.AccountType
        );

        if (!success)
        {
            return await SaveTriedCard(accountId, creditCardExist?.Id ?? Guid.Empty);
        }

        var creditCard = GetCreditCard(cardType, accountId);

        if (creditCard is null)
        {
            return await SaveTriedCard(accountId, creditCardExist?.Id ?? Guid.Empty);
        }

        var generateInformations = await generateCreditCardService.GenerateCreditCard(creditCard.CreditCardFlag);

        creditCard.CreditCardNumber = generateInformations.CreditCardNumber;
        creditCard.CreditCardCode = generateInformations.CreditCardCode;
        creditCard.CreditCardValidity = DateTime.ParseExact(generateInformations.CreditCardValidity, "dd/MM/yyyy", null);
        creditCard.CreditCardName = creditCardRequestResponse.Message.Data.Name;

        var creditCardCreated = await requestClientCreated.GetResponse<DataContractMessage<string>>(new NewCreditCardCreatedRequest { CreditCard = creditCard, AccountId = creditCard.AccountId});

        if (!creditCardCreated.Message.Success)
        {
            return CreateResponse(new DataContractMessage<string>(), $"Nao foi possivel solicitar um cartao para voce agora, tente novamente mais tarde.");
        }

        await creditCartRepository.SaveTriedCard(creditCard);

        return CreateResponse(new DataContractMessage<CreditCard> { Data = creditCard, Success = true});
    }

    [HttpGet("list-all-creditcards")]
    public async Task<IActionResult> ListAllCreditCards()
    {
        return CreateResponse(new DataContractMessage<IEnumerable<CreditCard>> { Data = await creditCartRepository.ListAllCreditCards(), Success = true });
    }

    private (CreditCardType, bool) DetermineCreditCardType(decimal salaryPerMonth, decimal averageBudgetPerMonth, AccountType accountType)
    {
        if (salaryPerMonth >= 15000m && averageBudgetPerMonth >= 10000m && accountType == AccountType.BusinessAccount)
        {
            return (CreditCardType.INFINITE, true);
        }

        else if (salaryPerMonth >= 15000m && averageBudgetPerMonth >= 6000m)
        {
            return (CreditCardType.BLACK, true);
        }

        else if (salaryPerMonth >= 6000m && averageBudgetPerMonth >= 2000m)
        {
            return (CreditCardType.PLATINUM, true);
        }

        else if (salaryPerMonth >= 3000m && averageBudgetPerMonth >= 1000m)
        {
            return (CreditCardType.GOLD, true);
        }

        else if (salaryPerMonth >= 1600m)
        {
            return (CreditCardType.STANDARD, true);
        }

        return (CreditCardType.STANDARD, false);
    }

    private CreditCard GetCreditCard(CreditCardType creditCardType, Guid accountId)
    {
        var cardMappings = new Dictionary<CreditCardType, CreditCard>
            {
                { CreditCardType.STANDARD, new CreditCard { CreditCardType = CreditCardType.STANDARD, CreditCardLimit = 500m, AccountId = accountId, Id = Guid.NewGuid(), CreditCardTried = true, CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.GOLD, new CreditCard { CreditCardType = CreditCardType.GOLD, CreditCardLimit = 1500m, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.PLATINUM, new CreditCard { CreditCardType = CreditCardType.PLATINUM, CreditCardLimit = 3000m, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.BLACK, new CreditCard { CreditCardType = CreditCardType.BLACK, CreditCardLimit = 8000m, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } },
                { CreditCardType.INFINITE, new CreditCard { CreditCardType = CreditCardType.INFINITE, CreditCardLimit = 20000m, AccountId = accountId, Id = Guid.NewGuid(), CreditCardNextAttempt = DateTime.Now.AddMinutes(5), CreditCardUsed = 0 } }
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

        await creditCartRepository.SaveTriedCard(saveCreditCard);

        return CreateResponse(new DataContractMessage<string>(), $"Nao foi possivel concender um cartao de credito para voce agora, tente novamente em {saveCreditCard.CreditCardNextAttempt}");
    }
}
