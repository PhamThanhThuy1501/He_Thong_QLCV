using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace QLCVan
{
    public partial class SuaCongVan : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["TenDN"] != null))
            {
                Response.Redirect("Gioithieu.aspx");
            }

            if (!Page.IsPostBack)
            {
                var cboLoaiCongvans = from d in db.tblLoaiCVs
                                      where d.MaLoaiCV.ToString() != null
                                      select d.TenLoaiCV;

                ddlLoaiCV.DataSource = cboLoaiCongvans;
                ddlLoaiCV.DataBind();

                var cbNguoiki = (from d in db.tblNoiDungCVs
                                 where d.MaLoaiCV.ToString() != null
                                 select d.NguoiKy).Distinct();

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
                              g.GhiChu,
                              g.NgayBanHanh,
                              g.NguoiKy,
                              g.NoiNhan,
                              TrichYeuND = g.TrichYeuND.Substring(0, 250) + " ... ",
                              g.TrangThai
                          };


                ddlLoaiCV.DataSource = db.tblLoaiCVs;
                ddlLoaiCV.DataTextField = "TenLoaiCV";
                ddlLoaiCV.DataValueField = "MaLoaiCV";
                ddlLoaiCV.DataBind();

                txtngaybanhanh.Attributes["placeholder"] = "dd/mm/yyyy";
                txtngaygui.Attributes["placeholder"] = "dd/mm/yyyy";

                if (Request.QueryString["id"] != null)
                {
                    tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV.ToString() == Request.QueryString["id"].ToString());
                    if (cv1 != null)
                    {
                        txttieude.Text = cv1.TieuDeCV;
                        txtngaybanhanh.Text = cv1.NgayBanHanh.HasValue ? cv1.NgayBanHanh.Value.ToString("dd-MM-yyyy") : "";
                        txtngaygui.Text = cv1.NgayGui.HasValue ? cv1.NgayGui.Value.ToString("dd-MM-yyyy") : "";
                        txtcqbh.Text = cv1.CoQuanBanHanh;
                        txtsocv.Text = cv1.SoCV;
                        txttrichyeu.Text = cv1.TrichYeuND;
                        ListBox1.DataTextField = "TenFile";
                        ListBox1.DataSource = db.tblFileDinhKems.Where(t => t.MaCV == cv1.MaCV);
                        ListBox1.DataBind();
                        txtNguoiDuyet.Text = cv1.NguoiDuyet;

                        //RadioButtonList1.SelectedIndex = (int)cv1.GuiHayNhan;
                        txtNguoiKy.Text = cv1.NguoiKy;
                        txtGhiChu.Text = cv1.GhiChu;
                    }
                }
            }
        }

        protected void lnk_Sua_Click(object sender, EventArgs e)
        {
            LinkButton lnk = (sender) as LinkButton;
            string str = lnk.CommandArgument;
            Response.Redirect("~/SuaCongVan.aspx?MaCV=" + str);
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
                Response.Redirect("~/SuaCongVan.aspx");
            }
        }

        protected void btnCapNhat_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["macv"] == null) return;

            tblNoiDungCV cv1 = db.tblNoiDungCVs.SingleOrDefault(t => t.MaCV.ToString() == Request.QueryString["macv"].ToString());
            if (cv1 != null)
            {
                DateTime ngayGui, ngayBanHanh;
                if (!DateTime.TryParseExact(
                    txtngaybanhanh.Text.Trim(),
                    "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out ngayBanHanh))
                {
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

                cv1.SoCV = txtsocv.Text;
                cv1.NgayGui = ngayGui;
                cv1.TieuDeCV = txttieude.Text;
                cv1.MaLoaiCV = int.Parse(ddlLoaiCV.SelectedValue.ToString());
                cv1.CoQuanBanHanh = txtcqbh.Text;
                cv1.TrichYeuND = txttrichyeu.Text;
                cv1.NgayBanHanh = ngayBanHanh;
                cv1.NguoiKy = txtNguoiKy.Text;
                cv1.GhiChu = txtGhiChu.Text;

                if (RadioButtonList1.SelectedIndex == 0)
                {
                    cv1.GuiHayNhan = 1;
                }
                else
                    cv1.GuiHayNhan = 0;

                for (int i = 0; i < ListBox1.Items.Count; i++)
                {
                    var files = db.tblFileDinhKems.Where(t => t.MaCV.ToString() == Request.QueryString["macv"].ToString());
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
                        file.Url = "~/Upload/" + ListBox1.Items[i].Text;
                        file.TenFile = ListBox1.Items[i].Text;
                        file.DateUpload = DateTime.Now.ToShortDateString();
                        file.MaCV = cv1.MaCV;
                        db.tblFileDinhKems.InsertOnSubmit(file);
                    }
                }

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Response.Write(ex.Message);
                    return;
                }

                Response.Redirect("SuaCongVan.aspx");
            }
        }

        protected void btnQuayLai_Click(object sender, EventArgs e)
        {
            Response.Redirect("Trangchu.aspx");
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

        }

        protected void gvnhapcnden_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
