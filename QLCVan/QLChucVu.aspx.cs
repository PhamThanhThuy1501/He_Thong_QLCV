using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class QLChucVu : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();

        private void LoadData()
        {
            var list = from cv in db.tblChucVus select cv;
            gvChucVu.DataSource = list.ToList();
            gvChucVu.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string ma = txtMaChucVuSR.Text.Trim();
            string ten = txtTenChucVuSR.Text.Trim();

            var query = db.tblChucVus.AsQueryable();

            if (!string.IsNullOrEmpty(ma))
                query = query.Where(x => x.MaChucVu.Contains(ma));
            if (!string.IsNullOrEmpty(ten))
                query = query.Where(x => x.TenChucVu.Contains(ten));

            gvChucVu.DataSource = query.ToList();
            gvChucVu.DataBind();
        }

        protected void gvChucVu_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvChucVu.PageIndex = e.NewPageIndex;
            LoadData();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string ma = txtMdMaChucVu.Text.Trim();
            string ten = txtMdTenChucVu.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addEmpty",
                       "showToast('Mã và Tên chức vụ không được để trống!', 'text-bg-danger');", true);
                return;
            }

            if (db.tblChucVus.Any(x => x.MaChucVu == ma))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "addcheckma",
                    "showToast('Mã chức vụ đã tồn tại!','text-bg-danger');", true);
                return;
            }

            var cv = new tblChucVu { MaChucVu = ma, TenChucVu = ten };
            db.tblChucVus.InsertOnSubmit(cv);
            db.SubmitChanges();
            LoadData();

            txtMdMaChucVu.Text = txtMdTenChucVu.Text = "";

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
            bool hasChild = db.tblChucVu_tblNhomQuyens.Any(x => x.MaChucVu == ma);

            if (hasChild)
            {
                // Gợi ý: dùng toast lỗi thay cho alert
                ScriptManager.RegisterStartupScript(this, GetType(), "delFailChild",
                    "showToast('Không thể xoá: Chức vụ còn nhóm quyền được gán.', 'text-bg-danger');", true);
                return;
            }

            if (!string.IsNullOrEmpty(ma))
            {
                var item = db.tblChucVus.FirstOrDefault(x => x.MaChucVu == ma);
                if (item != null)
                {
                    try
                    {
                        db.tblChucVus.DeleteOnSubmit(item);
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
                            "showToast('Không thể xoá chức vụ vì vẫn còn người dùng được gán.', 'text-bg-danger');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "delNotFound",
                        "showToast('Không tìm thấy chức vụ để xoá.', 'text-bg-warning');", true);
                }
            }
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string ma = hdfMaChucVu.Value.Trim();
            string ten = txtEditTen.Text.Trim();

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(ten))
            {
                // Báo lỗi bằng toast (có thể thay bằng alert nếu bạn muốn)
                ScriptManager.RegisterStartupScript(this, GetType(), "updEmpty",
                    "showToast('Tên chức vụ không được để trống!', 'text-bg-danger');", true);
                return;
            }

            var cv = db.tblChucVus.FirstOrDefault(x => x.MaChucVu == ma);
            if (cv != null)
            {
                cv.TenChucVu = ten;
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
                    "showToast('Không tìm thấy chức vụ.', 'text-bg-warning');", true);
            }
        }

    }
}
