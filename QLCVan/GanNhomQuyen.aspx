<%@ Page Title="Gán Nhóm Quyền" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true" 
    CodeBehind="GanNhomQuyen.aspx.cs" Inherits="QLCVan.GanNhomQuyen" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <style>
body { background:#fff; font-family:"Segoe UI",Arial,sans-serif; }
.page { width:100%; margin:0; padding:0; }

.content-header { background:transparent; padding:0; border-bottom:none; margin:0 auto 6px; }
.content-header-title { text-transform:uppercase; font-weight:700; font-size:20px; color:#444; margin:0 0 6px; }

.welcome-bar {
  background:#c00; color:#fff; border-radius:4px;
  padding:8px 0; margin:0 auto 26px auto; font-weight:bold;
  text-align:center; display:flex; align-items:center; justify-content:center;
  height:30px; overflow:hidden;
}
.welcome-bar marquee { font-size:16px; font-weight:bold; color:#fff; }

.page-title {
  font-size:20px; font-weight:bold; text-align:center; color:#111;
  margin:25px 0 20px 0;
}

/* ===== Thanh tìm kiếm ===== */
.search-bar {
  display:flex; align-items:center; justify-content:center;
  gap:30px; margin:0 auto 25px auto;
}
.search-bar label { font-weight:600; color:#111; }
.search-bar input {
  border:1px solid #ccc; border-radius:4px;
  padding:8px 10px; height:34px; width:280px; font-size:14px;
}
.btn-search {
  background:#C62828; color:#fff; border:none!important;
  height:36px; width:36px; cursor:pointer; border-radius:6px;
  display:flex; align-items:center; justify-content:center;
  font-size:16px; transition:background-color .25s ease;
}
.btn-search:hover { background:#BB0000; }

/* ===== Bảng ===== */
.table-wrapper { width:70%; margin:0 auto; background:#fff; }
.table { width:100%; border-collapse:collapse; background:#fff; table-layout:fixed; }
.table th,.table td {
  border:1px solid #ddd; padding:8px 10px;
  text-align:center; font-size:14px;
}
.table tr th {
  background-color:#c00!important; color:#fff!important;
  font-weight:600; text-transform:uppercase; border-bottom:2px solid #900;
}

/* ===== PHÂN TRANG NGOÀI BẢNG ===== */
.pager-out {
  width:70%;
 margin:22px auto 60px auto;
  text-align:center;

}
.pager-out a, .pager-out span {
  display:inline-flex; align-items:center; justify-content:center;
  width:42px; height:42px;
  border:1px solid #d1d5db; border-radius:4px;
  background:#fff; color:#4b5563; font-size:16px;
  font-weight:500; text-decoration:none; margin:0 5px;
  transition: background-color .18s ease, color .18s ease, border-color .18s ease, transform .06s ease;
}
.pager-out span {
  background:#C62828;
  color:#fff;
  border-color:#C62828;
  box-shadow:0 1px 2px rgba(0,0,0,.06);
}
.pager-out a:hover {
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

  <h3 class="page-title">
    GÁN NHÓM QUYỀN CHO CHỨC VỤ
    <asp:Label ID="lblTenNhom" runat="server"></asp:Label>
  </h3>

  <!-- Thanh tìm kiếm -->
  <div class="search-bar">
    <label>Tìm kiếm</label>
    <asp:TextBox ID="txtTenQuyen" runat="server" placeholder="Nhập tên nhóm quyền" />
    <asp:TextBox ID="txtMaQuyen" runat="server" placeholder="Nhập mã nhóm quyền" />
    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click">
      <i class="fa fa-search"></i>
    </asp:LinkButton>
  </div>

  <asp:HiddenField ID="hdfMaNhom" runat="server" />

  <!-- Bảng danh sách -->
  <div class="table-wrapper">
    <asp:GridView ID="gvGanQuyen" runat="server" AutoGenerateColumns="False"
      CssClass="table table-bordered gridview"
      AllowPaging="True" PageSize="5"
      OnPageIndexChanging="gvGanQuyen_PageIndexChanging"
      PagerStyle-CssClass="pagination pagination-source"
      BorderStyle="None"
      OnRowCommand="gvGanQuyen_RowCommand">
      <Columns>
        <asp:BoundField DataField="MaNhomQuyen" HeaderText="Mã nhóm quyền" />
        <asp:BoundField DataField="TenNhomQuyen" HeaderText="Tên nhóm quyền" />
        <asp:TemplateField HeaderText="Thao tác">
          <ItemTemplate>
            <asp:Button ID="btnGan" runat="server"
              CommandName="ToggleQuyen"
              CommandArgument='<%# Eval("MaNhomQuyen") %>'
              Text='<%# (bool)Eval("DaGan") ? "Đã gán" : "Gán" %>'
              CssClass='<%# (bool)Eval("DaGan") ? "btn btn-outline-primary" : "btn btn-primary" %>' />
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
    </asp:GridView>
  </div>

  <!-- ✅ Phân trang ngoài bảng -->
  <div id="pagerOutside" class="pager-out"></div>
</div>

<script>
(function () {
  function clonePager() {
    var grid = document.getElementById('<%= gvGanQuyen.ClientID %>');
            if (!grid) return;

            var src = grid.querySelector('.pagination');
            var out = document.getElementById('pagerOutside');
            if (!src || !out) return;

            out.innerHTML = '';
            src.querySelectorAll('a, span').forEach(el => out.appendChild(el.cloneNode(true)));
            src.style.display = 'none'; // ẩn phân trang gốc
        }

        if (document.readyState === 'loading')
            document.addEventListener('DOMContentLoaded', clonePager);
        else
            clonePager();

        if (typeof (Sys) !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager)
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(clonePager);
    })();
</script>
</asp:Content>
