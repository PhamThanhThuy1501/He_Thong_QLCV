using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.Script.Serialization;

namespace QLCVan
{
    public partial class ThemNguoiDung : Page
    {
        private static readonly string CS =
            ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
            }
        }

        private void BindDropdowns()
        {
            using (var con = new SqlConnection(CS))
            {
                con.Open();

                // Đơn vị
                using (var cmd = new SqlCommand("SELECT MaDonVi, TenDonVi FROM tblDonVi ORDER BY TenDonVi", con))
                using (var rd = cmd.ExecuteReader())
                {
                    ddlDonVi.DataSource = rd;
                    ddlDonVi.DataTextField = "TenDonVi";
                    ddlDonVi.DataValueField = "MaDonVi";
                    ddlDonVi.DataBind();
                }
                ddlDonVi.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Đơn vị", ""));

                // Chức vụ
                using (var cmd = new SqlCommand("SELECT MaChucVu, TenChucVu FROM tblChucVu ORDER BY TenChucVu", con))
                using (var rd = cmd.ExecuteReader())
                {
                    ddlChucVu.DataSource = rd;
                    ddlChucVu.DataTextField = "TenChucVu";
                    ddlChucVu.DataValueField = "MaChucVu";
                    ddlChucVu.DataBind();
                }
                ddlChucVu.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Chức vụ", ""));
            }
        }

        // ======= TIỆN ÍCH CHỐNG TRÙNG / TỰ SINH MÃ =======
        private bool MaNguoiDungExists(string ma)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM tblNguoiDung WHERE MaNguoiDung = @v", con))
            {
                cmd.Parameters.Add("@v", SqlDbType.NVarChar, 50).Value = ma ?? "";
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool TenDNExists(string tenDN)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM tblNguoiDung WHERE TenDN = @v", con))
            {
                cmd.Parameters.Add("@v", SqlDbType.NVarChar, 100).Value = tenDN ?? "";
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private bool EmailExists(string email)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM tblNguoiDung WHERE Email = @v", con))
            {
                cmd.Parameters.Add("@v", SqlDbType.NVarChar, 200).Value = email ?? "";
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        // Sinh mã ND001, ND002,... (KHÔNG dùng TRY_CONVERT)
        private string GenerateNextMaNguoiDung()
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@";
                WITH x AS (
                    SELECT CASE
                             WHEN MaNguoiDung LIKE 'ND[0-9]%' THEN CONVERT(int, SUBSTRING(MaNguoiDung, 3, 50))
                             ELSE 0
                           END AS n
                    FROM tblNguoiDung
                )
                SELECT ISNULL(MAX(n), 0) + 1 FROM x;", con))
            {
                con.Open();
                int next = Convert.ToInt32(cmd.ExecuteScalar());
                return "ND" + next.ToString("000");
            }
        }

        protected void btnThem_Click(object sender, EventArgs e)
        {
            // --- Lấy dữ liệu từ form ---
            string inputMa = (txtMaNguoiDung.Text ?? "").Trim(); // có thể để trống
            string tenDN = (txtTenDN.Text ?? "").Trim();
            string matKhau = (txtMatKhau.Text ?? "").Trim();
            string xacNhan = (txtXacNhanMK.Text ?? "").Trim();
            string hoTen = (txtHoTen.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string maDonVi = ddlDonVi.SelectedValue;
            string maChucVu = ddlChucVu.SelectedValue;
            string quyenHan = "User";                // mặc định (nếu UI không chọn)
            bool trangThai = rdoKichHoat.Checked;

            // --- Validate cơ bản (TOAST) ---
            if (string.IsNullOrWhiteSpace(tenDN) || string.IsNullOrWhiteSpace(email))
            {
                Toast("warning", "Vui lòng nhập đầy đủ Tên đăng nhập và Email!");
                return;
            }
            if (string.IsNullOrWhiteSpace(matKhau) || matKhau != xacNhan)
            {
                Toast("warning", "Mật khẩu trống hoặc xác nhận mật khẩu không khớp!");
                return;
            }
            if (string.IsNullOrEmpty(maDonVi) || string.IsNullOrEmpty(maChucVu))
            {
                Toast("warning", "Vui lòng chọn Đơn vị và Chức vụ!");
                return;
            }

            // --- Check trùng TenDN & Email ---
            if (TenDNExists(tenDN))
            {
                Toast("warning", "Tên đăng nhập đã tồn tại!");
                return;
            }
            if (EmailExists(email))
            {
                Toast("warning", "Email đã tồn tại!");
                return;
            }

            // --- Xác định MaNguoiDung (PK) ---
            string maND;
            if (string.IsNullOrWhiteSpace(inputMa))
            {
                // Không nhập -> tự sinh NDxxx duy nhất
                maND = GenerateNextMaNguoiDung();
            }
            else
            {
                // Có nhập -> kiểm tra trùng
                if (MaNguoiDungExists(inputMa))
                {
                    Toast("warning", "Mã người dùng đã tồn tại. Vui lòng nhập mã khác hoặc để trống để hệ thống tự sinh!");
                    return;
                }
                maND = inputMa;
            }

            // 🔹 Lấy MaNhom mặc định (ưu tiên nhóm có sẵn trong DB)
            string maNhom = null;
            using (var conLookup = new SqlConnection(CS))
            using (var cmdLookup = new SqlCommand("SELECT TOP 1 MaNhom FROM tblNhom ORDER BY MaNhom", conLookup))
            {
                conLookup.Open();
                var res = cmdLookup.ExecuteScalar();
                maNhom = res != null ? res.ToString() : null;
            }
            if (string.IsNullOrEmpty(maNhom))
            {
                maNhom = "NHOM01"; // đảm bảo tồn tại trong DB
            }

            // --- Insert vào DB ---
            try
            {
                using (var con = new SqlConnection(CS))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO tblNguoiDung
                        (MaNguoiDung, Email, TenDN, MatKhau, QuyenHan, TrangThai, HoTen, MaDonVi, MaChucVu, MaNhom)
                    VALUES
                        (@MaNguoiDung, @Email, @TenDN, @MatKhau, @QuyenHan, @TrangThai, @HoTen, @MaDonVi, @MaChucVu, @MaNhom);", con))
                {
                    cmd.Parameters.Add("@MaNguoiDung", SqlDbType.NVarChar, 50).Value = maND;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 200).Value = email;
                    cmd.Parameters.Add("@TenDN", SqlDbType.NVarChar, 100).Value = tenDN;
                    cmd.Parameters.Add("@MatKhau", SqlDbType.NVarChar, 200).Value = matKhau; // TODO: hash nếu cần
                    cmd.Parameters.Add("@QuyenHan", SqlDbType.NVarChar, 50).Value = quyenHan;
                    cmd.Parameters.Add("@TrangThai", SqlDbType.Bit).Value = trangThai;
                    cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 200).Value = hoTen;
                    cmd.Parameters.Add("@MaDonVi", SqlDbType.NVarChar, 20).Value = maDonVi;
                    cmd.Parameters.Add("@MaChucVu", SqlDbType.NVarChar, 50).Value = maChucVu;
                    cmd.Parameters.Add("@MaNhom", SqlDbType.NVarChar, 20).Value = maNhom;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // Flash + Redirect về danh sách để hiện toast thành công
                Session["flash.type"] = "success";
                Session["flash.msg"] = "Thêm người dùng thành công.";
                Response.Redirect("QLnguoidung.aspx"); // đúng tên file danh sách của bạn
            }
            catch (Exception ex)
            {
                // Hiện toast lỗi ngay tại trang thêm (không redirect)
                Toast("danger", "Lỗi khi thêm: " + ex.Message);
            }
        }

        /// <summary>
        /// Gọi toast Bootstrap 5 ở client.
        /// type: success | info | warning | danger
        /// </summary>
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
