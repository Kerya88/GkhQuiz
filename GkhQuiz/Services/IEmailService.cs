namespace GkhQuiz.Services
{
    public interface IEmailService
    {
        public Task SendVerificationCodeAsync(string email);
    }
}
