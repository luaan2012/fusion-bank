namespace fusion.bank.core.Messages.Responses
{
    public class CreatedKeyAccountResponse(bool created)
    {
        public bool Created { get; set; } = created;
    }
}
