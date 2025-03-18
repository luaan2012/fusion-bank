using fusion.bank.core.Enum;

namespace fusion.bank.core.Model.Errors
{
    public class InexistentAccountError : ErrorMessage
    {
        public InexistentAccountError()
        {
            Message = "Inexistent account";
            LevelError = LevelError.LOW;
        }
    }
}
