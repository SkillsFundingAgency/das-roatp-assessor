using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications");
        }

        public IActionResult NewApplications()
        {
            var vm = new NewApplicationsViewModel(3, 1, 1, 1);
            vm.AddApplication(new ApplicationViewModel { ApplicationReferenceNumber = "ABC123", OrganisationName  = "Org 1", ProviderRoute = "Main", SubmittedDate = DateTime.Now.AddDays(-1), Ukprn = "32497863456" });
            vm.AddApplication(new ApplicationViewModel { ApplicationReferenceNumber = "3465456", OrganisationName = "Org 3", ProviderRoute = "Main", SubmittedDate = DateTime.Now.AddDays(-3), Ukprn = "34658745654", Assessor1 = "Not Me" });
            return View(vm);
        }
    }
}
