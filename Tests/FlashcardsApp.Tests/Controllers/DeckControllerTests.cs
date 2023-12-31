﻿using FakeItEasy;
using FlashcardsApp.Controllers;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsApp.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;


namespace FlashcardsApp.Tests.Controllers
{
    public class DeckControllerTests
    {
        [Fact]
        public async Task DeckController_Index_ReturnsViewWithDecks()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DeckController>>();
            var mockRepository = new Mock<IDeckRepository>();
            var mockPhotoService = new Mock<IPhotoService>();
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();

            var decks = new List<Deck>
            {
                new Deck { Id = 1, Title = "Math Flashcards" },
                new Deck { Id = 2, Title = "Science Flashcards" }
            };

            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(decks);
            var controller = new DeckController(mockLogger.Object, mockRepository.Object, mockPhotoService.Object, mockFlashcardRepository.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Deck>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task DeckController_Create_Post_ReturnsJson()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DeckController>>();
            var mockRepository = new Mock<IDeckRepository>();
            var mockPhotoService = new Mock<IPhotoService>();
            var mockFlashcardRepository = new Mock<IFlashcardRepository>();
            var mockUrlHelper = new Mock<IUrlHelper>();

            var deckVM = new CreateDeckViewModel
            {
                Title = "Test Deck",
                Description = "Test Description"
            };
            var expectedRedirectUrl = $"/Deck/Index";
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns(expectedRedirectUrl);

            var controller = new DeckController(mockLogger.Object, mockRepository.Object, mockPhotoService.Object, mockFlashcardRepository.Object)
            {
                Url = mockUrlHelper.Object
            };

            // Act
            var result = await controller.Create(deckVM);

            // Assert
            Assert.IsType<JsonResult>(result);
        }
    }
}
