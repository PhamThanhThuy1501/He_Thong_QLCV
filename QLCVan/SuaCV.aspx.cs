using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class SuaCV : Page
    {
        // ĐÚNG TÊN BẢNG Ở ĐÂY (sửa nếu bạn đặt khác)
        private const string TABLE_CV = "tblNoiDungCV";
        private const string TABLE_LOAICV = "tblLoaiCV";
        private const string TABLE_USER = "tblNguoiDung";  // nếu bạn dùng bảng khác thì đổi
        private const string TABLE_DONVI = "tblDonVi";     // nếu bạn dùng bảng khác thì đổi

        private string ConnStr
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                    throw new InvalidOperationException("Thiếu 'QuanLyCongVanConnectionString' trong Web.config.");
                return cs.ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* if (Session["QuyenHan"] != null && Session["QuyenHan"].ToString().Trim() == "User")
 {
     ClientScript.RegisterStartupScript(GetType(), "noauth",
         "alert('Bạn không có quyền truy cập trang này !'); location.href='Trangchu.aspx';", true);
     return;
 }*/

            /* try
             {
                 EnsureTableNames();
             }
             catch (Exception ex)
             {
                 Alert(ex.Message);
                 return;
             }*/
            if (!IsPostBack)
            {
                string maCv = GetMaCVFromRequest();
                if (string.IsNullOrEmpty(maCv))
                {
                    ScriptManager.RegisterStartupScript(
     this,
     this.GetType(),
     "missingMaCV",
     "showToast('Thiếu mã công văn!', 'text-bg-warning');",
     true
 );

                    return;
                }

                // 1. bind trước
                BindLoaiCV();
                BindDonViNhan();
                BindNguoiDuyet();
                BindFileDinhKem(maCv);

                // 2. load công văn → chọn lại
                LoadCongVan(maCv);
            }
        }

        private string GetMaCVFromRequest()
        {
            string macv = Request["macv"];
            if (string.IsNullOrEmpty(macv))
                macv = Request["id"];
            return string.IsNullOrEmpty(macv) ? null : macv.Trim();
        }

        private void Alert(string msg)
        {
            string safe = HttpUtility.JavaScriptStringEncode(msg ?? "");
            string js = "alert('" + safe + "');";
            ClientScript.RegisterStartupScript(GetType(), "alert", js, true);
        }

        // ================== BIND LOẠI CV ==================
        private void BindLoaiCV()
        {
            ddlLoaiCV.Items.Clear();
            ddlLoaiCV.Items.Add(new ListItem("-- Chọn loại công văn --", ""));

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaLoaiCV, TenLoaiCV FROM " + TABLE_LOAICV + " ORDER BY TenLoaiCV", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string ma = rd["MaLoaiCV"].ToString();
                        string ten = rd["TenLoaiCV"] == DBNull.Value ? ("Loại " + ma) : rd["TenLoaiCV"].ToString();
                        ddlLoaiCV.Items.Add(new ListItem(ten, ma));
                    }
                }
            }

            // chỉ hiển thị, không cho đổi loại
            ddlLoaiCV.Enabled = false;
        }

        // ================== BIND ĐƠN VỊ NHẬN ==================
        private void BindDonViNhan()
        {
            ddlDonViNhan.Items.Clear();
            ddlDonViNhan.Items.Add(new ListItem("-- Chọn đơn vị nhận --", ""));

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaDonVi, TenDonVi FROM " + TABLE_DONVI + " ORDER BY TenDonVi", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string ma = rd["MaDonVi"].ToString().Trim();
                        string ten = rd["TenDonVi"].ToString().Trim();
                        ddlDonViNhan.Items.Add(new ListItem(ten, ma));
                    }
                }
            }

            ddlDonViNhan.Enabled = true;
        }

        // ================== BIND NGƯỜI DUYỆT ==================
        // CHỈ 1 HÀM NÀY THÔI, KHÔNG ĐƯỢC VIẾT 2 LẦN
        private void BindNguoiDuyet()
        {
            ddlNguoiDuyet.Items.Clear();
            ddlNguoiDuyet.Items.Add(new ListItem("-- Chọn người duyệt --", "0"));

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaNguoiDung, HoTen FROM " + TABLE_USER + " ORDER BY HoTen", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string id = rd["MaNguoiDung"].ToString().Trim();
                        string name = rd["HoTen"] == DBNull.Value ? ("Người " + id) : rd["HoTen"].ToString().Trim();
                        ddlNguoiDuyet.Items.Add(new ListItem(name, id));
                    }
                }
            }

            ddlNguoiDuyet.Enabled = true;
        }

        // ================== LOAD CÔNG VĂN ==================
        private void LoadCongVan(string maCv)
        {
            // JOIN sang bảng người để lấy tên người duyệt
            string sql =
                "SELECT cv.MaCV, cv.TieuDeCV, cv.SoCV, cv.CoQuanBanHanh, cv.TrichYeuND, cv.NguoiKy, cv.GhiChu, " +
                "       cv.NgayBanHanh, cv.NgayGui, cv.MaLoaiCV, cv.NoiNhan, " +
                "       lcv.PheDuyet, " +
                "       cv.NguoiDuyet AS MaNguoiDuyet, " +
                "       nd.HoTen AS TenNguoiDuyet " +
                "FROM " + TABLE_CV + " cv " +
                "LEFT JOIN " + TABLE_LOAICV + " lcv ON cv.MaLoaiCV = lcv.MaLoaiCV " +
                "LEFT JOIN " + TABLE_USER + " nd ON cv.NguoiDuyet = nd.MaNguoiDung " +
                "WHERE cv.MaCV = @MaCV";

            string pheDuyet = "0";
            string maLoaiCv = "";
            string noiNhanCu = "";
            string maNguoiDuyetCu = "";
            string tenNguoiDuyetDb = "";

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCv);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                    {
                        Alert("Không tìm thấy công văn.");
                        return;
                    }

                    // ===== text =====
                    txttieude.Text = rd["TieuDeCV"] == DBNull.Value ? "" : rd["TieuDeCV"].ToString();
                    txtsocv.Text = rd["SoCV"] == DBNull.Value ? "" : rd["SoCV"].ToString();
                    txtcqbh.Text = rd["CoQuanBanHanh"] == DBNull.Value ? "" : rd["CoQuanBanHanh"].ToString();
                    txttrichyeu.Text = rd["TrichYeuND"] == DBNull.Value ? "" : rd["TrichYeuND"].ToString();
                    txtNguoiKy.Text = rd["NguoiKy"] == DBNull.Value ? "" : rd["NguoiKy"].ToString();
                    txtGhiChu.Text = rd["GhiChu"] == DBNull.Value ? "" : rd["GhiChu"].ToString();

                    // ===== dates =====
                    if (rd["NgayBanHanh"] != DBNull.Value)
                        txtngaybanhanh.Text = ((DateTime)rd["NgayBanHanh"]).ToString("yyyy-MM-dd");
                    if (rd["NgayGui"] != DBNull.Value)
                        txtngaygui.Text = ((DateTime)rd["NgayGui"]).ToString("yyyy-MM-dd");

                    // ===== other fields =====
                    maLoaiCv = rd["MaLoaiCV"] == DBNull.Value ? "" : rd["MaLoaiCV"].ToString().Trim();
                    noiNhanCu = rd["NoiNhan"] == DBNull.Value ? "" : rd["NoiNhan"].ToString().Trim();
                    pheDuyet = rd["PheDuyet"] == DBNull.Value ? "0" : rd["PheDuyet"].ToString().Trim();
                    maNguoiDuyetCu = rd["MaNguoiDuyet"] == DBNull.Value ? "" : rd["MaNguoiDuyet"].ToString().Trim();
                    tenNguoiDuyetDb = rd["TenNguoiDuyet"] == DBNull.Value ? "" : rd["TenNguoiDuyet"].ToString().Trim();
                }
            }

            // ===== 1. chọn lại LOẠI CV =====
            if (!string.IsNullOrEmpty(maLoaiCv))
            {
                var it = ddlLoaiCV.Items.FindByValue(maLoaiCv);
                if (it != null)
                {
                    ddlLoaiCV.ClearSelection();
                    it.Selected = true;
                }
                else
                {
                    ddlLoaiCV.Items.Add(new ListItem("(Loại cũ) " + maLoaiCv, maLoaiCv));
                    ddlLoaiCV.SelectedValue = maLoaiCv;
                }
            }

            // ===== 2. chọn lại ĐƠN VỊ NHẬN =====
            if (!string.IsNullOrEmpty(noiNhanCu))
            {
                var it2 = ddlDonViNhan.Items.FindByValue(noiNhanCu);
                if (it2 != null)
                {
                    ddlDonViNhan.ClearSelection();
                    it2.Selected = true;
                }
                else
                {
                    ddlDonViNhan.Items.Add(new ListItem("(Đơn vị cũ) " + noiNhanCu, noiNhanCu));
                    ddlDonViNhan.SelectedValue = noiNhanCu;
                }
            }
            ddlDonViNhan.Enabled = true;

            // ===== 3. xử lý NGƯỜI DUYỆT =====
            if (pheDuyet == "1")
            {
                pnlNguoiDuyet.Visible = true;
                ddlNguoiDuyet.Enabled = true;

                if (!string.IsNullOrEmpty(maNguoiDuyetCu))
                {
                    // TH còn trong danh sách
                    var it3 = ddlNguoiDuyet.Items.FindByValue(maNguoiDuyetCu);
                    if (it3 != null)
                    {
                        ddlNguoiDuyet.ClearSelection();
                        it3.Selected = true;
                    }
                    else
                    {
                        // TH người duyệt đã xóa khỏi bảng → tự thêm để HIỆN
                        string textHien = !string.IsNullOrEmpty(tenNguoiDuyetDb)
                            ? tenNguoiDuyetDb
                            : "(Người duyệt cũ) " + maNguoiDuyetCu;

                        ddlNguoiDuyet.Items.Add(new ListItem(textHien, maNguoiDuyetCu));
                        ddlNguoiDuyet.ClearSelection();
                        ddlNguoiDuyet.SelectedValue = maNguoiDuyetCu;
                    }
                }

                // in ra label phụ
                if (!string.IsNullOrEmpty(tenNguoiDuyetDb))
                    lblTenNguoiDuyet.Text = " (" + tenNguoiDuyetDb + ")";
                else
                    lblTenNguoiDuyet.Text = "";
            }
            else
            {
                pnlNguoiDuyet.Visible = false;
            }
        }

        // ================== LƯU ==================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string maCv = GetMaCVFromRequest();
            if (string.IsNullOrEmpty(maCv))
            {
                Alert("Thiếu mã công văn.");
                return;
            }

            string tieuDe = txttieude.Text.Trim();
            string soCv = txtsocv.Text.Trim();
            string coQuan = txtcqbh.Text.Trim();
            string trichYeu = txttrichyeu.Text.Trim();
            string nguoiKy = txtNguoiKy.Text.Trim();
            string ghiChu = txtGhiChu.Text.Trim();
            string donViNhan = ddlDonViNhan.SelectedValue;

            DateTime? ngayBanHanh;
            DateTime? ngayGui;

            if (!TryParseDate(txtngaybanhanh.Text.Trim(), out ngayBanHanh))
            {
                Alert("Ngày ban hành không hợp lệ.");
                return;
            }
            if (!TryParseDate(txtngaygui.Text.Trim(), out ngayGui))
            {
                Alert("Ngày gửi không hợp lệ.");
                return;
            }

            string maNguoiDuyet = null;
            if (pnlNguoiDuyet.Visible && ddlNguoiDuyet.SelectedValue != "0")
            {
                maNguoiDuyet = ddlNguoiDuyet.SelectedValue;
            }

            string sql =
                "UPDATE " + TABLE_CV + " SET " +
                " TieuDeCV = @TieuDeCV, " +
                " SoCV = @SoCV, " +
                " CoQuanBanHanh = @CoQuanBanHanh, " +
                " TrichYeuND = @TrichYeuND, " +
                " NguoiKy = @NguoiKy, " +
                " GhiChu = @GhiChu, " +
                " NgayBanHanh = @NgayBanHanh, " +
                " NgayGui = @NgayGui, " +
                " NoiNhan = @NoiNhan ";

            if (maNguoiDuyet != null)
            {
                sql += ", NguoiDuyet = @NguoiDuyet ";
            }

            sql += " WHERE MaCV = @MaCV";

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TieuDeCV", (object)tieuDe ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoCV", (object)soCv ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CoQuanBanHanh", (object)coQuan ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TrichYeuND", (object)trichYeu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NguoiKy", (object)nguoiKy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GhiChu", (object)ghiChu ?? DBNull.Value);

                if (ngayBanHanh.HasValue)
                    cmd.Parameters.AddWithValue("@NgayBanHanh", ngayBanHanh.Value);
                else
                    cmd.Parameters.AddWithValue("@NgayBanHanh", DBNull.Value);

                if (ngayGui.HasValue)
                    cmd.Parameters.AddWithValue("@NgayGui", ngayGui.Value);
                else
                    cmd.Parameters.AddWithValue("@NgayGui", DBNull.Value);

                cmd.Parameters.AddWithValue("@NoiNhan", string.IsNullOrEmpty(donViNhan) ? (object)DBNull.Value : donViNhan);
                cmd.Parameters.AddWithValue("@MaCV", maCv);

                if (maNguoiDuyet != null)
                    cmd.Parameters.AddWithValue("@NguoiDuyet", maNguoiDuyet);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            ScriptManager.RegisterStartupScript(
     this,
     this.GetType(),
     "saveSuccess",
     "showToast('Đã lưu công văn!', 'text-bg-success');",
     true
 );

        }

        private bool TryParseDate(string input, out DateTime? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(input)) return true;

            DateTime d;
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out d))
            {
                result = d;
                return true;
            }
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out d))
            {
                result = d;
                return true;
            }
            return false;
        }

        protected void btnUp_Click(object sender, EventArgs e) { }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string maCv = GetMaCVFromRequest();
            if (string.IsNullOrEmpty(maCv))
            {
                Alert("Thiếu mã công văn.");
                return;
            }

            // Lấy danh sách các mục được chọn trong ListBox1
            var selectedIndices = ListBox1.GetSelectedIndices();

            if (ListBox1.Items.Count == 0)
            {
                Alert("Không có tệp nào để xóa.");
                return;
            }

            bool xoaTatCa = (selectedIndices == null || selectedIndices.Length == 0);
            int soFileXoa = 0;

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                if (xoaTatCa)
                {
                    // Xóa tất cả tệp của công văn
                    using (var cmd = new SqlCommand("DELETE FROM tblFileDinhKem WHERE MaCV = @MaCV", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaCV", maCv);
                        soFileXoa = cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // Xóa từng file được chọn
                    foreach (int idx in selectedIndices)
                    {
                        string url = ListBox1.Items[idx].Value;
                        if (string.IsNullOrEmpty(url)) continue;

                        using (var cmd = new SqlCommand("DELETE FROM tblFileDinhKem WHERE MaCV = @MaCV AND Url = @Url", conn))
                        {
                            cmd.Parameters.AddWithValue("@MaCV", maCv);
                            cmd.Parameters.AddWithValue("@Url", url);
                            soFileXoa += cmd.ExecuteNonQuery();
                        }

                        // Xóa file vật lý trên ổ đĩa (nếu có)
                        try
                        {
                            string filePath = Server.MapPath(url);
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);
                        }
                        catch (Exception)
                        {
                            // Không cần dừng chương trình nếu file vật lý không tồn tại
                        }
                    }
                }
            }

            // Load lại danh sách file
            BindFileDinhKem(maCv);

            if (soFileXoa > 0)
                Alert("Đã xóa " + soFileXoa + " tệp đính kèm.");
            else
                Alert("Không có tệp nào được xóa.");
        }
        private void BindFileDinhKem(string maCv)
        {
            ListBox1.Items.Clear();

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT TenFile, Url FROM tblFileDinhKem WHERE MaCV = @MaCV ORDER BY DateUpload DESC", conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCv);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string ten = rd["TenFile"] == DBNull.Value ? "" : rd["TenFile"].ToString();
                        string url = rd["Url"] == DBNull.Value ? "" : rd["Url"].ToString();
                        ListBox1.Items.Add(new ListItem(ten, url));
                    }
                }
            }

            // Nếu không có file nào → thêm dòng thông báo
            if (ListBox1.Items.Count == 0)
            {
                ListBox1.Items.Add(new ListItem("(Không có tệp đính kèm)", ""));
                ListBox1.Enabled = false;
            }
            else
            {
                ListBox1.Enabled = true;
            }
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Trangchu.aspx");
        }
    }
}
