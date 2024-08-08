using GJIService;
using GkhQuiz.Entities;
using GkhQuiz.Enums;
using System.Security.Cryptography;
using System.Text;

namespace GkhQuiz.Services
{
    public class QuizService : IQuizService
    {
        static readonly string HashBase = "huiktozalezet" + DateTime.Now.ToString("dd");
        readonly UrbanAppealServiceClient _serviceClient;

        public QuizService()
        {
            _serviceClient = new(UrbanAppealServiceClient.EndpointConfiguration.BasicHttpBinding_IUrbanAppealService);
        }

        public List<Quiz> GetNowQuizzes()
        {
            var responce = _serviceClient.GetListOnlineSurveyAsync(GetToken()).Result;

            return responce.RequestResult.Code switch
            {
                "00" => ParseSurveyResponse(responce.OnlineSurveys),
                "01" => throw new Exception("Нет активных опросов"),
                "02" => throw new Exception("Некорректный токен"),
                "07" => throw new Exception("Опрос уже пройден"),
                _ => throw new Exception("Неизвестный код ответа"),
            };
        }

        public Dictionary<string, string[]> GetMatchesROs(string enteredAddress)
        {
            var splitAddress = enteredAddress.Split(",").Select(x => x.Trim()).ToList();
        
            var request = new RealityListRequest
            {
                PlaceName = splitAddress[0],
                StreetName = splitAddress[1],
                House = splitAddress[2]
            };
        
            var responce = _serviceClient.GetRealityListAsync(request, GetToken()).Result;

            return responce.RequestResult.Code switch
            {
                "00" => ParseFoundROsResponse(responce.RealityAddreses),
                "01" => throw new Exception("Нет активных опросов"),
                "02" => throw new Exception("Некорректный токен"),
                "07" => throw new Exception("Опрос уже пройден"),
                _ => throw new Exception("Неизвестный код ответа"),
            };
        }

        public async void SendPassedQuiz(Quiz passedQuiz)
        {
            var quizProxy = ParseQuizToProxy(passedQuiz);

            await _serviceClient.CreateSurveyResultAsync(quizProxy, GetToken());
        }

        private static string GetToken()
        {
            //переводим строку в байт-массим  
            byte[] bytes = Encoding.ASCII.GetBytes(HashBase);

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

        private static List<Quiz> ParseSurveyResponse(OnlineSurveyProxy[] rawQuizzes)
        {
            var quizzes = rawQuizzes.Select(x => new Quiz
            {
                Id = x.Id,
                Name = x.Name,
                IntroText = x.IntroText,
                OutroText = x.OuttroText,
                Passed = false,
                Questions = x.Questions.Select(y => new Question
                {
                    Id = y.Id,
                    Name = y.Question,
                    QuestionType = y.IsPool 
                                        ? QuestionType.IsAnswerId
                                        : y.IsAddress  
                                            ? QuestionType.IsROId
                                            : y.IsFIO 
                                                ? QuestionType.IsFIO
                                                : QuestionType.NotSet,
                    Answers = y.Answers?.Select(x => new Answer
                    {
                        Id = x.Id,
                        Name = x.Answer
                    })
                    .ToList()
                })
                .ToList()
            })
            .ToList();

            return quizzes;
        }

        private static Dictionary<string, string[]> ParseFoundROsResponse(RealityAddres[] rawROs)
        {
           return rawROs.ToDictionary(x => x.Id, x => new string[] { x.Addres, x.UK });
        }

        private static OnlineSurveyProxy ParseQuizToProxy(Quiz passedQuiz)
        {
            var quizProxy = new OnlineSurveyProxy
            {
                Id = passedQuiz.Id,
                FIO = passedQuiz.Questions.Where(x => x.QuestionType == QuestionType.IsFIO).First().Answer,
                Questions = passedQuiz.Questions.Select(x => new OnlineSurveyQuestionProxy
                {
                    Id = x.Id,
                    AnswerId = x.Answer,
                    IsPool = x.QuestionType == QuestionType.IsAnswerId,
                    IsFIO = x.QuestionType == QuestionType.IsFIO,
                    IsAddress = x.QuestionType == QuestionType.IsROId,
                })
                .ToArray()
            };

            return quizProxy;
        }
    }
}
