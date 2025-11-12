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
        private string CurrentUserId()   // MaNguoiDung
        {
            return (Session["MaNguoiDung"] as string)?.Trim();
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Dangnhap.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadLoaiCongVan(); // ✅ load dropdown từ DB
                // Mặc định: xem công văn liên quan tài khoản
                ViewState["ViewAll"] = false;
                LoadData();
            }

            // Áp UI mỗi vòng đời để nút đúng quyền & trạng thái
            ApplyPermissionUI();
            UpdateToggleButtonsUI();
            // ---- Toast sau redirect (nếu có) ----
            if (Session["toastMsg"] != null)
            {
                var msg = (Session["toastMsg"] ?? "").ToString();
                var type = (Session["toastType"] ?? "text-bg-success").ToString();

                // an toàn chuỗi cho JS
                msg = System.Web.HttpUtility.JavaScriptStringEncode(msg);

                // dùng key ngẫu nhiên để tránh đụng key giữa các postback
                var key = "toast_" + Guid.NewGuid().ToString("N");

                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    key,
                    $"showToast('{msg}', '{type}');",
                    true
                );

                // xóa để không bắn lại ở lần load sau
                Session.Remove("toastMsg");
                Session.Remove("toastType");
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
        /* ===================== UI Helpers ===================== */

        // alert() an toàn – chống lỗi escape chuỗi
        private void Alert(string message)
        {
            var safe = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                Guid.NewGuid().ToString("N"),
                $"alert('{safe}');",
                true
            );
        }

        // Ẩn/hiện nút theo quyền
        private void ApplyPermissionUI()
        {
            bool canViewAll = PermissionHelper.HasPermission(maQuyenXemToanBoCongVan);

            // Nút Xem toàn bộ chỉ hiện khi có quyền Q016
            if (btnViewAll != null) btnViewAll.Visible = canViewAll;

            // Nút Xem của tôi luôn hiện để quay về chế độ cá nhân
            if (btnMyOnly != null) btnMyOnly.Visible = true;
        }

        // Cập nhật text/cảm quan 2 nút theo trạng thái
        private void UpdateToggleButtonsUI()
        {
            bool viewAll = ViewState["ViewAll"] as bool? == true;

            if (btnViewAll != null)
            {
                btnViewAll.Text = "↺ Xem toàn bộ công văn";
                // btnViewAll.Enable = PermissionHelper.HasPermission(...) (đã Visible theo quyền)
            }

            if (btnMyOnly != null)
            {
                btnMyOnly.Text = "↩ Xem công văn của tôi";
                // Khi đang ở chế độ của tôi, có thể disable nhẹ nhàng nếu muốn:
                // btnMyOnly.Enabled = viewAll;  // (tùy bạn, để mặc định Enabled=true cho dễ thao tác)
            }
        }

        /* ===================== DỮ LIỆU ===================== */

        /// <summary>
        /// Dữ liệu mặc định: công văn do user gửi + công văn user là người nhận
        /// (hợp nhất, bỏ trùng, sắp xếp mới nhất)
        /// </summary>
        private void LoadData()
        {
            string maNguoiDung = Session["MaNguoiDung"] as string;
            if (string.IsNullOrWhiteSpace(maNguoiDung))
                return;

            // Công văn do user gửi
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

            // Công văn user là người nhận (dựa tblGuiNhans)
            var congVanNhan = from gn in db.tblGuiNhans
                              join cv in db.tblNoiDungCVs on gn.MaCV equals cv.MaCV
                              join loai in db.tblLoaiCVs on cv.MaLoaiCV equals loai.MaLoaiCV
                              where gn.MaNguoiNhan == maNguoiDung
                              select new
                              {
                                  cv.MaCV,
                                  cv.SoCV,
                                  loai.TenLoaiCV,
                                  cv.TieuDeCV,
                                  cv.TrichYeuND,
                                  cv.TrangThai,
                                  cv.NgayGui,
                                  VaiTro = "Người nhận"
                              };

            // Hợp nhất & sắp xếp
            var allData = congVanGui.Concat(congVanNhan)
                                    .GroupBy(x => x.MaCV)
                                    .Select(g => g.First())
                                    .OrderByDescending(x => x.NgayGui)
                                    .ToList();

            GridView1.DataSource = allData;
            GridView1.DataBind();
        }

        /// <summary>
        /// Tải toàn bộ công văn trong hệ thống (chỉ khi có quyền Q016)
        /// </summary>
        private void LoadAllData()
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
        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            string maCV = hdDeleteId.Value; // Lấy mã công văn từ HiddenField trong modal

            if (!string.IsNullOrEmpty(maCV))
            {
                XoaCongVan(maCV); // Gọi hàm xóa bạn đã có sẵn
            }
            else
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "noId",
                    "showToast('Không xác định được công văn cần xóa!', 'text-bg-warning');",
                    true
                );
            }
        }
        private void XoaCongVan(string maCv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maCv))
                {
                    Toast("Mã công văn không hợp lệ!", "warning"); return;
                }

                var currentUserId = CurrentUserId();
                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    Toast("Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại!", "warning"); return;
                }

                var cv = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCv);
                if (cv == null) { Toast("Không tìm thấy công văn cần xóa!", "warning"); return; }

                // ✅ So sánh đúng kiểu định danh: MaNguoiDung ↔ MaNguoiGui
                var senderId = (cv.MaNguoiGui ?? "").Trim();
                if (!string.Equals(senderId, currentUserId, StringComparison.OrdinalIgnoreCase))
                {
                    Toast("Chỉ người gửi công văn mới có quyền xóa!", "danger"); return;
                }

                // Xóa phụ thuộc
                var files = db.tblFileDinhKems.Where(f => f.MaCV == maCv).ToList();
                if (files.Count > 0) db.tblFileDinhKems.DeleteAllOnSubmit(files);

                var gn = db.tblGuiNhans.Where(g => g.MaCV == maCv).ToList();
                if (gn.Count > 0) db.tblGuiNhans.DeleteAllOnSubmit(gn);

                db.SubmitChanges();

                // Xóa CV chính
                db.tblNoiDungCVs.DeleteOnSubmit(cv);
                db.SubmitChanges();

                Toast("Đã xóa công văn thành công!", "success");
                LoadData();
            }
            catch (Exception ex)
            {
                Alert("Lỗi khi xóa công văn: " + ex.Message);
            }

            void Toast(string msg, string type) =>
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                    $"showToast('{msg}', 'text-bg-{type}');", true);
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
                    if (cv != null)
                    {
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
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            this.GetType(),
                            "notFoundCV",
                            "showToast('Không tìm thấy công văn!', 'text-bg-warning');",
                            true
                        );
                    }
                    break;

                case "EditCV":
                    if (coQuyenSua)
                    {
                        var cv1 = (from c in db.tblNoiDungCVs
                                   where c.MaCV == maCV
                                   select c).FirstOrDefault();

                        if (cv1 != null)
                        {
                            if (cv1.MaNguoiGui == Session["MaNguoiDung"].ToString())
                            {
                                if (!string.IsNullOrEmpty(cv1.NguoiDuyet))
                                {
                                    if (cv1.TrangThai == "Đã được duyệt")
                                    {
                                        ScriptManager.RegisterStartupScript(
                                           this,
                                           this.GetType(),
                                           "noPermissionEditApproved",
                                           "showToast('Công văn đã được duyệt, không thể sửa!', 'text-bg-warning');",
                                           true
                                        );
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
                                    "noPermissionEditOwn",
                                    "showToast('Bạn không có quyền sửa công văn này!', 'text-bg-warning');",
                                    true
                                );
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(
                                this,
                                this.GetType(),
                                "notFoundEdit",
                                "showToast('Không tìm thấy công văn để sửa!', 'text-bg-warning');",
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
                            "showToast('Bạn không có quyền sửa công văn!', 'text-bg-warning');",
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
                            "noPermissionDelete",
                            "showToast('Bạn không có quyền xoá công văn!', 'text-bg-danger');",
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
