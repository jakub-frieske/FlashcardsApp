using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashcardsApp.Models
{
    public class Flashcard
    {
        [Key]
        public int Id { get; set; }
        public string Term { get; set; } = null!;
        [MaxLength(150)]
        public string Definition { get; set; } = null!;

        [ForeignKey("Decks")]
        public int DeckId { get; set; }
        public Deck? Deck { get; set; }
    }
}
