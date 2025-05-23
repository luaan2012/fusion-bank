﻿namespace fusion.bank.transfer.domain.Interfaces
{
    public interface ITransferRepository
    {
        Task SaveTransfer(Transfer transfer);
        Task UpdateTransfer(Transfer transfer);
        Task<List<Transfer>> ListAllSchedules();
        Task<Transfer> ListById(Guid transferId);
        Task<List<Transfer>> ListAllTransfers();
    }
}
