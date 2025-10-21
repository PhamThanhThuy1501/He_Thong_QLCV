<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QLNhomQuyen.aspx.cs" Inherits="QLCVan.QLNhomQuyen" %>

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
    .pager a,.pager span{display:inline-block;padding:3px 10px;border:1px solid #ddd;margin:0 3px;border-radius:3px;text-decoration:none}
    .pager span{background:#c00;color:#fff;border-color:#c00}
    .pager a{color:#0f172a}

    .cv-head{ font-weight:700; }
    .table {
  width: 70% !important;    /* đồng nhất với toolbar */
  margin: 0 auto;           /* căn giữa */
}

    /* Loại bỏ nền xám */
.table tbody tr {
    background-color: #fff !important;
}

/* Hover nhẹ */
.table-hover tbody tr:hover {
    background-color: #f6f6f6 !important;
}

/* Xóa đường line trên phân trang */
.pager {
    border-top: none !important;
}

.action-column {
    text-align: center !important;
}
.action-buttons {
    display: flex;
    justify-content: center;
    gap: 5px;
}


  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
      <div class="content-header">
  <h2 class="content-header-title">QUẢN LÝ NHÓM QUYỀN</h2>
</div>

<div class="welcome-bar">
  <marquee behavior="scroll" direction="left" scrollamount="6">
    Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
  </marquee>
</div>

    <center>
      <h3 class="section-title"><b>DANH SÁCH NHÓM QUYỀN</b></h3>

      <!-- giữ khóa cần xoá -->
      <asp:HiddenField ID="hdfMaNhomQuyen" runat="server" />
      <asp:HiddenField ID="hdDeleteId" runat="server" />

      <!-- Toolbar tìm kiếm -->
      <div class="toolbar d-flex align-items-center">
        <div class="d-flex align-items-center gap-2 flex-grow-1">
          <span class="search-caption">Tìm kiếm</span>
          <asp:TextBox ID="txtMaQuyenSR" runat="server" CssClass="form-control" placeholder="Nhập mã nhóm quyền" />
          <asp:TextBox ID="txtTenQuyenSR" runat="server" CssClass="form-control" placeholder="Nhập tên nhóm quyền" />
          <asp:Button ID="btnSearch" runat="server" Text=" " CssClass="btn-search-red" ToolTip="Tìm kiếm" OnClick="btnSearch_Click" />
        </div>

        <button type="button" class="btn btn-primary btn-add ms-2" data-bs-toggle="modal" data-bs-target="#addModal">
          Thêm nhóm quyền
        </button>
      </div>

      <!-- Bảng -->
     <asp:GridView ID="gvNhomQuyen" runat="server" AutoGenerateColumns="False"
    CssClass="table table-bordered table-hover"

    HeaderStyle-CssClass="grid-header-red"
    Width="60%" CellPadding="4" ForeColor="#333333"
    DataKeyNames="MaNhomQuyen"
    AllowPaging="True" PageSize="5"
    OnPageIndexChanging="gvNhomQuyen_PageIndexChanging">

    <PagerSettings Mode="Numeric" Position="Bottom" PageButtonCount="5" />
    <PagerStyle CssClass="pager" HorizontalAlign="Center" />
    <Columns>

      <asp:TemplateField HeaderText="Mã nhóm quyền">
        <HeaderStyle HorizontalAlign="Center" />
        <ItemTemplate>
          <asp:Label ID="lblMaNhomQuyen" runat="server" Text='<%# Eval("MaNhomQuyen") %>'></asp:Label>
        </ItemTemplate>
      </asp:TemplateField>

      <asp:TemplateField HeaderText="Tên nhóm quyền">
        <HeaderStyle HorizontalAlign="Center" />
        <ItemTemplate>
          <asp:Label ID="lblTenNhomQuyen" runat="server" Text='<%# Eval("TenNhomQuyen") %>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
          <asp:TextBox ID="txtTenNhomQuyen_Edit" runat="server" CssClass="form-control"
                       Text='<%# Eval("TenNhomQuyen") %>'></asp:TextBox>
        </EditItemTemplate>
      </asp:TemplateField>

      <asp:TemplateField HeaderText="Thao tác">
    <HeaderStyle HorizontalAlign="Center" />
    <ItemStyle HorizontalAlign="Center" />
    <ItemTemplate>
        <div style="display: flex; justify-content: center; gap: 8px;">
            <a type="button" class="btn btn-primary btn-sm"
               href='<%# "GanQuyen.aspx?ma=" + Eval("MaNhomQuyen") + "&ten=" + Eval("TenNhomQuyen") %>'>
               Gán Quyền
            </a>

            <asp:LinkButton ID="btnEdit" runat="server"
                CssClass="btn btn-warning btn-sm" ToolTip="Sửa"
                OnClientClick='<%# "openEditModal(\"" + Eval("MaNhomQuyen") + "\",\"" + Eval("TenNhomQuyen") + "\"); return false;" %>'>
                <i class="fa fa-edit"></i>
            </asp:LinkButton>

            <asp:LinkButton ID="btnDelete" runat="server"
                CssClass="btn btn-danger btn-sm" ToolTip="Xóa"
                OnClientClick='<%# "openDeleteModal(\"" + Eval("MaNhomQuyen") + "\"); return false;" %>'>
                <i class="fa fa-trash"></i>
            </asp:LinkButton>
        </div>
    </ItemTemplate>
</asp:TemplateField>


    </Columns>
</asp:GridView>

    </center>

    <!-- Modal thêm nhóm quyền -->
    <div class="modal fade" id="addModal" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">

        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="addModalLabel">Thêm nhóm quyền</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3"><asp:TextBox ID="txtMdMaNhomQuyen" runat="server" CssClass="form-control" placeholder="Nhập mã nhóm quyền..." /></div>
            <div class="mb-3"><asp:TextBox ID="txtMdTenNhomQuyen" runat="server" CssClass="form-control" placeholder="Nhập tên nhóm quyền..." /></div>
          </div>
          <div class="modal-footer">
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-success" Text="Thêm" OnClick="btnSave_Click" />
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Sửa nhóm quyền -->
    <div class="modal fade" id="editModal" tabindex="-1" aria-labelledby="editModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">

        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="editModalLabel">Sửa nhóm quyền</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
          </div>
          <div class="modal-body">
            <asp:HiddenField ID="hdfMaNhomQuyen_Edit" runat="server" />
            <div class="mb-3">
              <label for="txtEditMa" class="form-label">Mã nhóm quyền</label>
              <asp:TextBox ID="txtEditMa" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="mb-3">
              <label for="txtEditTen" class="form-label">Tên nhóm quyền</label>
              <asp:TextBox ID="txtEditTen" runat="server" CssClass="form-control" />
            </div>
          </div>
          <div class="modal-footer">
            <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-success" Text="Sửa" OnClick="btnUpdate_Click" />
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
        <h5 class="modal-title" id="confirmDeleteLabel">Xác nhận xóa nhóm quyền</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
      </div>
      <div class="modal-body">
        Bạn có chắc muốn xóa nhóm quyền này không?
        <asp:HiddenField ID="HiddenField1" runat="server" />
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
        // Mở modal xác nhận xoá và lưu mã nhóm quyền cần xoá
        function openDeleteModal(maNhomQuyen) {
            document.getElementById('<%= hdDeleteId.ClientID %>').value = maNhomQuyen;
            var modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
            modal.show();
            return false; // chặn postback của LinkButton
        }

        // Mở modal sửa và gán giá trị
        function openEditModal(ma, ten) {
            document.getElementById('<%= txtEditMa.ClientID %>').value = ma;
            document.getElementById('<%= txtEditTen.ClientID %>').value = ten;
            document.getElementById('<%= hdfMaNhomQuyen_Edit.ClientID %>').value = ma;
            var m = new bootstrap.Modal(document.getElementById('editModal'));
            m.show();
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
</asp:Content>
