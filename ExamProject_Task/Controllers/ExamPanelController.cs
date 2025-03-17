using ExamProject_Task.Repository;
using ExamProject_Task.Repository.DataAccess;
using ExamProject_Task.Repository.Dto;
using ExamProject_Task.Repository.Exams;
using ExamProject_Task.Repository.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamProject_Task.Controllers
{
    public class ExamPanelController : Controller
    {

        public ExamPanelController(IExamService examService, ApplicationDbContext context)
        {
            ExamService = examService;
            _context = context;
        }

        public IExamService ExamService { get; }
        private readonly ApplicationDbContext _context;

        public IActionResult Index()
        {
            return View("~/Pages/Shared/ExamsLayout.cshtml");
        }
        public IActionResult DoExam()
        {
            return View("~/Pages/Views/Exams/DoExam.cshtml");
        }

        public IActionResult Login()
        {
            return View("~/Pages/Views/Exams/UserLogin.cshtml");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> GetAllExams()
        {
            return Ok(await ExamService.GetAllExamsAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetExamData(int examId)
        {
            var exam = _context.Exams
                .Where(e => e.Id == examId) // 🔥 البحث عن الامتحان المطلوب فقط
                .Select(e => new
                {
                    ExamId = e.Id,
                    ExamName = e.Title,
                    Questions = e.Questions.Select(q => new
                    {
                        QuestionId = q.Id,
                        QuestionText = q.Title,
                        Choices = q.Choices.Select(c => new
                        {
                            ChoiceId = c.Id,
                            ChoiceText = c.Text
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefault(); // 🔥 إرجاع نتيجة واحدة فقط

            if (exam == null)
                return NotFound("لم يتم العثور على الامتحان المطلوب"); // ❌ لو الامتحان غير موجود

            return Json(exam);
        }


        [HttpPost]
        public async Task<IActionResult> SubmitExam([FromBody] ExamSubmissionDto submission)
        {
            if (submission == null || submission.Answers == null || !submission.Answers.Any())
                return BadRequest("الإجابات غير صالحة!");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب ID المستخدم الحالي
            if (userId == null)
                return Unauthorized("يجب تسجيل الدخول!");

            int correctAnswers = 0;
            int totalQuestions = submission.Answers.Count;

            List<UserAnswer> userAnswers = new List<UserAnswer>();

            foreach (var answer in submission.Answers)
            {
                var correctChoice = await _context.Choices
                    .Where(c => c.QuestionId == answer.QuestionId && c.IsCorrect) // جلب الاختيار الصحيح
                    .FirstOrDefaultAsync();

                bool isCorrect = correctChoice != null && correctChoice.Id == answer.SelectedChoiceId;
                if (isCorrect)
                    correctAnswers++;

                userAnswers.Add(new UserAnswer
                {
                    UserId = userId,
                    QuestionId = answer.QuestionId,
                    ChoiceId = answer.SelectedChoiceId
                });
            }

            await _context.UserAnswers.AddRangeAsync(userAnswers);
            await _context.SaveChangesAsync();

            //  تطبيق المعادلة لحساب النتيجة النهائية 
            int score = (int)((correctAnswers / (double)totalQuestions) * 100);
            bool isPassed = score >= 60; //  النجاح عند 60% أو أكثر
            int wrongAnswers = totalQuestions - correctAnswers; // عدد الإجابات الخطأ

            // حفظ النتيجة في قاعدة البيانات
            var examResult = new UserExamResult
            {
                UserId = userId,
                ExamId = submission.ExamId,
                Score = score,
                Passed = isPassed
            };

            await _context.UserExamResults.AddAsync(examResult);
            await _context.SaveChangesAsync();

            //  إرجاع البيانات المطلوبة
            return Ok(new
            {
                Score = score,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                Passed = isPassed ? "ناجح" : "راسب"
            });
        }



        //[HttpPost]
        //public async Task<IActionResult> SubmitExam([FromBody] ExamSubmissionDto submission)

        //{
        //    if (submission == null || submission.Answers == null || !submission.Answers.Any())
        //        return BadRequest("الإجابات غير صالحة!");

        //    //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    //if (userId == null)
        //    //    return Unauthorized("يجب تسجيل الدخول!");

        //    int correctAnswers = 0;
        //    int totalQuestions = submission.Answers.Count;

        //    List<UserAnswer> userAnswers = new List<UserAnswer>();

        //    foreach (var answer in submission.Answers)
        //    {
        //        var correctChoice = await _context.Choices
        //            .Where(c => c.QuestionId == answer.QuestionId && c.IsCorrect) // جلب الاختيار الصحيح للسؤال
        //            .FirstOrDefaultAsync();

        //        bool isCorrect = correctChoice != null && correctChoice.Id == answer.SelectedChoiceId;
        //        if (isCorrect) correctAnswers++;

        //        userAnswers.Add(new UserAnswer
        //        {
        //            UserId = "ed7f9970-0c08-46b9-8a5b-5a5a1f826d8f",
        //            QuestionId = answer.QuestionId,
        //            ChoiceId = answer.SelectedChoiceId
        //        });
        //    }

        //    await _context.UserAnswers.AddRangeAsync(userAnswers);
        //    await _context.SaveChangesAsync();

        //    // حساب النتيجة
        //    int score = (int)((correctAnswers / (double)totalQuestions) * 100);
        //    bool isPassed = score >= 60;

        //    // حفظ النتيجة
        //    var examResult = new UserExamResult
        //    {
        //        UserId = "ed7f9970-0c08-46b9-8a5b-5a5a1f826d8f",
        //        ExamId = submission.ExamId,
        //        Score = score,
        //        Passed = isPassed
        //    };

        //    await _context.UserExamResults.AddAsync(examResult);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "تم تسجيل الامتحان!", score = score, passed = isPassed });
        //}



    }
}
