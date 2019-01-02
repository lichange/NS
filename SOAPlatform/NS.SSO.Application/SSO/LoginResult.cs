
namespace NS.SSO.Application
{
    public class LoginResult :Response<string>
    {
        public string ReturnUrl;
        public string Token;
    }
}