using fusion.bank.core.Enum;

namespace fusion.bank.core.Request
{
    public class LoginRequest
    {
        public string LoginUser { get; set; }
        public string Password { get; set; }
        public LoginType LoginType { get; set; }
    }
}
