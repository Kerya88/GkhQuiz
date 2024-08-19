using System.Security.Cryptography;
using System.Text;

namespace GkhQuiz.Services
{
    public class CryptoService : ICryptoService
    {
        public string GetServiceToken()
        {
            return ComputeHash("huiktozalezet" + DateTime.Now.ToString("dd"));
        }

        public string GetAccessToken(string email, string code)
        {
            var codeHash = ComputeHash(code);

            return ComputeHash(email + $"eBaL_sVoU_mAt`_{codeHash}_RaZ");
        }

        public string SimpleGetAccessToken(string email, string code)
        {
            return ComputeHash(email + $"eBaL_sVoU_mAt`_{code}_RaZ");
        }

        public string ComputeHash(string hashBase)
        {
            //переводим строку в байт-массим  
            byte[] bytes = Encoding.ASCII.GetBytes(hashBase);

            //вычисляем хеш-представление в байтах  
            byte[] byteHash = MD5.HashData(bytes);

            string hash = string.Empty;

            //формируем одну цельную строку из массива  
            foreach (byte b in byteHash)
            {
                hash += string.Format("{0:x2}", b);
            }

            return hash;
        }
    }
}
