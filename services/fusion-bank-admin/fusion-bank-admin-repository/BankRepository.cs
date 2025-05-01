using fusion.bank.admin.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace fusion.bank.admin.repository
{
    public class BankRepository : IBankRepository
    {
        //private readonly IMongoCollection<Bank> bankCollection;

        //public BankRepository(IConfiguration configuration)
        //{
        //    var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
        //    var dataBase = client.GetDatabase("fusion_db");
        //    bankCollection = dataBase.GetCollection<Bank>("bankCollection");
        //}

        //public async Task<Bank> ListBankByISPB(string id)
        //{
        //    return (await bankCollection.FindAsync(d => d.ISPB == id)).FirstOrDefault();
        //}

        //public async Task<Bank> ListBankById(Guid id)
        //{
        //    return (await bankCollection.FindAsync(d => d.BankId == id)).FirstOrDefault();
        //}

        //public async Task<Bank> ListAccountBankById(Guid id)
        //{
        //    return (await bankCollection.FindAsync(d => d.Accounts.Exists(d => d.AccountId == id))).FirstOrDefault();
        //}

        //public async Task<Bank> ListAccountBankByKeyAccount(string keyAccount)
        //{
        //    return (await bankCollection.FindAsync(d => d.Accounts.Exists(d => d.KeyAccount == keyAccount))).FirstOrDefault();
        //}

        //public async Task<Bank> ListAccountBankByAccountNumber(string accountNumber)
        //{
        //    return (await bankCollection.FindAsync(d => d.Accounts.Exists(d => d.AccountNumber == accountNumber))).FirstOrDefault();
        //}

        //public async Task<IEnumerable<Bank>> ListAllBank()
        //{
        //    return await bankCollection.AsQueryable().ToListAsync();
        //}

        //public async Task SaveBank(Bank bank)
        //{
        //    await bankCollection.InsertOneAsync(bank);
        //}

        //public async Task UpdateBank(Bank bank)
        //{
        //    await bankCollection.DeleteOneAsync(d => d.BankId == bank.BankId);

        //    await bankCollection.InsertOneAsync(bank);
        //}

        //public async Task<Bank> UpdateBank(Guid bankId, BankEditRequest bankEditRequest)
        //{
        //    var filter = Builders<Bank>.Filter.Eq(x => x.BankId, bankId);

        //    var bank = (await bankCollection.FindAsync(d => d.BankId == bankId)).FirstOrDefault();

        //    if (bankEditRequest == null) return null;

        //    var updateDefinitions = new List<UpdateDefinition<Bank>>();

        //    var properties = typeof(BankEditRequest).GetProperties();

        //    foreach (var prop in properties)
        //    {
        //        var value = prop.GetValue(bankEditRequest);
        //        if (value != null)
        //        {
        //            updateDefinitions.Add(Builders<Bank>.Update.Set(prop.Name, value));
        //        }
        //    }

        //    if (updateDefinitions.Count <= 0) return null;

        //    var update = Builders<Bank>.Update.Combine(updateDefinitions);

        //    return await bankCollection.FindOneAndUpdateAsync<Bank>(filter, update);
        //}

        //public async Task<bool> DeleteBank(Guid bankId)
        //{
        //    return (await bankCollection.DeleteOneAsync(d => d.BankId == bankId)).DeletedCount > 0;
        //}
    }
}

