using fusion.bank.core.Enum;

namespace fusion.bank.core.Messages.Requests
{
    public record NewTransferAccountRequest(TransferType TransferType, string KeyAccount, decimal Amount, string AccountPayer);
}
