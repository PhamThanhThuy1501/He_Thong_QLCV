using System;
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
                LoadLoaiCongVan(); // ✅ load dropdown từ DB
                LoadData();
            }

        }

        // ✅ HÀM TẢI LOẠI CÔNG VĂN TỪ DB
        private void LoadLoaiCongVan()
        {
            var loaiCVs = db.tblLoaiCVs
                            .OrderBy(x => x.TenLoaiCV)
                            .Select(x => new { x.MaLoaiCV, x.TenLoaiCV })
                            .ToList();

            ddlLoai.DataSource = loaiCVs;
            ddlLoai.DataTextField = "TenLoaiCV";
            ddlLoai.DataValueField = "MaLoaiCV";
            ddlLoai.DataBind();

            // ✅ thêm dòng "--Tất cả--" giống bản cũ
            ddlLoai.Items.Insert(0, new ListItem("-- Tất cả --", ""));
        }
        private void LoadData()
        {
            // --- Lấy mã người dùng đăng nhập ---
            if (Session["MaNguoiDung"] == null)
                return;
            //if (PermissionHelper.HasPermission(maQuyenYeuCau))
            //{
            //    var q = from g in db.tblNoiDungCVs
            //            join h in db.tblLoaiCVs on g.MaLoaiCV equals h.MaLoaiCV
            //            select new { g, h };

            //    var data = q
            //         .OrderByDescending(x => x.g.NgayGui)
            //         .Select(x => new
            //         {
            //             x.g.MaCV,
            //             x.g.SoCV,
            //             TenLoaiCV = x.h.TenLoaiCV,
            //             x.g.NgayGui,
            //             TieuDeCV = x.g.TieuDeCV.Length > 50 ? x.g.TieuDeCV.Substring(0, 50) + "..." : x.g.TieuDeCV,
            //             x.g.CoQuanBanHanh,
            //             x.g.GhiChu,
            //             x.g.NgayBanHanh,
            //             x.g.NguoiKy,
            //             x.g.NoiNhan,
            //             TrichYeuND = x.g.TrichYeuND.Length > 200 ? x.g.TrichYeuND.Substring(0, 200) + "..." : x.g.TrichYeuND,
            //             x.g.TrangThai,         // bool
            //             x.g.GuiHayNhan         // int (0: đi, 1: đến)
            //         });

            //    GridView1.DataSource = data;
            //    GridView1.DataBind();
            //}
            //else
            //{
            var maNguoiDung = (Session["MaNguoiDung"] as string)?.Trim();
            if (string.IsNullOrWhiteSpace(maNguoiDung))
            {
                Response.Redirect("Dangnhap.aspx");
                return;
            }
            if (PermissionHelper.HasPermission(maQuyenXemToanBoCongVan))
            {
                var allCv = from cv in db.tblNoiDungCVs
                            join loai in db.tblLoaiCVs on cv.MaLoaiCV equals loai.MaLoaiCV
                            orderby cv.NgayGui descending
                            select new
                            {
                                cv.MaCV,
                                cv.SoCV,
                                cv.NgayGui,
                                TieuDeCV = cv.TieuDeCV.Length > 50 ? cv.TieuDeCV.Substring(0, 50) + "..." : cv.TieuDeCV,
                                TrichYeuND = cv.TrichYeuND.Length > 200 ? cv.TrichYeuND.Substring(0, 200) + "..." : cv.TrichYeuND,
                                loai.TenLoaiCV,
                                cv.TrangThai,
                                VaiTro = "Toàn hệ thống"
                            };
                GridView1.DataSource = allCv.ToList();
                GridView1.DataBind();
                return;
            }
            var congVanGui = from cv in db.tblNoiDungCVs
                             join loai in db.tblLoaiCVs on cv.MaLoaiCV equals loai.MaLoaiCV
                             where cv.MaNguoiGui == maNguoiDung.ToString()
                             select new
                             {
                                 cv.MaCV,
                                 cv.SoCV,
                                 cv.NgayGui,
                                 cv.TieuDeCV,
                                 cv.TrichYeuND,
                                 loai.TenLoaiCV,
                                 TrangThai = cv.TrangThai,
                                 VaiTro = "Người gửi"
                             };

            var congVanNhan = from gn in db.tblGuiNhans
                              join cv in db.tblNoiDungCVs on gn.MaCV equals cv.MaCV
                              join loai in db.tblLoaiCVs on cv.MaLoaiCV equals loai.MaLoaiCV
                              where gn.MaNguoiNhan == maNguoiDung.ToString()
                              select new
                              {
                                  cv.MaCV,
                                  cv.SoCV,
                                  cv.NgayGui,
                                  cv.TieuDeCV,
                                  cv.TrichYeuND,
                                  loai.TenLoaiCV,
                                  TrangThai = gn.TrangThaiNhan,
                                  VaiTro = "Người nhận"
                              };

            var allData = congVanGui.Concat(congVanNhan)
                                    .OrderByDescending(x => x.NgayGui)
                                    .ToList();

            GridView1.DataSource = allData.Select(x => new
            {
                x.MaCV,
                x.SoCV,
                x.NgayGui,
                TieuDeCV = x.TieuDeCV.Length > 50 ? x.TieuDeCV.Substring(0, 50) + "..." : x.TieuDeCV,
                TrichYeuND = x.TrichYeuND.Length > 200 ? x.TrichYeuND.Substring(0, 200) + "..." : x.TrichYeuND,
                //x.TenLoaiCV,
                x.TrangThai,
                x.VaiTro
            }).ToList();
            GridView1.DataBind();
            //}

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

            if (!string.IsNullOrEmpty(loai))
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

                // Xóa file đính kèm trước
                var fileDinhKemList = db.tblFileDinhKems.Where(f => f.MaCV == maCv).ToList();
                foreach (var file in fileDinhKemList)
                {
                    db.tblFileDinhKems.DeleteOnSubmit(file);
                }

                // Xóa nội dung công văn chính
                var cv = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCv);
                if (cv != null)
                {
                    db.tblNoiDungCVs.DeleteOnSubmit(cv);
                    db.SubmitChanges();

                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "deleteSuccess",
                        "alert('Đã xóa công văn thành công!');",
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
                        "alert(' Không tìm thấy công văn cần xóa!');",
                        true
                    );
                }
            }
            catch (Exception ex)
            {
                // Ghi log (nếu cần)
                System.Diagnostics.Debug.WriteLine("Lỗi khi xóa công văn: " + ex.Message);

                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "deleteError",
                    "alert(' Có lỗi xảy ra khi xóa công văn!');",
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
                        Response.Redirect("~/SuaCV.aspx?id=" + maCV);
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
