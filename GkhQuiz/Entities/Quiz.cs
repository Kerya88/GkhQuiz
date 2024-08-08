namespace GkhQuiz.Entities
{
    public class Quiz
    {
        //id опроса
        public string Id { get; set; }
        //название опроса
        public string Name { get; set; }
        //текст приветствия
        public string IntroText { get; set; }
        //текст прощания
        public string OutroText { get; set; }
        //Email опрвшиваемого
        public string Email { get; set; }
        //список вопросов
        public List<Question> Questions { get; set; }
        //тест пройден
        public bool Passed { get; set; }
    }
}
