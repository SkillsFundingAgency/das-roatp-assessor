using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Settings;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebConfiguration _configuration;

        public HomeController(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications", "Dashboard");
        }

        [Route("/Dashboard")]
        public IActionResult Dashboard()
        {
            return Redirect(_configuration.EsfaAdminServicesBaseUrl + "/Dashboard");
        }
    }
}
