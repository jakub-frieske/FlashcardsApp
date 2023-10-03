using FlashcardsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.ViewModels
{
    public class ExamHistoryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Score { get; set; }
        public string? Solved { get; set; }
    }
}
