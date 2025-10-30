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

        // tên bảng động (đề phòng DB khác tên)
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

        // GIÁ TRỊ GỐC để giữ lại khi lưu (dropdown bị khoá)
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

        // Chủ sở hữu công văn (MaNguoiGui trong tblNoiDungCV)
        private string OwnerUser
        {
            get { return ViewState["OwnerUser"] as string; }
            set { ViewState["OwnerUser"] = value; }
        }

        /* =========================================
         * Helpers chung
         * ========================================= */

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

        /* =========================================
         * Hỗ trợ xác thực & phân quyền
         * ========================================= */

        // Lấy ID người dùng hiện tại (ưu tiên các session phổ biến)
        private string GetCurrentUserId()
        {
            if (Session["MaNguoiGui"] != null && !string.IsNullOrWhiteSpace(Session["MaNguoiGui"].ToString()))
                return Session["MaNguoiGui"].ToString().Trim();

            if (Session["TenDN"] != null && !string.IsNullOrWhiteSpace(Session["TenDN"].ToString()))
                return Session["TenDN"].ToString().Trim();

            if (Session["MaNhanVien"] != null && !string.IsNullOrWhiteSpace(Session["MaNhanVien"].ToString()))
                return Session["MaNhanVien"].ToString().Trim();

            return "";
        }

        // Kiểm tra xem user hiện tại có trong tblGuiNhan không (được giao/xử lý công văn)
        // Cho phép match theo MaNguoiNhan (nvarchar) hoặc MaNguoiDung (int)
        private bool IsUserInGuiNhan(string maCV)
        {
            string currentUser = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(currentUser)) return false;

            int asInt;
            bool hasInt = int.TryParse(currentUser, out asInt);

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                @"SELECT COUNT(*) 
                  FROM dbo.tblGuiNhan 
                  WHERE MaCV = @MaCV
                    AND (
                          MaNguoiNhan = @MaNguoiNhan
                       OR (@HasInt = 1 AND MaNguoiDung = @MaNguoiDung)
                    );", conn))
            {
                cmd.Parameters.AddWithValue("@MaCV", maCV);
                cmd.Parameters.AddWithValue("@MaNguoiNhan", currentUser ?? "");
                cmd.Parameters.AddWithValue("@HasInt", hasInt ? 1 : 0);
                cmd.Parameters.AddWithValue("@MaNguoiDung", hasInt ? asInt : 0);

                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        // User có quyền cao hơn 'User'?
        private bool IsElevated()
        {
            if (Session["QuyenHan"] == null) return true; // nếu không có gán quyền, coi như không chặn
            string role = Session["QuyenHan"].ToString().Trim();
            return !role.Equals("User", StringComparison.OrdinalIgnoreCase);
        }

        // Quyết định cuối cùng: người này có quyền chỉnh sửa công văn MaCV hay không?
        private bool CanCurrentUserEdit(string maCV)
        {
            string currentUser = GetCurrentUserId();
            string owner = OwnerUser == null ? "" : OwnerUser.Trim();

            bool sameOwner = string.Equals(currentUser, owner, StringComparison.OrdinalIgnoreCase);
            bool involved = IsUserInGuiNhan(maCV);
            bool elevated = IsElevated();

            bool allow = sameOwner || involved || elevated;

            // debug dev -> console (F12), không quấy user
            string debugJs =
                "console.log('[SuaCV DEBUG] currentUser=" + HttpUtility.JavaScriptStringEncode(currentUser) +
                " | owner=" + HttpUtility.JavaScriptStringEncode(owner) +
                " | sameOwner=" + sameOwner.ToString() +
                " | involved=" + involved.ToString() +
                " | elevated=" + elevated.ToString() +
                " | allow=" + allow.ToString() + "');";

            if (Page != null && ScriptManager.GetCurrent(Page) != null)
                ScriptManager.RegisterStartupScript(this, GetType(), "dbgAuth", debugJs, true);
            else
                ClientScript.RegisterStartupScript(GetType(), "dbgAuth", debugJs, true);

            return allow;
        }

        // Nếu không có quyền => khoá UI
        private void LockEditingForUnauthorizedUser()
        {
            txttieude.Enabled = false;
            txtsocv.Enabled = false;
            txtcqbh.Enabled = false;
            txttrichyeu.Enabled = false;
            txtNguoiKy.Enabled = false;
            txtGhiChu.Enabled = false;

            txtngaybanhanh.Enabled = false;
            txtngaygui.Enabled = false;

            RadioButtonList1.Enabled = false;

            FileUpload1.Enabled = false;
            btnUp.Enabled = false;
            btnDelete.Enabled = false;
            ListBox1.Enabled = false;

            ddlLoaiCV.Enabled = false;
            ddlDonViNhan.Enabled = false;

            btnSave.Enabled = false;

            string debugJs = "console.warn('[SuaCV] Form bị khóa vì không đủ quyền chỉnh sửa');";
            if (Page != null && ScriptManager.GetCurrent(Page) != null)
                ScriptManager.RegisterStartupScript(this, GetType(), "warnLock", debugJs, true);
            else
                ClientScript.RegisterStartupScript(GetType(), "warnLock", debugJs, true);
        }

        // Nếu có quyền => bật lại các control cần chỉnh sửa (trừ dropdown cấm đổi)
        private void UnlockForAuthorizedUser()
        {
            txttieude.Enabled = true;
            txtsocv.Enabled = true;
            txtcqbh.Enabled = true;
            txttrichyeu.Enabled = true;
            txtNguoiKy.Enabled = true;
            txtGhiChu.Enabled = true;

            txtngaybanhanh.Enabled = true;
            txtngaygui.Enabled = true;

            RadioButtonList1.Enabled = true;

            FileUpload1.Enabled = true;
            btnUp.Enabled = true;
            btnDelete.Enabled = true;
            ListBox1.Enabled = true;

            // 2 dropdown nhạy cảm vẫn phải khóa theo nghiệp vụ
            ddlLoaiCV.Enabled = false;
            ddlDonViNhan.Enabled = false;

            btnSave.Enabled = true;
        }

        /* =========================================
         * Resolve bảng thật trong DB (phòng khác tên)
         * ========================================= */

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

        /* =========================================
         * Bind dropdowns
         * ========================================= */

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

        /* =========================================
         * Load dữ liệu CV lên form
         * ========================================= */

        private void LoadForEdit(string maCV)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaCV, TieuDeCV, SoCV, CoQuanBanHanh, TrichYeuND, NguoiKy, GhiChu," +
                "       NgayBanHanh, NgayGui, MaLoaiCV, GuiHayNhan, NoiNhan, MaNguoiGui " +
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
                    txttieude.Text   = Convert.ToString(rd["TieuDeCV"]);
                    txtsocv.Text     = Convert.ToString(rd["SoCV"]);
                    txtcqbh.Text     = Convert.ToString(rd["CoQuanBanHanh"]);
                    txttrichyeu.Text = Convert.ToString(rd["TrichYeuND"]);
                    txtNguoiKy.Text  = Convert.ToString(rd["NguoiKy"]);
                    txtGhiChu.Text   = Convert.ToString(rd["GhiChu"]);

                    // dates
                    txtngaybanhanh.Text = (rd["NgayBanHanh"] is DBNull)
                        ? ""
                        : ((DateTime)rd["NgayBanHanh"]).ToString("yyyy-MM-dd");

                    txtngaygui.Text = (rd["NgayGui"] is DBNull)
                        ? ""
                        : ((DateTime)rd["NgayGui"]).ToString("yyyy-MM-dd");

                    // ===== Loại CV (giá trị gốc) =====
                    string maLoai = rd["MaLoaiCV"] is DBNull ? "" : rd["MaLoaiCV"].ToString().Trim();
                    OrigMaLoaiCV = maLoai;
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
                            ddlLoaiCV.Items.Add(new ListItem("(Loại đã xóa) " + maLoai, maLoai));
                            ddlLoaiCV.ClearSelection();
                            ddlLoaiCV.SelectedValue = maLoai;
                        }
                    }

                    // ===== Đơn vị nhận (giá trị gốc) =====
                    string maDonVi = rd["NoiNhan"] is DBNull ? "" : rd["NoiNhan"].ToString().Trim();
                    OrigNoiNhan = maDonVi;
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
                            ddlDonViNhan.Items.Add(new ListItem("(Đơn vị đã xóa) " + maDonVi, maDonVi));
                            ddlDonViNhan.ClearSelection();
                            ddlDonViNhan.SelectedValue = maDonVi;
                        }
                    }

                    // GuiHayNhan (0 / 1)
                    int guiNhan = (rd["GuiHayNhan"] is DBNull) ? 1 : Convert.ToInt32(rd["GuiHayNhan"]);
                    RadioButtonList1.SelectedValue = (guiNhan == 1) ? "Gui" : "Nhan";

                    // Chủ sở hữu = MaNguoiGui
                    OwnerUser = rd["MaNguoiGui"] is DBNull
                        ? ""
                        : rd["MaNguoiGui"].ToString().Trim();
                }
            }

            // file đính kèm
            BindFileList(maCV);

            // khoá dropdown nhạy cảm luôn
            ddlLoaiCV.Enabled = false;
            ddlDonViNhan.Enabled = false;

            // phân quyền
            if (!CanCurrentUserEdit(maCV))
            {
                LockEditingForUnauthorizedUser();
            }
            else
            {
                UnlockForAuthorizedUser();
            }
        }

        /* =========================================
         * Page_Load
         * ========================================= */

        protected void Page_Load(object sender, EventArgs e)
        {
            // nếu chưa đăng nhập -> đá ra
            if (Session["TenDN"] == null
             && Session["MaNguoiGui"] == null
             && Session["MaNhanVien"] == null)
            {
                Response.Redirect("Gioithieu.aspx");
                return;
            }

            // nếu QuyenHan = User thì vẫn được vào trang,
            // nhưng sau này CanCurrentUserEdit sẽ quyết định có sửa được hay không.
            // Tuy nhiên nếu bạn MUỐN block hẳn User khỏi trang sửa luôn,
            // bỏ comment 4 dòng dưới:
            /*
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
            */

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

                // bind dropdown
                BindLoaiCV();
                BindDonViNhan();

                // load data CV + phân quyền
                LoadForEdit(macv);
            }
        }

        /* =========================================
         * Save / Update
         * ========================================= */

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string macv = GetMaCVFromRequest();
            if (string.IsNullOrEmpty(macv))
            {
                Alert("Thiếu mã công văn trên URL (macv). Vui lòng mở từ nút Sửa.");
                return;
            }

            // chặn post tay từ người không có quyền
            if (!CanCurrentUserEdit(macv))
            {
                Alert("Bạn không có quyền sửa công văn này.");
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

            // đưa file upload tạm vào ListBox1 (chưa ghi DB)
            HandleFileUploadToListBox();

            // gửi / nhận
            int guiHayNhan = 1;
            string sel = (RadioButtonList1.SelectedValue ?? "").Trim();
            if (string.Compare(sel, "Nhan", StringComparison.OrdinalIgnoreCase) == 0)
            {
                guiHayNhan = 0;
            }

            // GIỮ NGUYÊN GIÁ TRỊ GỐC dropdown (vì dropdown disable)
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

                    cmd.Parameters.Add("@NgayBanHanh", SqlDbType.DateTime).Value =
                        (object)ngayBanHanh ?? DBNull.Value;
                    cmd.Parameters.Add("@NgayGui", SqlDbType.DateTime).Value =
                        (object)ngayGui ?? DBNull.Value;

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

            // đồng bộ danh sách file đính kèm từ ListBox1 -> DB
            SaveFileListToDb(macv);

            AlertAndGo("Đã lưu công văn.", "Trangchu.aspx");
        }

        /* =========================================
         * File handling
         * ========================================= */

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

                        // tránh insert trùng
                        using (var check = new SqlCommand(
                            "SELECT COUNT(*) FROM " + T_FILE + " WHERE MaCV=@MaCV AND TenFile=@TenFile;", conn))
                        {
                            check.Parameters.AddWithValue("@MaCV", maCV);
                            check.Parameters.AddWithValue("@TenFile", li.Text);

                            int cnt = Convert.ToInt32(check.ExecuteScalar());
                            if (cnt > 0)
                                continue;
                        }

                        // thêm file mới
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

        /* =========================================
         * Các nút
         * ========================================= */

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
                // hành vi cũ: nếu không tick chọn file nào mà vẫn bấm Xóa → clear hết
                ListBox1.Items.Clear();
            }
        }
    }
}
