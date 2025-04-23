namespace Asp_REST.Dto
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string ExpiresIn { get; set; }

        public LoginResponse(string token, string expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
        }
    }

}
