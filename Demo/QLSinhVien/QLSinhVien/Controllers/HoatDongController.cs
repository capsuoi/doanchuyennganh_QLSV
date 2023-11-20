using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using QLSinhVien.Models;

namespace QLSinhVien.Controllers
{
    [Authorize]
    public class HoatDongController : Controller
    {
        private readonly Ql_SinhVienEntities _entities;

        public HoatDongController()
        {
            _entities = new Ql_SinhVienEntities();
        }

        // Giáo viên
        public ActionResult Index(string timkiem = "", string message = "")
        {
            var query = _entities.hoatdongs.Include(x => x.hocsinh_hoatdong).AsQueryable();
            if (!string.IsNullOrEmpty(timkiem))
                query = query.Where(x => x.ten.ToLower().Contains(timkiem));

            ViewBag.TimKiem = timkiem;
            ViewBag.Message = message;
            return View(query.OrderByDescending(x => x.ngaybatdau).ToArray());
        }

        public ActionResult Them()
        {
            return View(new hoatdong { ngaybatdau = DateTime.Now, ngayketthuc = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Them(hoatdong model)
        {
            if (string.IsNullOrEmpty(model.ten))
                ModelState.AddModelError(nameof(model.ten), "Tên không được bỏ trống");
            if (model.ngaybatdau <= DateTime.Now)
                ModelState.AddModelError(nameof(model.ngaybatdau), "Ngày bắt đầu phải sau ngày hiện tại");
            if (model.ngaybatdau > model.ngayketthuc)
                ModelState.AddModelError(nameof(model.ngayketthuc), "Ngày kết thúc phải lớn hơn ngày bắt đầu");
            if (!ModelState.IsValid)
                return View(model);

            _entities.hoatdongs.Add(model);
            _entities.SaveChanges();
            return RedirectToAction("Index", new { message = "Thêm hoạt động thành công!" });
        }

        public ActionResult Sua(int id)
        {
            var hoatdong = _entities.hoatdongs.FirstOrDefault(x => x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("Index");
            return View(hoatdong);
        }

        [HttpPost]
        public ActionResult Sua(int id, hoatdong model)
        {
            var hoatdong = _entities.hoatdongs.FirstOrDefault(x => x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("Index");

            if (string.IsNullOrEmpty(model.ten))
                ModelState.AddModelError(nameof(model.ten), "Tên không được bỏ trống");
            if (hoatdong.ngaybatdau != model.ngaybatdau && model.ngaybatdau <= DateTime.Now)
                ModelState.AddModelError(nameof(model.ngaybatdau), "Ngày bắt đầu phải sau ngày hiện tại");
            if (model.ngaybatdau > model.ngayketthuc)
                ModelState.AddModelError(nameof(model.ngayketthuc), "Ngày kết thúc phải lớn hơn ngày bắt đầu");
            if (!ModelState.IsValid)
                return View(model);

            hoatdong.ten = model.ten;
            hoatdong.soluongtoida = model.soluongtoida;
            hoatdong.ngaybatdau = model.ngaybatdau;
            hoatdong.ngayketthuc = model.ngayketthuc;
            hoatdong.mota = model.mota;
            _entities.Entry(hoatdong).State = EntityState.Modified;
            _entities.SaveChanges();
            return RedirectToAction("Index", new { message = "Sửa hoạt động thành công!" });
        }

        public ActionResult Xem(int id, string message = "")
        {
            var hoatdong = _entities.hoatdongs.Include(x => x.hocsinh_hoatdong.Select(hh => hh.nguoidung))
                        .Include(x => x.hocsinh_hoatdong.Select(hh => hh.diemdanhs)).FirstOrDefault(x => x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("Index");
            ViewBag.Message = message;
            return View(hoatdong);
        }

        [HttpPost]
        public ActionResult Duyet(int id, int[] hocsinh)
        {
            var hoatdong = _entities.hoatdongs.FirstOrDefault(x => x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("Index");

            if (hocsinh == null || hocsinh.Length == 0)
                return RedirectToAction("Xem", new { id, message = "Không có sinh viên nào được chọn!" });
            var hocsinhs = _entities.hocsinh_hoatdong.Where(x => x.mahoatdong == id && hocsinh.Contains(x.mahocsinh)).ToList()
                .Select(x =>
                {
                    x.dathamgia = true;
                    x.ngaythamgia = DateTime.Now;
                    return x;
                });
            _entities.hocsinh_hoatdong.AddOrUpdate(hocsinhs.ToArray());
            _entities.SaveChanges();

            return RedirectToAction("Xem", new { id, message = "Duyệt danh sách sinh viên tham gia thành công!" });
        }

        [HttpPost]
        public ActionResult DiemDanh(int id, DiemdanhModel[] hocsinh)
        {
            var hoatdong = _entities.hoatdongs.FirstOrDefault(x => x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("Index");
            var diemdanhs = _entities.hocsinh_hoatdong.Where(x => x.mahoatdong == id).ToList()
                .Select(x =>
                {
                    var res = new diemdanh
                    {
                        mahocsinh_hoatdong = x.mahs_hd,
                        ngaydiemdanh = DateTime.Now.Date
                    };
                    var model = hocsinh.FirstOrDefault(m => m.Ma == x.mahocsinh);
                    if (model == null)
                        res.comat = false;
                    else
                    {
                        res.comat = model.Status == "yes" || model.Status == "late";
                        res.denmuon = model.Status == "late";
                    }
                    return res;
                });
            _entities.diemdanhs.AddRange(diemdanhs);
            _entities.SaveChanges();
            return RedirectToAction("Xem", new { id, message = "Lưu thông tin điểm danh thành công!" });
        }

        public ActionResult Xoa(int id)
        {
            var hoatdong = _entities.hoatdongs.FirstOrDefault(x => x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("Index");

            var hocvien = _entities.hocsinh_hoatdong.Where(x => x.mahoatdong == id).ToList();
            var hocvienhoatdong = hocvien.Select(x => x.mahs_hd).ToList();
            var diemdanhs = _entities.diemdanhs.Where(x => hocvienhoatdong.Contains(x.mahocsinh_hoatdong)).ToList();
            _entities.diemdanhs.RemoveRange(diemdanhs);
            _entities.hocsinh_hoatdong.RemoveRange(hocvien);
            _entities.hoatdongs.Remove(hoatdong);
            _entities.SaveChanges();

            return RedirectToAction("Index", new { message = "Xóa hoạt động thành công!" });
        }

        // Học sinh
        public ActionResult CuaToi()
        {
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.tendangnhap == User.Identity.Name);
            var hoatdongs = _entities.hocsinh_hoatdong.Include(x => x.hoatdong).Include(x => x.diemdanhs).Where(x => x.mahocsinh == nguoidung.manguoidung).ToList();
            return View(hoatdongs);
        }

        public ActionResult DiemDanhCuaToi(int id, DateTime? month = null)
        {
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.tendangnhap == User.Identity.Name);
            var hoatdong = _entities.hocsinh_hoatdong.Include(x => x.hoatdong).Include(x => x.diemdanhs).FirstOrDefault(x => x.mahocsinh == nguoidung.manguoidung && x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("CuaToi", "TaiKhoan");

            if (month == null)
                month = DateTime.Now;

            ViewData["month"] = month;
            return View(hoatdong);
        }

        public ActionResult DangKy(string message = "")
        {
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.tendangnhap == User.Identity.Name);
            var date = DateTime.Now.AddDays(2).Date;
            var hoatdongs = _entities.hoatdongs.Include(x => x.hocsinh_hoatdong)
                .Where(x => x.ngaybatdau > date)
                .Where(x => !x.hocsinh_hoatdong.Any(h => h.mahocsinh == nguoidung.manguoidung)).ToArray();

            ViewBag.Message = message;
            return View(hoatdongs);
        }

        public ActionResult DangKyHoatDong(int id)
        {
            var hoatdong = _entities.hoatdongs.AsNoTracking().FirstOrDefault(x => x.mahoatdong == id);
            if (hoatdong == null)
                return RedirectToAction("DangKy");

            var nguoidung = _entities.nguoidungs.AsNoTracking().FirstOrDefault(x => x.tendangnhap == User.Identity.Name);

            if(_entities.hocsinh_hoatdong.Any(x => x.mahoatdong == hoatdong.mahoatdong && x.mahocsinh == nguoidung.manguoidung))
                return RedirectToAction("DangKy", new { message = "Bạn đã đăng ký tham gia hoạt động này rồi!" });

            _entities.Database.ExecuteSqlCommand("DISABLE TRIGGER dangky_hoatdong_trigger ON hocsinh_hoatdong;");
            var hs_hd = new hocsinh_hoatdong
            {
                mahocsinh = nguoidung.manguoidung,
                mahoatdong = hoatdong.mahoatdong,
                ngaydangky = DateTime.Now,
                dathamgia = false,
                ngaythamgia = null
            };
            _entities.hocsinh_hoatdong.Add(hs_hd);
            _entities.SaveChanges();
            _entities.Database.ExecuteSqlCommand("ENABLE TRIGGER dangky_hoatdong_trigger ON hocsinh_hoatdong;");
            return RedirectToAction("DangKy", new { message = "Gửi thông tin đăng ký tham gia thành công!" });
        }
    }
}