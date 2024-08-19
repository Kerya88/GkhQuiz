namespace GkhQuiz.Services
{
    public interface ICryptoService
    {
        public string GetServiceToken();
        public string GetAccessToken(string email, string code);
        public string SimpleGetAccessToken(string email, string code);
        public string ComputeHash(string hashBase);
    }
}
