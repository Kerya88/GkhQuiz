using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace GkhQuiz.Components.Pages
{
    public class QuizScript : ComponentBase
    {
        private static readonly List<string> QUESTIONS = ["Оцените работу вашей УК", "Посоветовали бы Вашу УК жителям других МКД?", "Хотели бы Вы сменить вашу УК?", "Хотели бы вы сменить способ управления?"];
        private static readonly List<List<string>> ANSWERS =
        [
            [" 0 баллов", " 1 балл", " 2 балла", " 3 балла", " 4 балла", " 5 баллов"],
            [" Категорически не посоветовал бы", " Скорее не посоветовал бы", " Затрудняюсь ответить", " Доволен УК, но советовать не буду", " Скорее посоветовал бы", " Точно посоветовал бы"],
            [" Очень хотел бы", " Скорее хотел бы", " Затрудняюсь ответить", " Не уверен, что новая УК будет лучше", " Скорее не хотел бы", " Точно не хотел бы"],
            [" Очень хотел бы", " Скорее хотел бы", " Затрудняюсь ответить", " Не уверен, что иной способ лучше", " Скорее не хотел бы", " Точно не хотел бы"]
        ];

        private ElementReference _nextBtn;

        [Parameter]
        public string Question { get; set; }
        [Parameter]
        public List<string> Answers { get; set; }
        [Parameter]
        public string SelectedAnswer { get; set; }
        [Parameter]
        public bool SwitchButtonsToFinish { get; set; }
        [Parameter]
        public bool TestFinished { get; set; }

        private List<string> _selectedAnswers;
        private int _stage;

        protected override void OnInitialized()
        {
            Question = QUESTIONS[0];
            Answers = ANSWERS[0];

            _selectedAnswers = [];
            _stage = 1;
        }

        public void NextQuestion()
        {
            if (!string.IsNullOrEmpty(SelectedAnswer))
            {
                Question = QUESTIONS[_stage];
                Answers = ANSWERS[_stage];

                _selectedAnswers.Add(SelectedAnswer);
                SelectedAnswer = string.Empty;

                _stage++;

                if (_stage == 4)
                {
                    SwitchButtonsToFinish = true;
                }
            }
        }

        public void FinishQuiz()
        {
            _selectedAnswers.Add(SelectedAnswer);
            SelectedAnswer = string.Empty;
            TestFinished = true;
        }
    }
}
