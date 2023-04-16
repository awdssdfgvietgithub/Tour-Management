using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLtour.Models;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;

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
        [HttpPost]
        public ActionResult XL_ChonMua(FormCollection fc1)
        {
            if (Session["tenTK"] != null)
            {
                GioHang g = LayGioHang();
                Item i = g.dsSP.FirstOrDefault(t => t.MaDD == fc1["id"].ToString());

                if (i == null)
                {
                    Item x = new Item(fc1["id"].ToString(), fc1["tendd"].ToString(), fc1["hinhanh"].ToString(), int.Parse(fc1["SLM"].ToLower()));
                    g.Them(x);
                }
                else
                {
                    i.SoLuongMua += int.Parse(fc1["SLM"].ToLower());
                }
                LuuGioHang(g);
                return RedirectToAction("Index");
            }
            else
            {
                return View("DangNhap");
            }
        }

        //======================== Tìm kiếm =========================
        public ActionResult TimKiem(string search)
        {
            DIEMTHAMQUAN a = database.DIEMTHAMQUANs.FirstOrDefault(t => t.TenDiaDanh.ToLower().Contains(search));
            return View("Index", a);
        }
        public ActionResult TimKiemNC()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XL_TimKiemNC(FormCollection fc)
        {
            
            string tendiadiem = fc["txtdiachi"].ToLower();
            string chude = fc["txtchude"].ToLower();
            List<DIEMTHAMQUAN> lst = database.DIEMTHAMQUANs.ToList();
            if (!string.IsNullOrEmpty(tendiadiem))
            {
                lst = lst.Where(l => l.Diachi.ToLower().Contains(tendiadiem)).ToList();
            }

            if (!string.IsNullOrEmpty(chude))
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
        public ActionResult Index(string search = "")
        {
            List<DIEMTHAMQUAN> a = database.DIEMTHAMQUANs.Where(t => t.TenDiaDanh.Contains(search)).ToList();
            return View(a);
        }

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
        public ActionResult Xoa(string id)
        {
            GioHang g = LayGioHang();
            Item i = g.dsSP.FirstOrDefault(t => t.MaDD == id);
            g.Xoa(i);
            LuuGioHang(g);
            return RedirectToAction("XemGioHang");
        }
        public ActionResult XoaAll()
        {
            GioHang g = LayGioHang();
            g.dsSP.Clear();
            LuuGioHang(g);
            return RedirectToAction("XemGioHang");
        }
    //[HttpPost]
    //    public ActionResult CapNhatGioHang(string id, FormCollection f)
    //    {
    //        GioHang g = LayGioHang();
    //        Item i = g.dsSP.SingleOrDefault(t => t.MaDD == id);
    //        if (i != null)
    //        {
    //            i.SoLuongMua = int.Parse(f["txtSoluong"].ToString());
    //        }         
    //        LuuGioHang(g);
    //        return RedirectToAction("XemGioHang");
    //    }
        public ActionResult XemGioHang()
        {
            GioHang g = LayGioHang();
            List<Item> dsTour = g.dsSP;
            return View(dsTour);
        }
        //======================== Đăng nhập/ đăng ký =========================
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection col)
        {
            if (col["ck_nv"] != null)
            {
                NHANVIEN kh = database.NHANVIENs.FirstOrDefault
                (k => k.MaNV == col["txtTK"] && k.MATKHAU == col["txtMK"]);
                if (kh != null)
                {
                    Session["kh"] = kh;
                    Session["tenTK"] = kh.HoTen;
                    return RedirectToAction("QuanLyDD", "QuanLy");
                }
            }
            else
            {
                KHACHHANG kh = database.KHACHHANGs.FirstOrDefault
                    (k => k.MaKH == col["txtTK"] && k.MATKHAU == col["txtMK"]);
                if (kh != null)
                {
                    Session["kh"] = kh;
                    Session["tenTK"] = kh.HoTen;
                    return RedirectToAction("Index");
                }
            }

            return View();
        }
        public ActionResult DangXuat()
        {
            Session["gh"] = null;
            Session["tenTK"] = null;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DangKy(KHACHHANG kh)
        {
            database.KHACHHANGs.InsertOnSubmit(kh);
            database.SubmitChanges();
            return RedirectToAction("DangNhap");
        }
    }
}
