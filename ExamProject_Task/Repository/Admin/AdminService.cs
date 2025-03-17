using ExamProject_Task.Repository.DataAccess;
using ExamProject_Task.Repository.Dto;
using ExamProject_Task.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamProject_Task.Repository.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Exam> _examRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Choice> _choiceRepository;
        private readonly UserManager<User> _userManager;
        public AdminService(
            IRepository<User> userRepository,
            IRepository<Exam> examRepository,
            IRepository<Question> questionRepository,
            IRepository<Choice> choiceRepository,
            UserManager<User> userManager
            )
        {
            _userRepository = userRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _choiceRepository = choiceRepository;
            _userManager = userManager;
        }
        //  إدارة المستخدمين
        public async Task<string> AddUser(string UserName,string Password)
        {
            
                if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
                {
                    throw new ArgumentException("Invalid data.");
                }

                var user = new User
                {
                    UserName = UserName
                };
             var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                return "تم تسجيل المستخدم بنجاح";
            }
            else
            {
                return "كلمة المرور ضعيفة او حدث خطاء";
            }
            


        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteAsync(userId);
        }

        //  إدارة الامتحانات
        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _examRepository.GetAllAsync();
        }

        public async Task<Exam> GetExamByIdAsync(int examId)
        {
            return await _examRepository.GetByIdAsync(examId);
        }

        public async Task AddExamAsync(Exam exam)
        {
            await _examRepository.AddAsync(exam);
        }

        public async Task UpdateExamAsync(Exam exam)
        {
            await _examRepository.UpdateAsync(exam);
        }

        public async Task DeleteExamAsync(int examId)
        {
            await _examRepository.DeleteAsync(examId);
        }

        // إدارة الأسئلة
        public async Task<IEnumerable<Question>> GetQuestionsByExamIdAsync(int examId)
        {
            return await _questionRepository.GetAllAsync();
        }

        public async Task AddQuestionAsync(Question question)
        {
            await _questionRepository.AddAsync(question);
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            await _questionRepository.UpdateAsync(question);
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            await _questionRepository.DeleteAsync(questionId);
        }

        //  إدارة الاختيارات
        public async Task<IEnumerable<Choice>> GetChoicesByQuestionIdAsync(int questionId)
        {
            return await _choiceRepository.GetAllAsync();
        }

        public async Task AddChoiceAsync(Choice choice)
        {
            await _choiceRepository.AddAsync(choice);
        }

        public async Task UpdateChoiceAsync(Choice choice)
        {
            await _choiceRepository.UpdateAsync(choice);
        }

        public async Task DeleteChoiceAsync(int choiceId)
        {
            await _choiceRepository.DeleteAsync(choiceId);
        }


    



    }
}
