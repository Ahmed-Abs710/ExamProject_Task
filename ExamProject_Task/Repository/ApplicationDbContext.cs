using ExamProject_Task.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamProject_Task.Repository
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }

        // جداول الامتحانات والأسئلة والاختيارات
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }

        // جداول نتائج المستخدمين وإجاباتهم
        public DbSet<UserExamResult> UserExamResults { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // تعريف العلاقة بين الأسئلة والاختيارات (واحد لمتعدد)
            builder.Entity<Choice>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // تعريف العلاقة بين إجابات المستخدمين والأسئلة (واحد لمتعدد)
            builder.Entity<UserAnswer>()
                .HasOne(ua => ua.Question)
                .WithMany()
                .HasForeignKey(ua => ua.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            // تعريف العلاقة بين إجابات المستخدمين والاختيارات (واحد لمتعدد)
            builder.Entity<UserAnswer>()
                .HasOne(ua => ua.Choice)
                .WithMany()
                .HasForeignKey(ua => ua.ChoiceId)
                .OnDelete(DeleteBehavior.NoAction);

            // تعريف العلاقة بين نتائج الامتحانات والمستخدمين (واحد لمتعدد)
            builder.Entity<UserExamResult>()
                .HasOne(uer => uer.User)
                .WithMany()
                .HasForeignKey(uer => uer.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // تعريف العلاقة بين نتائج الامتحانات والامتحانات (واحد لمتعدد)
            builder.Entity<UserExamResult>()
                .HasOne(uer => uer.Exam)
                .WithMany()
                .HasForeignKey(uer => uer.ExamId)
                .OnDelete(DeleteBehavior.Cascade);


        }

    }
}
