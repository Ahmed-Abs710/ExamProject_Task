using ExamProject_Task.Repository.Models;

namespace ExamProject_Task.Repository.Admin
{
    public interface IAdminService
    {
        // إدارة المستخدمين
        Task<string> AddUser(string UserName,string Password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task DeleteUserAsync(int userId);

        // إدارة الامتحانات
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<Exam> GetExamByIdAsync(int examId);
        Task AddExamAsync(Exam exam);
        Task UpdateExamAsync(Exam exam);
        Task DeleteExamAsync(int examId);

        // إدارة الأسئلة
        Task<IEnumerable<Question>> GetQuestionsByExamIdAsync(int examId);
        Task AddQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(int questionId);

        // إدارة الاختيارات
        Task<IEnumerable<Choice>> GetChoicesByQuestionIdAsync(int questionId);
        Task AddChoiceAsync(Choice choice);
        Task UpdateChoiceAsync(Choice choice);
        Task DeleteChoiceAsync(int choiceId);
    }
}
