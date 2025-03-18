using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.creditcard.domain.Enum;
using fusion.bank.creditcard.domain.Interfaces;
using fusion.bank.creditcard.domain.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
namespace fusion.bank.creditCard.api.Controllers;

[ApiController]
[Route("[controller]")]
public class CentralBankController(ILogger<CentralBankController> logger, ICreditCartRepository creditCartRepository, IRequestClient<NewCreditCardRequest> requestClient)  : MainController
{
    [HttpGet("request-creditcard/{accountId:Guid}")]
    public async Task<IActionResult> RequestCreditCard(Guid accountId)
    {
        var creditCardTried = await creditCartRepository.GetTriedCard(accountId);

        if (creditCardTried != null)
        {
            if (DateTime.UtcNow < creditCardTried.CreditCardNextAttempt)
            {
                return CreateResponse(new DataContractMessage<string>(), $"Ainda estamos analisando sua conta, tente novamente em {creditCardTried.CreditCardNextAttempt}");
            }
        }

        var creditCardRequestResponse = await requestClient.GetResponse<DataContractMessage<CreditCardRequestResponse>>(new NewCreditCardRequest(accountId));

        // Determina o tipo de cartão com base nos critérios
        var (cardType, success) = DetermineCreditCardType(
            creditCardRequestResponse.Message.Data.SalaryPerMonth,
            creditCardRequestResponse.Message.Data.AverageBudgetPerMonth,
            creditCardRequestResponse.Message.Data.AccountType
        );

        if (!success) return new CreditCard();

        var creditCard = GetCreditCard(cardType);

        if (creditCard is null) return creditCard;

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

    private CreditCard GetCreditCard(CreditCardType creditCardType)
    {
        var cardMappings = new Dictionary<CreditCardType, CreditCard>
            {
                { CreditCardType.STANDARD, new CreditCard { CreditCardType = CreditCardType.STANDARD, CreditCardLimit = 500m } },
                { CreditCardType.GOLD, new CreditCard { CreditCardType = CreditCardType.GOLD, CreditCardLimit = 1500m } },
                { CreditCardType.PLATINUM, new CreditCard { CreditCardType = CreditCardType.PLATINUM, CreditCardLimit = 3000m } },
                { CreditCardType.BLACK, new CreditCard { CreditCardType = CreditCardType.BLACK, CreditCardLimit = 8000m } },
                { CreditCardType.INFINITE, new CreditCard { CreditCardType = CreditCardType.INFINITE, CreditCardLimit = 20000m } }
            };

        return cardMappings.TryGetValue(creditCardType, out var creditCard) ? creditCard : null;
    }

    private void CheckTriedCard(CreditCard creditCard)
    {
        if (creditCard.CreditCardTriedTimes == 1)
        {

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

}
}
