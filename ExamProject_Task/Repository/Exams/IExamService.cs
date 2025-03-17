using ExamProject_Task.Repository.Models;

namespace ExamProject_Task.Repository.Exams
{
    public interface IExamService
    {
        Task<IEnumerable<Exam>> GetAllExamsAsync();
    }
}
