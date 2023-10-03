using FlashcardsApp.Context;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Repository
{
    public class DeckRepository : IDeckRepository
    {
        private readonly FlashcardAppDbContext _context;

        public DeckRepository(FlashcardAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Deck>> GetAllAsync()
        {
            return await _context.Decks.ToListAsync();
        }

        public async Task<IEnumerable<Deck>> GetAllWithFlashcardsAsync()
        {
            return await _context.Decks.Where(deck => deck.Flashcards != null && deck.Flashcards.Any()).Include(x => x.Flashcards).ToListAsync();
        }

        public IEnumerable<Deck> GetAll()
        {
            return _context.Decks.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }


        public bool Add(Deck deck)
        {
            _context.Add(deck);
            return Save();
        }

        public bool Delete(Deck deck)
        {
            _context.Remove(deck);

            var flashcard = deck.Flashcards;
            if (flashcard != null)
                foreach (var card in flashcard)
                {
                    _context.Remove(card);
                }

            return Save();
        }


        public bool Update(Deck deck)
        {
            if (!_context.Decks.Any(c => c.Id == deck.Id))
            {
                throw new ArgumentException($"Deck with id: {deck.Id} not found");
            }

            _context.Decks.Update(deck);
            return Save();
        }


        public async Task<Deck> GetByIdAsync(int id)
        {
            var deck = await _context.Decks.Include(x => x.Flashcards).FirstOrDefaultAsync(x => x.Id == id);

            return deck ?? throw new ArgumentException($"Deck with id: {id} not found");
        }

        public Deck GetById(int id)
        {
            var deck = _context.Decks.Include(x => x.Flashcards).FirstOrDefault(x => x.Id == id);
            return deck ?? throw new ArgumentException($"Deck with id: {id} not found");
        }
    }
}
