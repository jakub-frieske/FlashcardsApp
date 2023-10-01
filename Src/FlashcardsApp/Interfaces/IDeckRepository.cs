using FlashcardsApp.Models;

namespace FlashcardsApp.Interfaces
{
    public interface IDeckRepository
    {
        Task<IEnumerable<Deck>> GetAllAsync();
        Task<IEnumerable<Deck>> GetAllWithFlashcardsAsync();
        IEnumerable<Deck> GetAll();
        Deck GetById(int id);
        Task<Deck> GetByIdAsync(int id);

        bool Add(Deck flashcard);
        bool Update(Deck flashcard);
        bool Delete(Deck flashcard);
        bool Save();
    }
}
