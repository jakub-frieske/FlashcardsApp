using FlashcardsApp.Models;

namespace FlashcardsApp.Interfaces
{
    public interface IFlashcardRepository
    {
        Task<List<Flashcard>> GetAllAsync();
        Task<Flashcard> GetByIdAsync(int id);
        Task<Deck> GetDeckbyIdAsync(int id);

        bool Add(Flashcard flashcard);
        bool Update(Flashcard flashcard);
        bool Delete(Flashcard flashcard);
        bool Save();
    }
}
