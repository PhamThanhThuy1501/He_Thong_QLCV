<%@ Page Title="Quản lý chức vụ" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QLChucVu.aspx.cs" Inherits="QLCVan.QLChucVu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
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
            height: 13px;
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

        .search-bar {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 30px;
            margin: 0 auto 25px auto;
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
                width: 280px;
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

            .table tr th {
                background-color: #c00;
                color: #fff;
                font-weight: 600;
                text-transform: uppercase;
                border-bottom: 2px solid #900;
            }

        .grid-pager {
           text-align: center !important;
            margin-top: 25px;
        }
        .grid-pager table {
    margin: 0 auto; /* căn giữa table chứa số trang */
}

            .grid-pager a,
            .grid-pager span {
                border: none;
                background: none;
                padding: 6px 12px;
                border-radius: 4px;
                font-weight: 500;
                color: #111;
                text-decoration: none;
                transition: all 0.2s ease;
            }

                .grid-pager a:hover {
                    color: #c00;
                }

            .grid-pager span {
                background: #c00;
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

        <h3 class="page-title">DANH SÁCH CHỨC VỤ</h3>

        <!-- ✅ Thanh tìm kiếm -->
        <div class="search-bar">
            <label>Tìm kiếm:</label>
            <asp:TextBox ID="txtTenChucVuSR" runat="server" placeholder="Nhập tên chức vụ" />
            <asp:TextBox ID="txtMaChucVuSR" runat="server" placeholder="Nhập mã chức vụ" />
            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click">
                <i class="fa fa-search"></i>
            </asp:LinkButton>
            <button type="button"
                class="btn btn-primary btn-add ms-2"
                data-bs-toggle="modal"
                data-bs-target="#addModal">
                Thêm chức vụ
            </button>
        </div>

        <!-- ✅ Bảng danh sách -->
        <div class="table-wrapper">
            <asp:GridView ID="gvChucVu" runat="server" AutoGenerateColumns="False"

    CssClass="table"
    AllowPaging="True" PageSize="5"
    OnPageIndexChanging="gvChucVu_PageIndexChanging"
    PagerStyle-CssClass="grid-pager"
    BorderStyle="None">

    <Columns>
        <asp:BoundField DataField="MaChucVu" HeaderText="Mã chức vụ" />
        <asp:BoundField DataField="TenChucVu" HeaderText="Tên chức vụ" />
        <asp:TemplateField HeaderText="Thao Tác">
    <ItemTemplate>
        <a type="button" class="btn btn-primary btn-sm"
           href='<%# "GanNhomQuyen.aspx?ma=" + Eval("MaChucVu") + "&ten=" + Eval("TenChucVu") %>'>
           Gán Quyền
        </a>

        <!-- Nút sửa -->
        <button type="button"
                class="btn btn-warning btn-sm"
                data-bs-toggle="modal"
                data-bs-target="#editModal"
                data-ma='<%# Eval("MaChucVu") %>'
                data-ten="<%# Eval("TenChucVu") %>">
            <i class="fa fa-edit"></i>
        </button>

        <!-- Nút xóa -->
        <button type="button"
                class="btn btn-danger btn-sm"
                data-bs-toggle="modal"
                data-bs-target="#deleteModal"
                data-id='<%# Eval("MaChucVu") %>'>
            <i class="fa fa-trash"></i>
        </button>
    </ItemTemplate>
</asp:TemplateField>


                CssClass="table"
                AllowPaging="True" PageSize="5"
                OnPageIndexChanging="gvChucVu_PageIndexChanging"
                PagerStyle-CssClass="grid-pager"
                BorderStyle="None">

                <Columns>
                    <asp:BoundField DataField="MaChucVu" HeaderText="Mã chức vụ" />
                    <asp:BoundField DataField="TenChucVu" HeaderText="Tên chức vụ" />
                    <asp:TemplateField HeaderText="Thao Tác">
                        <ItemTemplate>
                                                            <a type="button" class="btn btn-primary"
                                    href="<%# "GanNhomQuyen.aspx?ma=" +Eval("MaChucVu")+"&ten=" + Eval("TenChucVu") %>"
                                    >
                                    Gán Quyền
                                </a>
                            <button
                                type="button"
                                class="fa fa-pencil btn btn-light"
                                style="font-size: 26px; color: blue; border: none;"
                                data-bs-toggle="modal"
                                data-bs-target="#editModal"
                                data-ma='<%# Eval("MaChucVu") %>'
                                data-ten="<%# Eval("TenChucVu") %>">
                            </button>
                            <button
                                type="button"
                                class="fa fa-trash btn btn-light"
                                aria-hidden="true"
                                style="font-size: 26px; color: red; border: none;"
                                data-bs-toggle="modal"
                                data-bs-target="#deleteModal"
                                data-id='<%#Eval("MaChucVu") %>'>
                            </button>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>

        <!-- Modal thêm chức vụ -->
        <div class="modal fade" id="addModal" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="addModalLabel">Thêm mới chức vụ</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
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

        <!-- Modal xoá chức vụ -->
        <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content border-danger">
                    <div class="modal-header">
                        <h5 class="modal-title" id="deleteModalLabel">Xác nhận xoá</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Đóng"></button>
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

        <!-- Modal sửa chức vụ -->
        <div class="modal fade" id="editModal" tabindex="-1" aria-labelledby="editModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="editModalLabel">Sửa chức vụ</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
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

        <script>
            document.addEventListener("DOMContentLoaded", function () {
                var deleteModal = document.getElementById('deleteModal');
                deleteModal.addEventListener('show.bs.modal', function (event) {
                    var button = event.relatedTarget;
                    var id = button.getAttribute('data-id');
                    var hidden = document.getElementById('<%= hdDeleteId.ClientID %>');
                    hidden.value = id;
                });
            });

            var editModal = document.getElementById('editModal');
            editModal.addEventListener('show.bs.modal', function (event) {
                var button = event.relatedTarget;
                var ma = button.getAttribute('data-ma');
                var ten = button.getAttribute('data-ten');
                document.getElementById('<%= txtEditMa.ClientID %>').value = ma;
                document.getElementById('<%= txtEditTen.ClientID %>').value = ten;
                document.getElementById('<%= hdfMaChucVu.ClientID %>').value = ma;
            });
        </script>
    </div>
</asp:Content>
