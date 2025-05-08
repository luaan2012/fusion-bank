namespace fusion.bank.events.domain.Request
{
    public record ListEventsRequest(string AccountId, int Limit = int.MaxValue);
}
