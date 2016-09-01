using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class TesController : Controller
    {
        //
        // GET: /Tes/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Tes/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Tes/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Tes/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Tes/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Tes/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Tes/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Tes/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
