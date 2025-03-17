using Microsoft.AspNetCore.Identity;

namespace ExamProject_Task.Repository.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int ChoiceId { get; set; }
        public Choice Choice { get; set; }
    }
}
