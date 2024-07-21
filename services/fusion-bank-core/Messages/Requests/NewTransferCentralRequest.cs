using fusion.bank.transfer.domain.Enum;

namespace fusion.bank.core.Messages.Requests
{
    public record NewTransferCentralRequest(TransferType TransferType, decimal Amount, string keyAccount);
}
