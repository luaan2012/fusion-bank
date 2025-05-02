using fusion.bank.core.Model;

namespace fusion.bank.admin.domain.Interfaces
{
    public interface ICreditCardRepository
    {
        Task SaveTriedCard(CreditCard creditCard);
        Task<CreditCard> GetTriedCard(Guid accountId);
        Task<IEnumerable<CreditCard>> ListAllCreditCards();
    }
}
