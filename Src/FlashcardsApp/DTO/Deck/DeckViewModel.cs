using FlashcardsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.ViewModels
{
    public class DeckViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }

        public List<Flashcard>? Flashcards { get; set; }
    }
}
