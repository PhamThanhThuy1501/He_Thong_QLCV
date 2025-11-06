using System;
using System.IO;
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
                LoadData();
            }

            // Áp UI mỗi vòng đời để nút đúng quyền & trạng thái
            ApplyPermissionUI();
            UpdateToggleButtonsUI();
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
                            if (!string.IsNullOrEmpty(cv1.NguoiDuyet))
                            {
                                if (cv1.TrangThai == "Đã được duyệt")
                                {
                                    Alert("Công văn đã được duyệt không thể sửa!");
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