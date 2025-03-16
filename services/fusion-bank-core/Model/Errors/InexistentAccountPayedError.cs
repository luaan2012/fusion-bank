using fusion.bank.core.Enum;

namespace fusion.bank.core.Model.Errors
{
    public class InexistentAccountPayedError : ErrorMessage
    {
        public InexistentAccountPayedError()
        {
            Message = "Inexistent payed account";
            LevelError = LevelError.LOW;
        }
    }
}
