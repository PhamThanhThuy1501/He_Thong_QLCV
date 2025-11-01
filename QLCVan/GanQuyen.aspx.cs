using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class GanQuyen : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Session["TenDN"] == null))
            {
                Response.Redirect("Dangnhap.aspx");
            }
            if (!IsPostBack)
            {

                string maNhom = Request.QueryString["ma"];
                string tenNhom = Request.QueryString["ten"];

                if (string.IsNullOrEmpty(maNhom))
                {
                    Response.Redirect("QLNhomQuyen.aspx");
                    return; 
                }
                lblTenNhom.Text = tenNhom;
                hdfMaNhom.Value = maNhom;
                LoadData(maNhom);
            }

        }
        private void LoadData(string maNhom)
        {
            var list = from q in db.tblQuyens
                       select new
                       {
                           q.MaQuyen,
                           q.TenQuyen,
                           DaGan = db.tblNhomQuyen_tblQuyens.Any(nq => nq.MaNhomQuyen == maNhom && nq.MaQuyen == q.MaQuyen)
                       };

            gvGanQuyen.DataSource = list.ToList();
            gvGanQuyen.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string maNhom = hdfMaNhom.Value;
            string ma = txtMaQuyen.Text.Trim().ToLower();
            string ten = txtTenQuyen.Text.Trim().ToLower();

            var query = from q in db.tblQuyens
                        where (ma == "" || q.MaQuyen.ToLower().Contains(ma))
                           && (ten == "" || q.TenQuyen.ToLower().Contains(ten))
                        select new
                        {
                            q.MaQuyen,
                            q.TenQuyen,
                            DaGan = db.tblNhomQuyen_tblQuyens.Any(nq => nq.MaNhomQuyen == maNhom && nq.MaQuyen == q.MaQuyen)
                        };

            gvGanQuyen.DataSource = query.ToList();
            gvGanQuyen.DataBind();
        }


        protected void gvGanQuyen_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "ToggleQuyen") return;

            string maQuyen = e.CommandArgument.ToString();
            string maNhom = hdfMaNhom.Value;

            // Lấy TÊN quyền theo mã (fallback về mã nếu không tìm thấy)
            string tenQuyen = db.tblQuyens
                                .Where(q => q.MaQuyen == maQuyen)
                                .Select(q => q.TenQuyen)
                                .FirstOrDefault() ?? maQuyen;

            try
            {
                var quyen = db.tblNhomQuyen_tblQuyens
                              .FirstOrDefault(x => x.MaNhomQuyen == maNhom && x.MaQuyen == maQuyen);

                bool added;
                if (quyen == null)
                {
                    db.tblNhomQuyen_tblQuyens.InsertOnSubmit(new tblNhomQuyen_tblQuyen
                    {
                        MaNhomQuyen = maNhom,
                        MaQuyen = maQuyen,
                    });
                    added = true;
                }
                else
                {
                    db.tblNhomQuyen_tblQuyens.DeleteOnSubmit(quyen);
                    added = false;
                }

                db.SubmitChanges();
                LoadData(maNhom);
                PermissionHelper.ReSyncPermission();

                var actionText = added ? "đã gán" : "đã bỏ gán";
                // lblTenNhom: label hiển thị TÊN NHÓM QUYỀN trên trang
                ShowMessage($"[{tenQuyen}] {actionText} cho nhóm quyền {lblTenNhom.Text} thành công.", false);
            }
            catch (Exception ex)
            {
                ShowMessage("Có lỗi khi cập nhật quyền: " + ex.Message, true);
            }
        }
        // Helper hiển thị thông báo (ưu tiên toast nếu có window.showToast; fallback alert)
        private void ShowMessage(string message, bool isError)
        {
            string safe = (message ?? "").Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ");
            string level = isError ? "error" : "success";

            string js = $@"
                (function() {{
                    if (window.showToast) {{
                        window.showToast('{safe}', '{level}');
                    }} else {{
                        alert('{safe}');
                    }}
                }})();";

            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), js, true);
        }

        protected void gvGanQuyen_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGanQuyen.PageIndex = e.NewPageIndex;
            LoadData(hdfMaNhom.Value);
        }
    }
}