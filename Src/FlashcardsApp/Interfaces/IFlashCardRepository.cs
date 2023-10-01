using FlashcardsApp.Models;

namespace FlashcardsApp.Interfaces
{
    public interface IFlashcardRepository
    {
        Task<IEnumerable<Flashcard>> GetAllAsync();
        Task<Flashcard> GetByIdAsync(int id);
        Task<Deck> GetDeckbyIdAsync(int id);
        Task<IEnumerable<Flashcard>> GetFlashcardByDeckAsync(string deck);

        bool Add(Flashcard flashcard);
        bool Update(Flashcard flashcard);
        bool Delete(Flashcard flashcard);
        bool Save();
    }
}
