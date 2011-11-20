using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Azure.WebRole.CapacityTesting.Models;
using Azure.WebRole.CapacityTesting.Services;

namespace Azure.WebRole.CapacityTesting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITableStorageWriterService _tsService;

        public HomeController(ITableStorageWriterService tsService)
        {
            if (tsService == null) throw new ArgumentNullException("tsService");

            _tsService = tsService;
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            CapacityTestData data = _tsService.WriteToStorage();
            return View(data);
        }

        public async Task<ActionResult> AsyncCtp()
        {
            CapacityTestData data = await _tsService.WriteToStorageAsyncCtp();

            return View(data);
        }

        public async Task<ActionResult> AsyncTpl()
        {
            CapacityTestData data = await _tsService.WriteToStorageAsyncTpl();

            return View(data);
        }
    }
}
