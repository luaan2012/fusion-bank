using fusion.bank.creditcard.domain.Models;

namespace fusion.bank.creditcard.domain.Interfaces
{
    public interface ICreditCartRepository
    {
        Task SaveTriedCard(CreditCard creditCard);
        Task<CreditCard> GetTriedCard(Guid accountId);
    }
}
