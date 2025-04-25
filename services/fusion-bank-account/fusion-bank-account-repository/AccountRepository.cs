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
            accountCollection = dataBase.GetCollection<Account>("accountCollection");
        }


        public async Task<Account> ListAccountById(Guid id)
        {
            return (await accountCollection.FindAsync(d => d.AccountId == id)).FirstOrDefault();
        }

        public async Task<Account> ListAccountByNumberAccount(string accountNumber)
        {
            return (await accountCollection.FindAsync(d => d.AccountNumber == accountNumber)).FirstOrDefault();
        }

        public async Task<Account> ListAccountByKey(Guid accountId, string keyAccount)
        {
            return (await accountCollection.FindAsync(d => d.KeyAccount == keyAccount && d.AccountId == accountId)).FirstOrDefault();
        }

        public async Task<Account> EditAccountByKey(string keyAccount, AccountEditRequest accountEditRequest)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.KeyAccount, keyAccount);

            var account = (await accountCollection.FindAsync(d => d.KeyAccount == keyAccount)).FirstOrDefault();

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

            var update = Builders<Account>.Update.Set(d => d.KeyAccount, null);

            return await accountCollection.FindOneAndUpdateAsync<Account>(filter, update);
        }

        public async Task<Account> EditKeyAccount(Guid accountId, string keyAccount)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.AccountId, accountId);

            var account = (await accountCollection.FindAsync(d => d.AccountId == accountId)).FirstOrDefault();

            if (account == null) return null;

            var update = Builders<Account>.Update.Set(d => d.KeyAccount, keyAccount);

            return await accountCollection.FindOneAndUpdateAsync<Account>(filter, update);
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

        public async Task SaveKeyByAccount(Guid idAccount, string keyAccount)
        {
            var filter = Builders<Account>.Filter.Eq(d => d.AccountId, idAccount);
            var update = Builders<Account>.Update.Set(d => d.KeyAccount, keyAccount);

            await accountCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetDarkMode(Guid idAccount, bool darkMode)
        {
            var filter = Builders<Account>.Filter.Eq(d => d.AccountId, idAccount);
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
            FilterDefinition<Account> filter = loginRequest.LoginType switch
            {
                LoginType.EMAIL => filterBuilder.Eq(d => d.Email, loginRequest.LoginUser),
                LoginType.CPFCNPJ => filterBuilder.Eq(d => d.Document, loginRequest.LoginUser),
                LoginType.ACCOUNT => filterBuilder.Eq(d => d.AccountNumber, loginRequest.LoginUser),
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
