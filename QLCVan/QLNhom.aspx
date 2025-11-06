<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QLNhom.aspx.cs" Inherits="QLCVan.QLNhom" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <!-- Bootstrap + Font Awesome -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />

  <style>
    .section-title{font-size:26px;font-weight:700;color:#0f172a;text-align:center;margin-bottom:20px}
    .grid-header-red th{background-color:#c00!important;color:#fff!important;text-align:center}

   /* ===== Phần tiêu đề + thanh chạy chữ ===== */
.content-header {
  background: transparent;
  padding: 0;
  border-bottom: none;
  margin: 0 auto 6px auto;
}

.content-header-title {
  text-transform: uppercase;
  font-weight: 700;
  font-size: 20px;
  color: #444;
  margin: 0 0 6px 0;
  letter-spacing: 0;
}


/* ===== Thanh chạy chữ giống hình mẫu ===== */
.welcome-bar {
  background: #c00;                  /* nền đỏ đậm */
  color: #fff;
  border-radius: 4px;                /* bo góc mềm */
  padding: 8px 0;                    /* cao vừa để chữ nằm giữa */
  margin: 0 auto 26px auto;
  font-weight: bold;                 /* in đậm */
  text-align: center;
  display: flex;
  align-items: center;               /* căn giữa theo chiều cao */
  justify-content: center;
  height: 30px;                      /* chiều cao cố định để đều */
  overflow: hidden;                  /* ẩn phần chữ thừa */
}

.welcome-bar marquee {
  font-size: 16px;                   /* chữ lớn hơn chút */
  font-weight: bold;
  color: #fff;
                
}

    /* Toolbar nhỏ gọn như ảnh */
    .toolbar { width:70%; margin:0 auto 14px auto; }
    .toolbar .form-control{
      height:36px; border-radius:6px; padding:6px 10px; font-size:14px; border-color:#dee2e6;
    }
    .search-caption{ font-weight:600; color:#212529; font-size:16px; margin-right:12px; white-space:nowrap; }

    /* Nút kính lúp đỏ */
    .btn-search-red{
      width:80px; height:36px; border-radius:6px; background:#c00; border:1px solid #c00; display:inline-block;
      padding:0; cursor:pointer; text-indent:-9999px; overflow:hidden;
      background-image:url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 512 512'%3E%3Cpath fill='%23ffffff' d='M500.3 443.7 382 325.4c28.4-34.9 45.5-79.4 45.5-127.4C427.5 88.1 339.4 0 231.8 0S36.1 88.1 36.1 197.9 124.2 395.7 231.8 395.7c48 0 92.5-17.1 127.4-45.5l118.3 118.3c7.5 7.5 19.8 7.5 27.3 0s7.5-19.8 0-27.3zM231.8 355.7c-87.1 0-157.9-70.8-157.9-157.9S144.7 39.9 231.8 39.9 389.7 110.7 389.7 197.8 318.9 355.7 231.8 355.7z'/%3E%3C/svg%3E");
      background-repeat:no-repeat; background-position:center; background-size:58% 58%;
    }
    .btn-search-red:hover{ background:#a00; border-color:#a00 }

    /* Nút thêm */
    .btn-add{ height:36px; border-radius:6px; padding:6px 14px; font-size:14px; font-weight:500; }

    /* Pager */
   /* === XÓA VIỀN & NỀN XÁM PHÂN TRANG === */
/* ==== PHÂN TRANG KHÔNG GẠCH CHÂN, KHÔNG ĐỔI MÀU, KHÔNG NỀN ==== */
.grid-pager {
  background: transparent !important;
  border: none !important;
  text-align: center !important;
  margin-top: 12px;
  padding: 8px 0;
}

.grid-pager table {
  
  border-collapse: separate;
  border-spacing: 6px;
  background: transparent !important;
  border-spacing: 25px;
  margin: 15px auto 10px auto !important;  /* 👈 tạo khoảng cách 25px giữa bảng và phân trang */
}

.grid-pager td {
  border: none !important;
  background: transparent !important;
  padding: 0;
}

/* Các nút trang */
.grid-pager a,
.grid-pager span {
  display: inline-block;
  min-width: 40px;
  height: 40px;
  line-height: 40px;
  text-align: center;
  border-radius: 6px;
  font-weight: 600;
  font-size: 14px;
  border: 1px solid #ccc;
  color: #333;
  background: #fff;
  transition: all 0.2s ease;
  text-decoration: none !important;
}

/* Hover – chỉ hơi đổi nền xám nhẹ */
.grid-pager a:hover {
  background: #f8f9fa !important;
  color: #333 !important;
  border-color: #ccc;
  text-decoration: none !important;
}

/* Khi click (active) hoặc đang được chọn: giữ nguyên màu chữ đen, nền trắng */
.grid-pager a:active,
.grid-pager span {
  background: #fff !important;
  color: #000 !important;
  border-color: #ccc;
  font-weight: 700;
  text-decoration: none !important;
}
/* === PHÂN TRANG NGOÀI BẢNG === */
.pager-out {
    width: 70%;
    margin: 22px auto 0 auto;
    text-align: center;
}

.pager-out a,
.pager-out span {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 42px;
    height: 42px;
    border: 1px solid #d1d5db;
    border-radius: 4px;
    background: #fff;
    color: #4b5563;
    font-size: 16px;
    font-weight: 500;
    text-decoration: none;
    margin: 0 12px;
}

.pager-out span {
    background: #fff;
    color: #4b5563;
    border: 1px solid #d1d5db;
}

.pager-out a:hover {
    border-color: #d1d5db;
}

/* ==== XÓA MÀU XÁM XEN KẼ TRONG BẢNG ==== */
/* ==== XÓA MÀU XÁM XEN KẼ TRONG GRIDVIEW ==== */





    .cv-head{ font-weight:700; }
    .table {
  width: 70% !important;    /* đồng nhất với toolbar */
  margin: 0 auto;           /* căn giữa */
}

  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
      <div class="content-header">
  <h2 class="content-header-title">QUẢN LÝ NHÓM</h2>
</div>

<div class="welcome-bar">
  <marquee behavior="scroll" direction="left" scrollamount="6">
    Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
  </marquee>
</div>

    <center>
      <h3 class="section-title"><b>DANH SÁCH ĐƠN VỊ</b></h3>

      <!-- giữ khóa cần xoá -->
      <asp:HiddenField ID="hdfID" runat="server" />
      <asp:HiddenField ID="hdfDeleteKey" runat="server" />

      <!-- Toolbar tìm kiếm -->
      <div class="toolbar d-flex align-items-center">
        <div class="d-flex align-items-center gap-2 flex-grow-1">
          <span class="search-caption">Tìm kiếm</span>
          <asp:TextBox ID="txtSearchMa" runat="server" CssClass="form-control" placeholder="Nhập mã đơn vị" />
          <asp:TextBox ID="txtSearchTen" runat="server" CssClass="form-control" placeholder="Nhập tên đơn vị" />
          <asp:Button ID="btnSearch" runat="server" Text=" " CssClass="btn-search-red" ToolTip="Tìm kiếm" OnClick="btnSearch_Click" />
        </div>

        <button type="button" class="btn btn-primary btn-add ms-2" data-bs-toggle="modal" data-bs-target="#addModal">
          Thêm đơn vị
        </button>
      </div>

  <!-- BẢNG -->
<asp:GridView ID="gvQLNhom" runat="server"
    AutoGenerateColumns="False"
    CssClass="table table-bordered table-hover gridview"
    HeaderStyle-CssClass="grid-header-red"
    DataKeyNames="MaDonVi"
    OnRowDeleting="rowDeleting"
    OnRowCancelingEdit="rowCancelingEdit"
    OnRowEditing="rowEditing"
    OnRowUpdating="rowUpdating"
    OnRowCommand="rowCommand"
    OnRowDataBound="gvQLNhom_RowDataBound"
    AllowPaging="True" PageSize="5"
    OnPageIndexChanging="gvQLNhom_PageIndexChanging"
    PagerStyle-CssClass="pagination pagination-source"
    BorderStyle="None">

    <PagerSettings Mode="Numeric" Position="Bottom" PageButtonCount="5" />

    <Columns>
        <asp:TemplateField HeaderText="Mã đơn vị">
            <ItemTemplate>
                <asp:Label ID="lblMaDonVi" runat="server" Text='<%# Eval("MaDonVi") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Tên đơn vị">
            <ItemTemplate>
                <asp:Label ID="lblTenDonVi" runat="server" Text='<%# Eval("TenDonVi") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtTenNhom" runat="server" CssClass="form-control"
                    Text='<%# Eval("TenDonVi") %>'></asp:TextBox>
            </EditItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Thao tác">
            <ItemTemplate>
                <div style="display:flex; justify-content:center; align-items:center; gap:12px;">
                    <!-- Nút Sửa -->
                    <asp:LinkButton ID="btnEdit" runat="server"
                        CommandName="Edit" CommandArgument='<%# Eval("MaDonVi") %>'
                        ToolTip="Sửa"
                        Style="border:1px solid #ccc; border-radius:8px; padding:6px 10px; background-color:#fff;">
                        <i class="fas fa-pen" style="color:#0B57D0; font-size:18px;"></i>
                    </asp:LinkButton>

                    <!-- Nút Xóa -->
                    <asp:LinkButton ID="btnDelete" runat="server"
                        ToolTip="Xóa"
                        Style="border:1px solid #ccc; border-radius:8px; padding:6px 10px; background-color:#fff;"
                        OnClientClick='<%# "return openDeleteModal(\"" + Eval("MaDonVi") + "\");" %>'>
                        <i class="fas fa-trash" style="color:#E60000; font-size:18px;"></i>
                    </asp:LinkButton>
                </div>
            </ItemTemplate>

            <EditItemTemplate>
                <asp:LinkButton ID="btnUpdate" runat="server"
                    CommandName="Update" CssClass="btn btn-success btn-sm me-1" ToolTip="Lưu">
                    <i class="fa fa-check"></i>
                </asp:LinkButton>
                <asp:LinkButton ID="btnCancel" runat="server"
                    CommandName="Cancel" CssClass="btn btn-secondary btn-sm" ToolTip="Hủy">
                    <i class="fa fa-times"></i>
                </asp:LinkButton>
            </EditItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<!-- Phân trang ngoài bảng -->
<div id="pagerOutside" class="pager-out"></div>


    </center>

    <!-- Modal thêm đơn vị -->
    <div class="modal fade" id="addModal" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">

        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="addModalLabel">Thêm đơn vị</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><asp:TextBox ID="txtMaDonVi" runat="server" CssClass="form-control" placeholder="Nhập mã đơn vị..." /></div>
            <div class="mb-3"><asp:TextBox ID="txtTenDonVi" runat="server" CssClass="form-control" placeholder="Nhập tên đơn vị..." /></div>
          </div>
          <div class="modal-footer">
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-success" Text="Thêm" OnClick="btnSave_Click" />
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Sửa đơn vị -->
    <div class="modal fade" id="editModal" tabindex="-1" aria-labelledby="editModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">

        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="editModalLabel">Sửa đơn vị</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
          </div>
          <div class="modal-body">
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="mb-3">
              <label for="txtEditMaDonVi" class="form-label">Mã đơn vị</label>
              <asp:TextBox ID="txtEditMaDonVi" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="mb-3">
              <label for="txtEditTenDonVi" class="form-label">Tên đơn vị</label>
              <asp:TextBox ID="txtEditTenDonVi" runat="server" CssClass="form-control" />
            </div>
          </div>
          <div class="modal-footer">
            <asp:Button ID="btnEditSave" runat="server" CssClass="btn btn-success" Text="Sửa" OnClick="btnEditSave_Click" />
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
          </div>
        </div>
      </div>
    </div>

   <!-- Modal Xác nhận xoá -->
<div class="modal fade" id="confirmDeleteModal" tabindex="-1" aria-labelledby="confirmDeleteLabel" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="confirmDeleteLabel">Xác nhận xóa đơn vị</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
      </div>
      <div class="modal-body">
        Bạn có chắc muốn xóa đơn vị này không?
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
        <asp:Button ID="btnConfirmDelete" runat="server" Text="Xóa" CssClass="btn btn-danger"
                    OnClick="btnConfirmDelete_Click" UseSubmitBehavior="false" />
      </div>
    </div>
  </div>
</div>


    <!-- Bootstrap + script mở modal -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script>
      // Mở modal xác nhận xoá và lưu mã đơn vị cần xoá
      function openDeleteModal(maDonVi) {
        document.getElementById('<%= hdfDeleteKey.ClientID %>').value = maDonVi;
            var modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
            modal.show();
            return false; // chặn postback của LinkButton
        }

        // (tuỳ chọn) Hiện modal sửa từ server bằng ClientScript.RegisterStartupScript("showEdit","showEditModal();", true)
        function showEditModal() {
            var m = new bootstrap.Modal(document.getElementById('editModal'));
            m.show();
        }
        function hideEditModal() {
            var el = document.getElementById('editModal');
            var m = bootstrap.Modal.getInstance(el);
            if (m) m.hide();
        }
    </script>
  </div>
        <!-- Toast container (fixed ở góc trên bên phải) -->
<div class="position-fixed top-0 end-0 p-3" style="z-index:1080">
  <div id="liveToast" class="toast align-items-center text-bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
    <div class="d-flex">
      <div id="toastBody" class="toast-body">Đã xoá thành công</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
  </div>
</div>

<script>
    // Hàm hiển thị toast với message + màu
    function showToast(message, bsBgClass) {
        var toastEl = document.getElementById('liveToast');
        var bodyEl = document.getElementById('toastBody');

        // đổi nội dung + màu nền (success / danger / info ...)
        bodyEl.textContent = message || 'Thành công';
        toastEl.classList.remove('text-bg-success', 'text-bg-danger', 'text-bg-info', 'text-bg-warning');
        toastEl.classList.add(bsBgClass || 'text-bg-success');

        var toast = new bootstrap.Toast(toastEl, { delay: 2000 });
        toast.show();
    }
</script>
    <script type="text/javascript">
        (function () {
            function clonePager() {
                var grid = document.getElementById('<%= gvQLNhom.ClientID %>');
                if (!grid) return;

                var src = grid.querySelector('.pagination');
                var out = document.getElementById('pagerOutside');
                if (!src || !out) return;

                out.innerHTML = '';
                var items = src.querySelectorAll('a, span');
                items.forEach(function (el) {
                    out.appendChild(el.cloneNode(true));
                });

                src.style.display = 'none';
            }

            if (document.readyState === 'loading')
                document.addEventListener('DOMContentLoaded', clonePager);
            else
                clonePager();

            if (typeof (Sys) !== 'undefined' &&
                Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(clonePager);
            }
        })();
    </script>

</asp:Content>
