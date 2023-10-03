using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FlashcardsApp.Controllers
{
    public class FlashcardController : Controller
    {
        private readonly ILogger<FlashcardController> _logger;
        private readonly IFlashcardRepository _repository;

        public FlashcardController(
            ILogger<FlashcardController> logger,
            IFlashcardRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Displays the flashcard creation form.
        /// </summary>
        /// <param name="deckId">The ID of the deck to which the flashcard belongs.</param>
        /// <returns>The partial view for creating a flashcard.</returns>
        [HttpGet]
        public async Task<IActionResult> Create(int deckId)
        {

            var deck = await _repository.GetDeckbyIdAsync(deckId);
            if (deck == null) return View("Error");

            var newFlashcard = new FlashcardViewModel
            {
                Term = "",
                Definition = "",
                DeckId = deck.Id
            };

            return PartialView("_Create", newFlashcard);
        }


        /// <summary>
        /// Handles the submission of a new flashcard.
        /// </summary>
        /// <param name="deckId">The ID of the deck to which the flashcard belongs.</param>
        /// <param name="newFlashcard">The view model representing the new flashcard.</param>
        /// <returns>Redirects to the deck detail page.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(int deckId, FlashcardViewModel newFlashcard)
        {
            _logger.LogDebug("Create flashcard");

            var deck = await _repository.GetDeckbyIdAsync(deckId);
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", newFlashcard);
            }
            var flashcard = new Flashcard
            {
                Term = newFlashcard.Term,
                Definition = newFlashcard.Definition,
                DeckId = deck.Id,
                Deck = deck
            };
            _repository.Add(flashcard);
            var redirectUrl = Url.Action("Detail", "Deck", new { id = deckId });

            //return RedirectToAction("Detail","Deck", new { id = deckId });
            return Json(new { redirectUrl });

        }

        /// <summary>
        /// Displays the flashcard edit form.
        /// </summary>
        /// <param name="id">The ID of the flashcard to edit.</param>
        /// <returns>The partial view for editing a flashcard.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var card = await _repository.GetByIdAsync(id);
            if (card == null) return View("Error");


            var cardVM = new FlashcardViewModel
            {
                Term = card.Term,
                Definition = card.Definition,
                DeckId = card.DeckId
            };

            return PartialView("_Edit", cardVM);

        }

        /// <summary>
        /// Handles the submission of an edited flashcard.
        /// </summary>
        /// <param name="id">The ID of the flashcard to edit.</param>
        /// <param name="cardVM">The view model representing the edited flashcard.</param>
        /// <returns>Redirects to the deck detail page.</returns>
        [HttpPost]
        public IActionResult Edit(int id, FlashcardViewModel cardVM)
        {
            _logger.LogDebug("Update flashcard");
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit deck");
                return PartialView("_Edit", cardVM);
            }

            var newFlashcard = new Flashcard
            {
                Id = id,
                Term = cardVM.Term,
                Definition = cardVM.Definition,
                DeckId = cardVM.DeckId,
            };
            _repository.Update(newFlashcard);


            return Json(new { redirectUrl = Url.Action("Detail", "Deck", new { id = cardVM.DeckId }) });
            // return RedirectToAction("Detail", "Deck", new { id = card.DeckId });
        }

        /// <summary>
        /// Handles the deletion of a flashcard.
        /// </summary>
        /// <param name="id">The ID of the flashcard to delete.</param>
        /// <returns>Redirects to the deck detail page.</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogDebug("Delete flashcard");
            var card = await _repository.GetByIdAsync(id);
            if (card == null) return View("Error");

            _repository.Delete(card);

            return RedirectToAction("Detail", "Deck", new { id = card.DeckId });
        }

        /// <summary>
        /// Handles errors and displays an error view.
        /// </summary>
        /// <returns>The error view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}