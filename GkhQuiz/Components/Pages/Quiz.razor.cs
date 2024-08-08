using GkhQuiz.Entities;
using GkhQuiz.Enums;
using GkhQuiz.Services;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.RegularExpressions;

namespace GkhQuiz.Components.Pages
{
    public class QuizScript : ComponentBase
    {
        readonly Regex _fioRegex = new(@"^[А-ЯЁ]{1}[а-яё]{2,}\s[А-ЯЁ]{1}[а-яё]{2,}\s[А-ЯЁ]{1}[а-яё]{2,}$");
        readonly Regex _roRegex = new(@"^[А-ЯЁа-яё\s]+,{1}\s*[А-ЯЁа-яё\s]+,{1}\s*\d+$");

        [Inject]
        IQuizService QuizService { get; set; }

        List<Entities.Quiz> _quizzes;
        int _stage;

        [Parameter]
        public Entities.Quiz Quiz { get; set; }
        [Parameter]
        public Question Question { get; set; }
        [Parameter]
        public Dictionary<string, string[]> FoundedROs { get; set; }
        [Parameter]
        public string SelectedAnswer { get; set; }
        [Parameter]
        public string SelectedRO { get; set; }
        [Parameter]
        public string ManagingOrganization { get; set; }
        [Parameter]
        public bool SwitchButtonsToFinish { get; set; }
        [Parameter]
        public bool TestStarted { get; set; }
        [Parameter]
        public bool TestFinished { get; set; }
        [Parameter]
        public bool ExceptionState { get; set; }
        [Parameter]
        public bool HasNextQuiz { get; set; }
        [Parameter]
        public bool NotFoundROs { get; set; }
        [Parameter]
        public bool FoundROs { get; set; }
        [Parameter]
        public bool SetManagingOrganization { get; set; }
        [Parameter]
        public string ExceptionMessage { get; set; }

        protected override void OnInitialized()
        {
            try
            {
                _quizzes = QuizService.GetNowQuizzes();
                SetDataOnPage(_quizzes[0]);
            }
            catch (Exception ex) 
            {
                SetExceptionOnPage(ex);
            }
        }

        public void StartQuiz()
        {
            TestStarted = true;
        }

        public void NextQuestion()
        {
            if (string.IsNullOrEmpty(SelectedRO))
            {
                var valid = ValidateAnswer();

                if (valid)
                {
                    Question.Answer = SelectedAnswer;

                    UpdateDataOnPage();
                }
            }
            else
            {
                Question.Answer = SelectedRO;
                ManagingOrganization = string.IsNullOrEmpty(FoundedROs[SelectedRO][1]) 
                    ? $"На сайте Электронного ЖКХ не найдена информация об управляющей компании по адресу {FoundedROs[SelectedRO][0]}"
                    : $"Ваша управляющая компания - {FoundedROs[SelectedRO][1]}";
                SetManagingOrganization = true;
                SelectedRO = string.Empty;

                UpdateDataOnPage();
            }
        }

        public void SearchRO()
        {
            var valid = ValidateAnswer();

            if (valid)
            {
                try
                {
                    var foundedROs = QuizService.GetMatchesROs(SelectedAnswer);

                    if (foundedROs.Count > 0)
                    {
                        NotFoundROs = false;
                        FoundROs = true;
                        FoundedROs = foundedROs;
                    }
                    else
                    {
                        NotFoundROs = true;
                        FoundROs = false;
                    }
                }
                catch (Exception ex)
                {
                    SetExceptionOnPage(ex);
                }
            }
        }

        public void FinishQuiz()
        {
            Question.Answer = SelectedAnswer;

            ManagingOrganization = string.Empty;
            SelectedAnswer = string.Empty;
            TestFinished = true;
            Quiz.Passed = true;
            HasNextQuiz = _quizzes.Where(x => !x.Passed).Any();

            QuizService.SendPassedQuiz(Quiz);
        }

        public void NextQuiz()
        {
            var quiz = _quizzes.Where(x => !x.Passed).First();
            SetDataOnPage(quiz);
        }

        private void SetExceptionOnPage(Exception ex)
        {
            ExceptionState = true;

            ExceptionMessage = ex.Message;
        }

        private void SetDataOnPage(Entities.Quiz quiz)
        {
            Quiz = quiz;

            _stage = 0;

            SwitchButtonsToFinish = false;
            TestStarted = false;
            TestFinished = false;
            ExceptionState = false;
            HasNextQuiz = false;
            NotFoundROs = false;
            FoundROs = false;
            SetManagingOrganization = false;

            Question = Quiz.Questions[_stage];
        }

        private void UpdateDataOnPage()
        {
            _stage++;

            Question = Quiz.Questions[_stage];

            SelectedAnswer = string.Empty;

            if (_stage == Quiz.Questions.Count - 1)
            {
                SwitchButtonsToFinish = true;
            }
        }

        private bool ValidateAnswer()
        {
            if (!string.IsNullOrEmpty(SelectedAnswer))
            {
                switch (Question.QuestionType)
                {
                    case QuestionType.IsFIO:
                        {
                            return _fioRegex.IsMatch(SelectedAnswer);
                        }
                    case QuestionType.IsROId:
                        {
                            return _roRegex.IsMatch(SelectedAnswer);
                        }
                    default:
                        {
                            return true;
                        }
                }
            }
            else
            {
                return false;
            }
        }

        
    }
}