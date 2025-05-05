using fusion.bank.core.Model;

namespace fusion.bank.account.domain.Response
{
    public class LoginResponse
    {
        public Account Account { get; set; }
        public string Token { get; set; }
    }
}
