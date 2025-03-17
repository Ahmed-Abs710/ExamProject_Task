using ExamProject_Task.Repository.DataAccess;
using ExamProject_Task.Repository.Models;

namespace ExamProject_Task.Repository.Exams
{
    public class ExamService : IExamService
    {
        private readonly IRepository<Exam> _examRepository;
        public ExamService(IRepository<Exam> examRepository)
        {
            _examRepository = examRepository;
        }
        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _examRepository.GetAllAsync();
        }
    }
}
