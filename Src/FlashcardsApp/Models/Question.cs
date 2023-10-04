using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string? UserAnswer { get; set; }

        public Flashcard Flashcard { get; set; } = null!;

    }
}
