using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsApp.Repository;
using FlashcardsApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace FlashcardsApp.Controllers
{
    public class DeckController : Controller
    {
        private readonly ILogger<DeckController> _logger;
        private readonly IDeckRepository _repository;
        private readonly IPhotoService _photoService;
        private readonly IFlashcardRepository _flashcardRepository;

        public DeckController(
            ILogger<DeckController> logger,
            IDeckRepository repository,
            IPhotoService photoService,
            IFlashcardRepository flashcardRepository)
        {
            _logger = logger;
            _repository = repository;
            _photoService = photoService;
            _flashcardRepository = flashcardRepository;
        }

        /// <summary>
        /// Displays a list of decks.
        /// </summary>
        /// <returns>The view with a list of decks.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var decks = await _repository.GetAllAsync();
            var decksWithFlashcards = await _repository.GetAllWithFlashcardsAsync();
            ViewData["ActivePage"] = ActivePage();
            ViewData["MayTakeExam"] = decksWithFlashcards.Count() > 0 ? "true" : "false";
            return View(decks);
        }

        /// <summary>
        /// Displays the create deck form.
        /// </summary>
        /// <returns>The partial view for creating a deck.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            var deckVM = new CreateDeckViewModel();
            return PartialView("_Create", deckVM);
        }

        /// <summary>
        /// Creates a new deck.
        /// </summary>
        /// <param name="deckVM">The view model for creating a deck.</param>
        /// <returns>
        /// JSON object with a redirect URL to the index page if successful,
        /// or a partial view with validation errors if unsuccessful.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateDeckViewModel deckVM)
        {
            _logger.LogDebug("Create deck");

            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(deckVM.ImageToUpload);
                var image = deckVM.ImageToUpload != null ? result.Url.ToString() : "../images/bg.png";

                var deck = new Deck
                {
                    Title = deckVM.Title,
                    Description = deckVM.Description,
                    Image = image,
                   
                };

                _repository.Add(deck);
                //return RedirectToAction("Index");
                return Json(new { redirectUrl = Url.Action("Index") });

            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return PartialView("_Create", deckVM);

        }

        /// <summary>
        /// Displays the details of a deck.
        /// </summary>
        /// <param name="id">The ID of the deck to display.</param>
        /// <returns>The view with deck details.</returns>
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Deck deck = await _repository.GetByIdAsync(id);
            if (deck == null) return View("Error");

            var allFlashcards = deck.Flashcards.ToList();


            var deckVM = new EditDeckViewModel
            {
                Id = deck.Id,
                Title = deck.Title,
                Description = deck.Description,
                Flashcards = allFlashcards
            };
            var decksWithFlashcards = await _repository.GetAllWithFlashcardsAsync();
            ViewData["ActivePage"] = ActivePage();
            ViewData["MayTakeExam"] = decksWithFlashcards.Count() > 0 ? "true" : "false";
            return View(deckVM);
        }

        /// <summary>
        /// Displays the edit deck form.
        /// </summary>
        /// <param name="id">The ID of the deck to edit.</param>
        /// <returns>The partial view for editing a deck.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var deck = await _repository.GetByIdAsync(id);
            if (deck == null) return View("Error");

            var allFlashcards = deck.Flashcards.ToList();

           
            var deckVM = new EditDeckViewModel
            {
                Id = deck.Id,
                Title = deck.Title,
                Description = deck.Description,
                Flashcards= allFlashcards,
                Image = deck.Image
            };

            return PartialView("_Edit", deckVM);
        }

        /// <summary>
        /// Edits an existing deck.
        /// </summary>
        /// <param name="id">The ID of the deck to edit.</param>
        /// <param name="deckVM">The view model for editing a deck.</param>
        /// <returns>
        /// JSON object with a redirect URL to the index page if successful,
        /// or a partial view with validation errors if unsuccessful.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditDeckViewModel deckVM)
        {
            _logger.LogDebug("Edit deck");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit deck");
                return PartialView("_Edit", deckVM);  
            }

            var newDeck = new Deck
            {
                Id = id,
                Title = deckVM.Title,
                Description = deckVM.Description,
                Image = deckVM.Image
            };
            _repository.Update(newDeck);

            var flashcard= deckVM.Flashcards;
            if (flashcard != null)
                foreach (var card in flashcard)
                    _flashcardRepository.Update(card);

            return Json(new { redirectUrl = Url.Action("Index") });
        }

        /// <summary>
        /// Displays the delete deck confirmation page.
        /// </summary>
        /// <param name="id">The ID of the deck to delete.</param>
        /// <returns>The partial view for confirming deck deletion.</returns>

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var deck = await _repository.GetByIdAsync(id);
            if (deck == null) return View("Error");

            var allFlashcards = deck.Flashcards.ToList();


            var deckVM = new EditDeckViewModel
            {
                Title = deck.Title,
                Description = deck.Description,
                Flashcards = allFlashcards
            };


            return PartialView("_Delete", deckVM); 
        }

        /// <summary>
        /// Deletes a deck.
        /// </summary>
        /// <param name="id">The ID of the deck to delete.</param>
        /// <param name="deckVM">The view model for confirming deck deletion.</param>
        /// <returns>
        /// JSON object with a redirect URL to the index page if successful,
        /// or a view for displaying an error if unsuccessful.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id, EditDeckViewModel deckVM)
        {
            _logger.LogDebug("Delete deck");

            var deck = await _repository.GetByIdAsync(id);

            if (deck == null)
            {
                return View("Error");
            }

            if (!string.IsNullOrEmpty(deck.Image))
            {
                _ = _photoService.DeletePhotoAsync(deck.Image);
            }

            _repository.Delete(deck);

            return Json(new { redirectUrl = Url.Action("Index") });
        }

        private string ActivePage()
        {
            string actionName = this.ControllerContext
                .RouteData
                .Values["action"]
                .ToString();

            string controllerName = this.ControllerContext
                .RouteData
                .Values["controller"]
                .ToString();

            return string.Format($"{controllerName}-{actionName}");
        }
    }
}
