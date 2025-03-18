using fusion.bank.core.Model;

namespace fusion.bank.creditcard.domain.Interfaces
{
    public interface ICreditCartRepository
    {
        Task SaveTriedCard(CreditCard creditCard);
        Task<CreditCard> GetTriedCard(Guid accountId);
        Task<IEnumerable<CreditCard>> ListAllCreditCards();
    }
}
