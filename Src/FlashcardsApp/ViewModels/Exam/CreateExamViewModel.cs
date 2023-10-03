using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlashcardsApp.ViewModels
{
    public class CreateExamViewModel
    {
        public string? DeckId { get; set; }
        public string QuestionsNumber { get; set; } = null!;
        public SelectList? Decks { get; set; }
        public SelectList? FlashcardsCount { get; set; }
    }
}
