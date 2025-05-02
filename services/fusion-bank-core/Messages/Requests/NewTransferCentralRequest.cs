using fusion.bank.core.Enum;

namespace fusion.bank.core.Messages.Requests
{
    public record NewTransferCentralRequest(TransferType TransferType, Guid AccountId, decimal AmountReceive, decimal AmountOwner, string KeyAccount, string AccountOwner);
}
