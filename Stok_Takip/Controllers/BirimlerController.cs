
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Stok_Takip.Models.Entity;

namespace Stok_Takip.Controllers
{
    [Authorize(Roles ="A,B")]
    public class BirimlerController : Controller
    {
        Stok_TakipEntities db = new Stok_TakipEntities();
        public ActionResult Index(string ara)
        {
            var model = db.Birimler.ToList();
            if (!string.IsNullOrEmpty(ara))
            {
                model = model.Where(x => x.Birim.Contains(ara)).ToList();
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Ekle()
        {
            return View();
        }
        [HttpPost]
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
        public ActionResult Ekle(Birimler p)
        {
            if (p.ID == 0) 
            {
                db.Entry(p).State = System.Data.Entity.EntityState.Added;
            }
            else if (p.ID > 0)
            {
                db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();
            return RedirectToAction("Index");

        }
        public ActionResult GüncelleBilgiGetir(int id)
        {
            SelectBigiGetir();
            var model = db.Birimler.Find(id);
            return View(model);
        }
        public ActionResult Guncelle(Birimler p)

        {
            if (!ModelState.IsValid)
            {
                SelectBigiGetir();

                return View("GüncelleBilgiGetir");
            }
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
            public ActionResult SilBilgiGetir(Birimler p)
        {
            var model = db.Birimler.Find(p.ID);
            if (model == null) return HttpNotFound();
            return View(model);
        }
        public ActionResult Sil(int id)
        {
            var birim = db.Birimler.Include(b => b.Ürünler).FirstOrDefault(b => b.ID == id);

            if (birim != null)
            {
                db.Birimler.Remove(birim);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}