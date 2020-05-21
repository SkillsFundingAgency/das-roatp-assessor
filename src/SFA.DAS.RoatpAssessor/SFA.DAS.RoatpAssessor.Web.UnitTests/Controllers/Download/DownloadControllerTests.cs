using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Download
{
    [TestFixture]
    public class DownloadControllerTests
    {
        private DownloadController _controller;
        private Mock<IRoatpApplicationApiClient> _apiClient;

        [SetUp]
        public void SetUp()
        {
            _apiClient = new Mock<IRoatpApplicationApiClient>();
            _controller = new DownloadController(_apiClient.Object);
        }

        [Test]
        public async Task When_downloading_a_file_then_the_file_is_returned_in_the_response()
        {
            var applicationId = Guid.NewGuid();
            var sequenceNo = 1;
            var sectionNo = 2;
            var pageId = "page";
            var questionId = "question";
            var fileName = "file";

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new MemoryStream();
            httpResponse.Content = new StreamContent(stream);

            var mediaType = "application/pdf";
            httpResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

            _apiClient.Setup(x => x.DownloadFile(applicationId, sequenceNo, sectionNo, pageId, questionId, fileName)).ReturnsAsync(httpResponse);

            var response = await _controller.DownloadFile(applicationId, sequenceNo, sectionNo, pageId, questionId, fileName);

            var fileStreamResponse = response as FileStreamResult;

            Assert.IsNotNull(fileStreamResponse.FileStream);
            Assert.AreEqual(mediaType, fileStreamResponse.ContentType);
            Assert.AreEqual(fileName, fileStreamResponse.FileDownloadName);
        }
    }
}
