using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashcardsApp.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string? UserAnswer { get; set; }

        public Flashcard? Flashcard { get; set; }

    }
}
