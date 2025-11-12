using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class SuaCV : Page
    {
        // ====== Tên bảng (đổi nếu DB bạn khác) ======
        private const string TABLE_CV = "tblNoiDungCV";
        private const string TABLE_LOAI = "tblLoaiCV";
        private const string TABLE_DV = "tblDonVi";
        private const string TABLE_FILE = "tblFileDinhKem";

        // ====== ConnectionString (không dùng biểu thức C#6) ======
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

        // ====== Helpers ======
        private static bool TryParseDate(string s, out DateTime? d)
        {
            d = null;
            if (string.IsNullOrWhiteSpace(s)) return true;

            DateTime d1;
            if (DateTime.TryParseExact(s.Trim(), "dd/MM/yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out d1))
            { d = d1; return true; }

            DateTime d2;
            if (DateTime.TryParseExact(s.Trim(), "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out d2))
            { d = d2; return true; }

            return false;
        }

        private string GetMaCVFromRequest()
        {
            string m = Request["macv"];
            if (string.IsNullOrEmpty(m)) m = Request["id"];
            return string.IsNullOrEmpty(m) ? null : m.Trim();
        }

        private string UploadRootVDir() { return "~/Upload/"; }
        private string UploadRootPDir() { return Server.MapPath(UploadRootVDir()); }
        private void SafeEnsureUploadFolder()
        {
            string p = UploadRootPDir();
            if (!Directory.Exists(p)) Directory.CreateDirectory(p);
        }
        private void TryDeletePhysicalByName(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName)) return;
                string phys = Path.Combine(UploadRootPDir(), fileName);
                if (File.Exists(phys)) File.Delete(phys);
            }
            catch { }
        }

        // ====== Page lifecycle ======
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Dangnhap.aspx");
                return;
            }
            /*  if (Session["QuyenHan"] != null && Session["QuyenHan"].ToString().Trim() == "User")
         {
             ClientScript.RegisterStartupScript(GetType(), "noauth",
                 "alert('Bạn không có quyền truy cập trang này !'); location.href='Trangchu.aspx';", true);
             return;
         }*/

            if (!IsPostBack)
            {
                string maCv = GetMaCVFromRequest();
                if (string.IsNullOrEmpty(maCv)) return;

                try
                {
                    BindLoaiCV(maCv);          // fill + select + disable
                    LoadCongVan(maCv);         // fill fields
                    BindDonViNhanReadonly(maCv); // load đơn vị nhận vào ListBox readonly
                    BindFileDinhKem(maCv);     // load file
                }
                catch
                {
                    // im lặng theo yêu cầu
                }
            }
        }


        // ====== Bind masters ======
        private void BindLoaiCV(string maCv)
        {
            ddlLoaiCV.Items.Clear();
            ddlLoaiCV.Items.Add(new ListItem("-- Chọn loại công văn --", ""));

            string maLoaiCv = null;

            // lấy mã loại của công văn trước
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT MaLoaiCV FROM " + TABLE_CV + " WHERE MaCV=@MaCV", conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCv);
                conn.Open();
                object v = cmd.ExecuteScalar();
                if (v != null && v != DBNull.Value) maLoaiCv = v.ToString();
            }

            // nạp danh mục loại
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT MaLoaiCV, TenLoaiCV FROM " + TABLE_LOAI + " ORDER BY TenLoaiCV", conn))
            {
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string text = rd["TenLoaiCV"] == DBNull.Value
                                      ? "Loại " + rd["MaLoaiCV"]
                                      : rd["TenLoaiCV"].ToString();
                        ddlLoaiCV.Items.Add(new ListItem(text, rd["MaLoaiCV"].ToString()));
                    }
                }
            }

            if (!string.IsNullOrEmpty(maLoaiCv))
            {
                ListItem it = ddlLoaiCV.Items.FindByValue(maLoaiCv);
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

            ddlLoaiCV.Enabled = false; // readonly
        }

        private void BindDonViNhanReadonly(string maCv)
        {
            lblDonViNhan.Text = string.Empty;  // Đảm bảo trước khi gán mới, xóa nội dung cũ

            string connStr = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;
            var tenDonVis = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Sử dụng HashSet để tránh trùng

            string noiNhanRaw = null;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT NoiNhan FROM tblNoiDungCV WHERE MaCV = @MaCV", conn))
                {
                    cmd.Parameters.AddWithValue("@MaCV", maCv);
                    var o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        noiNhanRaw = o.ToString().Trim();  // Lấy giá trị NoiNhan
                }

                // Tách giá trị NoiNhan thành các mã đơn vị
                if (!string.IsNullOrWhiteSpace(noiNhanRaw))
                {
                    var tokens = noiNhanRaw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(s => s.Trim())
                                           .Where(s => s.Length > 0)
                                           .ToList();

                    foreach (var tk in tokens)
                    {
                        using (var cmd = new SqlCommand("SELECT TenDonVi FROM tblDonVi WHERE MaDonVi = @v OR TenDonVi = @v", conn))
                        {
                            cmd.Parameters.AddWithValue("@v", tk);
                            var o = cmd.ExecuteScalar();
                            if (o != null && o != DBNull.Value)
                                tenDonVis.Add(o.ToString());
                            else
                                tenDonVis.Add(tk);  // Nếu không tìm thấy, dùng tk như tên đơn vị
                        }
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(@"
                SELECT DISTINCT dv.TenDonVi
                FROM tblGuiNhan gn
                JOIN tblNguoiDung nd ON nd.MaNguoiDung = gn.MaNguoiNhan
                JOIN tblDonVi dv ON dv.MaDonVi = nd.MaDonVi
                WHERE gn.MaCV = @MaCV", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaCV", maCv);
                        using (var rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                var ten = rd["TenDonVi"] == DBNull.Value ? "" : rd["TenDonVi"].ToString().Trim();
                                if (!string.IsNullOrEmpty(ten)) tenDonVis.Add(ten);
                            }
                        }
                    }
                }
            }

            lblDonViNhan.Text = tenDonVis.Count == 0 ? "(Chưa có đơn vị nhận)" : string.Join(", ", tenDonVis.OrderBy(s => s));
        }




        // ====== Load CV ======
        private void LoadCongVan(string maCv)
        {
            string sql =
                "SELECT TieuDeCV, SoCV, CoQuanBanHanh, TrichYeuND, NguoiKy, GhiChu, NgayBanHanh, NgayGui " +
                "FROM " + TABLE_CV + " WHERE MaCV=@MaCV";

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCv);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return;

                    txttieude.Text = rd["TieuDeCV"] == DBNull.Value ? "" : rd["TieuDeCV"].ToString();
                    txtsocv.Text = rd["SoCV"] == DBNull.Value ? "" : rd["SoCV"].ToString();
                    txtcqbh.Text = rd["CoQuanBanHanh"] == DBNull.Value ? "" : rd["CoQuanBanHanh"].ToString();
                    txttrichyeu.Text = rd["TrichYeuND"] == DBNull.Value ? "" : rd["TrichYeuND"].ToString();
                    txtNguoiKy.Text = rd["NguoiKy"] == DBNull.Value ? "" : rd["NguoiKy"].ToString();
                    txtGhiChu.Text = rd["GhiChu"] == DBNull.Value ? "" : rd["GhiChu"].ToString();

                    if (rd["NgayBanHanh"] != DBNull.Value)
                        txtngaybanhanh.Text = ((DateTime)rd["NgayBanHanh"]).ToString("dd/MM/yyyy");
                    if (rd["NgayGui"] != DBNull.Value)
                        txtngaygui.Text = ((DateTime)rd["NgayGui"]).ToString("dd/MM/yyyy");
                }
            }
        }

        // ====== Save (không cập nhật NoiNhan vì readonly; không có NguoiDuyet) ======
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string maCv = GetMaCVFromRequest();
            if (string.IsNullOrEmpty(maCv))
            {
                lblloi.Text = "Mã công văn không hợp lệ.";
                return;
            }

            string tieuDe = (txttieude.Text ?? "").Trim();
            if (string.IsNullOrEmpty(tieuDe))
            {
                lblloi.Text = "Tiêu đề không được để trống.";
                return;
            }

            string soCv = (txtsocv.Text ?? "").Trim();
            string coQuan = (txtcqbh.Text ?? "").Trim();
            string trichYeu = (txttrichyeu.Text ?? "").Trim();
            string nguoiKy = (txtNguoiKy.Text ?? "").Trim();
            string ghiChu = (txtGhiChu.Text ?? "").Trim();

            DateTime? ngayBanHanh, ngayGui;
            if (!TryParseDate(txtngaybanhanh.Text, out ngayBanHanh))
            {
                lblloi.Text = "Ngày ban hành không hợp lệ.";
                return;
            }
            if (!TryParseDate(txtngaygui.Text, out ngayGui))
            {
                lblloi.Text = "Ngày gửi không hợp lệ.";
                return;
            }

            string sql = "UPDATE " + TABLE_CV + " SET " +
                " TieuDeCV=@TieuDeCV, SoCV=@SoCV, CoQuanBanHanh=@CoQuanBanHanh, TrichYeuND=@TrichYeuND, " +
                " NguoiKy=@NguoiKy, GhiChu=@GhiChu, NgayBanHanh=@NgayBanHanh, NgayGui=@NgayGui " +
                " WHERE MaCV=@MaCV";

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TieuDeCV", (object)tieuDe ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SoCV", (object)soCv ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CoQuanBanHanh", (object)coQuan ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrichYeuND", (object)trichYeu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NguoiKy", (object)nguoiKy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GhiChu", (object)ghiChu ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayBanHanh", (object)ngayBanHanh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NgayGui", (object)ngayGui ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaCV", maCv);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Redirect về Trang chủ (không alert)
                    // ✔ đẩy thông điệp sang Trangchu.aspx
                    Session["toastMsg"] = "Đã lưu công văn!";
                    Session["toastType"] = "text-bg-success";

                    Response.Redirect(ResolveUrl("~/Trangchu.aspx")); // KHÔNG cần CompleteRequest
                }
            }
            catch (Exception ex)
            {
                lblloi.Text = "Có lỗi khi lưu dữ liệu: " + ex.Message;
            }

        }


        // ====== File đính kèm ======
        private void BindFileDinhKem(string maCv)
        {
            ListBox1.Items.Clear();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TenFile, ISNULL(Size,0) AS Size FROM " + TABLE_FILE + " WHERE MaCV=@MaCV ORDER BY DateUpload DESC", conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCv);
                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string ten = rd["TenFile"] == DBNull.Value ? "" : rd["TenFile"].ToString();
                        string size = rd["Size"] == DBNull.Value ? "0" : rd["Size"].ToString();
                        if (!string.IsNullOrEmpty(ten))
                            ListBox1.Items.Add(new ListItem(ten, size));
                    }
                }
            }
            ListBox1.Enabled = ListBox1.Items.Count > 0;
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            string maCv = GetMaCVFromRequest();
            if (string.IsNullOrEmpty(maCv)) { lblloi.Text = "Thiếu mã công văn."; return; }
            if (!FileUpload1.HasFile) { lblloi.Text = "Chưa chọn tệp."; return; }

            const long MaxBytes = 100L * 1024 * 1024; // 100MB
            if (FileUpload1.PostedFile.ContentLength > MaxBytes)
            {
                lblloi.Text = "File quá lớn (>100MB).";
                return;
            }

            try
            {
                SafeEnsureUploadFolder();

                string original = Path.GetFileName(FileUpload1.PostedFile.FileName);
                string name = Path.GetFileNameWithoutExtension(original);
                string ext = Path.GetExtension(original);

                // Đổi tên tệp nếu trùng
                string safe = original;
                string phys = Path.Combine(UploadRootPDir(), safe);
                int i = 1;
                while (File.Exists(phys))
                {
                    safe = name + " (" + i + ")" + ext;
                    phys = Path.Combine(UploadRootPDir(), safe);
                    i++;
                }

                FileUpload1.SaveAs(phys);

                using (SqlConnection conn = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO " + TABLE_FILE + " (MaCV, FileID, TenFile, Url, Size, DateUpload) " +
                    "VALUES (@MaCV, @FileID, @TenFile, @Url, @Size, @DateUpload)", conn))
                {
                    cmd.Parameters.AddWithValue("@MaCV", maCv);
                    cmd.Parameters.AddWithValue("@FileID", Guid.NewGuid().ToString());
                    cmd.Parameters.AddWithValue("@TenFile", safe);
                    cmd.Parameters.AddWithValue("@Url", UploadRootVDir() + safe);
                    cmd.Parameters.AddWithValue("@Size", FileUpload1.PostedFile.ContentLength);
                    cmd.Parameters.AddWithValue("@DateUpload", DateTime.Now.ToShortDateString());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Cập nhật UI
                ListBox1.Items.Add(new ListItem(safe, FileUpload1.PostedFile.ContentLength.ToString()));
                lblloi.Text = "";
            }
            catch (Exception ex)
            {
                lblloi.Text = "Không thể upload tệp: " + ex.Message;
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            string maCv = GetMaCVFromRequest();
            if (string.IsNullOrEmpty(maCv)) { lblloi.Text = "Thiếu mã công văn."; return; }
            if (ListBox1.Items.Count == 0) { lblloi.Text = "Không có tệp để xóa."; return; }

            var toDelete = new List<string>();
            foreach (ListItem it in ListBox1.Items)
                if (it.Selected && !string.IsNullOrEmpty(it.Text))
                    toDelete.Add(it.Text);

            if (toDelete.Count == 0) { lblloi.Text = "Hãy chọn tệp để xóa."; return; }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    foreach (string ten in toDelete)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "DELETE FROM " + TABLE_FILE + " WHERE MaCV=@MaCV AND TenFile=@TenFile", conn))
                        {
                            cmd.Parameters.AddWithValue("@MaCV", maCv);
                            cmd.Parameters.AddWithValue("@TenFile", ten);
                            cmd.ExecuteNonQuery();
                        }

                        string phys = Path.Combine(UploadRootPDir(), ten);
                        if (File.Exists(phys))
                        {
                            try { File.Delete(phys); } catch (Exception ex) { lblloi.Text = "Lỗi khi xóa tệp: " + ex.Message; }
                        }
                    }
                }

                // Refresh list UI
                for (int i = ListBox1.Items.Count - 1; i >= 0; i--)
                    if (ListBox1.Items[i].Selected) ListBox1.Items.RemoveAt(i);

                lblloi.Text = "";
            }
            catch (Exception ex)
            {
                lblloi.Text = "Không thể xóa tệp đã chọn: " + ex.Message;
            }
        }


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Trangchu.aspx");
        }
    }
}
