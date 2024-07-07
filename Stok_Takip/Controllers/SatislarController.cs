using Stok_Takip.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Stok_Takip.Controllers
{
    [Authorize(Roles = "A,B,C")]
    public class SatislarController : Controller
    {
        // GET: Satislar
        Stok_TakipEntities db = new Stok_TakipEntities();
        public ActionResult Index()
        {
            if (User.IsInRole("A"))
            {
                var model = db.Satislar.ToList();
                return View(model);
            }
            else
            {
                var userId = db.Kullanıcılar.First(x => x.KullanıcıAdi == User.Identity.Name).Id;
                var model = db.Satislar.Where(x => x.kullanıcıID == userId).ToList();
                return View(model);
            }
            
        }
        public ActionResult SatinAl(int id) 
        { 
            var model = db.Sepet.FirstOrDefault(x=>x.ID == id);
            return View(model);
        }
        [HttpPost]
        public ActionResult SatinAlMesaj(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = db.Sepet.FirstOrDefault(x => x.ID == id);
                    var stok = db.Ürünler.FirstOrDefault(x => x.ID == model.UrunID);
                    
                    if (stok != null)
                    {
                        if (stok.Miktarı == 1)
                        {
                            stok.Miktarı = 0;
                        }
                        else if (stok.Miktarı > 1)
                        {
                            stok.Miktarı = stok.Miktarı - model.BirimMiktar;
                        }
                        if (model != null)
                        {

                            var satis = new Satislar
                            {
                                kullanıcıID = model.KullaniciID,
                                UrunID = model.UrunID,
                                SepetID = model.ID,
                                BarkodNo = model.Ürünler.BarkodNo,
                                BirimFiyat = model.Ürünler.SatisFiyat,
                                BirimMiktar = model.BirimMiktar,
                                ToplamFiyat = model.ToplamFiyat,
                                KDV = model.Ürünler.BirimID,
                                Tarih = DateTime.Now,
                                Saat = DateTime.Now

                            };
                            db.Sepet.Remove(model);
                            db.Satislar.Add(satis);
                            db.SaveChanges();
                            ViewBag.mesaj = "Satın alma işlemi tamamlanmıştır.";
                        }
                        else
                        {
                            ViewBag.mesaj = "Sepet öğesi bulunamadı.";
                        }
                    }
                    else
                    {
                        ViewBag.mesaj = "Ürün bulunamadı.";
                    }
                }

            }
            catch (Exception ex) 
            {
                ViewBag.mesaj = "Satın alma işlemi başarısız."+ex.Message;
            }
            return View("mesaj");
        }

        public ActionResult HepsiniSatinAl()
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi == kullaniciadi);
                var model = db.Sepet.Where(x=>x.KullaniciID == kullanici.Id).ToList();
                var satinal = db.Sepet.FirstOrDefault(x=>x.KullaniciID == kullanici.Id);

                if (model != null)
                {
                    if(satinal == null) 
                    {
                        ViewBag.Tutar = "Sepetinizde ürün bulunmuyor";
                  
                    }
                    else if (satinal != null)
                    {
                        decimal toplamTutar = db.Sepet.Where(x=>x.KullaniciID == satinal.KullaniciID).Sum(x=>x.ToplamFiyat);
                        ViewBag.Tutar = "Toplam Tutar: " + toplamTutar + " TL";
                    }
                    return View(model);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult HepsiniSatinAl2()
        {
            var kullaniciadi = User.Identity.Name;
            var kullanici = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi == kullaniciadi);
            var model = db.Sepet.Where(x => x.KullaniciID == kullanici.Id).ToList();
            int row = 0;
            foreach (var item in model) 
            {
                var satis = new Satislar
                {
                    kullanıcıID = model[row].KullaniciID,
                    UrunID = model[row].UrunID,
                    SepetID = model[row].ID,
                    BarkodNo = model[row].Ürünler.BarkodNo,
                    BirimFiyat = model[row].Ürünler.SatisFiyat,
                    BirimMiktar = model[row].BirimMiktar,
                    ToplamFiyat = model[row].ToplamFiyat,
                    KDV = model[row].Ürünler.KDV,
                    BirimID = model[row].Ürünler.BirimID,
                    Tarih = DateTime.Now,
                    Saat = DateTime.Now               
                 
                };
               
                db.Satislar.Add(satis);
                row++;
            }
            foreach (var item in model)
            {
                var ürün = db.Ürünler.FirstOrDefault(x => x.ID == item.UrunID);
                if (ürün != null)
                {
                    if (ürün.Miktarı == 1)
                    {
                        ürün.Miktarı = 0;
                    }
                    else if (ürün.Miktarı > 1)
                    {
                        ürün.Miktarı -= item.BirimMiktar;
                    }
                }
            }
            db.Sepet.RemoveRange(model);
            db.SaveChanges();
            TempData["mesaj"] = "Satın alma işlemi başarıyla tamamlandı.";
            return RedirectToAction("Index", "Sepet");
        }
    }
}