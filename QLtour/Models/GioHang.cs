using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QLtour.Models;

namespace QLtour.Models
{
    public class GioHang
    {
        public List<Item> dsSP;
        public GioHang()
        {
            dsSP = new List<Item>();
        }

        public void Them(Item x)
        {
            dsSP.Add(x);
        }

        public void Xoa(Item x)
        {
            dsSP.Remove(x);
        }

        public int SLMatHang()
        {
            return dsSP.Count;
        }

        public int TongSL()
        {
            return dsSP.Sum(t => t.SoLuongMua);
        }
        public double TongTien()
        {
            return dsSP.Sum(t => t.ThanhTien);
        }
    }
}