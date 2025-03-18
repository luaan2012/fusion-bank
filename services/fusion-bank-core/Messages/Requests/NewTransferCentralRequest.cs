using fusion.bank.core.Enum;

namespace fusion.bank.core.Messages.Requests
{
    public record NewTransferCentralRequest(TransferType TransferType, decimal Amount, string KeyAccount, string AccountOwner);
}
