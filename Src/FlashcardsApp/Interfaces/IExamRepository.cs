using FlashcardsApp.Models;

namespace FlashcardsApp.Interfaces
{
    public interface IExamRepository
    {
        Task<ICollection<Exam>> GetAllAsync();
        Task<Exam> GetByIdAsync(int id);
        Exam CreateExam(int deckId, int questionsNumber);
        Exam CalculateScore(Exam exam);

        bool Add(Exam exam);
        bool Update(Exam exam);
        bool Delete(Exam exam);
        bool Save();
    }
}
