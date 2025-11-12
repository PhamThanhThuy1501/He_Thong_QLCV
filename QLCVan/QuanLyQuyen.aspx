<%@ Page Title="Quản lý quyền" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QuanLyQuyen.aspx.cs" Inherits="QLCVan.QuanLyQuyen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <style>
  body {
    background: #fff;
    font-family: "Segoe UI", Arial, sans-serif;
  }

  .page {
    width: 100%;
    margin: 0;
    padding: 0;
  }

  .main-title {
    font-size: 20px;
    font-weight: bold;
    text-transform: uppercase;
    color: #1f2937;
    margin-bottom: 5px;
  }

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
  overflow: hidden;                 /* ẩn phần chữ thừa */
}

.welcome-bar marquee {
  font-size: 16px;                   /* chữ lớn hơn chút */
  font-weight: bold;
  color: #fff;
                
}

  .page-title {
    font-size: 20px;
    font-weight: bold;
    text-align: center;
    color: #111;
    margin: 25px 0 20px 0;
  }

/* ✅ Thanh tìm kiếm căn giữa, giãn cách đều */
.search-bar {
  display: flex;
  align-items: center;
  justify-content: center;   /* căn giữa ngang */
  gap: 30px;                 /* 🔹 tăng khoảng cách giữa các phần tử */
  margin: 0 auto 25px auto;  /* cách dưới thêm một chút */
}

.search-bar label {
  font-weight: 600;
  color: #111;
  margin-right: 10px;
}

.search-bar input {
  border: 1px solid #ccc;
  border-radius: 4px;
  padding: 8px 10px;
  height: 34px;
  width: 280px;              /* 🔹 tăng độ rộng ô nhập */
  font-size: 14px;
}

.btn-search {
  background: #c00;
  color: #fff;
  border: none;
  height: 36px;
  width: 36px;
  cursor: pointer;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
}

.btn-search:hover {
  background: #a00;
}


  /* ✅ Bảng danh sách */
  .table-wrapper {
    width: 70%;
    margin: 0 auto;
    background: #fff;
  }

  .table {
    width: 100%;
    border-collapse: collapse;
    background: #fff;
  }

  .table th,
  .table td {
    border: 1px solid #ddd;
    padding: 8px 10px;
    text-align: center;
    font-size: 14px;
  }

/* 🔴 Tô đỏ hàng đầu tiên (header) của GridView */
.table tr th {
  background-color: #c00;   /* nền đỏ đậm */
  color: #fff;              /* chữ trắng */
  font-weight: 600;
  text-transform: uppercase;
  border-bottom: 2px solid #900; /* viền đỏ đậm phía dưới */
}



  /* ✅ Phân trang ra giữa, KHÔNG VIỀN */
  .grid-pager {
    display: flex;
    justify-content: center; /* 🔹 căn giữa ngang */
    align-items: center;
    gap: 10px;
    margin-top: 25px;
  }

  .grid-pager a,
  .grid-pager span {
    border: none;            /* ❌ bỏ viền */
    background: none;        /* ❌ bỏ nền trắng */
    padding: 6px 12px;
    border-radius: 4px;
    font-weight: 500;
    color: #111;
    text-decoration: none;
    transition: all 0.2s ease;
  }

  .grid-pager a:hover {
    color: #c00;             /* 🔹 khi hover chuyển sang đỏ */
  }

  .grid-pager span {
    background: #C00000;        /* 🔹 trang hiện tại tô đỏ */
    color: #fff;
  }
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
  <h2 class="content-header-title">QUẢN LÝ QUYỀN</h2>
</div>

<div class="welcome-bar">
  <marquee behavior="scroll" direction="left" scrollamount="6">
    Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
  </marquee>
</div>

    <h3 class="page-title">DANH SÁCH QUYỀN</h3>

    <!-- ✅ Thanh tìm kiếm bên trái -->
    <div class="search-bar">
      <label>Tìm kiếm:</label>
      <asp:TextBox ID="txtTenQuyen" runat="server" placeholder="Nhập tên quyền" />
      <asp:TextBox ID="txtMaQuyen" runat="server" placeholder="Nhập mã quyền" />
      <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click">
        <i class="fa fa-search"></i>
      </asp:LinkButton>
    </div>

    <!-- ✅ Bảng danh sách -->
    <div class="table-wrapper">
 <asp:GridView ID="gvQuyentbl" runat="server" AutoGenerateColumns="False"
  CssClass="table"
  AllowPaging="True" PageSize="5"
  OnPageIndexChanging="gvQuyen_PageIndexChanging"
  PagerStyle-CssClass="grid-pager"
  BorderStyle="None">

  <Columns>
    <asp:BoundField DataField="MaQuyen" HeaderText="Mã quyền" />
    <asp:BoundField DataField="TenQuyen" HeaderText="Tên quyền" />
    <asp:BoundField DataField="MoTa" HeaderText="Thao tác" />
  </Columns>
</asp:GridView>


        
    </div>
  </div>
</asp:Content>
