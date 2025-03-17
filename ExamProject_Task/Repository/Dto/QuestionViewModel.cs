namespace ExamProject_Task.Repository.Dto
{
    public class QuestionViewModel
    {
        public string Question { get; set; }
        public List<string> Choices { get; set; }
        public int CorrectAnswer { get; set; }
    }
}
