using Microsoft.AspNetCore.Identity;

namespace ExamProject_Task.Repository.Models
{
    public class UserExamResult
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
    }
}
