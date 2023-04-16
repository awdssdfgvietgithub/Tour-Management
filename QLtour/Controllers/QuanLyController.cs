using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLtour.Models;
using System.IO;

namespace QLtour.Controllers
{
    public class QuanLyController : Controller
    {
        DataClasses1DataContext database = new DataClasses1DataContext();
        //======================== Quản lý ===================================

        public ActionResult QuanLyDD()
        {
            Session["baoloi"] = "";
            List<DIEMTHAMQUAN> a = database.DIEMTHAMQUANs.ToList();
            return View(a);
        }
        public ActionResult QuanLyTour()
        {
            List<TOUR> a = database.TOURs.ToList();
            return View(a);
        }
        public ActionResult QuanLyKH()
        {
            List<KHACHHANG> a = database.KHACHHANGs.ToList();
            return View(a);
        }
        public ActionResult QuanLyVe()
        {
            List<VE> a = database.VEs.ToList();
            return View(a);
        }
        public ActionResult LichSuMV()
        {
            return View();
        }
        //====================================== QL TOUR ======================================================

        //--- Thêm tour 
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
            return RedirectToAction("QuanLyTour");
        }
        //--- Xoá tour
        [HttpGet]
        public ActionResult XoaTour(string ma)
        {
            TOUR a = database.TOURs.SingleOrDefault(n => n.MaTour == ma);
            //ViewBag.MaTour = a.MaTour;
            if (a == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(a);
        }
        [HttpPost, ActionName("XoaTour")]
        public ActionResult XacnhanxoaTour(string ma)
        {
            TOUR a = database.TOURs.SingleOrDefault(n => n.MaTour == ma);
            ViewBag.MaTour = a.MaTour;
            if (a == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            database.TOURs.DeleteOnSubmit(a);
            database.SubmitChanges();
            return RedirectToAction("QuanLyTour");
        }
        //--- Sửa tour
        public ActionResult SuaTour()
        {
            return View();
        }
        //===================================== ĐỊA DANH ==================================
        //--- Thêm địa danh
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
                return RedirectToAction("QuanLyDD");
            }
        }
        //--- Xoá địa danh
        public ActionResult XoaDD()
        {
            return View();
        }
        [HttpPost]
        public ActionResult XoaDD(string id)
        {
            Session["baoloi"] = "";
            DIEMTHAMQUAN nd_del = database.DIEMTHAMQUANs.Where(m => m.MaDiaDanh == id).First();
            List<TOUR> ktr_kn = database.TOURs.Where(b => b.MaDiaDanh == id).ToList();
            if (ktr_kn.Count() == 0)
            {
                database.DIEMTHAMQUANs.DeleteOnSubmit(nd_del);
                database.SubmitChanges();
                return RedirectToAction("QuanLyDD");
            }
            Session["baoloi"] = "Đã có người hiện đăng ký tour này";
            return View();
        }
        //--- Sửa địa danh
        public ActionResult SuaDD()
        {
            return View();
        }
        //=================================== QL Khách hàng =====================================
        //--- Thêm kh
        //--- Xóa kh
        //--- Sửa kh
    }
}
