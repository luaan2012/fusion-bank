using fusion.bank.core.Enum;
using fusion.bank.creditcard.domain.DTO;

namespace fusion.bank.creditcard.domain.Interfaces
{
    public interface IGenerateCreditCardService
    {
        Task<CreditCardDTO> GenerateCreditCard(CreditCardFlag creditCard);
    }
}
