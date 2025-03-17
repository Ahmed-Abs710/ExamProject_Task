namespace ExamProject_Task.Repository.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

}
