using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class SuaCV : Page
    {
        /* ========== Kết nối DB ========== */

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

        // tên bảng động
        private string T_NOIDUNGCV
        {
            get { return ViewState["T_NOIDUNGCV"] as string; }
            set { ViewState["T_NOIDUNGCV"] = value; }
        }

        private string T_LOAICV
        {
            get { return ViewState["T_LOAICV"] as string; }
            set { ViewState["T_LOAICV"] = value; }
        }

        private string T_FILE
        {
            get { return ViewState["T_FILE"] as string; }
            set { ViewState["T_FILE"] = value; }
        }

        // GIÁ TRỊ GỐC (để giữ lại khi lưu)
        private string OrigMaLoaiCV
        {
            get { return ViewState["OrigMaLoaiCV"] as string; }
            set { ViewState["OrigMaLoaiCV"] = value; }
        }

        private string OrigNoiNhan
        {
            get { return ViewState["OrigNoiNhan"] as string; }
            set { ViewState["OrigNoiNhan"] = value; }
        }

        /* ========== Helpers chung ========== */

        private object DbNullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : (object)s.Trim();
        }

        private bool TryParseDate(string input, out DateTime? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(input)) return true;

            DateTime d;
            if (DateTime.TryParseExact(
                    input,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out d))
            {
                result = d;
                return true;
            }

            if (DateTime.TryParseExact(
                    input,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out d))
            {
                result = d;
                return true;
            }

            return false;
        }

        private void Alert(string msg)
        {
            string safe = HttpUtility.JavaScriptStringEncode(msg ?? string.Empty);
            string script = "alert('" + safe + "');";

            if (Page != null && ScriptManager.GetCurrent(Page) != null)
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
            else
                ClientScript.RegisterStartupScript(GetType(), "alert", script, true);
        }

        private void AlertAndGo(string msg, string url)
        {
            string safeMsg = HttpUtility.JavaScriptStringEncode(msg ?? string.Empty);
            string safeUrl = ResolveUrl(url ?? "~/Trangchu.aspx");
            string js = "alert('" + safeMsg + "'); window.location='" + safeUrl + "';";

            if (Page != null && ScriptManager.GetCurrent(Page) != null)
                ScriptManager.RegisterStartupScript(this, GetType(), "alertgo", js, true);
            else
                ClientScript.RegisterStartupScript(GetType(), "alertgo", js, true);
        }

        private string GetMaCVFromRequest()
        {
            string macv = Request["macv"];
            if (string.IsNullOrEmpty(macv))
                macv = Request["id"];

            return string.IsNullOrEmpty(macv) ? null : macv.Trim();
        }

        /* ========== Resolve bảng thật trong DB ========== */

        private string ResolveTable(SqlConnection conn, string[] candidates)
        {
            foreach (string full in candidates)
            {
                string schema = "dbo";
                string table = full;
                int dot = full.IndexOf('.');

                if (dot >= 0)
                {
                    schema = full.Substring(0, dot);
                    table = full.Substring(dot + 1);
                }

                using (var cmd = new SqlCommand(@"
                    SELECT 1
                    FROM sys.tables t
                    JOIN sys.schemas s ON t.schema_id = s.schema_id
                    WHERE t.name = @t AND s.name = @s;", conn))
                {
                    cmd.Parameters.AddWithValue("@t", table);
                    cmd.Parameters.AddWithValue("@s", schema);

                    object o = cmd.ExecuteScalar();
                    if (o != null)
                        return schema + "." + table;
                }
            }

            return null;
        }

        private void EnsureTableNames()
        {
            if (!string.IsNullOrEmpty(T_NOIDUNGCV)
             && !string.IsNullOrEmpty(T_LOAICV)
             && !string.IsNullOrEmpty(T_FILE))
            {
                return;
            }

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                if (string.IsNullOrEmpty(T_NOIDUNGCV))
                {
                    T_NOIDUNGCV = ResolveTable(conn, new[] {
                        "dbo.tblNoiDungCV","dbo.tblNoiDungCVs",
                        "tblNoiDungCV","tblNoiDungCVs",
                        "dbo.NoiDungCV","dbo.NoiDungCVs",
                        "dbo.tblNoiDungCongVan","tblNoiDungCongVan"
                    });
                    if (string.IsNullOrEmpty(T_NOIDUNGCV))
                        throw new InvalidOperationException("Không tìm thấy bảng nội dung công văn trong DB.");
                }

                if (string.IsNullOrEmpty(T_LOAICV))
                {
                    T_LOAICV = ResolveTable(conn, new[] {
                        "dbo.tblLoaiCV","dbo.tblLoaiCVs",
                        "tblLoaiCV","tblLoaiCVs",
                        "dbo.LoaiCV","dbo.LoaiCVs"
                    });
                    if (string.IsNullOrEmpty(T_LOAICV))
                        throw new InvalidOperationException("Không tìm thấy bảng loại công văn trong DB.");
                }

                if (string.IsNullOrEmpty(T_FILE))
                {
                    T_FILE = ResolveTable(conn, new[] {
                        "dbo.tblFileDinhKem","dbo.tblFileDinhKems",
                        "tblFileDinhKem","tblFileDinhKems",
                        "dbo.FileDinhKem","dbo.FileDinhKems"
                    });
                    if (string.IsNullOrEmpty(T_FILE))
                        throw new InvalidOperationException("Không tìm thấy bảng file đính kèm trong DB.");
                }
            }
        }

        /* ========== Bind dropdowns ========== */

        private void BindLoaiCV()
        {
            ddlLoaiCV.Items.Clear();
            ddlLoaiCV.Items.Add(new ListItem("-- Chọn loại công văn  --", ""));

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaLoaiCV, TenLoaiCV FROM " + T_LOAICV + " ORDER BY TenLoaiCV;", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rd.Read())
                    {
                        string ma = rd["MaLoaiCV"] == DBNull.Value ? "" : rd["MaLoaiCV"].ToString();
                        string ten = rd["TenLoaiCV"] == DBNull.Value ? "" : rd["TenLoaiCV"].ToString();

                        if (!string.IsNullOrWhiteSpace(ma))
                        {
                            ddlLoaiCV.Items.Add(new ListItem(ten, ma));
                        }
                    }
                }
            }
        }

        private void BindDonViNhan()
        {
            ddlDonViNhan.Items.Clear();
            ddlDonViNhan.Items.Add(new ListItem("-- Chọn đơn vị nhận --", ""));

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaDonVi, TenDonVi FROM tblDonVi ORDER BY TenDonVi;", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string ma = rd["MaDonVi"] == DBNull.Value ? "" : rd["MaDonVi"].ToString().Trim();
                        string ten = rd["TenDonVi"] == DBNull.Value ? "" : rd["TenDonVi"].ToString().Trim();

                        if (!string.IsNullOrEmpty(ma))
                        {
                            ddlDonViNhan.Items.Add(new ListItem(ten, ma));
                        }
                    }
                }
            }
        }

        private void BindFileList(string maCV)
        {
            ListBox1.Items.Clear();

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT TenFile, Url FROM " + T_FILE + " WHERE MaCV=@MaCV ORDER BY DateUpload DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCV);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ListBox1.Items.Add(new ListItem(
                            Convert.ToString(rd["TenFile"]),
                            Convert.ToString(rd["Url"])
                        ));
                    }
                }
            }
        }

        /* ========== Load dữ liệu CV lên form ========== */

        private void LoadForEdit(string maCV)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaCV, TieuDeCV, SoCV, CoQuanBanHanh, TrichYeuND, NguoiKy, GhiChu," +
                "       NgayBanHanh, NgayGui, MaLoaiCV, GuiHayNhan, NoiNhan " +
                "FROM " + T_NOIDUNGCV + " WHERE MaCV = @MaCV;", conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCV);

                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                    {
                        Alert("Không tìm thấy công văn để sửa (MaCV=" + maCV + ").");
                        return;
                    }

                    // text fields
                    txttieude.Text = Convert.ToString(rd["TieuDeCV"]);
                    txtsocv.Text = Convert.ToString(rd["SoCV"]);
                    txtcqbh.Text = Convert.ToString(rd["CoQuanBanHanh"]);
                    txttrichyeu.Text = Convert.ToString(rd["TrichYeuND"]);
                    txtNguoiKy.Text = Convert.ToString(rd["NguoiKy"]);
                    txtGhiChu.Text = Convert.ToString(rd["GhiChu"]);

                    // dates
                    txtngaybanhanh.Text = (rd["NgayBanHanh"] is DBNull)
                        ? ""
                        : ((DateTime)rd["NgayBanHanh"]).ToString("yyyy-MM-dd");

                    txtngaygui.Text = (rd["NgayGui"] is DBNull)
                        ? ""
                        : ((DateTime)rd["NgayGui"]).ToString("yyyy-MM-dd");

                    // ===== Loại CV =====
                    string maLoai = rd["MaLoaiCV"] is DBNull ? "" : rd["MaLoaiCV"].ToString().Trim();
                    OrigMaLoaiCV = maLoai; // giữ bản gốc để save
                    if (!string.IsNullOrEmpty(maLoai))
                    {
                        ListItem itLoai = ddlLoaiCV.Items.FindByValue(maLoai);
                        if (itLoai != null)
                        {
                            ddlLoaiCV.ClearSelection();
                            itLoai.Selected = true;
                        }
                        else
                        {
                            // loại bị xóa khỏi danh mục
                            ddlLoaiCV.Items.Add(new ListItem("(Loại đã xóa) " + maLoai, maLoai));
                            ddlLoaiCV.ClearSelection();
                            ddlLoaiCV.SelectedValue = maLoai;
                        }
                    }

                    // ===== Đơn vị nhận =====
                    string maDonVi = rd["NoiNhan"] is DBNull ? "" : rd["NoiNhan"].ToString().Trim();
                    OrigNoiNhan = maDonVi; // giữ bản gốc để save
                    if (!string.IsNullOrEmpty(maDonVi))
                    {
                        ListItem itDV = ddlDonViNhan.Items.FindByValue(maDonVi);
                        if (itDV != null)
                        {
                            ddlDonViNhan.ClearSelection();
                            itDV.Selected = true;
                        }
                        else
                        {
                            // đơn vị không còn trong tblDonVi → vẫn add tạm cho người dùng nhìn
                            ddlDonViNhan.Items.Add(new ListItem("(Đơn vị đã xóa) " + maDonVi, maDonVi));
                            ddlDonViNhan.ClearSelection();
                            ddlDonViNhan.SelectedValue = maDonVi;
                        }
                    }

                    // GuiHayNhan
                    int guiNhan = (rd["GuiHayNhan"] is DBNull) ? 1 : Convert.ToInt32(rd["GuiHayNhan"]);
                    RadioButtonList1.SelectedValue = (guiNhan == 1) ? "Gui" : "Nhan";
                }
            }

            // file đính kèm
            BindFileList(maCV);

            // QUAN TRỌNG: KHÓA 2 dropdown NÀY LUÔN
            ddlLoaiCV.Enabled = false;
            ddlDonViNhan.Enabled = false;
        }

        /* ========== Page_Load ========== */

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Gioithieu.aspx");
                return;
            }

            if (Session["QuyenHan"] != null &&
                Session["QuyenHan"].ToString().Trim().Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "noauth",
                    "alert('Bạn không có quyền truy cập trang này !'); location.href='Trangchu.aspx';",
                    true
                );
                return;
            }

            try
            {
                EnsureTableNames();
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
                return;
            }

            if (!IsPostBack)
            {
                string macv = GetMaCVFromRequest();
                if (string.IsNullOrEmpty(macv))
                {
                    Alert("Thiếu mã công văn.");
                    return;
                }

                // 1. bind dropdown
                BindLoaiCV();
                BindDonViNhan();

                // 2. load data CV (set selected + ghi nhớ giá trị gốc + disable dropdown)
                LoadForEdit(macv);
            }
        }

        /* ========== Save / Update ========== */

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string macv = GetMaCVFromRequest();
            if (string.IsNullOrEmpty(macv))
            {
                Alert("Thiếu mã công văn trên URL (macv). Vui lòng mở từ nút Sửa.");
                return;
            }

            // parse ngày
            DateTime? ngayBanHanh;
            DateTime? ngayGui;
            if (!TryParseDate((txtngaybanhanh.Text ?? "").Trim(), out ngayBanHanh))
            {
                Alert("Ngày ban hành không hợp lệ.");
                return;
            }
            if (!TryParseDate((txtngaygui.Text ?? "").Trim(), out ngayGui))
            {
                Alert("Ngày gửi không hợp lệ.");
                return;
            }

            // file upload tạm thời
            HandleFileUploadToListBox();

            // Gửi / Nhận
            int guiHayNhan = 1;
            string sel = (RadioButtonList1.SelectedValue ?? "").Trim();
            if (string.Compare(sel, "Nhan", StringComparison.OrdinalIgnoreCase) == 0)
            {
                guiHayNhan = 0;
            }

            // LẤY GIÁ TRỊ GỐC, KHÔNG LẤY TỪ DROPDOWN (vì dropdown disable, user không được đổi)
            object maLoaiParam = DBNull.Value;
            int maLoaiInt;
            if (int.TryParse(OrigMaLoaiCV, out maLoaiInt))
            {
                maLoaiParam = maLoaiInt;
            }

            string noiNhanSave = (OrigNoiNhan ?? "").Trim();

            // UPDATE DB
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(
                    "UPDATE " + T_NOIDUNGCV + @"
                     SET TieuDeCV      = @TieuDeCV,
                         SoCV          = @SoCV,
                         CoQuanBanHanh = @CoQuanBanHanh,
                         TrichYeuND    = @TrichYeuND,
                         NguoiKy       = @NguoiKy,
                         GhiChu        = @GhiChu,
                         NgayBanHanh   = @NgayBanHanh,
                         NgayGui       = @NgayGui,
                         MaLoaiCV      = @MaLoaiCV,
                         NoiNhan       = @NoiNhan,
                         GuiHayNhan    = @GuiHayNhan
                     WHERE MaCV        = @MaCV;", conn))
                {
                    cmd.Parameters.AddWithValue("@TieuDeCV", DbNullIfEmpty(txttieude.Text));
                    cmd.Parameters.AddWithValue("@SoCV", DbNullIfEmpty(txtsocv.Text));
                    cmd.Parameters.AddWithValue("@CoQuanBanHanh", DbNullIfEmpty(txtcqbh.Text));
                    cmd.Parameters.AddWithValue("@TrichYeuND", DbNullIfEmpty(txttrichyeu.Text));
                    cmd.Parameters.AddWithValue("@NguoiKy", DbNullIfEmpty(txtNguoiKy.Text));
                    cmd.Parameters.AddWithValue("@GhiChu", DbNullIfEmpty(txtGhiChu.Text));

                    cmd.Parameters.Add("@NgayBanHanh", SqlDbType.Date).Value =
                        (object)ngayBanHanh ?? DBNull.Value;
                    cmd.Parameters.Add("@NgayGui", SqlDbType.Date).Value =
                        (object)ngayGui ?? DBNull.Value;

                    // giữ nguyên giá trị gốc
                    cmd.Parameters.Add("@MaLoaiCV", SqlDbType.Int).Value = maLoaiParam;
                    cmd.Parameters.AddWithValue("@NoiNhan", DbNullIfEmpty(noiNhanSave));

                    cmd.Parameters.Add("@GuiHayNhan", SqlDbType.Int).Value = guiHayNhan;

                    cmd.Parameters.AddWithValue("@MaCV", macv);

                    conn.Open();
                    int n = cmd.ExecuteNonQuery();
                    if (n == 0)
                    {
                        Alert("Không tìm thấy công văn để cập nhật (MaCV=" + macv + ").");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("Lỗi cập nhật công văn: " + ex.Message);
                return;
            }

            // sync file đính kèm
            SaveFileListToDb(macv);

            AlertAndGo("Đã lưu công văn.", "Trangchu.aspx");
        }

        /* ========== File handling ========== */

        private void HandleFileUploadToListBox()
        {
            if (!FileUpload1.HasFile) return;

            string uploadFolder = Server.MapPath("~/Upload/");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
            string physicalPath = Path.Combine(uploadFolder, filename);

            try
            {
                FileUpload1.SaveAs(physicalPath);

                bool existed = false;
                for (int i = 0; i < ListBox1.Items.Count; i++)
                {
                    if (string.Equals(ListBox1.Items[i].Text, filename, StringComparison.OrdinalIgnoreCase))
                    {
                        existed = true;
                        break;
                    }
                }

                if (!existed)
                {
                    ListBox1.Items.Add(new ListItem(filename, "~/Upload/" + filename));
                }
            }
            catch (Exception ex)
            {
                Alert("Upload tệp thất bại: " + ex.Message);
            }
        }

        private void SaveFileListToDb(string maCV)
        {
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    for (int i = 0; i < ListBox1.Items.Count; i++)
                    {
                        ListItem li = ListBox1.Items[i];
                        if (li == null || string.IsNullOrWhiteSpace(li.Text))
                            continue;

                        using (var check = new SqlCommand(
                            "SELECT COUNT(*) FROM " + T_FILE + " WHERE MaCV=@MaCV AND TenFile=@TenFile;", conn))
                        {
                            check.Parameters.AddWithValue("@MaCV", maCV);
                            check.Parameters.AddWithValue("@TenFile", li.Text);

                            int cnt = Convert.ToInt32(check.ExecuteScalar());
                            if (cnt > 0)
                                continue;
                        }

                        using (var insert = new SqlCommand(
                            "INSERT INTO " + T_FILE + @" (FileID, MaCV, TenFile, Url, DateUpload)
                             VALUES (@FileID, @MaCV, @TenFile, @Url, @DateUpload);", conn))
                        {
                            insert.Parameters.AddWithValue("@FileID", Guid.NewGuid().ToString());
                            insert.Parameters.AddWithValue("@MaCV", maCV);
                            insert.Parameters.AddWithValue("@TenFile", li.Text);
                            insert.Parameters.AddWithValue("@Url", li.Value);
                            insert.Parameters.AddWithValue("@DateUpload", DateTime.Now);

                            insert.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("Lỗi lưu tệp đính kèm: " + ex.Message);
            }
        }

        /* ========== Buttons ========== */

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Trangchu.aspx");
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            HandleFileUploadToListBox();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool anySelected = false;

            for (int i = ListBox1.Items.Count - 1; i >= 0; i--)
            {
                if (ListBox1.Items[i].Selected)
                {
                    ListBox1.Items.RemoveAt(i);
                    anySelected = true;
                }
            }

            if (!anySelected)
            {
                ListBox1.Items.Clear();
            }
        }
    }
}
