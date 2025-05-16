using fusion.bank.account.domain.Interfaces;
using fusion.bank.account.domain.Request;
using fusion.bank.core.Enum;
using fusion.bank.core.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace fusion.bank.account.repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMongoCollection<Account> accountCollection;

        public AccountRepository(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            var dataBase = client.GetDatabase("fusion_db");
            accountCollection = dataBase.GetCollection<Account>(configuration["CollectionName"]);
        }


        public async Task<Account> ListAccountById(Guid id)
        {
            return (await accountCollection.FindAsync(d => d.AccountId == id)).FirstOrDefault();
        }

        public async Task<Account> ListAccountByNumberAgencyAccount(string accountNumber, string agencyNumber)
        {
            return (await accountCollection.FindAsync(d => d.AccountNumber == accountNumber && d.Agency == agencyNumber)).FirstOrDefault();
        }

        public async Task<Account> ListAccountByNumberAccount(string accountNumber)
        {
            return (await accountCollection.FindAsync(d => d.AccountNumber == accountNumber)).FirstOrDefault();
        }

        public async Task<Account> ListAccountByKey(string keyAccount)
        {
            return (await accountCollection.FindAsync(d => d.KeyAccount == keyAccount)).FirstOrDefault();
        }

        public async Task<Account> EditAccount(Guid accountId, AccountEditRequest accountEditRequest)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.AccountId, accountId);

            var account = (await accountCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();

            if(account == null) return null;

            var updateDefinitions = new List<UpdateDefinition<Account>>();

            var properties = typeof(AccountEditRequest).GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(accountEditRequest);
                if (value != null)
                {
                    updateDefinitions.Add(Builders<Account>.Update.Set(prop.Name, value));
                }
            }

            if(updateDefinitions.Count <= 0) return null;

            var update = Builders<Account>.Update.Combine(updateDefinitions);

            return await accountCollection.FindOneAndUpdateAsync<Account>(filter, update);
        }

        public async Task<Account> DeleteKeyAccount(Guid accountId)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.AccountId, accountId);

            var account = (await accountCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();

            if (account == null) return null;

            var update = Builders<Account>.Update.Set(d => d.KeyAccount, string.Empty).Set(d => d.KeyTypePix, KeyType.CPF);

            await accountCollection.FindOneAndUpdateAsync<Account>(filter, update);

            return account;
        }

        public async Task<Account> EditKeyAccount(Guid accountId, string keyAccount)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.AccountId, accountId);

            var account = (await accountCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();

            if (account == null) return null;

            var update = Builders<Account>.Update.Set(d => d.KeyAccount, keyAccount);

            await accountCollection.FindOneAndUpdateAsync<Account>(filter, update);

            return account;
        }

        public async Task<bool> DeleteAccount(Guid accountId)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.AccountId, accountId);

            var account = (await accountCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();

            if (account == null) return false;

            var result = await accountCollection.DeleteOneAsync(d => d.AccountId == accountId);

            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Account>> ListAllAccount()
        {
            var filter = new BsonDocument();
            return (await accountCollection.FindAsync(filter)).ToList();
        }

        public async Task SaveAccount(Account account)
        {
            await accountCollection.InsertOneAsync(account);
        }

        public async Task SaveKeyByAccount(RegisterKeyRequest registerKey)
        {
            var filter = Builders<Account>.Filter.Eq(d => d.AccountId, registerKey.AccountId);
            var update = Builders<Account>.Update.Set(d => d.KeyAccount, registerKey.KeyPix).Set(e => e.KeyTypePix, registerKey.KeyTypePix);

            await accountCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetDarkMode(Guid accountId, bool darkMode)
        {
            var filter = Builders<Account>.Filter.Eq(d => d.AccountId, accountId);
            var update = Builders<Account>.Update.Set(d => d.DarkMode, darkMode);

            await accountCollection.UpdateOneAsync(filter, update);
        }


        public async Task UpdateAccount(Account account)
        {
            var filter = Builders<Account>.Filter.Eq("AccountId", account.AccountId);

            await accountCollection.ReplaceOneAsync(filter, account);
        }

        public async Task<Account> GetAccountPerTypeAndPassoword(LoginRequest loginRequest)
        {
            var filterBuilder = Builders<Account>.Filter;
            var loginFirst = string.Empty;
            var loginSecond = string.Empty;

            if(loginRequest.LoginType == LoginType.ACCOUNT)
            {
                loginFirst = loginRequest.LoginUser.Substring(0, 8);
                loginSecond = loginRequest.LoginUser.Substring(loginRequest.LoginUser.Length - 4, 4);
            }

            

            FilterDefinition<Account> filter = loginRequest.LoginType switch
            {
                LoginType.EMAIL => filterBuilder.Eq(d => d.Email, loginRequest.LoginUser),
                LoginType.CPFCNPJ => filterBuilder.Eq(d => d.Document, loginRequest.LoginUser),
                LoginType.ACCOUNT => filterBuilder.And(filterBuilder.Eq(d => d.AccountNumber, loginFirst), filterBuilder.Eq(d => d.Agency, loginSecond)),
                _ => throw new ArgumentException("Tipo de login inválido.")
            };

            var account = (await accountCollection.FindAsync(filter)).FirstOrDefault();

            if (account == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, account.Password))
            {
                return null;
            }

            return account;
        }
    }
}
