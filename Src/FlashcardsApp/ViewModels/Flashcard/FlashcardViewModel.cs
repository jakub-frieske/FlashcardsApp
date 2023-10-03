namespace FlashcardsApp.ViewModels
{
    public class FlashcardViewModel
    {
        public int Id { get; set; }
        public string Term { get; set; } = null!;
        public string Definition { get; set; } = null!;
        public int DeckId { get; set; }

    }
}