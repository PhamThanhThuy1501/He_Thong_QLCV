using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class QLnguoidung : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDropdowns();
                LoadNguoiDung();
            }
        }

        private void BindDropdowns()
        {
            // Đơn vị (Nhóm)
            var tbNhom = UserRepository.GetAllNhom();
            ddlDonVi.DataSource = tbNhom;
            ddlDonVi.DataTextField = "MoTa";
            ddlDonVi.DataValueField = "MaNhom";
            ddlDonVi.DataBind();
            ddlDonVi.Items.Insert(0, new ListItem("Đơn vị", ""));

            // Chức vụ
            var tbCV = UserRepository.GetAllChucVu();
            ddlChucVu.DataSource = tbCV;
            ddlChucVu.DataTextField = "TenChucVu";
            ddlChucVu.DataValueField = "MaChucVu";
            ddlChucVu.DataBind();
            ddlChucVu.Items.Insert(0, new ListItem("Chức vụ", ""));
        }

        private void LoadNguoiDung()
        {
            int? maNhom = null;
            if (int.TryParse(ddlDonVi.SelectedValue, out int tempNhom))
                maNhom = tempNhom;

            // Đổi kiểu của maChucVu từ int? -> string
            string maChucVu = ddlChucVu.SelectedValue;
            if (string.IsNullOrEmpty(maChucVu))
                maChucVu = null;

            var tb = UserRepository.GetUsers(
                txtSearchTenDN.Text.Trim(),
                txtSearchEmail.Text.Trim(),
                maNhom,
                maChucVu
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
            var id = hdnDeleteUser.Value;
            if (!string.IsNullOrWhiteSpace(id))
            {
                UserRepository.DeleteById(id);
                LoadNguoiDung();
            }
        }
    }

    /* ============ DAL gộp chung file (ADO.NET) ============ */
    internal static class UserRepository
    {
        private static readonly string CS =
            ConfigurationManager.ConnectionStrings["QLCVDb"].ConnectionString;

        public static DataTable GetUsers(string tenDN, string email, int? maNhom, string maChucVu)
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
                u.MaNhom,
                n.MoTa AS TenNhom,
                u.MaChucVu,
                cv.TenChucVu
            FROM dbo.tblNguoiDung u
            LEFT JOIN dbo.tblNhom n    ON n.MaNhom    = u.MaNhom
            LEFT JOIN dbo.tblChucVu cv ON cv.MaChucVu = u.MaChucVu
            WHERE (@TenDN IS NULL OR u.TenDN LIKE '%'+@TenDN+'%')
              AND (@Email IS NULL OR u.Email LIKE '%'+@Email+'%')
              AND (@MaNhom IS NULL OR u.MaNhom = @MaNhom)
              AND (@MaChucVu IS NULL OR u.MaChucVu = @MaChucVu)
            ORDER BY u.TenDN", con))
            {
                cmd.Parameters.AddWithValue("@TenDN", (object)(string.IsNullOrWhiteSpace(tenDN) ? null : tenDN) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)(string.IsNullOrWhiteSpace(email) ? null : email) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaNhom", (object)maNhom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaChucVu", (object)maChucVu ?? DBNull.Value);

                var tb = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(tb);
                return tb;
            }
        }


        public static DataTable GetAllNhom()
        {
            using (var con = new SqlConnection(CS))
            using (var da = new SqlDataAdapter("SELECT MaNhom, MoTa FROM dbo.tblNhom ORDER BY MoTa", con))
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
                                     string quyenHan, bool trangThai, int maNhom, int maChucVu)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.tblNguoiDung (MaNguoiDung, TenDN, MatKhau, HoTen, Email, QuyenHan, TrangThai, MaNhom, MaChucVu)
VALUES (@Id, @TenDN, @MatKhau, @HoTen, @Email, @QuyenHan, @TrangThai, @MaNhom, @MaChucVu)", con))
            {
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@TenDN", tenDN);
                cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                cmd.Parameters.AddWithValue("@HoTen", hoTen);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@QuyenHan", quyenHan);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaNhom", maNhom);
                cmd.Parameters.AddWithValue("@MaChucVu", maChucVu);
                con.Open(); return cmd.ExecuteNonQuery();
            }
        }

        public static int UpdateUser(string id, string tenDN, string matKhau, string hoTen,
                                     string email, string quyenHan, bool trangThai,
                                     int maNhom, int maChucVu)
        {
            using (var con = new SqlConnection(CS))
            using (var cmd = new SqlCommand(@"
UPDATE dbo.tblNguoiDung
SET TenDN=@TenDN, MatKhau=@MatKhau, HoTen=@HoTen, Email=@Email,
    QuyenHan=@QuyenHan, TrangThai=@TrangThai, MaNhom=@MaNhom, MaChucVu=@MaChucVu
WHERE MaNguoiDung=@Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@TenDN", tenDN);
                cmd.Parameters.AddWithValue("@MatKhau", matKhau);
                cmd.Parameters.AddWithValue("@HoTen", hoTen);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@QuyenHan", quyenHan);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaNhom", maNhom);
                cmd.Parameters.AddWithValue("@MaChucVu", maChucVu);
                con.Open(); return cmd.ExecuteNonQuery();
            }
        }
    }
}
