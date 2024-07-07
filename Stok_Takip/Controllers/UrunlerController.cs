using Stok_Takip.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Stok_Takip.Controllers
{
    [Authorize]
    public class UrunlerController : Controller
    {
        Stok_TakipEntities db = new Stok_TakipEntities();
        public ActionResult Index(string ara)
        {
            var model = db.Ürünler.ToList();
            if (!string.IsNullOrEmpty(ara))
            {
                model = model.Where(x => x.UrunAdı.Contains(ara) || x.Markalar.Markalar1.Contains(ara)).ToList();
            }
            return View(model);
        }

        private void Yenile(Ürünler model)
        {

            List<Kategoriler> kategorilist = db.Kategoriler.OrderBy(x => x.Kategori).ToList();
            model.KategoriListesi = (from x in kategorilist
                                     select new SelectListItem
                                     {
                                         Text = x.Kategori,
                                         Value = x.ID.ToString()
                                     }
                ).ToList();

            List<Birimler> birimlist = db.Birimler.OrderBy(x => x.Birim).ToList();
            model.BirimListesi = (from x in birimlist
                                  select new SelectListItem
                                  {
                                      Text = x.Birim,
                                      Value = x.ID.ToString()
                                  }
                                     ).ToList();

            List<Markalar> markaList = db.Markalar.OrderBy(x => x.Markalar1).ToList();
            model.MarkaListesi = (from x in markaList
                                  select new SelectListItem
                                  {
                                      Text = x.Markalar1,
                                      Value = x.ID.ToString()
                                  }
                                     ).ToList();

        }
        [HttpGet]
        public ActionResult Ekle()
        {
            var model = new Ürünler();
            Yenile(model);

            return View(model);
        }

        public ActionResult Ekle(Ürünler p)
        {
            if (!ModelState.IsValid)
            {
                var model = new Ürünler();
                Yenile(model);
                return View(model);

            }
            p.Tarih = DateTime.Now;
            db.Entry(p).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult MiktarEkle(int id)
        {
            var model = db.Ürünler.Find(id);
            return View();
        }
        [HttpPost]
        public ActionResult MiktarEkle(Ürünler p)
        {
            var model = db.Ürünler.Find(p.ID);
            model.Miktarı = model.Miktarı + p.Miktarı;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]

        public JsonResult GetMarka(int id)
        {
            var model = new Ürünler();
            List<Markalar> markalars = db.Markalar.Where(x => x.KategoriID == id).OrderBy(x => x.Markalar1).ToList();
            model.MarkaListesi = (from x in markalars
                                  select new SelectListItem
                                  {
                                      Text = x.Markalar1,
                                      Value = x.ID.ToString()
                                  }
                  ).ToList();
            model.MarkaListesi.Insert(0, new SelectListItem { Text = "---Seçiniz---", Value = "" });
            return Json(model.MarkaListesi, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GüncelleBilgiGetir(int id)
        {
            var model = db.Ürünler.Find(id);
            Yenile(model);
            List<Markalar> markalars = db.Markalar.Where(x => x.KategoriID == model.KategoriID).OrderBy(x => x.Markalar1).ToList();
            model.MarkaListesi = (from x in markalars
                                  select new SelectListItem
                                  {
                                      Text = x.Markalar1,
                                      Value = x.ID.ToString()
                                  }).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Guncelle(Ürünler p)
        {
            if (!ModelState.IsValid)
            {
                var model = db.Ürünler.Find(p.ID);
                Yenile(model);
                List<Markalar> markalars = db.Markalar.Where(x => x.KategoriID == model.KategoriID).OrderBy(x => x.Markalar1).ToList();
                model.MarkaListesi = (from x in markalars
                                      select new SelectListItem
                                      {
                                          Text = x.Markalar1,
                                          Value = x.ID.ToString()
                                      }).ToList();
                return View(model);
            }
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Sil(int id)
        {
            var model = db.Ürünler.FirstOrDefault(x => x.ID == id);
            db.Entry(model).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
