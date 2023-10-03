using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models
{
    public class Deck
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        [MaxLength(200)]
        public string? Description { get; set; }
        public string? Image { get; set; }

        public ICollection<Flashcard>? Flashcards { get; set; }
    }
}
