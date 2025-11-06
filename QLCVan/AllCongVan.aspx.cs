using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class AllCongVan : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Dangnhap.aspx");
                return;
            }

            // Nếu muốn ràng buộc quyền xem toàn bộ:
            if (!PermissionHelper.HasPermission("Q016"))
            {
                Alert("Bạn không có quyền xem toàn bộ công văn!");
                Response.Redirect("~/Trangchu.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindAll();
            }
        }

        /* ========= LOAD & SEARCH TOÀN BỘ ========= */

        private void BindAll()
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
                           cv.TrichYeuND,
                           cv.TrangThai
                       }).ToList();

            GridViewAll.DataSource = all;
            GridViewAll.DataBind();
        }

        private void BindAllWithFilter()
        {
            string keyword = TextBox1.Text.Trim();
            string tieuDe = txtTieuDe.Text.Trim();
            string loai = ddlLoai.SelectedValue;

            DateTime fromDate, toDate;
            var q = from cv in db.tblNoiDungCVs
                    join loaiCV in db.tblLoaiCVs on cv.MaLoaiCV equals loaiCV.MaLoaiCV
                    select new { cv, loaiCV };

            if (!string.IsNullOrEmpty(keyword))
                q = q.Where(x => x.cv.SoCV.Contains(keyword));

            if (!string.IsNullOrEmpty(tieuDe))
                q = q.Where(x => x.cv.TieuDeCV.Contains(tieuDe));

            if (!string.IsNullOrEmpty(loai) && int.TryParse(loai, out int loaiCVVal))
                q = q.Where(x => x.cv.MaLoaiCV == loaiCVVal);

            if (DateTime.TryParse(txtFromDate.Text.Trim(), out fromDate))
                q = q.Where(x => x.cv.NgayGui >= fromDate.Date);

            if (DateTime.TryParse(txtToDate.Text.Trim(), out toDate))
            {
                var toNext = toDate.Date.AddDays(1);
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
                    x.cv.TrichYeuND,
                    x.cv.TrangThai
                })
                .ToList();

            GridViewAll.DataSource = data;
            GridViewAll.DataBind();
        }

        private bool HasFilter()
        {
            return !string.IsNullOrWhiteSpace(TextBox1.Text)
                || !string.IsNullOrWhiteSpace(txtTieuDe.Text)
                || !string.IsNullOrWhiteSpace(ddlLoai.SelectedValue)
                || !string.IsNullOrWhiteSpace(txtFromDate.Text)
                || !string.IsNullOrWhiteSpace(txtToDate.Text);
        }

        protected void btnSearchAll_Click(object sender, EventArgs e)
        {
            GridViewAll.PageIndex = 0;
            if (HasFilter()) BindAllWithFilter();
            else BindAll();
        }

        protected void GridViewAll_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridViewAll.PageIndex = e.NewPageIndex;
            if (HasFilter()) BindAllWithFilter();
            else BindAll();
        }

        /* ========= THAO TÁC XEM / SỬA / XÓA ========= */

        protected void lnkAll_Command(object sender, CommandEventArgs e)
        {
            string maCV = e.CommandArgument.ToString();
            bool coQuyenSua = PermissionHelper.HasPermission("Q003");
            bool coQuyenXoa = PermissionHelper.HasPermission("Q004");

            switch (e.CommandName)
            {
                case "ViewCV":
                    var cv = db.tblNoiDungCVs.FirstOrDefault(x => x.MaCV == maCV);
                    if (cv == null) { Alert("Không tìm thấy công văn!"); return; }
                    if (!string.IsNullOrEmpty(cv.NguoiDuyet))
                        Response.Redirect($"CTCVDuyet.aspx?id={maCV}");
                    else
                        Response.Redirect($"CTCVKhongDuyetDaGui.aspx?id={maCV}");
                    break;

                case "EditCV":
                    if (!coQuyenSua) { Alert("Bạn không có quyền sửa công văn!"); return; }
                    var cv1 = db.tblNoiDungCVs.FirstOrDefault(x => x.MaCV == maCV);
                    if (cv1 == null) { Alert("Không tìm thấy công văn!"); return; }

                    if (cv1.MaNguoiGui == (Session["MaNguoiDung"]?.ToString() ?? ""))
                    {
                        if (!string.IsNullOrEmpty(cv1.NguoiDuyet))
                        {
                            if (cv1.TrangThai == "Đã được duyệt") { Alert("Công văn đã được duyệt không thể sửa!"); }
                            else { Response.Redirect("~/SuaCongVan.aspx?id=" + maCV); }
                        }
                        else
                        {
                            Response.Redirect("~/SuaCV.aspx?id=" + maCV);
                        }
                    }
                    else
                    {
                        Alert("Bạn không có quyền sửa công văn!");
                    }
                    break;

                case "DeleteCV":
                    if (!coQuyenXoa) { Alert("Bạn không có quyền xoá công văn!"); return; }
                    XoaCongVan(maCV);
                    if (HasFilter()) BindAllWithFilter();
                    else BindAll();
                    break;
            }
        }

        private void XoaCongVan(string maCv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maCv)) { Alert("Mã công văn không hợp lệ!"); return; }

                var fileDinhKemList = db.tblFileDinhKems.Where(f => f.MaCV == maCv).ToList();
                if (fileDinhKemList.Any()) db.tblFileDinhKems.DeleteAllOnSubmit(fileDinhKemList);

                var guiNhanList = db.tblGuiNhans.Where(g => g.MaCV == maCv).ToList();
                if (guiNhanList.Any()) db.tblGuiNhans.DeleteAllOnSubmit(guiNhanList);

                var cv = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == maCv);
                if (cv != null)
                {
                    db.tblNoiDungCVs.DeleteOnSubmit(cv);
                    db.SubmitChanges();
                    Alert("Đã xóa công văn và dữ liệu liên quan thành công!");
                }
                else
                {
                    Alert("Không tìm thấy công văn cần xóa!");
                }
            }
            catch (Exception ex)
            {
                Alert("Lỗi khi xóa công văn: " + ex.Message);
            }
        }

        private void Alert(string message)
        {
            var safe = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            ScriptManager.RegisterStartupScript(
                this, this.GetType(), Guid.NewGuid().ToString("N"),
                $"alert('{safe}');", true);
        }
    }
}
