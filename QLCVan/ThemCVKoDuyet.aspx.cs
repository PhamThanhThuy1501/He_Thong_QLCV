using System;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace QLCVan
{
    public partial class ThemCVKoDuyet : Page
    {
        InfoDataContext db = new InfoDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Bảo vệ session/quyền giống NhapNDCV
            if (Session["TenDN"] == null)
            {
                Response.Redirect("Gioithieu.aspx");
                return;
            }
            if ((Session["QuyenHan"] + "").Trim().Equals("User", StringComparison.OrdinalIgnoreCase))
            {
                Response.Write("<script>alert('Bạn không có quyền truy cập trang này !');document.location.href='Trangchu.aspx';</script>");
                return;
            }

            if (!IsPostBack)
            {
                // Bind loại công văn
                ddlLoaiCV.DataSource = db.tblLoaiCVs;
                ddlLoaiCV.DataTextField = "TenLoaiCV";
                ddlLoaiCV.DataValueField = "MaLoaiCV";
                ddlLoaiCV.DataBind();
                ddlLoaiCV.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Chọn loại công văn --", ""));

                // Đơn vị nhận: nếu có bảng nguồn riêng thì bind; tạm để rỗng để bạn nối sau
                // ddlDonViNhan.DataSource = ...
                // ddlDonViNhan.DataBind();

                txtNgayBanHanh.Attributes["placeholder"] = "dd/mm/yyyy";
                txtNgayGui.Attributes["placeholder"] = "dd/mm/yyyy";
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            string uploadDir = Server.MapPath("~/Upload/");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            if (FileUpload1.HasFile)
            {
                try
                {
                    // Lọc tên file an toàn + (tùy chọn) whitelist đuôi
                    string safeName = Path.GetFileName(FileUpload1.FileName);
                    string ext = Path.GetExtension(safeName).ToLowerInvariant();
                    // TODO: nếu muốn whitelist: var ok = new[] { ".pdf",".doc",".docx",".xls",".xlsx",".jpg",".png" }.Contains(ext);

                    string serverPath = Path.Combine(uploadDir, safeName);
                    FileUpload1.SaveAs(serverPath);

                    // Lưu vào ListBox: Text = tên file, Value = URL ảo để dùng lại
                    string virtualUrl = "~/Upload/" + safeName;
                    ListBox1.Items.Add(new System.Web.UI.WebControls.ListItem(safeName, virtualUrl));
                    lblLoi.Text = "";
                }
                catch (Exception ex)
                {
                    lblLoi.Text = "Lỗi upload: " + ex.Message;
                }
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            int idx = ListBox1.SelectedIndex;
            if (idx < 0) return;

            string virtualUrl = ListBox1.Items[idx].Value; // "~/Upload/xxx.ext"
            try
            {
                string physical = Server.MapPath(virtualUrl);
                if (File.Exists(physical)) File.Delete(physical);
            }
            catch { /* nuốt lỗi xóa file vật lý, vẫn xóa khỏi list */ }

            ListBox1.Items.RemoveAt(idx);
        }

        protected void btnQuayLai_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trangchu.aspx", false);
        }

        protected void btnThem_Click(object sender, EventArgs e)
        {
            // Validate đơn giản phía server
            if (string.IsNullOrWhiteSpace(txtTieuDe.Text) ||
                string.IsNullOrWhiteSpace(txtSoCV.Text) ||
                string.IsNullOrWhiteSpace(ddlLoaiCV.SelectedValue) ||
                string.IsNullOrWhiteSpace(txtNgayBanHanh.Text) ||
                string.IsNullOrWhiteSpace(txtNgayGui.Text))
            {
                lblLoi.Text = "Vui lòng nhập đủ thông tin bắt buộc.";
                return;
            }

            DateTime ngayBH, ngayGui;
            if (!DateTime.TryParseExact(txtNgayBanHanh.Text.Trim(), "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out ngayBH))
            {
                lblLoi.Text = "Ngày ban hành không đúng định dạng dd/MM/yyyy.";
                return;
            }
            if (!DateTime.TryParseExact(txtNgayGui.Text.Trim(), "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out ngayGui))
            {
                lblLoi.Text = "Ngày gửi không đúng định dạng dd/MM/yyyy.";
                return;
            }

            // Lưu DB tương tự NhapNDCV (không có Người duyệt)
            string maCV = Guid.NewGuid().ToString();
            var cv = new tblNoiDungCV
            {
                MaCV = maCV,
                SoCV = txtSoCV.Text.Trim(),
                TieuDeCV = txtTieuDe.Text.Trim(),
                MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue),
                CoQuanBanHanh = txtCQBH.Text.Trim(),
                TrichYeuND = txtTrichYeu.Text.Trim(),
                NgayBanHanh = ngayBH,
                NgayGui = ngayGui,
                NguoiKy = txtNguoiKy.Text.Trim(),
                GhiChu = txtGhiChu.Text.Trim(),
                // TrangThai: theo logic cũ NhapNDCV
                TrangThai = "Đang trình",
                GuiHayNhan = (rblBaoMat.SelectedValue == "1" ? 1 : 0) // tái dụng field theo cách cũ (nếu DB đang dùng)
            };

            db.tblNoiDungCVs.InsertOnSubmit(cv);

            // File đính kèm: chèn các item trong ListBox vào bảng file
            if (ListBox1.Items.Count > 0)
            {
                foreach (System.Web.UI.WebControls.ListItem it in ListBox1.Items)
                {
                    var f = new tblFileDinhKem
                    {
                        FileID = Guid.NewGuid().ToString(),
                        MaCV = maCV,
                        TenFile = it.Text,
                        Url = it.Value,                // "~/Upload/xxx"
                        Size = 0,                      // nếu cần, có thể đọc lại kích cỡ file
                        DateUpload = DateTime.Now.ToShortDateString()
                    };
                    db.tblFileDinhKems.InsertOnSubmit(f);
                }
            }

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                lblLoi.Text = "Lỗi lưu dữ liệu: " + ex.Message;
                return;
            }

            Response.Redirect("ThemCVKoDuyet.aspx");
        }
    }
}
