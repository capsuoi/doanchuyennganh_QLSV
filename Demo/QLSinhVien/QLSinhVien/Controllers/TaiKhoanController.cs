using QLSinhVien.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace QLSinhVien.Controllers
{
    [Authorize]
    public class TaiKhoanController : Controller
    {
        private readonly Ql_SinhVienEntities _entities;

        public TaiKhoanController()
        {
            _entities = new Ql_SinhVienEntities();
        }

        // GET: Account
        public ActionResult Index(string timkiem = "", string message = "")
        {
            var query = _entities.nguoidungs.AsQueryable();
            if (!string.IsNullOrEmpty(timkiem))
                query = query.Where(x => x.tendangnhap.ToLower().Contains(timkiem.ToLower()) || x.hoten.ToLower().Contains(timkiem.ToLower())
                                    || x.tenlop.ToLower().Contains(timkiem.ToLower()) || x.email.ToLower().Contains(timkiem.ToLower())
                                    || x.chuyennganh.ToLower().Contains(timkiem.ToLower()) || x.cccd.ToLower().Contains(timkiem.ToLower())
                                    || x.sdt.ToLower().Contains(timkiem.ToLower()) || x.quocgia.ToLower().Contains(timkiem.ToLower())
                                    || x.diachi.ToLower().Contains(timkiem.ToLower()) || x.quoctich.ToLower().Contains(timkiem.ToLower()));

            ViewBag.TimKiem = timkiem;
            ViewBag.Message = message;
            return View(query.OrderByDescending(x => x.manguoidung).ToArray());
        }

        public ActionResult Them()
        {
            return View(new nguoidung { ngaysinh = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Them(nguoidung model)
        {
            if (string.IsNullOrEmpty(model.hoten))
                ModelState.AddModelError(nameof(model.hoten), "Tên người dùng không được bỏ trống");
            if (string.IsNullOrEmpty(model.tenlop))
                ModelState.AddModelError(nameof(model.tenlop), "Tên lớp không được bỏ trống");
            if (string.IsNullOrEmpty(model.chuyennganh))
                ModelState.AddModelError(nameof(model.chuyennganh), "Tên chuyên ngành không được bỏ trống");
            if (string.IsNullOrEmpty(model.email))
                ModelState.AddModelError(nameof(model.email), "Địa chỉ email không được bỏ trống");
            if (string.IsNullOrEmpty(model.tendangnhap))
                ModelState.AddModelError(nameof(model.tendangnhap), "Mã không được bỏ trống");
            else if (_entities.nguoidungs.Any(x => x.tendangnhap == model.tendangnhap))
                ModelState.AddModelError(nameof(model.tendangnhap), "Mã này đã được sử dụng!");
            if (model.ngaysinh == default || model.ngaysinh >= DateTime.Now.Date)
                ModelState.AddModelError(nameof(model.ngaysinh), "Ngày sinh không chính xác");
            if (string.IsNullOrEmpty(model.matkhau))
                ModelState.AddModelError(nameof(model.matkhau), "Mật khẩu không được bỏ trống");
            if (!ModelState.IsValid)
                return View(model);

            model.matkhau = CalculateMD5(model.matkhau);
            model.cccd = string.Empty;
            model.sdt = string.Empty;
            model.quocgia = string.Empty;
            model.diachi = string.Empty;
            model.quoctich = string.Empty;

            _entities.nguoidungs.Add(model);
            _entities.SaveChanges();

            return RedirectToAction("Index", new { message = "Thêm tài khoản mới thành công!" });
        }

        public ActionResult Sua(int id)
        {
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.manguoidung == id);
            if (nguoidung == null)
                return RedirectToAction("Index");

            return View(nguoidung);
        }

        [HttpPost]
        public ActionResult Sua(int id, nguoidung model)
        {
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.manguoidung == id);
            if (nguoidung == null)
                return RedirectToAction("Index");

            if (string.IsNullOrEmpty(model.hoten))
                ModelState.AddModelError(nameof(model.hoten), "Tên người dùng không được bỏ trống");
            if (string.IsNullOrEmpty(model.tenlop))
                ModelState.AddModelError(nameof(model.tenlop), "Tên lớp không được bỏ trống");
            if (string.IsNullOrEmpty(model.email))
                ModelState.AddModelError(nameof(model.email), "Địa chỉ email không được bỏ trống");
            if (string.IsNullOrEmpty(model.chuyennganh))
                ModelState.AddModelError(nameof(model.chuyennganh), "Tên chuyên ngành không được bỏ trống");
            if (model.ngaysinh == default || model.ngaysinh >= DateTime.Now.Date)
                ModelState.AddModelError(nameof(model.ngaysinh), "Ngày sinh không chính xác");
            if (!ModelState.IsValid)
                return View(model);

            nguoidung.hoten = model.hoten;
            nguoidung.tenlop = model.tenlop;
            nguoidung.chuyennganh = model.chuyennganh;
            nguoidung.ngaysinh = model.ngaysinh;
            nguoidung.lahocsinh = model.lahocsinh;
            nguoidung.bikhoa = model.bikhoa;
            nguoidung.gioitinh = model.gioitinh;
            if (!string.IsNullOrEmpty(model.matkhau))
                model.matkhau = CalculateMD5(model.matkhau);

            _entities.Entry(nguoidung).State = EntityState.Modified;
            _entities.SaveChanges();

            return RedirectToAction("Index", new { message = "Chỉnh sửa tài khoản thành công!" });
        }

        public ActionResult Xoa(int id)
        {
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.manguoidung == id);
            if (nguoidung == null)
                return RedirectToAction("Index");

            var hoatdongs = _entities.hocsinh_hoatdong.Include(x => x.diemdanhs).Where(x => x.mahocsinh == id).ToList();
            if (hoatdongs.Any())
            {
                var diemdanhs = hoatdongs.SelectMany(x => x.diemdanhs).ToList();
                hoatdongs = hoatdongs.Select(x => { x.diemdanhs = null; return x; }).ToList();
                _entities.diemdanhs.RemoveRange(diemdanhs);
                _entities.hocsinh_hoatdong.RemoveRange(hoatdongs);
            }
            _entities.nguoidungs.Remove(nguoidung);
            _entities.SaveChanges();
            return RedirectToAction("Index", new { message = "Xóa tài khoản thành công" });
        }

        [AllowAnonymous]
        public ActionResult DangNhap(string ReturnUrl = "")
        {
            return View(new LoginModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DangNhap(LoginModel model, string ReturnUrl = "")
        {
            if (!ModelState.IsValid)
                return View(model);

            var taikhoan = _entities.nguoidungs.FirstOrDefault(x => model.Username == x.tendangnhap);
            if (taikhoan == null || taikhoan.matkhau != CalculateMD5(model.Password))
            {
                ModelState.AddModelError(nameof(model.Username), "Tài khoản hoặc mật khẩu không chính xác!");
                return View(model);
            }
            if(taikhoan.bikhoa)
            {
                ModelState.AddModelError(nameof(model.Username), "Tài khoản của bạn đang bị khóa, vui lòng thử lại sau!");
                return View(model);
            }    
            FormsAuthentication.SetAuthCookie(model.Username, false);
            if (!string.IsNullOrEmpty(ReturnUrl))
                return Redirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction(nameof(DangNhap));
        }

        public ActionResult CuaToi(string message = "")
        {
            var nguoidung = _entities.nguoidungs.Include(x => x.hocsinh_hoatdong.Select(h => h.hoatdong)).FirstOrDefault(x => x.tendangnhap == User.Identity.Name);
            ViewBag.Message = message;
            return View(nguoidung);
        }

        [HttpPost]
        public ActionResult CuaToi(nguoidung model)
        {
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.tendangnhap == User.Identity.Name);
            nguoidung.gioitinh = model.gioitinh;
            nguoidung.email = model.email;
            nguoidung.ngaysinh = model.ngaysinh;
            nguoidung.cccd = model.cccd;
            nguoidung.sdt = model.sdt;
            nguoidung.quoctich = model.quoctich;
            nguoidung.diachi = model.diachi;
            nguoidung.quocgia = model.quocgia;
            _entities.Entry(nguoidung).State = EntityState.Modified;
            _entities.SaveChanges();

            return RedirectToAction("CuaToi", "TaiKhoan", new { message = "Thay đổi thông tin thành công" });
        }

        [HttpPost]
        public ActionResult DoiMatKhau(ChangePasswordModel model)
        {
            if (model.MatKhauMoi != model.NhacLaiMatKhau)
                return RedirectToAction("CuaToi", "TaiKhoan", new { message = "Mật khẩu không trùng khớp" });
            var nguoidung = _entities.nguoidungs.FirstOrDefault(x => x.tendangnhap == User.Identity.Name);
            if (nguoidung.matkhau != CalculateMD5(model.MatKhau))
                return RedirectToAction("CuaToi", "TaiKhoan", new { message = "Mật khẩu không chính xác" });

            nguoidung.matkhau = CalculateMD5(model.MatKhauMoi);
            _entities.Entry(nguoidung).State = EntityState.Modified;
            _entities.SaveChanges();
            return RedirectToAction("CuaToi", "TaiKhoan", new { message = "Thay đổi mật khẩu thành công" });
        }

        private string CalculateMD5(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}