using fusion.bank.core.Enum;

namespace fusion.bank.transfer.domain.Request
{
    public record TransferRequest(Guid AccountId, string AccountNumberReceiver, string KeyAccount, decimal Amount, string NamePayer, string NameReceiver, string DocummentPayer, string DocumentoReceiver, string AccountNumberPayer, 
        string DocumentReceiver, string AgencyNumberReceiver, bool IsSchedule, DateTime ScheduleDate, TransferType TransferType);
}
