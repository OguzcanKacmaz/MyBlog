using Gunluk.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gunluk.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GonderilerController : Controller
    {
        private readonly UygulamaDbContext _db;

        public GonderilerController(UygulamaDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string? durum)
        {
            ViewBag.Mesaj = durum == "eklendi" ? "Gönderi Başarıyla Oluşturuldu" : durum == "duzenlendi" ? "Gönderiniz Başarıyla Güncellendi" : durum == "silindi" ? "Gönderiniz Başarıyla Silindi" : null;
            return View(_db.Gonderiler.Include(x => x.Kategori).ToList());
        }
        public IActionResult Yeni()
        {
            KategorileriYukle();
            return View("Yonet");
        }

        private void KategorileriYukle()
        {
            ViewBag.Kategoriler = _db.Kategoriler.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Ad }).ToList();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Yeni(GonderiViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Gonderi gonderi = new Gonderi()
                {
                    Baslik = vm.Baslik,
                    Icerik = vm.Icerik,
                    KategoriId = vm.KategoriId!.Value
                };

                _db.Gonderiler.Add(gonderi);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index), new { Durum = "eklendi" });
            }
            KategorileriYukle();
            return View("Yonet");
        }
        public IActionResult Duzenle(int id)
        {
            var gonderi = _db.Gonderiler.Find(id);
            if (gonderi == null)
                return NotFound();
            var vm = new GonderiViewModel()
            {
                Baslik = gonderi.Baslik,
                Icerik = gonderi.Icerik,
                Id = gonderi.Id,
                KategoriId = gonderi.KategoriId
            };
            KategorileriYukle();
            return View("Yonet", vm);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Duzenle(GonderiViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var gonderi = _db.Gonderiler.Find(vm.Id);
                if (gonderi == null)
                    return NotFound();
                gonderi.Baslik = vm.Baslik;
                gonderi.Icerik = vm.Icerik;
                gonderi.DegistirilmeZamani = DateTime.Now;
                gonderi.KategoriId = vm.KategoriId!.Value;
                _db.SaveChanges();
                return RedirectToAction(nameof(Index), new { Durum = "duzenlendi" });
            }
            KategorileriYukle();
            return View("Yonet");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            var gonderi = _db.Gonderiler.Find(id);

            if (gonderi == null)
                return NotFound();

            _db.Gonderiler.Remove(gonderi);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), new { Durum = "silindi" });
        }
    }
}
