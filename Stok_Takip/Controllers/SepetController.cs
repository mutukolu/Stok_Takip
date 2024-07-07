
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Stok_Takip.Models.Entity;

namespace Stok_Takip.Controllers
{
    public class SepetController : Controller
    {
        // GET: Sepet
        Stok_TakipEntities db = new Stok_TakipEntities();
        public ActionResult Index(decimal? Tutar)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi == kullaniciadi);
                if (kullanici != null)
                {
                    var sepetListesi = db.Sepet.Where(x => x.KullaniciID == kullanici.Id).ToList();
                    if (sepetListesi.Any())
                    {
                        decimal toplamTutar = sepetListesi.Sum(x => x.ToplamFiyat);
                        ViewBag.Tutar = "Toplam Tutar: " + toplamTutar.ToString("F2") + " TL";
                        return View(sepetListesi);
                    }
                    else
                    {
                        ViewBag.Tutar = "Sepetinizde ürün bulunmuyor";
                        return View(new List<Sepet>()); 
                    }
                }
            }
            return HttpNotFound();
        }
        public ActionResult SepeteEkle(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi == kullaniciadi);
                var u = db.Ürünler.Find(id);

                if (model == null || u == null)
                {
                    return HttpNotFound();
                }

                var sepet = db.Sepet.FirstOrDefault(x => x.KullaniciID == model.Id && x.UrunID == id);
                if (sepet != null)
                {
                    if (sepet.BirimMiktar + 1 > u.Miktarı) 
                    {
                        TempData["StokUyarisi"] = "Stokta yeterli miktarda ürün yok.";
                        return RedirectToAction("Index");
                    }
                    sepet.BirimMiktar++;
                    sepet.ToplamFiyat = u.SatisFiyat * sepet.BirimMiktar;
                }
                else
                {
                    if (1 > u.Miktarı) 
                    {
                        TempData["StokUyarisi"] = "Stokta yeterli miktarda ürün yok.";
                        return RedirectToAction("Index");
                    }
                    var s = new Sepet
                    {
                        KullaniciID = model.Id,
                        UrunID = u.ID,
                        BirimMiktar = 1,
                        ToplamFiyat = u.SatisFiyat,
                        Tarih = DateTime.Now,
                        Saat = DateTime.Now,
                    };
                    db.Sepet.Add(s);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return HttpNotFound();
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

        public ActionResult TotalCount(int? count)
        {
            if (User.Identity.IsAuthenticated)
            {
                var model = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi == User.Identity.Name);
                count = db.Sepet.Where(x => x.KullaniciID == model.Id).Count();
                ViewBag.Count = count;
                if (count == 0)
                {
                    ViewBag.Count = "";
                }
                return PartialView();
            }
            return HttpNotFound();
        }
        public ActionResult Arttir(int id)
        {
            var model = db.Sepet.Find(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }

            if (model.Ürünler.Miktarı == 0)
            {
                TempData["StokUyarisi"] = "Stokta yeterli miktarda ürün yok.";
                return RedirectToAction("Index");

            }
            else if (model.Ürünler.Miktarı > 1)
            {
                model.BirimMiktar++;
                model.ToplamFiyat = model.Ürünler.SatisFiyat * model.BirimMiktar;
                model.Ürünler.Miktarı -= 1;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Azalt(int id)
        {
            var model = db.Sepet.Find(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }

            if (model.BirimMiktar == 1)
            {
                model.Ürünler.Miktarı += 1;
                db.Sepet.Remove(model);
            }
            else if (model.BirimMiktar > 1)
            {
                model.BirimMiktar--;
                model.ToplamFiyat = model.Ürünler.SatisFiyat * model.BirimMiktar;
                model.Ürünler.Miktarı += 1;
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Sil(int id)
        {
            var model = db.Sepet.Find(id);
            if (model != null)
            {
                db.Sepet.Remove(model);
                db.SaveChanges();
            }

           return RedirectToAction("Index");
        }
        public ActionResult HepsiniSil()
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullanıcıadı = User.Identity.Name;
                var model = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi.Equals(kullanıcıadı));
                var temızle = db.Sepet.Where(x => x.KullaniciID.Equals(model.Id));
                db.Sepet.RemoveRange(temızle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return HttpNotFound();
        }

        public ActionResult Dinamikmiktar(int id, decimal miktari)
        {
            var model = db.Sepet.Find(id);
            model.BirimMiktar = miktari;
            model.ToplamFiyat = model.Ürünler.SatisFiyat * model.BirimMiktar;
            if (model.Ürünler.Miktarı >= model.BirimMiktar)
            {
                model.Ürünler.Miktarı -= model.BirimMiktar;
                db.SaveChanges();
            }
            else
            {
                TempData["Stok"] = "Stokta yeterli miktarda ürün yok.";
            }

            return RedirectToAction("Index");  
        }

    }
}