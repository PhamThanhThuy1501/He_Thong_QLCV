<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QLnguoidung.aspx.cs" Inherits="QLCVan.QLnguoidung" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
  <style>
    body { background:#f3f4f6; }
    .page { background:transparent; padding:0; margin:0; }

    .content-header { background:transparent; padding:0; border-bottom:none; margin:0 auto 6px; }
    .content-header-title{ text-transform:uppercase; font-weight:700; font-size:20px; color:#444; margin:0 0 6px; }

        .welcome-bar {
            background: #c00; /* nền đỏ đậm */
            color: #fff;
            border-radius: 4px; /* bo góc mềm */
            padding: 8px 0; /* cao vừa để chữ nằm giữa */
            margin: 0 auto 26px auto;
            font-weight: bold; /* in đậm */
            text-align: center;
            display: flex;
            align-items: center; /* căn giữa theo chiều cao */
            justify-content: center;
            height: 30px; /* chiều cao cố định để đều */
            overflow: hidden; /* ẩn phần chữ thừa */
        }

            .welcome-bar marquee {
                font-size: 16px; /* chữ lớn hơn chút */
                font-weight: bold;
                color: #fff;
            }

    .page-title{             
            font-size: 20px;
            font-weight: bold;
            text-align: center;
            color: #111;
            margin: 25px 0 20px 0; }

    .btn-add{ background:#0d6efd; color:#fff; border:none; padding:8px 18px; font-weight:600; border-radius:6px;
      text-decoration:none; transition:.2s }
    .btn-add:hover{ background:#2b6fe8; box-shadow:0 2px 6px rgba(0,0,0,.08) }

    .action-bar{ max-width:1200px; margin:0 auto 14px; }

    .search-bar,.table-wrapper{ max-width:1200px; margin:0 auto 24px }
    .search-bar{ background:#f8f9fa; border:1px solid #ddd; border-radius:5px; padding:15px 18px; box-shadow:0 1px 3px rgba(0,0,0,.05) }
    .search-label{ font-weight:400; font-size:15px; margin-bottom:10px; }
    .search-group{ display:grid; grid-template-columns:repeat(auto-fit,minmax(140px,1fr)); gap:12px 18px; align-items:end; max-width:1050px; margin:auto }
    .field{ display:flex; flex-direction:column }
    .field label{ font-size:15px; font-weight:700; color:#333; margin-bottom:6px }

    .form-control,.form-select{ height:36px; font-size:13.5px; max-width:190px }
    .btn-search{ height:36px; font-size:13.5px }

    .table thead{ background:#C62828!important; color:#fff!important; text-align:center }
    .table th{ background:#C62828; color:#fff; text-align:center }
    .table td,.table th{ vertical-align:middle; text-align:center }

    .badge-status{ display:inline-flex; justify-content:center; align-items:center; width:140px; padding:8px 12px; border-radius:12px; color:#fff; font-weight:600; font-size:14px }
    .badge-active{ background:#28a745 } .badge-locked{ background:#dc3545 }

    .btn-action{ border:none; background:none; color:#0d6efd; margin:0 3px } .btn-action:hover{ color:#063b8f }
    .btn-delete{ color:#dc3545 } .btn-delete:hover{ color:#a61c1c }

    .grid-pager{ text-align:center; padding:10px 0 }
    .grid-pager a,.grid-pager span{ display:inline-block; margin:0 4px; padding:6px 10px; border:1px solid #e5e7eb; border-radius:6px; background:#fff; text-decoration:none }
    .grid-pager span{ background:#0d6efd; color:#fff; border-color:#0d6efd }

 /* ===== BẢNG NGƯỜI DÙNG ===== */
body { background:#f3f4f6; font-family:"Segoe UI",Arial,sans-serif; }
.table { width:100%; border-collapse:collapse; background:#fff; }
.table th,.table td { border:1px solid #ddd; text-align:center; vertical-align:middle; padding:8px 10px; font-size:14px; }
.table thead th { background:#C62828; color:#fff; text-transform:uppercase; font-weight:600; }
.badge-status { display:inline-flex; justify-content:center; align-items:center; width:120px; padding:6px 10px; border-radius:12px; color:#fff; font-weight:600; font-size:13px; }
.badge-locked{ background:#dc3545; }

/* ===== NÚT SỬA / XÓA ===== */
.table td:last-child { text-align:center; white-space:nowrap; }
.actions { display:flex; justify-content:center; align-items:center; gap:8px; border:none; }

/* Nút chung */
.btn-action,.btn-delete{
  display:inline-flex; align-items:center; justify-content:center;
  width:38px; height:38px; background:#fff; border:1px solid #e5e7eb;
  border-radius:10px; text-decoration:none; transition:.2s;
}
.btn-action i{ color:#0B57D0; font-size:16px; }
.btn-delete i{ color:#DC2626; font-size:16px; }

.btn-action:hover{ background:#F3F6FF; border-color:#BCD3FF; }
.btn-delete:hover{ background:#FFF5F5; border-color:#F3B6B6; }

.table td:last-child span,
.table td:last-child,
.actions *{ border:none!important; outline:none!important; }



  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="page">
    <div class="content-header">
      <h2 class="content-header-title">QUẢN LÝ NGƯỜI DÙNG</h2>
    </div>
    <div class="welcome-bar">
      <marquee behavior="scroll" direction="left" scrollamount="6">
        Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
      </marquee>
    </div>

    <h3 class="page-title">DANH SÁCH NGƯỜI DÙNG</h3>

    <div class="action-bar text-end mb-3">
      <a href="ThemNguoiDung.aspx" class="btn btn-add">
        <i class="fa fa-user-plus"></i> Thêm người dùng
      </a>
    </div>

    <!-- TÌM KIẾM -->
    <div class="search-bar">
      <div class="search-label">Tìm kiếm</div>
      <div class="search-group">
        <div class="field">
          <label for="txtSearchTenDN">Tên đăng nhập:</label>
          <asp:TextBox ID="txtSearchTenDN" runat="server" CssClass="form-control" placeholder="Nhập tên đăng nhập" />
        </div>
        <div class="field">
          <label for="txtSearchEmail">Email:</label>
          <asp:TextBox ID="txtSearchEmail" runat="server" CssClass="form-control" placeholder="Nhập email" />
        </div>
        <div class="field">
          <label for="ddlDonVi">Đơn vị:</label>
          <asp:DropDownList ID="ddlDonVi" runat="server" CssClass="form-select" />
        </div>
        <div class="field">
          <label for="ddlChucVu">Chức vụ:</label>
          <asp:DropDownList ID="ddlChucVu" runat="server" CssClass="form-select" />
        </div>
        <div class="field">
          <label>&nbsp;</label>
          <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-danger btn-search" Text="Tìm kiếm" OnClick="btnSearch_Click" />
        </div>
      </div>
    </div>

    <!-- BẢNG -->
    <div class="table-wrapper">
      <asp:GridView ID="gvNguoiDung" runat="server" AutoGenerateColumns="False"
        CssClass="table table-bordered"
        DataKeyNames="MaNguoiDung"
        AllowPaging="True" PageSize="5"
        OnPageIndexChanging="gvNguoiDung_PageIndexChanging"
        PagerStyle-CssClass="grid-pager">

        <PagerSettings Mode="Numeric" Position="Bottom" PageButtonCount="5" />

        <Columns>
          <asp:BoundField DataField="TenDN" HeaderText="Tên đăng nhập" />
          <asp:BoundField DataField="Email" HeaderText="Email" />
          <asp:BoundField DataField="TenNhom" HeaderText="Đơn vị" />
          <asp:BoundField DataField="TenChucVu" HeaderText="Chức vụ" />

          <asp:TemplateField HeaderText="Trạng thái">
            <ItemTemplate>
              <asp:Literal ID="litStatus" runat="server" Mode="PassThrough"
                Text='<%# Convert.ToBoolean(Eval("TrangThai"))
                      ? "<span class=\"badge-status badge-active\">Đang kích hoạt</span>"
                      : "<span class=\"badge-status badge-locked\">Đã khóa</span>" %>' />
            </ItemTemplate>
          </asp:TemplateField>

          <asp:TemplateField HeaderText="Thao tác">
            <ItemTemplate>
              <a href='<%# "SuaNguoiDung.aspx?id=" + Eval("MaNguoiDung") %>' class="btn-action" title="Sửa">
                <i class="fa fa-pen"></i>
              </a>
              <button type="button" class="btn btn-delete" data-bs-toggle="modal"
                      data-bs-target="#confirmDeleteModal"
                      onclick="setDeleteUser('<%# Eval("MaNguoiDung") %>')">
                <i class="fa fa-trash"></i>
              </button>
            </ItemTemplate>
          </asp:TemplateField>
        </Columns>
      </asp:GridView>
    </div>

    <!-- Modal xác nhận xoá -->
    <div class="modal fade" id="confirmDeleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">
      <div class="modal-dialog">
        <div class="modal-content border-danger">
          <div class="modal-header">
            <h5 class="modal-title">Xác nhận xóa người dùng</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <p>Bạn có chắc muốn xóa người dùng này không?</p>
            <asp:HiddenField ID="hdnDeleteUser" runat="server" />
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
            <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger"
                        Text="Xóa" OnClick="btnConfirmDelete_Click" />
          </div>
        </div>
      </div>
    </div>
  </div>
    <!-- Toast container -->
<div class="position-fixed top-0 end-0 p-3" style="z-index:1080">
  <div id="appToast" class="toast align-items-center text-white border-0" role="alert" aria-live="assertive" aria-atomic="true">
    <div class="d-flex">
      <div id="appToastBody" class="toast-body">
        <!-- message sẽ được inject -->
      </div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
  </div>
</div>

<div class="position-fixed top-0 end-0 p-3" style="z-index:1080">
  <div id="appToast" class="toast align-items-center text-white border-0" role="alert" aria-live="assertive" aria-atomic="true">
    <div class="d-flex">
      <div id="appToastBody" class="toast-body"></div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
  </div>
</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<script>
    function showToast(type, message) {
        const el = document.getElementById('appToast');
        const body = document.getElementById('appToastBody');
        el.className = 'toast align-items-center text-white border-0';
        el.classList.add({ success: 'bg-success', info: 'bg-info', warning: 'bg-warning', danger: 'bg-danger' }[type] || 'bg-secondary');
        body.textContent = message;
        new bootstrap.Toast(el, { delay: 2500 }).show();
    }
</script>

  <script>
    function setDeleteUser(id) {
      document.getElementById('<%= hdnDeleteUser.ClientID %>').value = id;
      }
  </script>
  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>
