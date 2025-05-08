using fusion.bank.core.Enum;

namespace fusion.bank.core.Messages.Requests
{
    public record NewTransferCentralRequest(TransferType TransferType, Guid AccountId, decimal AmountReceiver, decimal AmountPayer, string KeyAccount, string AccountPayer);
}
