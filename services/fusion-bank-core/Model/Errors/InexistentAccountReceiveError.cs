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

    public class BalanceIsNotSufficient : ErrorMessage
    {
        public BalanceIsNotSufficient()
        {
            Message = "Amount request is less than balance of the account";
            LevelError = LevelError.LOW;
        }
    }
}
