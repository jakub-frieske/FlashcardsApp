using FlashcardsApp.Controllers;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlashcardsApp.Tests.Controllers
{
    public class FlashcardControllerTests
    {
        [Fact]
        public async Task FlashcardController_Create_Get_ReturnsPartialViewWithFlashcardForm()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FlashcardController>>();
            var mockRepository = new Mock<IFlashcardRepository>();

            var deckId = 1;
            var deck = new Deck { Id = deckId };

            mockRepository.Setup(repo => repo.GetDeckbyIdAsync(deckId)).ReturnsAsync(deck);

            var controller = new FlashcardController(mockLogger.Object, mockRepository.Object);

            // Act
            var result = await controller.Create(deckId);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsAssignableFrom<FlashcardViewModel>(partialViewResult.ViewData.Model);
            Assert.Equal(deckId, model.DeckId);
        }


        [Fact]
        public async Task FlashcardController_Create_Post_ReturnsJson()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<FlashcardController>>();
            var mockRepository = new Mock<IFlashcardRepository>();
            var mockUrlHelper = new Mock<IUrlHelper>();

          
            var deckId = 1;
            var deck = new Deck { Id = deckId, Title="Deck test" };
            mockRepository.Setup(repo => repo.GetDeckbyIdAsync(deckId)).ReturnsAsync(deck);

            var flashcardVM = new FlashcardViewModel { 
                Term="Term test", 
                Definition="Definition Test",
                DeckId = deck.Id };

            var expectedRedirectUrl = $"/Deck/Detail/{deckId}";
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(expectedRedirectUrl);


            var controller = new FlashcardController(mockLogger.Object, mockRepository.Object);
            controller.Url = mockUrlHelper.Object;

            // Act
            var result = await controller.Create(deckId, flashcardVM);

            // Assert
            Assert.IsType<JsonResult>(result);
        }
    }
}
