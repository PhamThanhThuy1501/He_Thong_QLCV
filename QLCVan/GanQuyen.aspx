<%@ Page Title="Gán Quyền" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="GanQuyen.aspx.cs" Inherits="QLCVan.GanQuyen" EnableEventValidation="false" %>

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
            height: 13px; /* chiều cao cố định để đều */
            overflow: hidden; /* ẩn phần chữ thừa */
        }

            .welcome-bar marquee {
                font-size: 16px; /* chữ lớn hơn chút */
                font-weight: bold;
                color: #fff;
            }

        .page-title {
            font-size: 20px;
            font-weight: bold;
            text-align: start;
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
                background-color: #c00; /* nền đỏ đậm */
                color: #fff; /* chữ trắng */
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
                border: none; /* ❌ bỏ viền */
                background: none; /* ❌ bỏ nền trắng */
                padding: 6px 12px;
                border-radius: 4px;
                font-weight: 500;
                color: #111;
                text-decoration: none;
                transition: all 0.2s ease;
            }

                .grid-pager a:hover {
                    color: #c00; /* 🔹 khi hover chuyển sang đỏ */
                }

            .grid-pager span {
                background: #c00; /* 🔹 trang hiện tại tô đỏ */
                color: #fff;
            }
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

        <h3 class="page-title">
            GÁN QUYỀN CHO NHÓM QUYỀN 
            <asp:Label ID="lblTenNhom" runat="server" ></asp:Label>
        </h3>

        <!-- Thanh tìm kiếm -->
        <div class="search-bar">
            <label>Tìm kiếm:</label>
            <asp:TextBox ID="txtTenQuyen" runat="server" placeholder="Nhập tên quyền" />
            <asp:TextBox ID="txtMaQuyen" runat="server" placeholder="Nhập mã quyền" />
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
                    <asp:BoundField DataField="MaQuyen" HeaderText="Mã quyền" />
                    <asp:BoundField DataField="TenQuyen" HeaderText="Tên quyền" />

                    <asp:TemplateField HeaderText="Thao tác">
                        <ItemTemplate>
                            <asp:Button ID="btnGan" runat="server"
                                CommandName="ToggleQuyen"
                                CommandArgument='<%# Eval("MaQuyen") %>'
                                Type="button"
                                Text='<%# (bool)Eval("DaGan") ? "Đã gán" : "Gán" %>'
                                CssClass='<%# (bool)Eval("DaGan") ? "btn btn-outline-primary " : "btn btn-primary " %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
