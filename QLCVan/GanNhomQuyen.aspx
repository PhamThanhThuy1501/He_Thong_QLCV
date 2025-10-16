<%@ Page Title="Gán Nhóm Quyền" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true" 
    CodeBehind="GanNhomQuyen.aspx.cs" Inherits="QLCVan.GanNhomQuyen" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Bootstrap 5 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
    <style>
        /* Giữ nguyên toàn bộ style gốc */
        body { background: #fff; font-family: "Segoe UI", Arial, sans-serif; }
        .page { width: 100%; margin: 0; padding: 0; }
        .main-title { font-size: 20px; font-weight: bold; text-transform: uppercase; color: #1f2937; margin-bottom: 5px; }
        .content-header { background: transparent; padding: 0; border-bottom: none; margin: 0 auto 6px auto; }
        .content-header-title { text-transform: uppercase; font-weight: 700; font-size: 20px; color: #444; margin: 0 0 6px 0; }
        .welcome-bar { background: #c00; color: #fff; border-radius: 4px; padding: 8px 0; margin: 0 auto 26px auto;
                       font-weight: bold; text-align: center; display: flex; align-items: center; justify-content: center;
                       height: 13px; overflow: hidden; }
        .welcome-bar marquee { font-size: 16px; font-weight: bold; color: #fff; }
        .page-title { font-size: 20px; font-weight: bold; text-align: start; color: #111; margin: 25px 0 20px 0; }
        .search-bar { display: flex; align-items: center; justify-content: center; gap: 30px; margin: 0 auto 25px auto; }
        .search-bar label { font-weight: 600; color: #111; margin-right: 10px; }
        .search-bar input { border: 1px solid #ccc; border-radius: 4px; padding: 8px 10px; height: 34px; width: 280px; font-size: 14px; }
        .btn-search { background: #c00; color: #fff; border: none; height: 36px; width: 36px; cursor: pointer; border-radius: 4px;
                      display: flex; align-items: center; justify-content: center; font-size: 16px; }
        .btn-search:hover { background: #a00; }
        .table-wrapper { width: 70%; margin: 0 auto; background: #fff; }
        .table { width: 100%; border-collapse: collapse; background: #fff; }
        .table th, .table td { border: 1px solid #ddd; padding: 8px 10px; text-align: center; font-size: 14px; }
        .table tr th { background-color: #c00; color: #fff; font-weight: 600; text-transform: uppercase; border-bottom: 2px solid #900; }
        .grid-pager { display: flex; justify-content: center; align-items: center; gap: 10px; margin-top: 25px; }
        .grid-pager a, .grid-pager span { border: none; background: none; padding: 6px 12px; border-radius: 4px; font-weight: 500;
                                          color: #111; text-decoration: none; transition: all 0.2s ease; }
        .grid-pager a:hover { color: #c00; }
        .grid-pager span { background: #c00; color: #fff; }
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
            GÁN NHÓM QUYỀN CHO CHỨC VỤ 
            <asp:Label ID="lblTenNhom" runat="server"></asp:Label>
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
