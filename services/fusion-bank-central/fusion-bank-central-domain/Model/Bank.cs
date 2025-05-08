using fusion.bank.central.domain.Enum;
using fusion.bank.central.Request;
using fusion.bank.core.Helpers;
using fusion.bank.core.Messages.Producers;
using MongoDB.Bson;
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

        [BsonRepresentation(BsonType.String)]
        public BankType BankType { get; internal set; }
        public decimal MaintenanceFee { get; internal set; }
        public List<NewAccountProducer> Accounts { get; internal set; }
        public List<BankAdvantages> BankAdvantages { get; internal set; }

        public void CreateBank(BankRequest bankRequest)
        {
            Name = bankRequest.Name;
            NameNormalized = Name.Replace(" ", "").Normalize().ToUpper();
            Address = bankRequest.Address;
            City = bankRequest.City;
            State = bankRequest.State;  
            Accounts = [];
            MaintenanceFee = bankRequest.Fee;
            BankAdvantages = new List<BankAdvantages>();
            BankType = bankRequest.BankType;

            // Adicionar vantagens do array AdvantageTypes
            if (bankRequest.AdvantageType != null)
            {
                foreach (var advantageType in bankRequest.AdvantageType)
                {
                    BankAdvantages.Add(new BankAdvantages
                    {
                        Type = advantageType,
                        IsActive = true
                    });
                }
            }

            CreateSwiftCode();
            CreateISBPCode();
        }

        public void RemoveAdvantages(AdvantageType advantageType)
        {
            BankAdvantages.RemoveAll(d => d.Type == advantageType);
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

    public class BankAdvantages
    {
        public bool IsActive { get; set; }
        [BsonRepresentation(BsonType.String)]
        public AdvantageType Type { get; set; }
        public string Title => Type switch
        {
            AdvantageType.TEDFREE => "TED grátis ilimitado",
            AdvantageType.CARDFREE => "Cartão sem anuidade",
            AdvantageType.CASHBACK1 => "Cashback de 1%",
            AdvantageType.CASHBACK2 => "Cashback de 2%",
            AdvantageType.CASHBACK5 => "Cashback de 5%",
            AdvantageType.CASHBACK10 => "Cashback de 10%",
            AdvantageType.CARDPLATINUM => "Cartão Platinum",
            AdvantageType.CARDBLACK => "Cartão Black exclusivo%",
            AdvantageType.FEE1 => "Taxas de 1%%",
            AdvantageType.FEE2 => "Taxas de 2%%",
            AdvantageType.FEELESSTOPAY => "Desconto a pagar antes do vencimento",
            AdvantageType.APPROVAL24 => "Aprovação em 24h",
            AdvantageType.CONSULTANCY => "Assessoria exclusiva",
            AdvantageType.CDB150 => "CDB 150% do CDI",
            AdvantageType.MILESILIMITED => "Milhas ilimitadas",
            _ => "Sem taxa de manutenção"
        };
    }
}
