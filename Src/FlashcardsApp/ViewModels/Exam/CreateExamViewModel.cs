using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlashcardsApp.ViewModels
{
    public class CreateExamViewModel
    {
        public string deckId { get; set; }
        public string questionsNumber { get; set; }
        public SelectList? decks { get; set; }
        public SelectList? flashcardsCount { get; set; }
    }
}
