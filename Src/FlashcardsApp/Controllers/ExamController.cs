using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsApp.Repository;
using FlashcardsApp.Services;
using FlashcardsApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace FlashcardsApp.Controllers
{
    public class ExamController : Controller
    {
        private readonly ILogger<ExamController> _logger;
        private readonly IExamRepository _repository;
        private readonly IDeckRepository _deckRepository;

        public ExamController(
            ILogger<ExamController> logger, 
            IExamRepository repository, 
            IDeckRepository deckRepository)
        {
            _logger = logger;
            _repository = repository;
            _deckRepository = deckRepository;
        }

        /// <summary>
        /// Displays the details of an exam.
        /// </summary>
        /// <param name="id">The ID of the exam to display.</param>
        /// <returns>The view with exam details.</returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var exam = await _repository.GetByIdAsync(id);
            if (exam == null)
            {
                return NotFound(); 
            }

            var examVM = new ExamViewModel()
            {
                Id = exam.Id,
                Title = exam.Title,
                Questions = exam.Questions.ToList(),

            };

            var decksWithFlashcards = await _deckRepository.GetAllWithFlashcardsAsync();
            ViewData["ActivePage"] = ActivePage();
            ViewData["MayTakeExam"] = decksWithFlashcards.Count() > 0 ? "true" : "false";
            return View(examVM);
        }

        /// <summary>
        /// Handles the submission of an exam.
        /// </summary>
        /// <param name="examVM">The view model representing the exam.</param>
        /// <returns>Redirects to the exam detail page.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(ExamViewModel examVM)
        {
            var exam = new Exam()
            {
                Id = examVM.Id,
                Title = examVM.Title,
                Questions = examVM.Questions
            };

            var result = _repository.CalculateScore(exam);
            _repository.Update(result);

            return RedirectToAction("Detail", new { id = result.Id } );
        }

        
        /// <summary>
        /// Displays a list of exam history.
        /// </summary>
        /// <returns>The view with a list of exams.</returns>
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var exams = await _repository.GetAllAsync();
            var decksWithFlashcards = await _deckRepository.GetAllWithFlashcardsAsync();
            ViewData["ActivePage"] = ActivePage();
            ViewData["MayTakeExam"] = decksWithFlashcards.Count() > 0 ? "true" : "false";
            return View(exams);
        }

        /// <summary>
        /// Displays the details of a specific exam.
        /// </summary>
        /// <param name="id">The ID of the exam to display.</param>
        /// <returns>The view with exam details.</returns>
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {

            Exam examVM = await _repository.GetByIdAsync(id);
            if (examVM == null) return View("Error");


            var decksWithFlashcards = await _deckRepository.GetAllWithFlashcardsAsync();
            ViewData["ActivePage"] = ActivePage();
            ViewData["MayTakeExam"] = decksWithFlashcards.Count() > 0 ? "true" : "false";
            return View(examVM);
        }

        /// <summary>
        /// Creates a new exam with random flashcards.
        /// </summary>
        /// <returns>Redirects to the exam index page.</returns>
        [HttpGet]
        public async Task<IActionResult> RandomFlashcards()
        {
            var decks = await _deckRepository.GetAllWithFlashcardsAsync();
            if (decks.Count() == 0)
            {
                return NotFound();
            }
            Deck defaultDeck = decks.OrderBy(d => Guid.NewGuid() ).FirstOrDefault();

            var newExam = _repository.CreateExam(defaultDeck.Id, 1);
            _repository.Add(newExam);
            return RedirectToAction("Index", new { id = newExam.Id });
        }

        /// <summary>
        /// Displays the exam creation form.
        /// </summary>
        /// <param name="defaultFlashcardSet">The default flashcard set ID.</param>
        /// <returns>The partial view for creating an exam.</returns>
        [HttpGet]
        public async Task<IActionResult> Create(int? defaultFlashcardSet = 1)
        {
            var decks = await _deckRepository.GetAllWithFlashcardsAsync();
            if(decks.Count() == 0)
            {
                return PartialView("_Create");
            }
            var defaultDeck = decks.Where(x => x.Id == defaultFlashcardSet).FirstOrDefault();
            int flashcardsCount = defaultDeck.Flashcards.Count() ;

            if (defaultDeck == null)
            {
                var newDeck = decks.FirstOrDefault();
                defaultFlashcardSet = newDeck.Id;
                flashcardsCount = newDeck.Flashcards.Count();
            }

            var examVM = new CreateExamViewModel()
            {
                decks = new SelectList(decks, "Id", "Title", defaultFlashcardSet),
                flashcardsCount = new SelectList(Enumerable.Range(1, flashcardsCount >0 ? flashcardsCount : 1), 1)
            };


            return PartialView("_Create", examVM);
        }


        /// <summary>
        /// Sets the dropdown list options dynamically.
        /// </summary>
        /// <param name="type">The type of dropdown to update.</param>
        /// <param name="value">The selected value.</param>
        /// <returns>The updated JSON response.</returns>
        [HttpPost]
        public JsonResult setDropDrownList(string type, int value)
        {
            var examVM = new CreateExamViewModel();
            switch (type)
            {
                case "deckId":
                    var defaultDeck = _deckRepository.GetById(value);
                    var flashcardsCount = defaultDeck.Flashcards.Count;
                    var selectList = new SelectList(Enumerable.Range(1, flashcardsCount));
                    examVM.flashcardsCount = selectList;
                    break;
            }
            return Json(examVM);
        }

        /// <summary>
        /// Handles the creation of a new exam.
        /// </summary>
        /// <param name="model">The view model representing the new exam.</param>
        /// <returns>Redirects to the exam index page.</returns>
        [HttpPost]
        public ActionResult Create(CreateExamViewModel model)
        {
            _logger.LogDebug("Create exam");

            if (ModelState.IsValid)
            {
                var newExam = _repository.CreateExam(int.Parse(model.deckId), int.Parse(model.questionsNumber));
                _repository.Add(newExam);

                //return RedirectToAction("Index", new { id = newExam.Id });
                return Json(new { redirectUrl = Url.Action("Index", "Exam", new { id = newExam.Id }) });

            }

            return PartialView("_Create", model);
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