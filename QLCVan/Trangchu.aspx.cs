using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class Trangchu : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        string maQuyenYeuCau = "RAll";
        string maQuyenXemToanBoCongVan = "Q016";

        protected void Page_Load(object sender, EventArgs e)
        {


            if (Session["TenDN"] == null)
            {
                Response.Redirect("Dangnhap.aspx");
            }

            if (!IsPostBack)
            {
                LoadData();
            }

        }

        private void LoadData()
        {
            if (Session["MaNguoiDung"] == null)
                return;

            string maNguoiDung = Session["MaNguoiDung"].ToString().Trim();

            // 🔹 Lấy mã đơn vị của user
            string maDonViNguoiDung = db.tblNguoiDungs
                                        .Where(x => x.MaNguoiDung == maNguoiDung)
                                        .Select(x => x.MaDonVi)
                                        .FirstOrDefault();

            if (string.IsNullOrEmpty(maDonViNguoiDung))
            {
                GridView1.DataSource = null;
                GridView1.DataBind();
                return;
            }

            // 🔹 Công văn do user gửi
            var congVanGui = from cv in db.tblNoiDungCVs
                             join loai in db.tblLoaiCVs on cv.MaLoaiCV equals loai.MaLoaiCV
                             where cv.MaNguoiGui == maNguoiDung
                             select new
                             {
                                 cv.MaCV,
                                 cv.SoCV,
                                 loai.TenLoaiCV,
                                 cv.TieuDeCV,
                                 cv.TrichYeuND,
                                 cv.TrangThai,
                                 cv.NgayGui,
                                 VaiTro = "Người gửi"
                             };

            // 🔹 Công văn gửi đến đơn vị của user
            var congVanNhan = from cvdv in db.tblNoiDungCV_DonViNhans
                              join cv in db.tblNoiDungCVs on cvdv.MaCV equals cv.MaCV
                              join loai in db.tblLoaiCVs on cv.MaLoaiCV equals loai.MaLoaiCV
                              where cvdv.MaDonViNhan == maDonViNguoiDung
                              select new
                              {
                                  cv.MaCV,
                                  cv.SoCV,
                                  loai.TenLoaiCV,
                                  cv.TieuDeCV,
                                  cv.TrichYeuND,
                                  cv.TrangThai,
                                  cv.NgayGui,
                                  VaiTro = "Đơn vị nhận"
                              };

            // 🔹 Hợp nhất kết quả và sắp xếp mới nhất lên đầu
            var allData = congVanGui.Concat(congVanNhan)
                .GroupBy(x => x.MaCV)
                .Select(g => g.First()) // chỉ lấy 1 bản ghi duy nhất mỗi MaCV
                .OrderByDescending(x => x.NgayGui)
                .ToList();


            // 🔹 Gán vào GridView
            GridView1.DataSource = allData;
            GridView1.DataBind();
        }



        protected void lnk_Xoa_Click(object sender, EventArgs e)
        {
            //string permisson = (Session["QuyenHan"] as string)?.Trim();

            //if (string.Equals(permisson, "Admin", StringComparison.OrdinalIgnoreCase))
            //{
            //    LinkButton lnk = sender as LinkButton;
            //    string maCv = lnk.CommandArgument;

            //    foreach (tblFileDinhKem item in db.tblFileDinhKems.Where(f => f.MaCV == maCv))
            //        db.tblFileDinhKems.DeleteOnSubmit(item);

            //    tblNoiDungCV cv = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCv);
            //    if (cv != null)
            //    {
            //        db.tblNoiDungCVs.DeleteOnSubmit(cv);
            //        db.SubmitChanges();
            //        LoadData();
            //    }
            //}
        }

        protected void GridView1_PageIndexChanging1(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            LoadData();
        }

        // DÙNG LẠI: nếu cần text đơn giản
        public string kttrangthai(object obj)
        {
            bool trangthai = bool.Parse(obj.ToString());
            return trangthai ? "Đã duyệt" : "Chưa duyệt";
        }

        // **TRẠNG THÁI HIỂN THỊ DẠNG BADGE – CỐ ĐỊNH THEO 3 GIÁ TRỊ**
        // - true  -> "Đã gửi" (xanh đặc)
        // - false + GuiHayNhan = 0 -> "Không duyệt" (đỏ viền)
        // - false + GuiHayNhan != 0 -> "Đang trình" (cam viền)
        public string GetTrangThai(object oTrangThai, object oGuiHayNhan)
        {
            bool trangThai = false;
            int guiHayNhan = -1;
            if (oTrangThai != null) bool.TryParse(oTrangThai.ToString(), out trangThai);
            if (oGuiHayNhan != null) int.TryParse(oGuiHayNhan.ToString(), out guiHayNhan);

            if (trangThai)
                return "<span class='badge badge--success'>Đã gửi</span>";

            if (guiHayNhan == 0)
                return "<span class='badge badge--danger'>Không duyệt</span>";

            return "<span class='badge badge--warning'>Đang trình</span>";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string maNguoiDung = Session["MaNguoiDung"]?.ToString();
            if (string.IsNullOrWhiteSpace(maNguoiDung))
            {
                Response.Redirect("Dangnhap.aspx");
                return;
            }

            string keyword = TextBox1.Text.Trim();
            string tieuDe = txtTieuDe.Text.Trim();
            string loai = ddlLoai.SelectedValue;
            DateTime fromDate, toDate;


            IQueryable<CVLoaiCV> q;

            if (PermissionHelper.HasPermission("Q016"))
            {
                q = from cv in db.tblNoiDungCVs
                    join loaiCV in db.tblLoaiCVs on cv.MaLoaiCV equals loaiCV.MaLoaiCV
                    select new CVLoaiCV { cv = cv, loaiCV = loaiCV };
            }
            else
            {
                var congVanGui = from cv in db.tblNoiDungCVs
                                 join loaiCV in db.tblLoaiCVs on cv.MaLoaiCV equals loaiCV.MaLoaiCV
                                 where cv.MaNguoiGui == maNguoiDung
                                 select new CVLoaiCV { cv = cv, loaiCV = loaiCV };

                var congVanNhan = from gn in db.tblGuiNhans
                                  join cv in db.tblNoiDungCVs on gn.MaCV equals cv.MaCV
                                  join loaiCV in db.tblLoaiCVs on cv.MaLoaiCV equals loaiCV.MaLoaiCV
                                  where gn.MaNguoiNhan == maNguoiDung
                                  select new CVLoaiCV { cv = cv, loaiCV = loaiCV };

                q = congVanGui.Concat(congVanNhan);
            }

            if (!string.IsNullOrEmpty(keyword))
                q = q.Where(x => x.cv.SoCV.Contains(keyword));

            if (!string.IsNullOrEmpty(tieuDe))
                q = q.Where(x => x.cv.TieuDeCV.Contains(tieuDe));

            if (!string.IsNullOrEmpty(loai) && loai != "0")
            {
                int loaiCV = int.Parse(loai);
                q = q.Where(x => x.cv.MaLoaiCV == loaiCV);
            }

            if (DateTime.TryParse(txtFromDate.Text.Trim(), out fromDate))
                q = q.Where(x => x.cv.NgayGui >= fromDate);

            if (DateTime.TryParse(txtToDate.Text.Trim(), out toDate))
                q = q.Where(x => x.cv.NgayGui <= toDate);

            var data = q
                .OrderByDescending(x => x.cv.NgayGui)
                .Select(x => new
                {
                    x.cv.MaCV,
                    x.cv.SoCV,
                    x.loaiCV.TenLoaiCV,
                    x.cv.NgayGui,
                    TieuDeCV = x.cv.TieuDeCV.Length > 50 ? x.cv.TieuDeCV.Substring(0, 50) + "..." : x.cv.TieuDeCV,
                    x.cv.CoQuanBanHanh,
                    x.cv.GhiChu,
                    x.cv.NgayBanHanh,
                    x.cv.NguoiKy,
                    x.cv.NoiNhan,
                    TrichYeuND = x.cv.TrichYeuND.Length > 200 ? x.cv.TrichYeuND.Substring(0, 200) + "..." : x.cv.TrichYeuND,
                    x.cv.TrangThai
                });

            GridView1.DataSource = data.ToList();
            GridView1.DataBind();
        }
        private void XoaCongVan(string maCv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maCv))
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "invalidId",
                        "alert('Mã công văn không hợp lệ!');",
                        true
                    );
                    return;
                }

                // 🔹 Xóa trước trong các bảng phụ có ràng buộc FK
                var fileDinhKemList = db.tblFileDinhKems.Where(f => f.MaCV == maCv).ToList();
                if (fileDinhKemList.Any())
                    db.tblFileDinhKems.DeleteAllOnSubmit(fileDinhKemList);

                var donViNhanList = db.tblNoiDungCV_DonViNhans.Where(d => d.MaCV == maCv).ToList();
                if (donViNhanList.Any())
                    db.tblNoiDungCV_DonViNhans.DeleteAllOnSubmit(donViNhanList);

                var guiNhanList = db.tblGuiNhans.Where(g => g.MaCV == maCv).ToList();
                if (guiNhanList.Any())
                    db.tblGuiNhans.DeleteAllOnSubmit(guiNhanList);

                // 🔹 Sau đó mới xóa nội dung công văn chính
                var cv = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCv);
                if (cv != null)
                {
                    db.tblNoiDungCVs.DeleteOnSubmit(cv);
                    db.SubmitChanges();

                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "deleteSuccess",
                        "alert('Đã xóa công văn và dữ liệu liên quan thành công!');",
                        true
                    );

                    LoadData();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "notFound",
                        "alert('Không tìm thấy công văn cần xóa!');",
                        true
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi khi xóa công văn: " + ex.Message);

                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "deleteError",
                    $"alert('Lỗi khi xóa công văn: {ex.Message.Replace("'", "\\'")}');",
                    true
                );
            }
        }

        protected void lnk_Command(object sender, CommandEventArgs e)
        {
            string maCV = e.CommandArgument.ToString();
            bool coQuyenSua = PermissionHelper.HasPermission("Q003");
            bool coQuyenXoa = PermissionHelper.HasPermission("Q004");

            switch (e.CommandName)
            {
                case "ViewCV":
                    var cv = (from c in db.tblNoiDungCVs
                              where c.MaCV == maCV
                              select c).FirstOrDefault();
                    if (!string.IsNullOrEmpty(cv.NguoiDuyet))
                    {
                        // Nếu đã có người duyệt
                        Response.Redirect($"CTCVDuyet.aspx?id={maCV}");
                    }
                    else
                    {
                        // Nếu chưa có người duyệt
                        Response.Redirect($"CTCVKhongDuyetDaGui.aspx?id={maCV}");
                    }
                    break;
                case "EditCV":
                    if (coQuyenSua)
                    {
                        var cv1 = (from c in db.tblNoiDungCVs
                                   where c.MaCV == maCV
                                   select c).FirstOrDefault();
                        if (cv1.MaNguoiGui == Session["MaNguoiDung"].ToString())
                        {
                            if (!string.IsNullOrEmpty(cv1.NguoiDuyet))
                            {
                                if (cv1.TrangThai == "Đã được duyệt")
                                {
                                    ScriptManager.RegisterStartupScript(
                                       this,
                                       this.GetType(),
                                       "noPermissionEdit",
                                       "alert('Công văn đã được duyệt không thể sửa!');",
                                       true);
                                }
                                else
                                {
                                    Response.Redirect("~/SuaCongVan.aspx?id=" + maCV);
                                }
                            }
                            else
                            {
                                Response.Redirect("~/SuaCV.aspx?id=" + maCV);
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(
                            this,
                            this.GetType(),
                            "noPermissionEdit",
                            "alert('Bạn không có quyền sửa công văn!');",
                            true
                        );
                        }

                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            this.GetType(),
                            "noPermissionEdit",
                            "alert('Bạn không có quyền sửa công văn!');",
                            true
                        );
                    }
                    break;
                case "DeleteCV":
                    if (coQuyenXoa)
                    {
                        XoaCongVan(maCV);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            this.GetType(),
                            "noPermissionEdit",
                            "alert('Bạn không có quyền xoá công văn!');",
                            true
                        );
                    }
                    break;
            }
        }

    }
    public class CVLoaiCV
    {
        public tblNoiDungCV cv { get; set; }
        public tblLoaiCV loaiCV { get; set; }
    }
}