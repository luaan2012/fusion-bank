using fusion.bank.core.Helpers;
using fusion.bank.core.Messages.Producers;
using MongoDB.Bson.Serialization.Attributes;

namespace fusion.bank.central.domain.Model
{
    public class Bank
    {
        [BsonId]
        public Guid BankId { get; internal set; } = Guid.NewGuid();
        public string Name { get; internal set; }
        public string NameNormalized { get; internal set; }
        public string ISPB { get; internal set; }
        public string Address { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string SwiftCode { get; internal set; }
        public List<NewAccountProducer> Accounts { get; internal set; }
        
        public void CreateBank(string name, string city, string address, string state)
        {
            Name = name;
            NameNormalized = Name.Replace(" ", "").Normalize().ToUpper();
            Address = address;
            City = city;
            State = state;  
            Accounts = [];

            CreateSwiftCode();
            CreateISBPCode();
        }

        internal void CreateSwiftCode()
        {
            SwiftCode = $"{NameNormalized.ToUpper().Substring(0, 4)}{RandomHelper.GetRandomLetters(Address, 2)}{RandomHelper.GetRandomLetters(City, 2)}{RandomHelper.GetRandomLetters(State, 1)}{RandomHelper.GenerateRandomNumbers(2)}";
        }

        internal void CreateISBPCode()
        {
            ISPB = $"{RandomHelper.GetRandomLetters(SwiftCode, 6)}{RandomHelper.GenerateRandomNumbers(8)}";
        }

        public void AddAccount(NewAccountProducer account)
        {
            Accounts.Add(account);
        }

        public void UpdateAccount(Guid id, NewAccountProducer accountUpdate)
        {
            var account = Accounts.Find(d => d.AccountId == id);

            Accounts.Remove(account);
            Accounts.Add(accountUpdate);
        }
    }
}
