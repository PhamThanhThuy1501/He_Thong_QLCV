using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class QuanLyQuyen : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        string maQuyenYeuCau = "Q008";

        private List<tblQuyen> GetDanhSachQuyen()
        {
            var list = from q in db.tblQuyens
                       select q;
            return list.ToList<tblQuyen>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Session["TenDN"] == null))
            {
                Response.Redirect("Dangnhap.aspx");
            }
            if (!PermissionHelper.HasPermission(maQuyenYeuCau))
            {
                Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
                Response.End();
            }


            if (!IsPostBack)
                LoadQuyen();
        }

        private void LoadQuyen()
        {
            gvQuyentbl.DataSource = GetDanhSachQuyen();
            gvQuyentbl.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //string keywordTen = txtTenQuyen.Text.Trim().ToLower();
            //string keywordMa = txtMaQuyen.Text.Trim().ToLower();

            //var ds = GetDanhSachQuyen();
            //var ketQua = ds.Where(q =>
            //    (string.IsNullOrEmpty(keywordTen) || q.TenQuyen.ToLower().Contains(keywordTen)) &&
            //    (string.IsNullOrEmpty(keywordMa) || q.MaQuyen.ToLower().Contains(keywordMa))
            //).ToList();

            //gvQuyen.DataSource = ketQua;
            //gvQuyen.DataBind();
            // Lấy từ khóa tìm kiếm và chuẩn hóa
            string keywordTen = txtTenQuyen.Text.Trim();
            string keywordMa = txtMaQuyen.Text.Trim();


            IQueryable<tblQuyen> query = db.tblQuyens;


            if (!string.IsNullOrEmpty(keywordTen))
            {
                query = query.Where(q => q.TenQuyen.ToLower().Contains(keywordTen.ToLower()));
            }

            if (!string.IsNullOrEmpty(keywordMa))
            {
                query = query.Where(q => q.MaQuyen.ToLower().Contains(keywordMa.ToLower()));
            }

            gvQuyentbl.DataSource = query.ToList();
            gvQuyentbl.DataBind();
        }

        protected void gvQuyen_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvQuyentbl.PageIndex = e.NewPageIndex;
            LoadQuyen();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

        }


    }
}
