namespace ExamProject_Task.Repository.Dto
{
    public class ExamSubmissionDto
    {
        public int ExamId { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }
}
