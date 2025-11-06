using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class Trangchu : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        string maQuyenYeuCau = "RAll";
        string maQuyenXemToanBoCongVan = "Q016"; // Quyền xem toàn bộ

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

            // Áp UI mỗi vòng đời để nút đúng quyền & trạng thái
            ApplyPermissionUI();
            UpdateToggleButtonsUI();
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
            var all = (from cv in db.tblNoiDungCVs
                       join loai in db.tblLoaiCVs on cv.MaLoaiCV equals loai.MaLoaiCV
                       orderby cv.NgayGui descending
                       select new
                       {
                           cv.MaCV,
                           cv.SoCV,
                           loai.TenLoaiCV,
                           cv.NgayGui,
                           TieuDeCV = cv.TieuDeCV.Length > 50 ? cv.TieuDeCV.Substring(0, 50) + "..." : cv.TieuDeCV,
                           cv.CoQuanBanHanh,
                           cv.GhiChu,
                           cv.NgayBanHanh,
                           cv.NguoiKy,
                           cv.NoiNhan,
                           TrichYeuND = cv.TrichYeuND.Length > 200 ? cv.TrichYeuND.Substring(0, 200) + "..." : cv.TrichYeuND,
                           cv.TrangThai
                       }).ToList();

            GridView1.DataSource = all;
            GridView1.DataBind();
        }

        protected void GridView1_PageIndexChanging1(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;

            bool viewAll = ViewState["ViewAll"] as bool? == true;
            if (viewAll && PermissionHelper.HasPermission(maQuyenXemToanBoCongVan))
                LoadAllData();
            else
                LoadData();
        }

        /* ===================== TÌM KIẾM ===================== */

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
            string loai = ddlLoai.SelectedValue; // "" = tất cả; "0" = đi; "1" = đến; ...

            DateTime fromDate, toDate;
            IQueryable<CVLoaiCV> q;

            // Nếu có quyền xem toàn bộ (Q016) => nguồn là tất cả
            if (PermissionHelper.HasPermission(maQuyenXemToanBoCongVan) && (ViewState["ViewAll"] as bool? == true))
            {
                q = from cv in db.tblNoiDungCVs
                    join loaiCV in db.tblLoaiCVs on cv.MaLoaiCV equals loaiCV.MaLoaiCV
                    select new CVLoaiCV { cv = cv, loaiCV = loaiCV };
            }
            else
            {
                // Chỉ công văn liên quan
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

            if (!string.IsNullOrEmpty(loai) && int.TryParse(loai, out int loaiCVVal))
                q = q.Where(x => x.cv.MaLoaiCV == loaiCVVal);

            if (DateTime.TryParse(txtFromDate.Text.Trim(), out fromDate))
                q = q.Where(x => x.cv.NgayGui >= fromDate.Date); // >= 00:00

            if (DateTime.TryParse(txtToDate.Text.Trim(), out toDate))
            {
                DateTime toNext = toDate.Date.AddDays(1);        // < ngày kế tiếp
                q = q.Where(x => x.cv.NgayGui < toNext);
            }

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
                })
                .ToList();

            // Khi bấm Tìm kiếm, mình coi như đang lọc theo điều kiện hiện tại
            ViewState["ViewAll"] = (ViewState["ViewAll"] as bool? == true) && PermissionHelper.HasPermission(maQuyenXemToanBoCongVan);
            GridView1.PageIndex = 0;
            GridView1.DataSource = data;
            GridView1.DataBind();

            UpdateToggleButtonsUI();
            ApplyPermissionUI();
        }

        /* ===================== 2 NÚT CHUYỂN CHẾ ĐỘ ===================== */

        // Xem toàn bộ (chỉ khi có Q016)
        protected void btnViewAll_Click(object sender, EventArgs e)
        {
            if (!PermissionHelper.HasPermission(maQuyenXemToanBoCongVan))
            {
                Alert("Bạn không có quyền xem toàn bộ công văn!");
                return;
            }

            // Xóa bộ lọc UI
            TextBox1.Text = string.Empty;
            txtTieuDe.Text = string.Empty;
            if (ddlLoai.Items.Count > 0) ddlLoai.SelectedIndex = 0;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;

            ViewState["ViewAll"] = true;
            GridView1.PageIndex = 0;
            LoadAllData();

            UpdateToggleButtonsUI();
            ApplyPermissionUI();
        }

        // Xem công văn của tôi (luôn cho phép)
        protected void btnMyOnly_Click(object sender, EventArgs e)
        {
            ViewState["ViewAll"] = false;
            GridView1.PageIndex = 0;
            LoadData();

            UpdateToggleButtonsUI();
            ApplyPermissionUI();
        }

        /* ===================== XÓA & HÀNH ĐỘNG ===================== */

        private void XoaCongVan(string maCv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maCv))
                {
                    Alert("Mã công văn không hợp lệ!");
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
                        "showToast('Đã xóa công văn thành công!', 'text-bg-success');",
                        true
                    );

                    bool viewAll = ViewState["ViewAll"] as bool? == true;
                    if (viewAll && PermissionHelper.HasPermission(maQuyenXemToanBoCongVan))
                        LoadAllData();
                    else
                        LoadData();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "notFound",
                        "showToast('Không tìm thấy công văn cần xóa!', 'text-bg-warning');",
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
                    "showToast('Có lỗi xảy ra khi xóa công văn!', 'text-bg-danger');",
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
                    if (cv == null)
                    {
                        Alert("Không tìm thấy công văn!");
                        return;
                    }

                    if (!string.IsNullOrEmpty(cv.NguoiDuyet))
                        Response.Redirect($"CTCVDuyet.aspx?id={maCV}");
                    else
                        Response.Redirect($"CTCVKhongDuyetDaGui.aspx?id={maCV}");
                    break;

                case "EditCV":
                    if (coQuyenSua)
                    {
                        var cv1 = (from c in db.tblNoiDungCVs
                                   where c.MaCV == maCV
                                   select c).FirstOrDefault();
                        if (cv1 == null)
                        {
                            Alert("Không tìm thấy công văn!");
                            return;
                        }

                        if (cv1.MaNguoiGui == (Session["MaNguoiDung"]?.ToString() ?? ""))
                        {
                            if (cv1.MaNguoiGui == Session["MaNguoiDung"].ToString())
                            {
                                if (!string.IsNullOrEmpty(cv1.NguoiDuyet))
                                {
                                    Alert("Công văn đã được duyệt không thể sửa!");
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
                                    "noPermissionEditOwn",
                                    "showToast('Bạn không có quyền sửa công văn này!', 'text-bg-warning');",
                                    true
                                );
                            }
                        }
                        else
                        {
                            Alert("Bạn không có quyền sửa công văn!");
                        }
                    }
                    else
                    {
                        Alert("Bạn không có quyền sửa công văn!");
                    }
                    break;

                case "DeleteCV":
                    if (coQuyenXoa)
                        XoaCongVan(maCV);
                    else
                        Alert("Bạn không có quyền xoá công văn!");
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
