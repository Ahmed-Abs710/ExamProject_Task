using ExamProject_Task.Repository;
using ExamProject_Task.Repository.Admin;
using ExamProject_Task.Repository.Dto;
using ExamProject_Task.Repository.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamProject_Task.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AdminController(IAdminService adminService, ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _adminService = adminService;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View("~/Pages/Shared/AdminLayout.cshtml");
        }

        public IActionResult Users()
        {
            return View("~/Pages/Views/Admin/Users.cshtml");
        }

        public IActionResult Exams()
        {
            return View("~/Pages/Views/Admin/Exams.cshtml");
        }

        public IActionResult ManageExams()
        {
            return View("~/Pages/Views/Admin/ManageExams.cshtml");
        }

        public IActionResult Login()
        {
            return View("~/Pages/Views/Admin/AdminLogin.cshtml");
        }

        //  إدارة المستخدمين

        [HttpPost("users")]
        public async Task<ActionResult<string>> AddUser(string UserName, string Password)
        {
          string res = await _adminService.AddUser(UserName, Password);
            return Ok(res);
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return Ok(await _adminService.GetAllUsersAsync());
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromQuery]int id)
        {
            await _adminService.DeleteUserAsync(id);
            return NoContent();
        }

        //  إدارة الامتحانات
        [HttpGet("exams")]
        public async Task<ActionResult<IEnumerable<Exam>>> GetAllExams()
        {
            return Ok(await _adminService.GetAllExamsAsync());
        }

        [HttpGet("exams/{id}")]
        public async Task<ActionResult<Exam>> GetExamById(int id)
        {
            var exam = await _adminService.GetExamByIdAsync(id);
            if (exam == null) return NotFound();
            return Ok(exam);
        }

        [HttpPost("exams")]
        public async Task<IActionResult> AddExam([FromBody] Exam exam)
        {
            await _adminService.AddExamAsync(exam);
            return CreatedAtAction(nameof(GetExamById), new { id = exam.Id }, exam);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExam([FromQuery]int id, [FromBody] Exam exam)
        {
           // if (id != exam.Id) return BadRequest();
            await _adminService.UpdateExamAsync(exam);
            return Ok("تم التعديل");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteExam([FromQuery]int id)
        {
            await _adminService.DeleteExamAsync(id);
            return NoContent();
        }

        //  إدارة الأسئلة
        [HttpGet("questions/{examId}")]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsByExamId(int examId)
        {
            return Ok(await _adminService.GetQuestionsByExamIdAsync(examId));
        }

        [HttpPost("questions")]
        public async Task<IActionResult> AddQuestion([FromBody] Question question)
        {
            await _adminService.AddQuestionAsync(question);
            return Created("", question);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuestion([FromBody] Question question)
        {
            //if (id != question.Id) return BadRequest();
            await _adminService.UpdateQuestionAsync(question);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteQuestion([FromQuery] int id)
        {
            await _adminService.DeleteQuestionAsync(id);
            return NoContent();
        }

        //  إدارة الاختيارات
        [HttpGet("choices/{questionId}")]
        public async Task<ActionResult<IEnumerable<Choice>>> GetChoicesByQuestionId(int questionId)
        {
            return Ok(await _adminService.GetChoicesByQuestionIdAsync(questionId));
        }

        [HttpPost("choices")]
        public async Task<IActionResult> AddChoice([FromBody] Choice choice)
        {
            await _adminService.AddChoiceAsync(choice);
            return Created("", choice);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChoice([FromBody] Choice choice)
        {
            //if (id != choice.Id) return BadRequest();
            await _adminService.UpdateChoiceAsync(choice);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChoice([FromQuery]int id)
        {
            await _adminService.DeleteChoiceAsync(id);
            return NoContent();
        }

        [HttpPost]
        [Route("api/exams/save")]
        public async Task<IActionResult> SaveExam([FromBody] ExamViewModel model)
        {
            if (model == null || model.Questions == null || model.Questions.Count == 0)
            {
                return BadRequest("البيانات غير صحيحة!");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1️⃣ حفظ الامتحان في جدول Exams
                    var exam = new Exam
                    {
                        Title = model.Name
                    };

                    _context.Exams.Add(exam);
                    await _context.SaveChangesAsync();

                    // 2️⃣ حفظ الأسئلة وربطها بالامتحان
                    foreach (var q in model.Questions)
                    {
                        var question = new Question
                        {
                            ExamId = exam.Id, // ربط السؤال بالامتحان
                            Title = q.Question
                        };

                        _context.Questions.Add(question);
                        await _context.SaveChangesAsync();

                        //  حفظ الاختيارات وربطها بالسؤال
                        for (int i = 0; i < q.Choices.Count; i++)
                        {
                            var choice = new Choice
                            {
                                QuestionId = question.Id,
                                Text = q.Choices[i],
                                IsCorrect = (i == q.CorrectAnswer) // يحدد الاختيار الصحيح
                            };

                            _context.Choices.Add(choice);
                        }
                        await _context.SaveChangesAsync();
                    }

                    //  حفظ كل البيانات في قاعدة البيانات
                    await transaction.CommitAsync();
                    return Ok("تم حفظ الامتحان بنجاح!");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"حدث خطأ: {ex.Message}");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExamData()
        {
                var exams = _context.Exams
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
                    }).ToList();

                return Json(exams);
        }



        [HttpPost]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login attempt");

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "User";
            if (result.Succeeded)
            {
                return Ok(new { message = "Login successful", role = userRole });
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }
    }


}

