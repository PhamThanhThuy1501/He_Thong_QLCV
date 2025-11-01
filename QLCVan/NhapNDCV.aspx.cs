using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
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

            if ((Session["TenDN"] == null))
            {
                Response.Redirect("Dangnhap.aspx");
            }

            /// check quyền bằng mã nhá , nếu user ko có mã quyền thì ko vào dc
       /*     if (!PermissionHelper.HasPermission("Q002"))
            {
                Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
                Response.End();
            }
*/
            if (!Page.IsPostBack)
            {
                LoadLoaiCV();
                lstDonViNhan.DataSource = db.tblDonVis.ToList();
                lstDonViNhan.DataTextField = "TenDonVi";   // cột hiển thị
                lstDonViNhan.DataValueField = "MaDonVi";   // cột lưu giá trị
                lstDonViNhan.DataBind();
                pnlNguoiDuyet.Visible = false;

                // BỎ HOÀN TOÀN ĐOẠN NÀY (VÌ SAI):
                //var cboLoaiCongvans = from d in db.tblLoaiCVs
                //                      where d.MaLoaiCV.ToString() != null
                //                      select d.TenLoaiCV;
                //ddlLoaiCV.DataSource = cboLoaiCongvans;
                //ddlLoaiCV.DataBind();

                // ✅ Dùng đoạn đúng 1 lần duy nhất:
                ddlLoaiCV.DataSource = db.tblLoaiCVs.ToList();
                ddlLoaiCV.DataTextField = "TenLoaiCV";
                ddlLoaiCV.DataValueField = "MaLoaiCV";
                ddlLoaiCV.DataBind();
                ddlLoaiCV.Items.Insert(0, new ListItem("-- Chọn loại công văn --", ""));

                // ✅ Phần GridView giữ nguyên
                var gv1 = from g in db.tblNoiDungCVs
                          join h in db.tblLoaiCVs on g.MaLoaiCV equals h.MaLoaiCV
                          orderby g.NgayGui descending
                          select new
                          {
                              g.MaCV,
                              g.SoCV,
                              h.TenLoaiCV,
                              g.NgayGui,
                              g.TieuDeCV,
                              g.CoQuanBanHanh,
                              g.GhiChu,
                              g.NgayBanHanh,
                              g.NguoiKy,
                              g.NoiNhan,
                              TrichYeuND = g.TrichYeuND.Substring(0, 250) + " ... ",
                              g.TrangThai
                          };

                gvnhapcnden.DataSource = gv1;
                gvnhapcnden.DataBind();

                // Placeholder cho date
                txtngaybanhanh.Attributes["placeholder"] = "dd/mm/yyyy";
                txtngaygui.Attributes["placeholder"] = "dd/mm/yyyy";



                if (Request.QueryString["macv"] != null)
                {
                    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV.ToString() == (Request.QueryString["macv"].ToString()));

                    if (cv1 != null)
                    {
                        txttieude.Text = cv1.TieuDeCV;
                        // Sửa tên biến để khớp với giao diện mới
                        txtngaybanhanh.Text = cv1.NgayGui.ToString();
                        txtngaygui.Text = cv1.NgayBanHanh.ToString();
                        txtcqbh.Text = cv1.CoQuanBanHanh;
                        txtsocv.Text = cv1.SoCV;
                        txttrichyeu.Text = cv1.TrichYeuND;
                        // Nút "Thêm" đã được hiển thị, không cần ẩn đi
                        // btnthem.Visible = false;
                        // Nút "Lưu" đã được thay bằng "Quay lại"
                        // btnsua.Visible = true;
                        ListBox1.DataTextField = "TenFile";
                        ListBox1.DataSource = cv1.tblFileDinhKems;
                        ListBox1.DataBind();
                        RadioButtonList1.SelectedIndex = (int)cv1.GuiHayNhan;

                        // Cập nhật các trường mới
                        txtNguoiKy.Text = cv1.NguoiKy;
                        txtGhiChu.Text = cv1.GhiChu;
                        // Các trường khác tương tự...
                    }
                }

            }
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
                        ddlNguoiDuyet.Items.Add(new ListItem(reader["HoTen"].ToString(), reader["MaNguoiDung"].ToString()));
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
                    ddlLoaiCV.DataTextField = "TenLoaiCV";  // ✅ hiển thị tên
                    ddlLoaiCV.DataValueField = "MaLoaiCV";  // ✅ lưu mã
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
            using (SqlCommand cmd = new SqlCommand("SELECT PheDuyet FROM tblLoaiCV WHERE MaLoaiCV = @id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(ddlLoaiCV.SelectedValue);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null && (result.ToString() == "1" || result.ToString().ToUpper() == "TRUE"))
                    pheDuyet = true;
            }

            // ✅ Nếu có duyệt => hiện panel, chờ chọn đơn vị để load người duyệt
            pnlNguoiDuyet.Visible = pheDuyet;
            ddlNguoiDuyet.Items.Clear();
            ddlNguoiDuyet.Items.Add(new ListItem("-- Chọn người duyệt --", "0"));
        }






        protected void lnk_Sua_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (LinkButton)sender;
            string maCV = lnk.CommandArgument;
            Response.Redirect("~/SuaCongVan.aspx?macv=" + maCV);
        }


        protected void lnk_Xoa_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (sender) as LinkButton;
            string str = lnk.CommandArgument;

            foreach (tblFileDinhKem item in db.tblFileDinhKems.Where(f => f.MaCV == str))
            {
                db.tblFileDinhKems.DeleteOnSubmit(item);
            }
            tblNoiDungCV cv = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV == str);
            if (cv != null)
            {
                db.tblNoiDungCVs.DeleteOnSubmit(cv);
                db.SubmitChanges();
                Page_Load(sender, e);

                Response.Redirect("~/NhapNDCV.aspx");

            }
        }

        protected void btnthem_Click(object sender, EventArgs e)
        {
            try
            {
                string maNguoiGui = Session["MaNguoiDung"]?.ToString();
                if (maNguoiGui == null)
                {
                    Response.Write("<script>alert('Phiên đăng nhập hết hạn!');</script>");
                    return;
                }

                // 🔹 Validate loại công văn
                if (string.IsNullOrEmpty(ddlLoaiCV.SelectedValue))
                {
                    Response.Write("<script>alert('Vui lòng chọn loại công văn!');</script>");
                    return;
                }

                // 🔹 Parse ngày
                if (!DateTime.TryParseExact(txtngaygui.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ngayGui) ||
                    !DateTime.TryParseExact(txtngaybanhanh.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ngayBanHanh))
                {
                    Response.Write("<script>alert('Định dạng ngày không hợp lệ!');</script>");
                    return;
                }

                // 🔹 Tạo công văn mới
                string maCV = Guid.NewGuid().ToString();

                tblNoiDungCV cv = new tblNoiDungCV
                {
                    MaCV = maCV,
                    SoCV = txtsocv.Text.Trim(),
                    TieuDeCV = txttieude.Text.Trim(),
                    MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue),
                    CoQuanBanHanh = txtcqbh.Text.Trim(),
                    TrichYeuND = txttrichyeu.Text.Trim(),
                    NguoiKy = txtNguoiKy.Text.Trim(),
                    MaNguoiGui = maNguoiGui,
                    BaoMat = RadioButtonList1.SelectedValue == "Có" ? "1" : "0",
                    GhiChu = txtGhiChu.Text.Trim(),
                    TrangThai = pnlNguoiDuyet.Visible ? "Đang trình" : "Đã gửi",
                    NgayGui = ngayGui,
                    NgayBanHanh = ngayBanHanh,
                    NguoiDuyet = pnlNguoiDuyet.Visible && ddlNguoiDuyet.SelectedIndex > 0
                                ? ddlNguoiDuyet.SelectedItem.Text
                                : null
                };

                db.tblNoiDungCVs.InsertOnSubmit(cv);
                db.SubmitChanges();

                // 🔹 Lưu đơn vị nhận
                foreach (ListItem item in lstDonViNhan.Items)
                {
                    if (item.Selected)
                    {
                        db.tblNoiDungCV_DonViNhans.InsertOnSubmit(new tblNoiDungCV_DonViNhan
                        {
                            MaCV = maCV,
                            MaDonViNhan = item.Value // chính là MaDV
                        });
                    }
                }
                db.SubmitChanges();

                // 🔹 Lưu file đính kèm (nếu có)
                foreach (ListItem item in ListBox1.Items)
                {
                    db.tblFileDinhKems.InsertOnSubmit(new tblFileDinhKem
                    {
                        MaCV = maCV,
                        FileID = Guid.NewGuid().ToString(),
                        Size = Convert.ToInt32(item.Value),
                        DateUpload = DateTime.Now.ToShortDateString(),
                        TenFile = item.Text,
                        Url = "~/Upload/" + item.Text
                    });
                }
                db.SubmitChanges();

                Response.Write("<script>alert('Thêm công văn thành công!'); window.location='NhapNDCV.aspx';</script>");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Lỗi khi thêm công văn: {ex.Message}');</script>");
            }
        }






        public string kttrangthai(object obj)
        {
            bool trangthai = bool.Parse(obj.ToString());
            if (trangthai)
            {
                return "Đã duyệt";
            }
            else
            {
                return "Chưa duyệt";
            }
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            string UploadFolder = Server.MapPath("/Upload/");
            if (FileUpload1.HasFile)
                try
                {
                    string filename = FileUpload1.PostedFile.FileName;
                    string FileNameOnServer = UploadFolder + filename;
                    FileUpload1.SaveAs(FileNameOnServer);

                    ListItem item = new ListItem(filename, Convert.ToString(FileUpload1.PostedFile.ContentLength));
                    ListBox1.Items.Add(item);

                }
                catch (Exception ex)
                {
                    lblloi.Text = "Lỗi: " + ex.Message.ToString();
                }
            else
            {
                /* lblchuachonfile.Text = "Bạn chưa chọn file!";*/
            }
        }
        void RemoveFile(int index)
        {
            if (index >= ListBox1.Items.Count) return;
            string filename = ListBox1.Items[index].Value;
            System.IO.File.Delete(filename);
            ListBox1.Items.RemoveAt(index);
        }
        protected void btnRemove_Click(object sender, EventArgs e)
        {

            RemoveFile(ListBox1.SelectedIndex);
        }

        protected void btnReAll_Click(object sender, EventArgs e)
        {
            while (ListBox1.Items.Count > 0) RemoveFile(0);
        }

        protected void btnsua_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["macv"] != null)
            {
                tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV.ToString() == (Request.QueryString["macv"].ToString()));
                if (cv1 != null)
                {
                    // ✅ XỬ LÝ NGÀY KIỂU dd/MM/yyyy AN TOÀN
                    DateTime ngayGui, ngayBanHanh;
                    if (!DateTime.TryParseExact(
                        txtngaybanhanh.Text.Trim(),
                        "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out ngayBanHanh))
                    {
                        // Nếu lỗi format ngày thì không lưu
                        return;
                    }

                    if (!DateTime.TryParseExact(
                        txtngaygui.Text.Trim(),
                        "dd/MM/yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out ngayGui))
                    {
                        return;
                    }

                    // ✅ GÁN GIÁ TRỊ AN TOÀN
                    cv1.SoCV = txtsocv.Text;
                    cv1.NgayGui = ngayGui;
                    cv1.TieuDeCV = txttieude.Text;
                    cv1.MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue.ToString());
                    cv1.CoQuanBanHanh = txtcqbh.Text;
                    cv1.TrichYeuND = txttrichyeu.Text;
                    cv1.NgayBanHanh = ngayBanHanh;

                    if (RadioButtonList1.SelectedIndex == 0)
                    {
                        cv1.GuiHayNhan = 1;
                    }
                    else
                        cv1.GuiHayNhan = 0;

                    // ✅ XỬ LÝ FILE AN TOÀN
                    for (int i = 0; i < ListBox1.Items.Count; i++)
                    {
                        var files = db.tblFileDinhKems.Where(t => t.MaCV.ToString() == (Request.QueryString["macv"].ToString()));
                        bool check = true;
                        foreach (var item in files)
                        {
                            if (ListBox1.Items[i].Text == item.TenFile)
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check &&
                            !string.IsNullOrEmpty(ListBox1.Items[i].Value) &&
                            !string.IsNullOrEmpty(ListBox1.Items[i].Text))
                        {
                            tblFileDinhKem file = new tblFileDinhKem();
                            file.FileID = Guid.NewGuid().ToString();
                            file.Url = ListBox1.Items[i].Value.ToString();
                            file.TenFile = (ListBox1.Items[i].Text);
                            file.DateUpload = DateTime.Now.ToShortDateString();
                            file.MaCV = cv1.MaCV;
                            db.tblFileDinhKems.InsertOnSubmit(file);
                        }
                    }

                    // ✅ CHẶN LỖI Ở SubmitChanges
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        Response.Write(ex.Message);
                        return;
                    }

                    Response.Redirect("NhapNDCV.aspx");
                    // btnthem.Visible = true;
                    // btnsua.Visible = false;
                }
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
            ddlNguoiDuyet.Text = "";
            txtGhiChu.Text = "";

        }

        protected void gvnhapcnden_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gv1 = from g in db.tblNoiDungCVs
                      from h in db.tblLoaiCVs
                      where g.MaLoaiCV == h.MaLoaiCV
                      orderby g.NgayGui descending
                      select new
                      {
                          g.MaCV,
                          g.SoCV,
                          h.TenLoaiCV,
                          g.NgayGui,
                          g.TieuDeCV,
                          g.CoQuanBanHanh,
                          g.NgayBanHanh,
                          g.NguoiKy,
                          TrichYeuND = g.TrichYeuND.Substring(0, 200) + ". . ."
                      };
            gvnhapcnden.PageIndex = e.NewPageIndex;
            gvnhapcnden.DataSource = gv1;
            gvnhapcnden.DataBind();

        }

        protected void gvnhapcnden_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

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