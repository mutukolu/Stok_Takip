using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stok_Takip.Models.Entity;

namespace Stok_Takip.Controllers
{
    [Authorize(Roles = "A,B")]
    public class MarkalarController : Controller
    {
        Stok_TakipEntities db = new Stok_TakipEntities();
        public ActionResult Index(string ara)
        {
            var model = db.Markalar.ToList();
            if (!string.IsNullOrEmpty(ara))
            {
                model = model.Where(x => x.Markalar1.Contains(ara) || x.Kategoriler.Kategori.Contains(ara)).ToList();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Ekle()
        {
            SelectBigiGetir();
            return View();
        }

        private void SelectBigiGetir()
        {
            var model = new Markalar();
            List<SelectListItem> liste = new List<SelectListItem>(from x in db.Kategoriler
                                                                  select new SelectListItem
                                                                  {
                                                                      Value = x.ID.ToString(),
                                                                      Text = x.Kategori
                                                                  }
                                                                  ).ToList();

            ViewBag.l = liste;
            
        }

        [HttpPost]
        public ActionResult Ekle(Markalar m)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.kategoriID = new SelectList(db.Kategoriler, "ID", "Kategori", m.KategoriID);
                return View();
            }
            db.Entry(m).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult GüncelleBilgiGetir(int id)
        {
            SelectBigiGetir();
            var model = db.Markalar.Find(id);
            return View(model);
        }
        public ActionResult Guncelle(Markalar m)
        {
            if (!ModelState.IsValid)
            {
                SelectBigiGetir();

                return View("GüncelleBilgiGetir");
            }
            db.Entry(m).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult SilBilgiGetir(Markalar m)
        {

            var getir = db.Markalar.Find(m.ID);
            return View(getir);
        }
        public ActionResult Sil(Markalar m)
        {

            db.Entry(m).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}