<%@ Page Title="Quản lý chức vụ" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QLChucVu.aspx.cs" Inherits="QLCVan.QLChucVu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

    <style>
body {
  background: #fff;
  font-family: "Segoe UI", Arial, sans-serif;
}

.page { width: 100%; margin: 0; padding: 0; }

.content-header { background: transparent; padding: 0; border-bottom: none; margin: 0 auto 6px; }
.content-header-title { text-transform: uppercase; font-weight: 700; font-size: 20px; color: #444; margin: 0 0 6px; }

.welcome-bar {
  background: #c00;
  color: #fff;
  border-radius: 4px;
  padding: 8px 0;
  margin: 0 auto 26px auto;
  font-weight: bold;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
  height: 30px;
  overflow: hidden;
}
.welcome-bar marquee { font-size: 16px; font-weight: bold; color: #fff; }

.page-title {
  font-size: 20px;
  font-weight: bold;
  text-align: center;
  color: #111;
  margin: 25px 0 20px 0;
}

/* ===== TÌM KIẾM ===== */
.search-bar {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 30px;
  margin: 0 auto 25px auto;
}

.search-bar label { font-weight: 600; color: #111; }

.search-bar input {
  border: 1px solid #ccc;
  border-radius: 4px;
  padding: 8px 10px;
  height: 34px;
  width: 280px;
  font-size: 14px;
}

.btn-search {
  background: #0d6efd !important;
  color: #fff;
  border: none;
  padding: 8px 18px;
  font-weight: 600;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}
.btn-search:hover { background: #0b5ed7 !important; }

/* ===== BẢNG ===== */
.table-wrapper { width: 70%; margin: 0 auto; background: #fff; }

.table {
  width: 100%;
  border-collapse: collapse;
  background: #fff;
  table-layout: fixed;
}

.table th, .table td {
  border: 1px solid #ddd;
  padding: 8px 10px;
  text-align: center;
  font-size: 14px;
}
.table tr th {
  background-color: #c00 !important;
  color: #fff !important;
  font-weight: 600;
  text-transform: uppercase;
  border-bottom: 2px solid #900;
}

.table tr th:nth-child(1), .table tr td:nth-child(1) { width: 20%; }
.table tr th:nth-child(2), .table tr td:nth-child(2) { width: auto; }
.table tr th:nth-child(3), .table tr td:nth-child(3) {
  width: 27%;
  white-space: nowrap;
}

/* ===== NÚT THAO TÁC ===== */
.table tr td:nth-child(3) > a, .table tr td:nth-child(3) > button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 30px;
  margin: 0 6px;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 600;
  border: 1px solid transparent;
}

/* Gán quyền */
.table tr td:nth-child(3) > *:first-child {
  min-width: 88px;
  padding: 0 12px;
  background: #0d6efd;
  color: #fff;
  border-color: #0d6efd;
}
.table tr td:nth-child(3) > *:first-child:hover { background:#0b5ed7; }

/* Sửa */
.table tr td:nth-child(3) > *:nth-child(2) {
  width: 30px;
  background:#fff; color:#0B57D0; border-color:#d1d5db;
}
.table tr td:nth-child(3) > *:nth-child(2):hover { background:#f3f4f6; }

/* Xóa */
.table tr td:nth-child(3) > *:last-child {
  width: 30px;
  background:#fff; color:#DC2626; border-color:#d1d5db;
}
.table tr td:nth-child(3) > *:last-child:hover { background:#fee2e2; }

/* ===== PHÂN TRANG NGOÀI BẢNG – GIỐNG QLNGUOIDUNG ===== */
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

.pager-out a:hover { border-color: #d1d5db; }
/* Bỏ gạch dưới cho LinkButton & giữ không có viền/đổ bóng */
.search-bar a,
.search-bar a:link,
.search-bar a:visited,
.search-bar a:hover,
.search-bar a:focus {
  text-decoration: none !important;
  box-shadow: none !important;
  outline: none !important;
}

/* Nút tìm kiếm đỏ */
.btn-search {
  background: #c00 !important;   /* đỏ */
  color: #fff !important;
  border: none !important;
  padding: 8px 18px;
  font-weight: 600;
  border-radius: 6px;
  cursor: pointer;
  display: inline-flex;           /* để icon căn giữa */
  align-items: center;
  justify-content: center;
}

/* Hover/Focus màu đỏ đậm hơn */
.btn-search:hover,
.btn-search:focus {
  background: #a00 !important;
  color: #fff !important;
  text-decoration: none !important;  /* phòng trường hợp browser thêm underline */
}

/* (tuỳ chọn) đảm bảo icon trắng */
.btn-search i { color: #fff; }
/* ===== PHÂN TRANG TRONG GRIDVIEW (pager gốc) ===== */
.gridview .pager a,
.gridview .pager span{
  display:inline-flex; align-items:center; justify-content:center;
  width:40px; height:40px;
  border:1px solid #d1d5db; border-radius:4px;
  background:#fff; color:#4b5563;
  font-size:16px; font-weight:500; text-decoration:none;
  transition: background-color .18s ease, color .18s ease, border-color .18s ease, transform .06s ease;
}

/* Trang hiện tại: tô đỏ */
.gridview .pager span{
  background:#C62828;           /* đỏ chủ đạo */
  color:#fff;
  border-color:#C62828;
  box-shadow:0 1px 2px rgba(0,0,0,.06);
}

/* Hover: chuyển đỏ + nhô lên nhẹ */
.gridview .pager a:hover{
  background:#C62828;
  color:#fff;
  border-color:#C62828;
  transform:translateY(-1px);
}

/* Active (nhấn giữ) */
.gridview .pager a:active{ transform:translateY(0); }

/* Focus bàn phím */
.gridview .pager a:focus-visible{
  outline:2px solid rgba(13,110,253,.35);
  outline-offset:2px;
}

/* ===== PHÂN TRANG NGOÀI BẢNG (pager-out) ===== */
.pager-out a,
.pager-out span{
  display:inline-flex; align-items:center; justify-content:center;
  width:42px; height:42px;
  border:1px solid #d1d5db; border-radius:4px;
  background:#fff; color:#4b5563;
  font-size:16px; font-weight:500; text-decoration:none; margin:0 5px;
  transition: background-color .18s ease, color .18s ease, border-color .18s ease, transform .06s ease;
}

/* Trang hiện tại: tô đỏ */
.pager-out span{
  background:#C62828;
  color:#fff;
  border-color:#C62828;
  box-shadow:0 1px 2px rgba(0,0,0,.06);
}

/* Hover: đỏ + nhô lên */
.pager-out a:hover{
  background:#C62828;
  color:#fff;
  border-color:#C62828;
  transform:translateY(-1px);
}

.pager-out a:active{ transform:translateY(0); }
.pager-out a:focus-visible{
  outline:2px solid rgba(13,110,253,.35);
  outline-offset:2px;
}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="page">
  <div class="content-header">
    <h2 class="content-header-title">QUẢN LÝ CHỨC VỤ</h2>
  </div>

  <div class="welcome-bar">
    <marquee behavior="scroll" direction="left" scrollamount="6">
      Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
    </marquee>
  </div>

  <h3 class="page-title">DANH SÁCH CHỨC VỤ</h3>

  <!-- Thanh tìm kiếm -->
  <div class="search-bar">
    <label>Tìm kiếm</label>
    <asp:TextBox ID="txtTenChucVuSR" runat="server" placeholder="Nhập tên chức vụ" />
    <asp:TextBox ID="txtMaChucVuSR" runat="server" placeholder="Nhập mã chức vụ" />
    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click">
      <i class="fa fa-search"></i>
    </asp:LinkButton>
    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addModal">
      <i class="fa fa-plus"></i> Thêm chức vụ
    </button>
  </div>

  <!-- Bảng danh sách -->
  <div class="table-wrapper">
    <asp:GridView ID="gvChucVu" runat="server" AutoGenerateColumns="False"
      CssClass="table table-bordered gridview"
      AllowPaging="True" PageSize="5"
      OnPageIndexChanging="gvChucVu_PageIndexChanging"
      PagerStyle-CssClass="pagination pagination-source"
      BorderStyle="None">

      <Columns>
        <asp:BoundField DataField="MaChucVu" HeaderText="Mã chức vụ" />
        <asp:BoundField DataField="TenChucVu" HeaderText="Tên chức vụ" />
        <asp:TemplateField HeaderText="Thao tác">
          <ItemTemplate>
            <a type="button" class="btn btn-primary btn-sm"
               href='<%# "GanNhomQuyen.aspx?ma=" + Eval("MaChucVu") + "&ten=" + Eval("TenChucVu") %>'>
               Gán quyền
            </a>

            <button type="button" class="btn btn-warning btn-sm"
                    data-bs-toggle="modal" data-bs-target="#editModal"
                    data-ma='<%# Eval("MaChucVu") %>'
                    data-ten="<%# Eval("TenChucVu") %>">
              <i class="fa fa-pen"></i>
            </button>

            <button type="button" class="btn btn-danger btn-sm"
                    data-bs-toggle="modal" data-bs-target="#deleteModal"
                    data-id='<%# Eval("MaChucVu") %>'>
              <i class="fa fa-trash"></i>
            </button>
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
    </asp:GridView>
  </div>

  <!-- ✅ Phân trang ngoài bảng -->
  <div id="pagerOutside" class="pager-out"></div>

  <!-- Modal thêm -->
  <div class="modal fade" id="addModal" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="addModalLabel">Thêm mới chức vụ</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <asp:TextBox ID="txtMdMaChucVu" runat="server" CssClass="form-control" placeholder="Nhập mã chức vụ..." />
          </div>
          <div class="mb-3">
            <asp:TextBox ID="txtMdTenChucVu" runat="server" CssClass="form-control" placeholder="Nhập tên chức vụ..." />
          </div>
        </div>
        <div class="modal-footer">
          <asp:Button ID="btnSave" runat="server" Text="Thêm" CssClass="btn btn-success" OnClick="btnSave_Click" />
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
        </div>
      </div>
    </div>
  </div>

  <!-- Modal xóa -->
  <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content border-danger">
        <div class="modal-header">
          <h5 class="modal-title" id="deleteModalLabel">Xác nhận xoá</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body">
          <p>Bạn có chắc muốn xoá chức vụ này không?</p>
          <asp:HiddenField ID="hdDeleteId" runat="server" />
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Huỷ</button>
          <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger" Text="Xoá" OnClick="btnConfirmDelete_Click" />
        </div>
      </div>
    </div>
  </div>

  <!-- Modal sửa -->
  <div class="modal fade" id="editModal" tabindex="-1" aria-labelledby="editModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Sửa chức vụ</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>
        <div class="modal-body">
          <asp:HiddenField ID="hdfMaChucVu" runat="server" />
          <div class="mb-3">
            <label class="form-label">Mã chức vụ</label>
            <asp:TextBox ID="txtEditMa" runat="server" CssClass="form-control" ReadOnly="true" />
          </div>
          <div class="mb-3">
            <label class="form-label">Tên chức vụ</label>
            <asp:TextBox ID="txtEditTen" runat="server" CssClass="form-control" />
          </div>
        </div>
        <div class="modal-footer">
          <asp:Button ID="btnUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-success" OnClick="btnUpdate_Click" />
          <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
        </div>
      </div>
    </div>
  </div>

  <!-- Toast -->
  <div class="position-fixed top-0 end-0 p-3" style="z-index:1080">
    <div id="liveToast" class="toast align-items-center text-bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
      <div class="d-flex">
        <div id="toastBody" class="toast-body">Đã xoá thành công</div>
        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
      </div>
    </div>
  </div>

  <!-- Script -->
  <script>
      // Hiển thị modal sửa/xoá
      document.addEventListener("DOMContentLoaded", function () {
          var deleteModal = document.getElementById('deleteModal');
          deleteModal.addEventListener('show.bs.modal', function (event) {
              var id = event.relatedTarget.getAttribute('data-id');
              document.getElementById('<%= hdDeleteId.ClientID %>').value = id;
      });

      var editModal = document.getElementById('editModal');
      editModal.addEventListener('show.bs.modal', function (event) {
        var btn = event.relatedTarget;
        document.getElementById('<%= txtEditMa.ClientID %>').value = btn.getAttribute('data-ma');
        document.getElementById('<%= txtEditTen.ClientID %>').value = btn.getAttribute('data-ten');
        document.getElementById('<%= hdfMaChucVu.ClientID %>').value = btn.getAttribute('data-ma');
      });
    });

    // Hiển thị toast
    function showToast(message, bsBgClass) {
      var toastEl = document.getElementById('liveToast');
      var bodyEl = document.getElementById('toastBody');
      bodyEl.textContent = message || 'Thành công';
      toastEl.classList.remove('text-bg-success', 'text-bg-danger', 'text-bg-info', 'text-bg-warning');
      toastEl.classList.add(bsBgClass || 'text-bg-success');
      new bootstrap.Toast(toastEl, { delay: 2000 }).show();
    }

    // Clone phân trang giống QLNgườiDùng
    (function () {
      function clonePager() {
        var grid = document.getElementById('<%= gvChucVu.ClientID %>');
              if (!grid) return;
              var src = grid.querySelector('.pagination');
              var out = document.getElementById('pagerOutside');
              if (!src || !out) return;
              out.innerHTML = '';
              src.querySelectorAll('a, span').forEach(function (el) {
                  out.appendChild(el.cloneNode(true));
              });
              src.style.display = 'none';
          }
          if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', clonePager);
          else clonePager();
          if (typeof (Sys) !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager)
              Sys.WebForms.PageRequestManager.getInstance().add_endRequest(clonePager);
      })();
  </script>
</div>
</asp:Content>
