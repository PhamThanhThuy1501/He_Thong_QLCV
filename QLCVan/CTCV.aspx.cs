using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace QLCVan
{
    public partial class SuaCV : System.Web.UI.Page
    {
        // ====== Kết nối (phương án B) ======
        private string ConnStr
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                    throw new InvalidOperationException(
                        "Thiếu 'QuanLyCongVanConnectionString' trong <connectionStrings> (Web.config root).");
                return cs.ConnectionString;
            }
        }

        // ====== Cache tên bảng (lưu ViewState để postback vẫn còn) ======
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

        // ====== Helpers ======
        private object DbNullIfEmpty(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : (object)s.Trim();
        }

        private bool TryParseDate(string input, out DateTime? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(input)) return true;
            DateTime d;
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
            { result = d; return true; }
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
            { result = d; return true; }
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

        // Dò tên bảng tồn tại trong DB (candidates có thể gồm 'dbo.tblNoiDungCVs', 'tblNoiDungCV', ...)
        private string ResolveTable(SqlConnection conn, string[] candidates)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                string full = candidates[i];
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
                        return schema + "." + table; // trả về đầy đủ "schema.table"
                }
            }
            return null;
        }

        private void EnsureTableNames()
        {
            if (!string.IsNullOrEmpty(T_NOIDUNGCV) && !string.IsNullOrEmpty(T_LOAICV) && !string.IsNullOrEmpty(T_FILE))
                return;

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                if (string.IsNullOrEmpty(T_NOIDUNGCV))
                {
                    T_NOIDUNGCV = ResolveTable(conn, new string[] {
                        "dbo.tblNoiDungCVs","dbo.tblNoiDungCV",
                        "tblNoiDungCVs","tblNoiDungCV",
                        "dbo.NoiDungCVs","dbo.NoiDungCV",
                        "dbo.tblNoiDungCongVan","tblNoiDungCongVan"
                    });
                    if (string.IsNullOrEmpty(T_NOIDUNGCV))
                        throw new InvalidOperationException("Không tìm thấy bảng nội dung công văn (NoiDungCV) trong DB.");
                }

                if (string.IsNullOrEmpty(T_LOAICV))
                {
                    T_LOAICV = ResolveTable(conn, new string[] {
                        "dbo.tblLoaiCVs","dbo.tblLoaiCV",
                        "tblLoaiCVs","tblLoaiCV",
                        "dbo.LoaiCVs","dbo.LoaiCV"
                    });
                    if (string.IsNullOrEmpty(T_LOAICV))
                        throw new InvalidOperationException("Không tìm thấy bảng loại công văn (LoaiCV) trong DB.");
                }

                if (string.IsNullOrEmpty(T_FILE))
                {
                    T_FILE = ResolveTable(conn, new string[] {
                        "dbo.tblFileDinhKems","dbo.tblFileDinhKem",
                        "tblFileDinhKems","tblFileDinhKem",
                        "dbo.FileDinhKems","dbo.FileDinhKem"
                    });
                    if (string.IsNullOrEmpty(T_FILE))
                        throw new InvalidOperationException("Không tìm thấy bảng file đính kèm (FileDinhKem) trong DB.");
                }
            }
        }

        // ====== Page events ======
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Gioithieu.aspx");
                return;
            }
            if (Session["QuyenHan"] != null && Session["QuyenHan"].ToString().Trim() == "User")
            {
                ClientScript.RegisterStartupScript(GetType(), "noauth",
                    "alert('Bạn không có quyền truy cập trang này !'); location.href='Trangchu.aspx';", true);
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
                BindLoaiCV();

                string macv = Request["macv"];
                if (string.IsNullOrEmpty(macv)) macv = Request["id"];
                if (!string.IsNullOrEmpty(macv))
                    LoadForEdit(macv.Trim());
            }
        }

        // ====== Data ======
        private void BindLoaiCV()
        {
            ddlLoaiCV.Items.Clear();
            ddlLoaiCV.Items.Add(new ListItem("-- Chọn loại công văn  --", ""));

            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(
                    "SELECT DISTINCT MaLoaiCV, TenLoaiCV FROM " + T_LOAICV + " ORDER BY TenLoaiCV;", conn))
                {
                    conn.Open();
                    using (var rd = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rd.Read())
                        {
                            string text = rd["TenLoaiCV"] == DBNull.Value ? "" : rd["TenLoaiCV"].ToString();
                            string value = rd["MaLoaiCV"] == DBNull.Value ? "" : rd["MaLoaiCV"].ToString();
                            if (!string.IsNullOrWhiteSpace(value))
                                ddlLoaiCV.Items.Add(new ListItem(text, value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("Không tải được danh mục loại công văn. Chi tiết: " + ex.Message);
            }
        }

        private void LoadForEdit(string macv)
        {
            // Nội dung CV
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT MaCV, TieuDeCV, SoCV, CoQuanBanHanh, TrichYeuND, NguoiKy, GhiChu, NgayBanHanh, NgayGui, MaLoaiCV, GuiHayNhan " +
                "FROM " + T_NOIDUNGCV + " WHERE MaCV = @macv;", conn))
            {
                cmd.Parameters.AddWithValue("@macv", macv);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                    {
                        Alert("Không tìm thấy công văn để sửa (MaCV=" + macv + ").");
                        return;
                    }

                    txttieude.Text = Convert.ToString(rd["TieuDeCV"]);
                    txtsocv.Text = Convert.ToString(rd["SoCV"]);
                    txtcqbh.Text = Convert.ToString(rd["CoQuanBanHanh"]);
                    txttrichyeu.Text = Convert.ToString(rd["TrichYeuND"]);
                    txtNguoiKy.Text = Convert.ToString(rd["NguoiKy"]);
                    txtGhiChu.Text = Convert.ToString(rd["GhiChu"]);

                    if (!(rd["NgayBanHanh"] is DBNull))
                        txtngaybanhanh.Text = ((DateTime)rd["NgayBanHanh"]).ToString("yyyy-MM-dd");
                    if (!(rd["NgayGui"] is DBNull))
                        txtngaygui.Text = ((DateTime)rd["NgayGui"]).ToString("yyyy-MM-dd");

                    if (!(rd["MaLoaiCV"] is DBNull))
                    {
                        string val = Convert.ToString(rd["MaLoaiCV"]);
                        ListItem it = ddlLoaiCV.Items.FindByValue(val);
                        if (it != null) ddlLoaiCV.SelectedValue = val;
                    }

                    int guiNhan = (rd["GuiHayNhan"] is DBNull) ? 1 : Convert.ToInt32(rd["GuiHayNhan"]);
                    RadioButtonList1.SelectedValue = (guiNhan == 1) ? "Gui" : "Nhan";
                }
            }

            // File đính kèm
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT TenFile, Url FROM " + T_FILE + " WHERE MaCV=@macv ORDER BY DateUpload DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@macv", macv);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    ListBox1.Items.Clear();
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

        // ====== Buttons ======
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string macvParam = Request["macv"];
            if (string.IsNullOrEmpty(macvParam)) macvParam = Request["id"];
            if (string.IsNullOrEmpty(macvParam))
            {
                Alert("Thiếu mã công văn trên URL (macv). Vui lòng mở từ nút Sửa.");
                return;
            }
            string macv = macvParam.Trim();

            DateTime? ngayBanHanh;
            DateTime? ngayGui;

            string strNBH = (txtngaybanhanh.Text ?? string.Empty).Trim();
            string strNG = (txtngaygui.Text ?? string.Empty).Trim();

            if (!TryParseDate(strNBH, out ngayBanHanh)) { Alert("Ngày ban hành không hợp lệ."); return; }
            if (!TryParseDate(strNG, out ngayGui)) { Alert("Ngày gửi không hợp lệ."); return; }


            // Gui/Nhan
            int guiHayNhan = 1;
            string sel = (RadioButtonList1.SelectedValue ?? "").Trim();
            if (string.Compare(sel, "Nhan", StringComparison.OrdinalIgnoreCase) == 0) guiHayNhan = 0;

            // MaLoaiCV
            object maLoaiParam = DBNull.Value;
            int maLoai;
            if (int.TryParse(ddlLoaiCV.SelectedValue, out maLoai))
                maLoaiParam = maLoai;

            // UPDATE
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(@"
                    UPDATE " + /* bảng nội dung */ "" + @" " + T_NOIDUNGCV + @"
                    SET TieuDeCV=@TieuDeCV, SoCV=@SoCV, CoQuanBanHanh=@CoQuanBanHanh, TrichYeuND=@TrichYeuND,
                        NguoiKy=@NguoiKy, GhiChu=@GhiChu,
                        NgayBanHanh=@NgayBanHanh, NgayGui=@NgayGui,
                        MaLoaiCV=@MaLoaiCV, GuiHayNhan=@GuiHayNhan
                    WHERE MaCV=@MaCV;", conn))
                {
                    cmd.Parameters.AddWithValue("@TieuDeCV", DbNullIfEmpty(txttieude.Text));
                    cmd.Parameters.AddWithValue("@SoCV", DbNullIfEmpty(txtsocv.Text));
                    cmd.Parameters.AddWithValue("@CoQuanBanHanh", DbNullIfEmpty(txtcqbh.Text));
                    cmd.Parameters.AddWithValue("@TrichYeuND", DbNullIfEmpty(txttrichyeu.Text));
                    cmd.Parameters.AddWithValue("@NguoiKy", DbNullIfEmpty(txtNguoiKy.Text));
                    cmd.Parameters.AddWithValue("@GhiChu", DbNullIfEmpty(txtGhiChu.Text));

                    cmd.Parameters.Add("@NgayBanHanh", SqlDbType.Date).Value = (object)ngayBanHanh ?? DBNull.Value;
                    cmd.Parameters.Add("@NgayGui", SqlDbType.Date).Value = (object)ngayGui ?? DBNull.Value;
                    cmd.Parameters.Add("@MaLoaiCV", SqlDbType.Int).Value = maLoaiParam;
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

            // Thêm file còn thiếu
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    for (int i = 0; i < ListBox1.Items.Count; i++)
                    {
                        ListItem li = ListBox1.Items[i];
                        if (li == null || string.IsNullOrWhiteSpace(li.Text)) continue;

                        using (var check = new SqlCommand(
                            "SELECT COUNT(*) FROM " + T_FILE + " WHERE MaCV=@MaCV AND TenFile=@TenFile;", conn))
                        {
                            check.Parameters.AddWithValue("@MaCV", macv);
                            check.Parameters.AddWithValue("@TenFile", li.Text);
                            int cnt = Convert.ToInt32(check.ExecuteScalar());
                            if (cnt > 0) continue;
                        }

                        using (var insert = new SqlCommand(@"
INSERT INTO " + T_FILE + @" (FileID, MaCV, TenFile, Url, DateUpload)
VALUES (@FileID, @MaCV, @TenFile, @Url, @DateUpload);", conn))
                        {
                            insert.Parameters.AddWithValue("@FileID", Guid.NewGuid().ToString());
                            insert.Parameters.AddWithValue("@MaCV", macv);
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

            AlertAndGo("Đã lưu công văn.", "Trangchu.aspx");
            return; // đảm bảo không chạy tiếp


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


        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Trangchu.aspx");
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
            if (!anySelected) ListBox1.Items.Clear();
        }

    }
}
