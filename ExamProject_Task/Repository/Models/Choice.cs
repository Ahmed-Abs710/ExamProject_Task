namespace ExamProject_Task.Repository.Models
{
    public class Choice
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; } // لتحديد الاختيار الصحيح
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
