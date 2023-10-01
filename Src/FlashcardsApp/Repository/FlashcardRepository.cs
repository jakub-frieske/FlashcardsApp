using FlashcardsApp.Context;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Repository
{
    public class FlashcardRepository : IFlashcardRepository
    {
        private readonly FlashcardAppDbContext _context;

        public FlashcardRepository(FlashcardAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Flashcard>> GetAllAsync()
        {
            return await _context.Flashcards.ToListAsync();
        }
      
        public async Task<Deck> GetDeckbyIdAsync(int id)
        {
            return await _context.Decks.Include(x => x.Flashcards).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Flashcard> GetByIdAsync(int id)
        {
            return await _context.Flashcards.FindAsync(id);
        }

        public async Task<IEnumerable<Flashcard>> GetFlashcardByDeckAsync(string deck)
        {
            return await _context.Flashcards.Where(d => d.Deck.Title.Contains(deck)).ToListAsync();
        }


        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }


        public bool Add(Flashcard flashcard)
        {
            _context.Add(flashcard);
            return Save();
        }

        public bool Delete(Flashcard flashcard)
        {
            _context.Remove(flashcard);
            return Save();
        }


        public bool Update(Flashcard flashcard)
        {
            if (!_context.Flashcards.Any(c => c.Id == flashcard.Id))
            {
                throw new ArgumentException($"Flashcard with id: {flashcard.Id} not found");
            }

            _context.Flashcards.Update(flashcard);
            return Save();
        }
    }
}
