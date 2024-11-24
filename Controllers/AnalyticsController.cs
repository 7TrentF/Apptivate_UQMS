using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Apptivate_UQMS_WebApp.Controllers
{

    //   AAAND here we go 
    public class AnalyticsController : Controller
    {
        // GET: Analytics
        public ActionResult AnalyticsReporting()
        {
            return View();
        }

        // GET: Analytics/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Analytics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Analytics/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Analytics/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Analytics/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Analytics/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Analytics/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}