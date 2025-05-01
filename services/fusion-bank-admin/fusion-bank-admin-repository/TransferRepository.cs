using fusion.bank.admin.domain.Interfaces;

namespace fusion.bank.transfer.repository
{
    public class TransferRepository : ITransferRepository
    {
        //private readonly IMongoCollection<Transfer> transferCollection;

        //public TransferRepository(IConfiguration configuration)
        //{
        //    var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
        //    var dataBase = client.GetDatabase("fusion_db");
        //    transferCollection = dataBase.GetCollection<Transfer>("transferCollection");
        //}

        //public async Task SaveTransfer(Transfer transfer)
        //{
        //    await transferCollection.InsertOneAsync(transfer);
        //}

        //public async Task UpdateTransfer(Transfer transfer)
        //{
        //    var filter = Builders<Transfer>.Filter.Eq(d => d.TransferId, transfer.TransferId);

        //    await transferCollection.ReplaceOneAsync(filter, transfer);
        //}

        //public async Task<List<Transfer>> ListAllSchedules()
        //{
        //    var filter = Builders<Transfer>.Filter.And(
        //            Builders<Transfer>.Filter.Eq(d => d.TransferStatus, domain.Enum.TransferStatus.CREATED),
        //            Builders<Transfer>.Filter.Lte(d => d.ScheduleDate, DateTime.Now)
        //        );

        //    return await (await transferCollection.FindAsync(filter)).ToListAsync();
        //}

        //public async Task<List<Transfer>> ListAllTransfers()
        //{
        //    var filter = FilterDefinition<Transfer>.Empty;

        //    return await (await transferCollection.FindAsync(filter)).ToListAsync();
        //}

        //public async Task<Transfer> ListById(Guid transferId)
        //{
        //    var filter = Builders<Transfer>.Filter.Eq(d => d.TransferId, transferId);

        //    return await (await transferCollection.FindAsync(filter)).FirstOrDefaultAsync();
        //}
    }
}
