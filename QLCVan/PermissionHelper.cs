using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLCVan
{
    public class PermissionHelper
    {
        public static bool HasPermission(string MaQuyen)
        {
            var list = HttpContext.Current.Session["ListQuyen"] as List<string>;
            if (list == null) return false;
            return list.Contains(MaQuyen);
        }

        public static void ReSyncPermission()
        {
            var context = HttpContext.Current;
            if (context.Session["MaNguoiDung"] == null)
            {
                return;
            }
            string MaNguoiDung = context.Session["MaNguoiDung"].ToString();
            using (var db = new InfoDataContext())
            {
                context.Session.Remove("ListQuyen");
                var listQuyen = (
                    from nd in db.tblNguoiDungs
                    join cv in db.tblChucVus on nd.MaChucVu equals cv.MaChucVu
                    join cvnq in db.tblChucVu_tblNhomQuyens on cv.MaChucVu equals cvnq.MaChucVu
                    join nq in db.tblNhomQuyens on cvnq.MaNhomQuyen equals nq.MaNhomQuyen
                    join nqq in db.tblNhomQuyen_tblQuyens on nq.MaNhomQuyen equals nqq.MaNhomQuyen
                    join q in db.tblQuyens on nqq.MaQuyen equals q.MaQuyen
                    where nd.MaNguoiDung == MaNguoiDung
                    select q.MaQuyen

               ).Distinct().ToList();

                context.Session["ListQuyen"] = listQuyen;
            }
        }
    }
}