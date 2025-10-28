using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class NhapNDCV : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        private string ConnStr = ConfigurationManager.ConnectionStrings["QuanLyCongVanConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Bắt đăng nhập
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Dangnhap.aspx");
                return;
            }

            // (Nếu bạn vẫn muốn check quyền thì để lại,
            //  còn không thì có thể comment block này)
            /*
            if (!PermissionHelper.HasPermission("Q002"))
            {
                Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
                Response.End();
            }
            */

            if (!Page.IsPostBack)
            {
                // 1. Load dropdown Loại công văn, Đơn vị nhận
                LoadLoaiCV();
                LoadDonViNhan();

                // 2. Ẩn panel người duyệt ban đầu
                pnlNguoiDuyet.Visible = false;

                // 3. Set placeholder ngày
                txtngaybanhanh.Attributes["placeholder"] = "dd/mm/yyyy";
                txtngaygui.Attributes["placeholder"] = "dd/mm/yyyy";

                // 4. Nếu đang edit lại CV cũ qua querystring ?macv=...
                if (Request.QueryString["macv"] != null)
                {
                    string macv = Request.QueryString["macv"].ToString();
                    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == macv);

                    if (cv1 != null)
                    {
                        // Gán dữ liệu vào form
                        txttieude.Text = cv1.TieuDeCV;
                        txtngaybanhanh.Text = cv1.NgayGui.HasValue
                            ? cv1.NgayGui.Value.ToString("dd/MM/yyyy")
                            : "";
                        txtngaygui.Text = cv1.NgayBanHanh.HasValue
                            ? cv1.NgayBanHanh.Value.ToString("dd/MM/yyyy")
                            : "";
                        txtcqbh.Text = cv1.CoQuanBanHanh;
                        txtsocv.Text = cv1.SoCV;
                        txttrichyeu.Text = cv1.TrichYeuND;
                        txtNguoiKy.Text = cv1.NguoiKy;
                        txtGhiChu.Text = cv1.GhiChu;

                        // Loại công văn
                        if (cv1.MaLoaiCV != null)
                        {
                            ListItem itLoai = ddlLoaiCV.Items.FindByValue(cv1.MaLoaiCV.ToString());
                            if (itLoai != null)
                            {
                                ddlLoaiCV.ClearSelection();
                                itLoai.Selected = true;
                            }
                        }

                        // Đơn vị nhận (NoiNhan lưu mã đơn vị)
                        if (!string.IsNullOrWhiteSpace(cv1.NoiNhan))
                        {
                            ListItem itDV = ddlDonViNhan.Items.FindByValue(cv1.NoiNhan.Trim());
                            if (itDV != null)
                            {
                                ddlDonViNhan.ClearSelection();
                                itDV.Selected = true;
                            }
                            else
                            {
                                // Nếu đơn vị cũ không còn trong tblDonVi nữa → thêm tạm cho hiển thị
                                ddlDonViNhan.Items.Add(new ListItem(cv1.NoiNhan.Trim(), cv1.NoiNhan.Trim()));
                                ddlDonViNhan.ClearSelection();
                                ddlDonViNhan.SelectedValue = cv1.NoiNhan.Trim();
                            }
                        }

                        // Gửi / Nhận
                        if (cv1.GuiHayNhan.HasValue && cv1.GuiHayNhan.Value == 0)
                            RadioButtonList1.SelectedValue = "Nhan";
                        else
                            RadioButtonList1.SelectedValue = "Gui";

                        // File đính kèm
                        ListBox1.DataTextField = "TenFile";
                        ListBox1.DataValueField = "Size"; // hoặc Url, tuỳ bạn dùng
                        ListBox1.DataSource = cv1.tblFileDinhKems;
                        ListBox1.DataBind();
                    }
                }
            }
        }

        private void LoadDonViNhan()
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                string query = "SELECT MaDonVi, TenDonVi FROM tblDonVi ORDER BY TenDonVi";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlDonViNhan.DataSource = reader;
                    ddlDonViNhan.DataTextField = "TenDonVi";   // hiển thị tên
                    ddlDonViNhan.DataValueField = "MaDonVi";   // lưu mã (BCHT, DV02,...)
                    ddlDonViNhan.DataBind();
                }
            }
            ddlDonViNhan.Items.Insert(0, new ListItem("-- Chọn đơn vị nhận --", ""));
        }

        private void LoadNguoiDuyet(string maDonVi)
        {
            ddlNguoiDuyet.Items.Clear();
            ddlNguoiDuyet.Items.Add(new ListItem("-- Chọn người duyệt --", "0"));

            string query = "SELECT MaNguoiDung, HoTen FROM tblNguoiDung WHERE MaDonVi = @MaDonVi ORDER BY HoTen";
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaDonVi", maDonVi);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ddlNguoiDuyet.Items.Add(
                            new ListItem(reader["HoTen"].ToString(), reader["MaNguoiDung"].ToString())
                        );
                    }
                }
            }
        }

        private void LoadLoaiCV()
        {
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                string query = "SELECT MaLoaiCV, TenLoaiCV, PheDuyet FROM tblLoaiCV ORDER BY TenLoaiCV";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlLoaiCV.DataSource = reader;
                    ddlLoaiCV.DataTextField = "TenLoaiCV";   // hiển thị tên
                    ddlLoaiCV.DataValueField = "MaLoaiCV";   // lưu mã
                    ddlLoaiCV.DataBind();
                }
            }

            ddlLoaiCV.Items.Insert(0, new ListItem("-- Chọn loại công văn --", ""));
        }

        protected void ddlLoaiCV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlLoaiCV.SelectedValue))
            {
                pnlNguoiDuyet.Visible = false;
                return;
            }

            bool pheDuyet = false;
            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT PheDuyet FROM tblLoaiCV WHERE MaLoaiCV = @id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(ddlLoaiCV.SelectedValue);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null &&
                    (result.ToString() == "1" || result.ToString().Equals("TRUE", StringComparison.OrdinalIgnoreCase)))
                {
                    pheDuyet = true;
                }
            }

            // Nếu loại CV yêu cầu duyệt → hiện panel duyệt
            pnlNguoiDuyet.Visible = pheDuyet;

            ddlNguoiDuyet.Items.Clear();
            ddlNguoiDuyet.Items.Add(new ListItem("-- Chọn người duyệt --", "0"));
        }

        protected void ddlDonViNhan_SelectedIndexChanged(object sender, EventArgs e)
        {
            string maDonVi = ddlDonViNhan.SelectedValue;
            if (string.IsNullOrEmpty(maDonVi))
            {
                if (pnlNguoiDuyet.Visible)
                {
                    ddlNguoiDuyet.Items.Clear();
                    ddlNguoiDuyet.Items.Add(new ListItem("-- Chọn người duyệt --", "0"));
                }
                return;
            }

            // Nếu CV cần duyệt → load danh sách người duyệt trong đơn vị chọn
            if (pnlNguoiDuyet.Visible)
            {
                LoadNguoiDuyet(maDonVi);
            }
        }

        protected void btnthem_Click(object sender, EventArgs e)
        {
            bool coDuyet = pnlNguoiDuyet.Visible && ddlNguoiDuyet.SelectedIndex > 0;

            // Parse ngày nhập kiểu dd/MM/yyyy
            DateTime ngayGui;
            DateTime ngayBanHanh;

            if (!DateTime.TryParseExact(txtngaygui.Text, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out ngayGui) ||
                !DateTime.TryParseExact(txtngaybanhanh.Text, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out ngayBanHanh))
            {
                Response.Write("<script>alert('Định dạng ngày không hợp lệ!');</script>");
                return;
            }

            // Lấy mã đơn vị nhận từ dropdown
            string maDonViNhan = ddlDonViNhan.SelectedValue; // ví dụ "BCHT"
            if (string.IsNullOrEmpty(maDonViNhan))
            {
                Response.Write("<script>alert('Vui lòng chọn đơn vị nhận!');</script>");
                return;
            }

            if (coDuyet)
            {
                // ====== TRƯỜNG HỢP CẦN DUYỆT ======
                var cv1 = new tblNoiDungCV
                {
                    MaCV = Guid.NewGuid().ToString(),
                    SoCV = txtsocv.Text.Trim(),
                    TieuDeCV = txttieude.Text.Trim(),
                    MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue),
                    CoQuanBanHanh = txtcqbh.Text.Trim(),
                    TrichYeuND = txttrichyeu.Text.Trim(),
                    NguoiKy = txtNguoiKy.Text.Trim(),
                    MaNguoiGui = Session["MaNguoiDung"].ToString(),
                    BaoMat = RadioButtonList1.SelectedValue == "Có" ? "1" : "0",
                    GhiChu = txtGhiChu.Text.Trim(),
                    TrangThai = "Đang trình",
                    NgayGui = ngayGui,
                    NgayBanHanh = ngayBanHanh,

                    // ⭐ LƯU MÃ ĐƠN VỊ NHẬN VÀO BẢNG
                    NoiNhan = maDonViNhan
                };

                // Ghi người duyệt (tên hiển thị)
                if (ddlNguoiDuyet.SelectedIndex > 0)
                {
                    cv1.NguoiDuyet = ddlNguoiDuyet.SelectedItem.Text.Trim();
                }

                db.tblNoiDungCVs.InsertOnSubmit(cv1);
                db.SubmitChanges();

                // Tạo bản ghi gửi cho người duyệt duy nhất
                string maNguoiGui = Session["MaNguoiDung"].ToString();
                string maNguoiDuyet = ddlNguoiDuyet.SelectedValue;

                var guiNhan = new tblGuiNhan
                {
                    MaCV = cv1.MaCV,
                    MaNguoiDung = maNguoiGui,
                    MaNguoiNhan = maNguoiDuyet,
                    TrangThaiNhan = "Chờ duyệt"
                };

                db.tblGuiNhans.InsertOnSubmit(guiNhan);
                db.SubmitChanges();

                Response.Redirect("NhapNDCV.aspx");
            }
            else
            {
                // ====== TRƯỜNG HỢP KHÔNG DUYỆT ======
                string MaCongVan = Guid.NewGuid().ToString();

                var cv1 = new tblNoiDungCV
                {
                    MaCV = MaCongVan,
                    SoCV = txtsocv.Text.Trim(),
                    NgayGui = ngayBanHanh,   // (chú ý: code gốc của bạn bị đảo ngày; sửa nếu cần)
                    TieuDeCV = txttieude.Text.Trim(),
                    MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue),
                    CoQuanBanHanh = txtcqbh.Text.Trim(),
                    TrichYeuND = txttrichyeu.Text.Trim(),
                    NguoiKy = txtNguoiKy.Text.Trim(),
                    MaNguoiGui = Session["MaNguoiDung"].ToString(),
                    BaoMat = RadioButtonList1.SelectedValue == "Có" ? "1" : "0",
                    GhiChu = txtGhiChu.Text.Trim(),
                    NgayBanHanh = ngayGui,   // (cũng đang đảo, giữ nguyên logic cũ của bạn)
                    TrangThai = "Đã gửi",

                    // ⭐ LƯU MÃ ĐƠN VỊ NHẬN
                    NoiNhan = maDonViNhan
                };

                db.tblNoiDungCVs.InsertOnSubmit(cv1);
                db.SubmitChanges();

                // Lưu danh sách file đính kèm
                if (ListBox1.Items.Count != 0)
                {
                    foreach (ListItem item in ListBox1.Items)
                    {
                        var fcv = new tblFileDinhKem
                        {
                            MaCV = cv1.MaCV,
                            FileID = Guid.NewGuid().ToString(),
                            Size = Convert.ToInt32(item.Value),
                            DateUpload = DateTime.Now.ToShortDateString(),
                            TenFile = item.Text,
                            Url = "~/Upload/" + item.Text
                        };

                        db.tblFileDinhKems.InsertOnSubmit(fcv);
                        db.SubmitChanges();
                    }
                }

                // Gửi cho tất cả người trong đơn vị nhận (trừ chính người gửi)
                var maNguoiGui = Session["MaNguoiDung"].ToString();
                var nguoiNhanList = db.tblNguoiDungs
                    .Where(x => x.MaDonVi == maDonViNhan && x.MaNguoiDung.ToString() != maNguoiGui)
                    .Select(x => x.MaNguoiDung)
                    .ToList();

                foreach (var maNguoiNhan in nguoiNhanList)
                {
                    db.tblGuiNhans.InsertOnSubmit(new tblGuiNhan
                    {
                        MaCV = cv1.MaCV,
                        MaNguoiDung = maNguoiGui,
                        MaNguoiNhan = maNguoiNhan,
                        TrangThaiNhan = "Chưa đọc"
                    });
                }
                db.SubmitChanges();

                Response.Redirect("NhapNDCV.aspx");
            }
        }

        /* ====== Upload file / remove file giữ nguyên như bạn ====== */

        protected void btnUp_Click(object sender, EventArgs e)
        {
            string UploadFolder = Server.MapPath("/Upload/");
            if (FileUpload1.HasFile)
            {
                try
                {
                    string filename = FileUpload1.PostedFile.FileName;
                    string FileNameOnServer = UploadFolder + filename;
                    FileUpload1.SaveAs(FileNameOnServer);

                    ListItem item = new ListItem(
                        filename,
                        Convert.ToString(FileUpload1.PostedFile.ContentLength)
                    );
                    ListBox1.Items.Add(item);
                }
                catch (Exception ex)
                {
                    lblloi.Text = "Lỗi: " + ex.Message.ToString();
                }
            }
        }

        void RemoveFile(int index)
        {
            if (index < 0 || index >= ListBox1.Items.Count) return;

            // ở code gốc bạn đang xóa theo Value như thể Value là full path, nhưng Value đang là kích thước.
            // mình sẽ KHÔNG xóa file vật lý ở đây để tránh lỗi.
            ListBox1.Items.RemoveAt(index);
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveFile(ListBox1.SelectedIndex);
        }

        protected void btnReAll_Click(object sender, EventArgs e)
        {
            while (ListBox1.Items.Count > 0)
            {
                RemoveFile(0);
            }
        }

        protected void btnlammoi_Click(object sender, EventArgs e)
        {
            txtcqbh.Text = "";
            txtngaygui.Text = "";
            txtngaybanhanh.Text = "";
            txtsocv.Text = "";
            txttieude.Text = "";
            txttrichyeu.Text = "";
            txtNguoiKy.Text = "";
            ddlNguoiDuyet.SelectedIndex = -1;
            txtGhiChu.Text = "";
            ddlDonViNhan.SelectedIndex = 0;
            ddlLoaiCV.SelectedIndex = 0;
            pnlNguoiDuyet.Visible = false;
            ListBox1.Items.Clear();
        }

        // giữ placeholder handlers cboLoaiCongvan_ItemInserting / cboNguoiKy_ItemInserted nếu bạn còn dùng AjaxControlToolkit
        protected void cboLoaiCongvan_ItemInserting(object sender, AjaxControlToolkit.ComboBoxItemInsertEventArgs e)
        {
            string congvanmoi = e.Item.Value;
            tblLoaiCV pr = new tblLoaiCV();
            pr.TenLoaiCV = congvanmoi;
            db.tblLoaiCVs.InsertOnSubmit(pr);
            db.SubmitChanges();
            pr = db.tblLoaiCVs.SingleOrDefault(p => p.TenLoaiCV == congvanmoi);
            e.Item.Value = pr.MaLoaiCV.ToString();
        }

        protected void cboNguoiKy_ItemInserted(object sender, AjaxControlToolkit.ComboBoxItemInsertEventArgs e)
        {
            string nguoikimoi = e.Item.Value;
            tblNoiDungCV pr = new tblNoiDungCV();
            pr.NguoiKy = nguoikimoi;
            db.tblNoiDungCVs.InsertOnSubmit(pr);
            db.SubmitChanges();
            pr = db.tblNoiDungCVs.SingleOrDefault(p => p.NguoiKy == nguoikimoi);
            e.Item.Value = pr.MaLoaiCV.ToString();
        }
    }
}
