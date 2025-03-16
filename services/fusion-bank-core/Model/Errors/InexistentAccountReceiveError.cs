using fusion.bank.core.Enum;

namespace fusion.bank.core.Model.Errors
{
    public class InexistentAccountReceiveError : ErrorMessage
    {
        public InexistentAccountReceiveError()
        {
            Message = "Inexistent receive account";
            LevelError = LevelError.LOW;
        }
    }
}
