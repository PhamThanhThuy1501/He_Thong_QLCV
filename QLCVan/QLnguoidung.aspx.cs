using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;                       // để dùng ScriptManager
using System.Web.Script.Serialization;     // để JSON-encode message


namespace QLCVan
{
    public partial class QLnguoidung : System.Web.UI.Page
    {

        string maQuyenYeuCau = "Q011";

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!PermissionHelper.HasPermission(maQuyenYeuCau))
            //{
            //    Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
            //    Response.End();
            //}

            if (!IsPostBack)
            {
                BindDropdowns();
                LoadNguoiDung();
                // >>> ĐỌC FLASH TỪ SESSION <<<
                var t = Session["flash.type"] as string;
                var m = Session["flash.msg"] as string;
                if (!string.IsNullOrEmpty(t) && !string.IsNullOrEmpty(m))
                {
                    Toast(t, m);                      // ví dụ: ("success", "Thêm người dùng thành công.")
                    Session.Remove("flash.type");      // xoá để không hiện lại nếu refresh
                    Session.Remove("flash.msg");
                }
            }
        }

        private void BindDropdowns()
        {
            var tbDonVi = UserRepository.GetAllDonVi();
            ddlDonVi.DataSource = tbDonVi;
            ddlDonVi.DataTextField = "TenDonVi"; // hoặc "TenDonVi" theo column
            ddlDonVi.DataValueField = "MaDonVi";
            ddlDonVi.DataBind();
            ddlDonVi.Items.Insert(0, new ListItem("Đơn vị", ""));

            var tbCV = UserRepository.GetAllChucVu();
            ddlChucVu.DataSource = tbCV;
            ddlChucVu.DataTextField = "TenChucVu";
            ddlChucVu.DataValueField = "MaChucVu";
            ddlChucVu.DataBind();
            ddlChucVu.Items.Insert(0, new ListItem("Chức vụ", ""));
        }


        private void LoadNguoiDung()
        {
            string maDonVi = string.IsNullOrWhiteSpace(ddlDonVi.SelectedValue) ? null : ddlDonVi.SelectedValue;
            string maCV = string.IsNullOrWhiteSpace(ddlChucVu.SelectedValue) ? null : ddlChucVu.SelectedValue;

            var tb = UserRepository.GetUsers(
                txtSearchTenDN.Text.Trim(),
                txtSearchEmail.Text.Trim(),
                maDonVi,
                maCV
            );

            gvNguoiDung.DataSource = tb;
            gvNguoiDung.DataBind();
        }


        protected void btnSearch_Click(object sender, EventArgs e) => LoadNguoiDung();

        protected void gvNguoiDung_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNguoiDung.PageIndex = e.NewPageIndex;
            LoadNguoiDung();
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var id = hdnDeleteUser.Value;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var rows = UserRepository.DeleteById(id);
                    if (rows > 0)
                    {
                        // xóa xong: clear hidden để tránh bấm nhầm lần 2
                        hdnDeleteUser.Value = string.Empty;

                        // reload danh sách
                        LoadNguoiDung();

                        // thông báo thành công
                        Toast("success", "Đã xóa người dùng thành công.");
                    }
                    else
                    {
                        Toast("warning", "Không tìm thấy người dùng để xóa.");
                    }
                }
                else
                {
                    Toast("warning", "Thiếu mã người dùng cần xóa.");
                }
            }
            catch (Exception ex)
            {
                // có lỗi khi xóa
                Toast("danger", "Có lỗi xảy ra khi xóa: " + ex.Message);
            }
        }
        private void Toast(string type, string message)
        {
            var jsonMsg = new JavaScriptSerializer().Serialize(message); // encode an toàn
            ScriptManager.RegisterStartupScript(
                this, GetType(),
                Guid.NewGuid().ToString("N"),            // key ngẫu nhiên để không bị ghi đè
                $"showToast('{type}', {jsonMsg});",      // gọi hàm JS đã đặt trong .aspx / master
                true
            );
        }

    }

    /* ============ DAL gộp chung file (ADO.NET) ============ */
    internal static class UserRepository
    {
        private static readonly string CS =
            ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString1"].ConnectionString;

        public static DataTable GetUsers(string tenDN, string email, string maDonVi, string maChucVu)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
SELECT 
    u.MaNguoiDung,
    u.TenDN,
    u.Email,
    u.HoTen,
    u.QuyenHan,
    u.TrangThai,
    u.MaDonVi,
    u.MaDonVi AS MaNhom,        -- alias cho những chỗ còn dùng MaNhom
    dv.TenDonVi AS TenDonVi,
    dv.TenDonVi AS TenNhom,     -- alias để GridView (TenNhom) không lỗi
    u.MaChucVu,
    cv.TenChucVu
FROM dbo.tblNguoiDung u
LEFT JOIN dbo.tblDonVi dv ON dv.MaDonVi = u.MaDonVi
LEFT JOIN dbo.tblChucVu cv ON cv.MaChucVu = u.MaChucVu
WHERE (@TenDN  IS NULL OR u.TenDN  LIKE '%'+@TenDN+'%')
  AND (@Email  IS NULL OR u.Email  LIKE '%'+@Email+'%')
  AND (@MaDonVi IS NULL OR u.MaDonVi = @MaDonVi)
  AND (@MaCV   IS NULL OR u.MaChucVu = @MaCV)
ORDER BY u.TenDN", con))
            {
                cmd.Parameters.AddWithValue("@TenDN", (object)(string.IsNullOrWhiteSpace(tenDN) ? null : tenDN) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)(string.IsNullOrWhiteSpace(email) ? null : email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaDonVi", (object)maDonVi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCV", (object)maChucVu ?? DBNull.Value);

                var tb = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(tb);
                return tb;
            }
        }




        public static DataTable GetAllDonVi()
        {
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaDonVi, TenDonVi FROM dbo.tblDonVi ORDER BY TenDonVi", con))
            {
                var tb = new DataTable(); da.Fill(tb); return tb;
            }
        }


        public static DataTable GetAllChucVu()
        {
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaChucVu, TenChucVu FROM dbo.tblChucVu ORDER BY TenChucVu", con))
            {
                var tb = new DataTable(); da.Fill(tb); return tb;
            }
        }

        public static int DeleteById(string maNguoiDung)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand("DELETE FROM dbo.tblNguoiDung WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", maNguoiDung);
                con.Open(); return cmd.ExecuteNonQuery();
            }
        }

        /* Thêm/sửa — dùng cho các trang Them/Sua nếu cần */
        public static int InsertUser(string tenDN, string matKhau, string hoTen, string email,
                               string quyenHan, bool trangThai, string maDonVi, string maChucVu)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.tblNguoiDung
(MaNguoiDung, Email, TenDN, MatKhau, QuyenHan, TrangThai, HoTen, MaDonVi, MaChucVu)
VALUES (@Id, @Email, @TenDN, @MatKhau, @QuyenHan, @TrangThai, @HoTen, @MaDonVi, @MaChucVu)", con))
            {
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@TenDN", tenDN);
                cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                cmd.Parameters.AddWithValue("@HoTen", hoTen);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@QuyenHan", quyenHan);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaDonVi", (object)maDonVi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaChucVu", (object)maChucVu ?? DBNull.Value);
                con.Open(); return cmd.ExecuteNonQuery();
            }
        }


        public static int UpdateUser(string id, string tenDN, string matKhau, string hoTen,
                              string email, string quyenHan, bool trangThai,
                              string maDonVi, string maChucVu)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
UPDATE dbo.tblNguoiDung
SET TenDN=@TenDN, MatKhau=@MatKhau, HoTen=@HoTen, Email=@Email,
    QuyenHan=@QuyenHan, TrangThai=@TrangThai, MaDonVi=@MaDonVi, MaChucVu=@MaChucVu
WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@TenDN", tenDN);
                cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                cmd.Parameters.AddWithValue("@HoTen", hoTen);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@QuyenHan", quyenHan);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaDonVi", (object)maDonVi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaChucVu", (object)maChucVu ?? DBNull.Value);
                con.Open(); return cmd.ExecuteNonQuery();
            }
        }

    }
}
