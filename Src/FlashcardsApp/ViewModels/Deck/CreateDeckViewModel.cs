namespace FlashcardsApp.ViewModels
{
    public class CreateDeckViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public IFormFile? ImageToUpload { get; set; }

    }
}
