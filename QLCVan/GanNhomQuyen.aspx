<%@ Page Title="Gán Nhóm Quyền" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="GanNhomQuyen.aspx.cs" Inherits="QLCVan.GanNhomQuyen" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap 5 CSS -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

<!-- Font Awesome (nếu có icon) -->
<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
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
    background: #c00;          /* nền đỏ */
    color: #fff;
    border-radius: 4px;        /* bo góc */
    padding: 6px 10px;         /* khoảng cách trong */
    margin: 0 auto 20px auto;  /* cách dưới */
    font-weight: bold;
    text-align: center;
      height: 30px;
    overflow: hidden;
}

.welcome-bar marquee {
    font-size: 16px;
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
            justify-content: center; /* căn giữa ngang */
            gap: 30px; /* 🔹 tăng khoảng cách giữa các phần tử */
            margin: 0 auto 25px auto; /* cách dưới thêm một chút */
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
                width: 280px; /* 🔹 tăng độ rộng ô nhập */
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
            .btn-search,
.btn-search i {
  text-decoration: none !important;   /* xoá gạch chân */
  outline: none !important;           /* bỏ viền khi focus */
}


.table-wrapper {
    width: 70%;
    margin: 0 auto;
    background: #fff;
    text-align: center; /* giúp pager căn giữa */
}

/* Bảng */
.table {
    width: 100%;
    border-collapse: collapse;
    background: #fff;
}

/* Header đỏ */
.table tr th {
    background-color: #c00;
    color: #fff;
    font-weight: 600;
    text-transform: uppercase;
    border-bottom: 2px solid #900;
}

/* Thay thế toàn bộ rule .grid-pager trước đó bằng đoạn này */
.grid-pager {
  display: inline-flex;
  gap: 8px;
  padding: 0;                /* bỏ padding -> không có "hộp" quanh */
  border: none !important;   /* bỏ viền ngoài hoàn toàn */
  background: transparent;   /* bỏ nền */
  margin: 20px auto 0 auto;  /* căn giữa */
  box-shadow: none;          /* bỏ shadow nếu có */
}

/* Các ô số / link */
.grid-pager a {
  display: inline-block;
  padding: 8px 12px;
  border-radius: 6px;
  border: 1px solid #ddd;   /* ô riêng lẻ có viền nhẹ */
  background: #fff;
  color: #111;
  text-decoration: none;
  font-weight: 600;
}

/* Hover cho ô không phải trang hiện tại */
.grid-pager a:hover {
  color: #c00;
  border-color: #ccc;
  background: #fff;
}

/* Trang hiện tại */
.grid-pager span {
  display: inline-block;
  padding: 8px 12px;
  border-radius: 6px;
  border: 1px solid #c00;
  background: #c00;
  color: #fff;
  font-weight: 700;
}

/* Thu nhỏ khi màn hình nhỏ */
@media (max-width: 600px) {
  .grid-pager a, .grid-pager span { padding:6px 8px; }
}


/* Nếu bạn muốn nút đầu/ cuối (First/Last) nhỏ hơn */
.grid-pager .pager-aux {
    padding: 6px 8px;
    font-weight: 600;
}

/* Responsive: thu nhỏ khi màn hình nhỏ */
@media (max-width: 600px) {
    .grid-pager { gap:4px; padding:8px; }
    .grid-pager a, .grid-pager span { padding:6px 8px; }
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
            <asp:Label ID="lblTenNhom" runat="server" ></asp:Label>
        </h3>

        <!-- Thanh tìm kiếm -->
        <div class="search-bar">
            <label>Tìm kiếm:</label>
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
                CssClass="table"
                AllowPaging="True" PageSize="5"
                OnPageIndexChanging="gvGanQuyen_PageIndexChanging"
                PagerStyle-CssClass="grid-pager"
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
    </div>
</asp:Content>
