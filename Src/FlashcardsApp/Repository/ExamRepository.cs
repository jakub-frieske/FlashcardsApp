using FlashcardsApp.Context;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FlashcardsApp.Repository
{
    public class ExamRepository : IExamRepository
    {
        private readonly FlashcardAppDbContext _context;

        public ExamRepository(FlashcardAppDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Exam>> GetAllAsync()
        {
            return await _context.Exams.OrderByDescending(e => e.Solved).ToListAsync();
        }

        public async Task<Exam> GetByIdAsync(int id)
        {
            var exam = await _context.Exams.Include(x => x.Questions).FirstOrDefaultAsync(x => x.Id == id);
            return exam ?? throw new ArgumentException($"Exam with id: {id} not found");
        }

        public Exam GetById(int id)
        {
            var exam = _context.Exams.Where(x => x.Id == id).FirstOrDefault();
            return exam ?? throw new ArgumentException($"Exam with id: {id} not found");

        }

        public Exam CreateExam(int deckId, int questionsNumber)
        {
            var deck = _context.Decks.Include(x => x.Flashcards).Where(d => d.Id == deckId).FirstOrDefault();
            var flashcards = deck?.Flashcards?.OrderBy(r => Guid.NewGuid()).Take(questionsNumber).ToList();

            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            var listQuestions = (from flashcard in flashcards
                                 select new Question()
                                 {
                                     Flashcard = flashcard
                                 }).ToList();

            Exam newExam = new()
            {
                Title = "Exam " + formattedDateTime,
                Questions = listQuestions
            };

            return newExam;
        }

        public Exam CalculateScore(Exam exam)
        {

            int points = 0;

            foreach (var item in exam.Questions)
            {
                if (item.UserAnswer != null && item.Flashcard != null && item.Flashcard.Definition != null)
                {
                    points += CheckQuestionAnswer(item.UserAnswer, item.Flashcard.Definition) ? 1 : 0;
                }
            }

            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("dddd, dd MMMM yyyy HH:mm:ss");

            exam.Solved = formattedDateTime;
            exam.Score = $"{(points * 100) / exam.Questions.Count}% {points}/{exam.Questions.Count}";

            return exam;
        }

        private static bool CheckQuestionAnswer(string userAnswer, string correctAnswer)
        {
            return String.Equals(userAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }


        public bool Add(Exam exam)
        {
            _context.Add(exam);
            return Save();
        }

        public bool Delete(Exam exam)
        {
            _context.Remove(exam);
            return Save();
        }


        public bool Update(Exam exam)
        {
            if (!_context.Exams.Any(c => c.Id == exam.Id))
            {
                throw new ArgumentException($"Exam with id: {exam.Id} not found");
            }

            _context.Exams.Update(exam);
            return Save();
        }
    }
}
