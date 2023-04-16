using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLtour.Models;
using System.IO;

namespace QLtour.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult CSDatTour()
        {
            return View();
        }
        public ActionResult CSBaoMat()
        {
            return View();
        }
        //
        // GET: /Home/
        DataClasses1DataContext database = new DataClasses1DataContext();
        //======================== Menu =========================
        public ActionResult menuDiaDiemDuLich()
        {
            List<DIEMTHAMQUAN> a = database.DIEMTHAMQUANs.ToList();
            return PartialView(a);
        }

        //======================== Xem chi tiết =========================
        public ActionResult XemChiTiet_DiaDanh(string id)
        {
            List<TOUR> a = database.TOURs.Where(t => t.MaDiaDanh == id).ToList();
            DIEMTHAMQUAN b = database.DIEMTHAMQUANs.FirstOrDefault(t => t.MaDiaDanh == id);
            ViewBag.DTQ = b.HinhAnh;
            ViewBag.TDD = b.TenDiaDanh;
            //ViewBag.Gia = b.ChiPhi;
            return View(a);
        }

        //======================== Tìm kiếm =========================
        public ActionResult TimKiem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XL_TimKiem(FormCollection fc)
        {
            string tendiadiem = fc["txtdiachi"].ToLower();
            string chude = fc["txtchude"].ToLower();
            List<DIEMTHAMQUAN> lst = database.DIEMTHAMQUANs.ToList();
            if(!string.IsNullOrEmpty(tendiadiem))
            {
                lst = lst.Where(l => l.Diachi.ToLower().Contains(tendiadiem)).ToList();
            }

             if(!string.IsNullOrEmpty(chude))
            {
                lst = lst.Where(l => l.TenDiaDanh.ToLower().Contains(chude)).ToList();
            }
            return View("Index", lst);
        }

        //======================== Hiển thị =========================
        public ActionResult HTSP_TheoDD(string name)
        {
            List<DIEMTHAMQUAN> lst = database.DIEMTHAMQUANs.Where(t => t.Diachi == name).ToList();
            return View("Index", lst);
        }
        public ActionResult Index()
        {
            List<DIEMTHAMQUAN> a = database.DIEMTHAMQUANs.ToList();
            return View(a);
        }

        //============================================================================================================================

        //======================== Thêm tour =========================
        public ActionResult ThemTour()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemTour(TOUR tour)
        {
            ViewBag.MaNV = new SelectList(database.NHANVIENs.ToList(), "MaNV", "HoTen");
            database.TOURs.InsertOnSubmit(tour);
            database.SubmitChanges();
            return RedirectToAction("Index");
        }
        //======================== Xoá tour =========================
        public ActionResult XoaTour()
        {
            return View();
        }
        //======================== Sửa tour =========================
        public ActionResult SuaTour()
        {
            return View();
        }

        //============================================================================================================================

        //======================== Thêm địa danh =========================
        public ActionResult ThemDD()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemDD(DIEMTHAMQUAN diemthamquan, HttpPostedFileBase fileupload)
        {
            if (fileupload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/HinhAnhDD"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    diemthamquan.HinhAnh = fileName;
                    database.DIEMTHAMQUANs.InsertOnSubmit(diemthamquan);
                    database.SubmitChanges();
                }
                return RedirectToAction("Index");

            }
        }
        //======================== Xoá địa danh =========================
        public ActionResult XoaDD()
        {
            return View();
        }
        //======================== Sửa địa danh =========================
        public ActionResult SuaDD()
        {
            return View();
        }
        //============================================================================================================================

        //======================= Giỏ hàng ========================
        public GioHang LayGioHang()
        {
            GioHang gio = (GioHang)Session["gh"];
            if (gio == null)
            {
                gio = new GioHang();
            }
            return gio;
        }
        public void LuuGioHang(GioHang gio)
        {
            Session["gh"] = gio;
        }
        public ActionResult ChonMua(string id)
        {
            GioHang g = LayGioHang();
            Item i = g.dsSP.FirstOrDefault(t => t.MaDD == id);
            if (i == null)
            {
                Item x = new Item(id);
                g.Them(x);
            }
            else
            {
                i.SoLuongMua++;
            }
            LuuGioHang(g);
            return RedirectToAction("Index");
        }
        public ActionResult Xoa(string id)
        {
            GioHang g = LayGioHang();
            Item i = g.dsSP.FirstOrDefault(t => t.MaDD == id);
            g.Xoa(i);
            LuuGioHang(g);
            return RedirectToAction("XemGioHang");
        }
        public ActionResult XemGioHang()
        {
            GioHang g = LayGioHang();
            List<Item> dsTour = g.dsSP;
            return View(dsTour);
        }
        //======================== Đăng nhập/ đăng ký =========================


        //======================== Quản lý ===================================

        public ActionResult QuanLyDD()
        {
            List<DIEMTHAMQUAN> a = database.DIEMTHAMQUANs.ToList();
            return View(a);
        }
        public ActionResult QuanLyTour()
        {
            List<TOUR> a = database.TOURs.ToList();
            return View(a);
        }
    }
}
