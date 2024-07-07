using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Stok_Takip.Models.Entity;

namespace Stok_Takip.Controllers
{
    [Authorize(Roles = "A,B")]
    public class KategoriController : Controller
    {
        Stok_TakipEntities db = new Stok_TakipEntities();
        public ActionResult Index(string ara)
        {
            var model = db.Kategoriler.ToList();
            if (!string.IsNullOrEmpty(ara))
            {
                model = model.Where(x => x.Kategori.Contains(ara)).ToList();
            }
            return View(model);
        }

        public ActionResult Ekle() 
        {
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

        public ActionResult Ekle2(Kategoriler p)
        {
            if(!ModelState.IsValid) return View("Ekle");
            db.Kategoriler.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult GüncelleBilgiGetir (int id)
        {
            SelectBigiGetir();
            var model = db.Kategoriler.Find(id);
            return View(model);
        }
        public ActionResult Guncelle(Kategoriler p) 

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
        public ActionResult SilBilgiGetir(Kategoriler p) 
        {

            var model = db.Kategoriler.Find(p.ID);
            if (model == null) return HttpNotFound();
            return View(model) ;
        }
        public ActionResult Sil(Kategoriler p) 
        {
            
            db.Entry(p).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}