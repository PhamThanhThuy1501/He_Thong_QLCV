using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class QLNhomQuyen : System.Web.UI.Page
    {

        InfoDataContext db = new InfoDataContext();
        string maQuyenYeuCau = "Q009";
        private void LoadData()
        {
            var list = from nq in db.tblNhomQuyens select nq;
            gvNhomQuyen.DataSource = list.ToList<tblNhomQuyen>();
            gvNhomQuyen.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Session["TenDN"] == null))
            {
                Response.Redirect("Dangnhap.aspx");
            }
            //if (!PermissionHelper.HasPermission(maQuyenYeuCau))
            //{
            //    Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.history.back();</script>");
            //    Response.End();
            //}
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string ma = txtMaQuyenSR.Text.Trim();
            string ten = txtTenQuyenSR.Text.Trim();

            var query = db.tblNhomQuyens.AsQueryable();

            if (!string.IsNullOrEmpty(ma))
                query = query.Where(x => x.MaNhomQuyen.Contains(ma));
            if (!string.IsNullOrEmpty(ten))
                query = query.Where(x => x.TenNhomQuyen.Contains(ten));

            gvNhomQuyen.DataSource = query.ToList();
            gvNhomQuyen.DataBind();
        }

        protected void gvNhomQuyen_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNhomQuyen.PageIndex = e.NewPageIndex;
            LoadData();
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMdMaNhomQuyen.Text.Trim();
            string ten = txtMdTenNhomQuyen.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addEmpty",
                      "showToast('Mã và Tên nhóm quyền không được để trống!', 'text-bg-danger');", true);
                // Xoá text trong modal
                txtMdMaNhomQuyen.Text = "";
                txtMdTenNhomQuyen.Text = "";
                return;
            }

            // Kiểm tra trùng mã
            var exist = db.tblNhomQuyens.FirstOrDefault(x => x.MaNhomQuyen == ma);
            if (exist != null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addcheckma",
                    "showToast('Mã  nhóm quyền đã tồn tại!','text-bg-danger');", true);
                // Xoá text trong modal
                txtMdMaNhomQuyen.Text = "";
                txtMdTenNhomQuyen.Text = "";
                return;
            }

            // Thêm mới
            tblNhomQuyen nq = new tblNhomQuyen
            {
                MaNhomQuyen = ma,
                TenNhomQuyen = ten
            };
            db.tblNhomQuyens.InsertOnSubmit(nq);
            db.SubmitChanges();

            // Refresh lại GridView
            LoadData();

            // Xoá text trong modal
            txtMdMaNhomQuyen.Text = "";
            txtMdTenNhomQuyen.Text = "";

            // Đóng modal an toàn + hiện toast
            ScriptManager.RegisterStartupScript(this, GetType(), "addOk",
            @"
    (function(){
      var el = document.getElementById('addModal');
      if (el) {
        var md = (bootstrap.Modal.getInstance ? bootstrap.Modal.getInstance(el) : null);
        try {
          if (!md && bootstrap.Modal.getOrCreateInstance) {
            md = bootstrap.Modal.getOrCreateInstance(el);
          } else if (!md) {
            md = new bootstrap.Modal(el);
          }
          md && md.hide();
        } catch(e) {}
      }
      if (typeof showToast === 'function') {
        showToast('Thêm thành công', 'text-bg-success');
      } else {
        console.log('Toast: Thêm thành công'); // fallback
      }
    })();
    ", true);
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            string ma = hdDeleteId.Value;
            bool hasChild = db.tblNhomQuyen_tblQuyens.Any(x => x.MaNhomQuyen == ma);

            if (hasChild)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "delFailChild",
                    "showToast('Không thể xóa nhóm quyền vì vẫn còn quyền được gán.', 'text-bg-danger');", true);
                return;
            }
            if (!string.IsNullOrEmpty(ma))
            {
                var item = db.tblNhomQuyens.FirstOrDefault(x => x.MaNhomQuyen == ma);
                if (item != null)
                {
                    try
                    {
                        db.tblNhomQuyens.DeleteOnSubmit(item);
                        db.SubmitChanges();

                        // Reload grid
                        LoadData();

                        // Đóng modal + hiện toast thành công
                        ScriptManager.RegisterStartupScript(this, GetType(), "delOk",
                            @"
                    (function(){
                      var el = document.getElementById('deleteModal');
                      var md = bootstrap.Modal.getInstance(el);
                      if(md){ md.hide(); }
                      showToast('Đã xoá thành công', 'text-bg-success');
                    })();
                    ", true);
                    }
                    catch (Exception)
                    {
                        // Có người dùng đang gán, hoặc lỗi ràng buộc DB
                        ScriptManager.RegisterStartupScript(this, GetType(), "delFailRef",
                            "showToast('Không thể xóa nhóm quyền vì vẫn còn quyền được gán.', 'text-bg-danger');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "delNotFound",
                        "showToast('Không tìm thấy nhóm quyền để xoá.', 'text-bg-warning');", true);
                }

               
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string ma = hdfMaNhomQuyen.Value.Trim(); 
            string ten = txtEditTen.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "updEmpty",
                    "showToast('Tên nhóm quyền không được để trống!', 'text-bg-danger');", true);
                return;
            }


                var nhom = db.tblNhomQuyens.FirstOrDefault(x => x.MaNhomQuyen == ma);
                if (nhom != null)
                {
                    nhom.TenNhomQuyen = ten; // ✅ chỉ update tên
                    db.SubmitChanges();
                     LoadData();

                // Đóng modal Sửa + hiện toast thành công
                ScriptManager.RegisterStartupScript(this, GetType(), "updOk",
                @"
        (function(){
          var el = document.getElementById('editModal');
          if (el) {
            var md = (bootstrap.Modal.getInstance ? bootstrap.Modal.getInstance(el) : null);
            try {
              if (!md && bootstrap.Modal.getOrCreateInstance) {
                md = bootstrap.Modal.getOrCreateInstance(el);
              } else if (!md) {
                md = new bootstrap.Modal(el);
              }
              md && md.hide();
            } catch(e) {}
          }
          if (typeof showToast === 'function') {
            showToast('Cập nhật thành công', 'text-bg-success');
          } else {
            console.log('Toast: Cập nhật thành công');
          }
        })();
        ", true);
                }
                else
                {
                ScriptManager.RegisterStartupScript(this, GetType(), "updNotFound",
               "showToast('Không tìm thấy nhóm quyền.', 'text-bg-warning');", true);
            }

                    }
    }
}