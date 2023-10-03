using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } =null!;
    }
}
