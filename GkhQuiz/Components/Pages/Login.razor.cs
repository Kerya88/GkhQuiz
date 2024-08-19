using GkhQuiz.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GkhQuiz.Components.Pages
{
    public class LoginScript : ComponentBase
    {
        readonly Regex _emailRegex = new(@"^\S+@\S+\.\S+$");

        [Inject]
        IEmailService EmailService { get; set; }
        [Inject]
        ICryptoService CryptoService { get; set; }
        [Inject]
        ISessionStorageService SessionStorageService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public LoginViewModel LoginData { get; set; }
        [Parameter]
        public string ExceptionMessage { get; set; }

        public LoginScript()
        {
            LoginData = new LoginViewModel();
        }

        public async Task SendCodeAsync()
        {
            ExceptionMessage = string.Empty;
            await SessionStorageService.Clear();

            if (!string.IsNullOrEmpty(LoginData.Email) && _emailRegex.IsMatch(LoginData.Email))
            {
                try
                {
                    await EmailService.SendVerificationCodeAsync(LoginData.Email);
                }
                catch
                {
                    ExceptionMessage = "Ошибка при отпрвке электронного письма";
                }
            }
        }

        public async Task CheckCodeAsync()
        {
            if (!string.IsNullOrEmpty(LoginData.VerificationCode))
            {
                var hash = CryptoService.GetAccessToken(LoginData.Email, LoginData.VerificationCode);
                var userData = await SessionStorageService.GetAsync<StorageProxy>("UserData");

                if (hash == userData.Hash && DateTime.Now < userData.ExpiredAt)
                {
                    NavigationManager.NavigateTo("/", true);
                }
                else
                {
                    ExceptionMessage = "Вы указали неверный код или время его действия истекло";
                }
            }
            else
            {
                ExceptionMessage = "Вы не указали код";
            }
        }

        public class LoginViewModel
        {
            [Required]
            [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Не соответствует формату электронной почты")]
            public string Email { get; set; }
            [StringLength(6, ErrorMessage = "Длина кода должна быть 6 символов")]
            public string VerificationCode { get; set; }
        }

        private class StorageProxy(string email, string code, string hash)
        {
            public string Email { get; set; } = email;
            public string Code { get; set; } = code;
            public string Hash { get; set; } = hash;
            public DateTime ExpiredAt { get; set; } = DateTime.Now.AddMinutes(5);
        }
    }
}
