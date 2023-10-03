using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Score { get; set; }
        public string? Solved { get; set; }

        public ICollection<Question> Questions { get; set; } = null!;
    }
}
