using Microsoft.AspNetCore.Identity;

namespace ExamProject_Task.Repository.Models
{
    public class User : IdentityUser
    {
        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
