using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class SuaNguoiDung : System.Web.UI.Page
    {
        private static string CS => ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString1"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                LoadUser();
            }
        }

        private void BindDropdowns()
        {
            // Đơn vị
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaDonVi, TenDonVi FROM dbo.tblDonVi ORDER BY TenDonVi", con))
            {
                var tb = new DataTable(); da.Fill(tb);
                ddlDonVi.DataSource = tb;
                ddlDonVi.DataTextField = "TenDonVi";
                ddlDonVi.DataValueField = "MaDonVi";
                ddlDonVi.DataBind();
            }
            ddlDonVi.Items.Insert(0, new ListItem("Đơn vị", ""));

            // Chức vụ
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaChucVu, TenChucVu FROM dbo.tblChucVu ORDER BY TenChucVu", con))
            {
                var tb = new DataTable(); da.Fill(tb);
                ddlChucVu.DataSource = tb;
                ddlChucVu.DataTextField = "TenChucVu";
                ddlChucVu.DataValueField = "MaChucVu";
                ddlChucVu.DataBind();
            }
            ddlChucVu.Items.Insert(0, new ListItem("Chức vụ", ""));
        }

        private void LoadUser()
        {
            string id = Request.QueryString["id"];
            if (string.IsNullOrWhiteSpace(id))
            {
                Toast("warning", "Thiếu mã người dùng cần sửa.");
                return;
            }

            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT TOP 1 * FROM dbo.tblNguoiDung WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var tb = new DataTable(); da.Fill(tb);
                    if (tb.Rows.Count == 0)
                    {
                        Toast("warning", "Không tìm thấy người dùng.");
                        return;
                    }

                    var r = tb.Rows[0];
                    txtMaNguoiDung.Text = r["MaNguoiDung"].ToString();
                    txtTenDN.Text       = r["TenDN"].ToString();
                    txtMatKhau.Attributes["value"] = r["MatKhau"].ToString(); // chỉ để hiển thị
                    txtHoTen.Text       = r["HoTen"].ToString();
                    txtEmail.Text       = r["Email"].ToString();

                    bool trangThai = r["TrangThai"] != DBNull.Value && Convert.ToBoolean(r["TrangThai"]);
                    rdoKichHoat.Checked = trangThai;
                    rdoKhongKichHoat.Checked = !trangThai;

                    if (r["MaDonVi"]  != DBNull.Value) ddlDonVi.SelectedValue  = r["MaDonVi"].ToString();
                    if (r["MaChucVu"] != DBNull.Value) ddlChucVu.SelectedValue = r["MaChucVu"].ToString();
                }
            }
        }

        // ===== Helpers lấy hiện trạng =====
        private string GetCurrentPassword(string maNguoiDung)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT MatKhau FROM dbo.tblNguoiDung WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", maNguoiDung);
                con.Open();
                var obj = cmd.ExecuteScalar();
                return obj == null ? "" : obj.ToString();
            }
        }

        private string GetCurrentMaNhom(string maNguoiDung)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT MaNhom FROM dbo.tblNguoiDung WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", maNguoiDung);
                con.Open();
                var obj = cmd.ExecuteScalar();
                return obj == null ? null : obj.ToString();
            }
        }

        // ===== Validate/Unique helpers =====
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            // Regex email cơ bản, đủ dùng cho form
            var re = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return re.IsMatch(email);
        }

        private bool TenDNExistsForOther(string id, string tenDN)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT COUNT(1) FROM dbo.tblNguoiDung WHERE TenDN=@TenDN AND MaNguoiDung<>@Id", con))
            {
                cmd.Parameters.AddWithValue("@TenDN", tenDN ?? "");
                cmd.Parameters.AddWithValue("@Id", id ?? "");
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool EmailExistsForOther(string id, string email)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"SELECT COUNT(1) FROM dbo.tblNguoiDung WHERE Email=@Email AND MaNguoiDung<>@Id", con))
            {
                cmd.Parameters.AddWithValue("@Email", email ?? "");
                cmd.Parameters.AddWithValue("@Id", id ?? "");
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            var id        = (txtMaNguoiDung.Text ?? "").Trim();
            var tenDN     = (txtTenDN.Text ?? "").Trim();
            var hoTen     = (txtHoTen.Text ?? "").Trim();
            var email     = (txtEmail.Text ?? "").Trim();
            var maDonVi   = ddlDonVi.SelectedValue;
            var maChucVu  = ddlChucVu.SelectedValue;
            var trangThai = rdoKichHoat.Checked;

            // ===== VALIDATE ĐẦU VÀO =====
            if (string.IsNullOrWhiteSpace(id))
            {
                Toast("warning", "Thiếu mã người dùng.");
                return;
            }
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                Toast("warning", "Vui lòng nhập Họ và tên.");
                return;
            }
            if (string.IsNullOrWhiteSpace(tenDN))
            {
                Toast("warning", "Vui lòng nhập Tên đăng nhập.");
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                Toast("warning", "Vui lòng nhập Email.");
                return;
            }
            if (!IsValidEmail(email))
            {
                Toast("warning", "Email không đúng định dạng.");
                return;
            }
            // Nếu bạn muốn bắt buộc chọn Đơn vị/Chức vụ, để nguyên 2 check sau; nếu không, xoá chúng đi.
            if (string.IsNullOrEmpty(maDonVi))
            {
                Toast("warning", "Vui lòng chọn Đơn vị.");
                return;
            }
            if (string.IsNullOrEmpty(maChucVu))
            {
                Toast("warning", "Vui lòng chọn Chức vụ.");
                return;
            }

            // Nếu người dùng nhập mật khẩu mới, enforce rule tối thiểu
            var isChangePassword = !string.IsNullOrWhiteSpace(txtMatKhau.Text);
            if (isChangePassword && txtMatKhau.Text.Length < 6)
            {
                Toast("warning", "Mật khẩu mới phải có ít nhất 6 ký tự.");
                return;
            }

            // Unique cho user khác (exclude chính mình)
            if (TenDNExistsForOther(id, tenDN))
            {
                Toast("warning", "Tên đăng nhập đã được dùng bởi người khác.");
                return;
            }
            if (EmailExistsForOther(id, email))
            {
                Toast("warning", "Email đã được dùng bởi người khác.");
                return;
            }

            // Lấy mật khẩu hiện tại nếu không đổi
            var matKhau = isChangePassword ? txtMatKhau.Text : GetCurrentPassword(id);

            // Giữ nguyên nhóm hiện có
            var maNhom = GetCurrentMaNhom(id);

            // ===== UPDATE =====
            try
            {
                using (var con = new SqlConnection(CS))
                using (var cmd = new SqlCommand(@"
UPDATE dbo.tblNguoiDung SET
    Email     = @Email,
    TenDN     = @TenDN,
    MatKhau   = @MatKhau,
    TrangThai = @TrangThai,
    HoTen     = @HoTen,
    MaDonVi   = @MaDonVi,
    MaChucVu  = @MaChucVu,
    QuyenHan  = @QuyenHan,   -- nếu DB không dùng, bỏ dòng này + tham số
    MaNhom    = @MaNhom      -- hoặc bỏ nếu không muốn đổi nhóm
WHERE MaNguoiDung = @Id;", con))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value       = id;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value   = email;
                    cmd.Parameters.Add("@TenDN", SqlDbType.NVarChar, 100).Value   = tenDN;
                    cmd.Parameters.Add("@MatKhau", SqlDbType.NVarChar, 200).Value = matKhau; // TODO: hash nếu cần
                    cmd.Parameters.Add("@TrangThai", SqlDbType.Bit).Value         = trangThai;
                    cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 200).Value   = hoTen;
                    cmd.Parameters.Add("@MaDonVi", SqlDbType.NVarChar, 20).Value  = string.IsNullOrWhiteSpace(maDonVi) ? (object)DBNull.Value : maDonVi;
                    cmd.Parameters.Add("@MaChucVu", SqlDbType.NVarChar, 50).Value = string.IsNullOrWhiteSpace(maChucVu) ? (object)DBNull.Value : maChucVu;

                    // Quyền hạn mặc định (hoặc bỏ khỏi SQL nếu không dùng)
                    cmd.Parameters.Add("@QuyenHan", SqlDbType.NVarChar, 50).Value  = "User";

                    // Giữ nguyên MaNhom hiện có (nếu null thì cho DBNull)
                    cmd.Parameters.Add("@MaNhom", SqlDbType.NVarChar, 20).Value    = string.IsNullOrEmpty(maNhom) ? (object)DBNull.Value : maNhom;

                    con.Open();
                    var affected = cmd.ExecuteNonQuery();
                    if (affected <= 0)
                    {
                        Toast("warning", "Không có bản ghi nào được cập nhật.");
                        return;
                    }
                }

                // Đồng bộ quyền nếu bạn cần (không bắt buộc)
                try { PermissionHelper.ReSyncPermission(); } catch { /* ignore */ }

                // Flash + Redirect để hiển thị toast thành công ở trang danh sách
                Session["flash.type"] = "success";
                Session["flash.msg"]  = "Cập nhật người dùng thành công.";
                Response.Redirect("QLnguoidung.aspx");
            }
            catch (Exception ex)
            {
                // Hiện toast lỗi ngay tại trang sửa (không redirect)
                Toast("danger", "Lỗi khi cập nhật: " + ex.Message);
            }
        }

        protected void btnQuayLai_Click(object sender, EventArgs e)
        {
            Response.Redirect("QLNguoiDung.aspx");
        }

        // ===== Toast helper =====
        private void Toast(string type, string message)
        {
            var json = new JavaScriptSerializer().Serialize(message);
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                Guid.NewGuid().ToString("N"),
                $"showToast('{type}', {json});",
                true
            );
        }
    }
}
