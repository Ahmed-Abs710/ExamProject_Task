namespace ExamProject_Task.Repository.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
    }
}
