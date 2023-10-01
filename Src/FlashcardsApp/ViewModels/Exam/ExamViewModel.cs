using FlashcardsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.ViewModels
{
    public class ExamViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Score { get; set; }
        public string? Solved { get; set; }

        [BindProperty]
        public List<Question> Questions { get; set; }
    }
}
