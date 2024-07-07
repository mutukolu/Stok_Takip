using MailKit.Net.Smtp;
using MimeKit;
using Stok_Takip.Models.Entity;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Stok_Takip.Controllers
{
    [AllowAnonymous]
    public class KullanıcılarController : Controller
    {
        Stok_TakipEntities db = new Stok_TakipEntities();
        // GET: Kullanıcılar
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Kullanıcılar k)
        {
            var kullanici = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi == k.KullanıcıAdi && x.Sifre == k.Sifre);
            if (kullanici != null)
            {
                FormsAuthentication.SetAuthCookie(k.KullanıcıAdi, false);
                return RedirectToAction("Index", "Urunler");
            }
            ViewBag.hata = "Kullanıcı adı veya şifre yanlış";
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(Kullanıcılar k)
        {
            var model = db.Kullanıcılar.Where(x => x.Email == k.Email).FirstOrDefault();
            if (model != null)
            {
                try
                {
                    Guid rastgele = Guid.NewGuid();
                    model.Sifre = rastgele.ToString().Substring(0, 8);
                    db.SaveChanges();

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("projemutu@gmail.com", "projemutu@gmail.com"));
                    message.To.Add(new MailboxAddress(model.Email, model.Email));
                    message.Subject = "Şifre sıfırlama";

                    var builder = new BodyBuilder();
                    builder.HtmlBody = $"Merhaba {model.AdiSoyadi},<br/>Kullanıcı Adınız = {model.KullanıcıAdi}<br/>Şifreniz = {model.Sifre}";

                    message.Body = builder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        try
                        {
                            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);

                            client.Authenticate("projemutu@gmail.com", "p e l w i w n h te j h a i g w");

                            client.Send(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"E-posta gönderilemedi: {ex.Message}");
                        }
                        finally
                        {
                            client.Disconnect(true);
                        }
                    }
                    return RedirectToAction("Login");
                }
                catch (Exception)
                {

                    ViewBag.hata = "Şifre sıfırlama e-postası gönderilirken bir hata oluştu.";
                }
            }
            else
            {
                ViewBag.hata = "Böyle bir e-mail adresi bulunamadı";
            }

            return View();
        }

        [HttpGet]
        public ActionResult Kayıt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Kayıt(Kullanıcılar k)
        {
            if (!ModelState.IsValid)
            {
                return View(k);
            }
            int maxId = db.Kullanıcılar.Max(u => u.Id); 
            k.Id = maxId + 1;
            k.Rol = "C";
            db.Kullanıcılar.Add(k);
            db.SaveChanges();
            return RedirectToAction("Login");
        }
        public ActionResult Guncelle()
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = db.Kullanıcılar.FirstOrDefault(x => x.KullanıcıAdi == kullaniciadi);

                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    return View(new Kullanıcılar());
                }
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Guncelle(Kullanıcılar k)
        {
            if (ModelState.IsValid)
            {
                var kullanıcı = db.Kullanıcılar.Find(k.Id);
                if (kullanıcı != null)
                {
                    kullanıcı.KullanıcıAdi = k.KullanıcıAdi;
                    kullanıcı.Sifre = k.Sifre;
                    kullanıcı.AdiSoyadi = k.AdiSoyadi;
                    kullanıcı.Telefon = k.Telefon;
                    kullanıcı.Adres = k.Adres;
                    kullanıcı.Email = k.Email;
                    kullanıcı.Tarih = DateTime.Now;

                    db.Entry(kullanıcı).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    if (User.Identity.Name != k.KullanıcıAdi)
                    {
                        FormsAuthentication.SignOut();
                        FormsAuthentication.SetAuthCookie(k.KullanıcıAdi, false);
                    }

                    return RedirectToAction("Index", "Urunler");
                }
            }

            return View(k);
        }

        public ActionResult Gecmis(string ad, string soyad)
        {
            var userId = db.Kullanıcılar.First(x => x.KullanıcıAdi == User.Identity.Name).Id;
            var model = db.Satislar.Where(x => x.kullanıcıID == userId).ToList();
            return View(model);
        }
    }
}
