namespace SimpleAPI
{
    public class JWTSettings
    {
        public JWTSettings()
        {
            SecretKey = "ctor_jwts-are-awesome";
            Issuer = "ctor_dotnet_security_issuer";
            Audience = "ctor_dotnetsecurityaudience";
        }
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}