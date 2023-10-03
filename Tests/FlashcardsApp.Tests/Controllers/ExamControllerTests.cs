using FlashcardsApp.Controllers;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FlashcardsApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlashcardsApp.Tests.Controllers
{
    public class ExamControllerTests
    {
        [Fact]
        public async Task ExamController_Index_ReturnsViewWithExamQuestions()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExamController>>();
            var mockRepository = new Mock<IExamRepository>();
            var mockDeckRepository = new Mock<IDeckRepository>();

            var examId = 1;
            var questions = new List<Question>
            {
                new Question { Id = 1 },
                new Question { Id = 2 }
            };

            mockRepository.Setup(repo => repo.GetByIdAsync(examId)).ReturnsAsync(new Exam { Id = examId, Questions = questions });

            var controller = new ExamController(mockLogger.Object, mockRepository.Object, mockDeckRepository.Object);

            // Act
            var result = await controller.Index(examId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ExamViewModel>(viewResult.ViewData.Model);
            Assert.Equal(examId, model.Id);
            Assert.Equal(2, model.Questions.Count);
        }

        [Fact]
        public Task ExamController_Index_Post_ReturnsRedirectToAction()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ExamController>>();
            var mockRepository = new Mock<IExamRepository>();
            var mockDeckRepository = new Mock<IDeckRepository>();

            var examId = 1;
            var examVM = new ExamViewModel
            {
                Id = 1,
                Title = "Sample Exam",
                Questions = new List<Question>(),
            };

            var examResult = new Exam
            {
                Id = 1
            };

            mockRepository.Setup(repo => repo.CalculateScore(It.IsAny<Exam>())).Returns(examResult);

            var controller = new ExamController(mockLogger.Object, mockRepository.Object, mockDeckRepository.Object);

            // Act
            var result = controller.Index(examVM);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirectToActionResult.ActionName);
            Assert.Equal(examId, redirectToActionResult.RouteValues?["id"]);

            return Task.CompletedTask;

        }
    }
}
