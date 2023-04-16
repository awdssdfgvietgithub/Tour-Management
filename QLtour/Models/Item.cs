using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QLtour.Models;

namespace QLtour.Models
{
    public class Item
    {
        DataClasses1DataContext database = new DataClasses1DataContext();
        public string MaDD { get; set; }
        public string TenDD { get; set; }
        public string AnhDD { get; set; }
        public double GiaBan { get; set; }
        //public string TenHDV { get; set; }
        public int SoLuongMua { get; set; }
        public Item(string mdd)
        {
            MaDD = mdd;
            DIEMTHAMQUAN t = database.DIEMTHAMQUANs.FirstOrDefault(i => i.MaDiaDanh == mdd);
            TenDD = t.TenDiaDanh;
            AnhDD = t.HinhAnh;
            //GiaBan = Convert.ToDouble(t.ChiPhi.ToString());
            //TenHDV = database.NHANVIENs.FirstOrDefault(i => i.MaNV == database.TOURs.FirstOrDefault(e => e.MaDiaDanh == t.MaDiaDanh).MaHDV).HoTen;
            SoLuongMua = 1;
        }
        public Item(string mdd, string ten, string hinh, int sl)
        {
            MaDD = mdd;
            TenDD = ten;
            AnhDD = hinh;
            //GiaBan = Convert.ToDouble(t.ChiPhi.ToString());
            //TenHDV = database.NHANVIENs.FirstOrDefault(i => i.MaNV == database.TOURs.FirstOrDefault(e => e.MaDiaDanh == t.MaDiaDanh).MaHDV).HoTen;
            SoLuongMua = sl;
        }
        public double ThanhTien
        {
            get { return SoLuongMua * GiaBan; }
        }
    }
}