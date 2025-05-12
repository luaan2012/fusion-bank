using fusion.bank.core.Model;

namespace fusion.bank.creditcard.domain.Interfaces
{
    public interface ICreditCardRepository
    {
        Task SaveTriedCard(CreditCard creditCard);
        Task<bool> ToggleBlockdCard(Guid id, bool isBlocked);
        Task<bool> VirtaulCreditCardDelete(Guid id);
        Task<CreditCard> ListCreditCardById(Guid id);
        Task<CreditCard> ListCreditCardByAccountId(Guid accountId);
        Task<IEnumerable<CreditCard>> ListAllCreditCards();
    }
}
