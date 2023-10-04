using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Context
{
    public class FlashcardAppDbContext : DbContext
    {
        public DbSet<Deck> Decks { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }

        public FlashcardAppDbContext(DbContextOptions<FlashcardAppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Deck>()
                .HasMany(i => i.Flashcards)
                .WithOne(c => c.Deck)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
