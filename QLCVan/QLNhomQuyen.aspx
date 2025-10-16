<%@ Page Title="Quản lý nhóm quyền" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QLNhomQuyen.aspx.cs" Inherits="QLCVan.QLNhomQuyen"   %>

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
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

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

        <h3 class="page-title">DANH SÁCH NHÓM QUYỀN</h3>

        <!-- ✅ Thanh tìm kiếm bên trái -->
        <div class="search-bar">
            <label>Tìm kiếm:</label>
            <asp:TextBox ID="txtTenQuyenSR" runat="server" placeholder="Nhập tên quyền" />
            <asp:TextBox ID="txtMaQuyenSR" runat="server" placeholder="Nhập mã quyền" />
            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click">
        <i class="fa fa-search"></i>
            </asp:LinkButton>
            <button type="button"
                class="btn btn-primary btn-add ms-2"
                data-bs-toggle="modal"
                data-bs-target="#addModal">
                Thêm nhóm quyền
            </button>
        </div>

        <!-- ✅ Bảng danh sách -->
        <div class="table-wrapper">
            <asp:GridView ID="gvNhomQuyen" runat="server" AutoGenerateColumns="False"
                CssClass="table"
                AllowPaging="True" PageSize="5"
                OnPageIndexChanging="gvNhomQuyen_PageIndexChanging"
                PagerStyle-CssClass="grid-pager"
                BorderStyle="None">

                <Columns>
                    <asp:BoundField DataField="MaNhomQuyen" HeaderText="Mã nhóm quyền" />
                    <asp:BoundField DataField="TenNhomQuyen" HeaderText="Tên nhóm quyền" />
                    <asp:TemplateField HeaderText="Thao Tác">
                        <ItemTemplate>
                                <a type="button" class="btn btn-primary"
                                    href="<%# "GanQuyen.aspx?ma=" +Eval("MaNhomQuyen")+"&ten=" + Eval("TenNhomQuyen") %>"
                                    >
                                    Gán Quyền
                                </a>
                            <button
                                type="button"
                                class="fa fa-pencil btn btn-light"
                                style="font-size: 26px; color: blue; border: none;"
                                data-bs-toggle="modal"
                                data-bs-target="#editModal"
                                data-ma='<%# Eval("MaNhomQuyen") %>'
                                data-ten="<%# Eval("TenNhomQuyen") %>">
                            </button>
                            <button
                                type="button"
                                class="fa fa-trash btn btn-light"
                                aria-hidden="true"
                                style="font-size: 26px; color: red; border: none;"
                                data-bs-toggle="modal"
                                data-bs-target="#deleteModal"
                                data-id='<%#Eval("MaNhomQuyen") %>'>
                            </button>
                        </ItemTemplate>


                    </asp:TemplateField>
                </Columns>
            </asp:GridView>



        </div>
        <%--========Modal=============--%>
        <!-- Modal thêm nhóm quyền -->
        <div class="modal fade" id="addModal" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="addModalLabel">Thêm mới nhóm quyền</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <asp:TextBox ID="txtMdMaNhomQuyen" runat="server" CssClass="form-control" placeholder="Nhập mã nhóm quyền..." />
                        </div>
                        <div class="mb-3">
                            <asp:TextBox ID="txtMdTenNhomQuyen" runat="server" CssClass="form-control" placeholder="Nhập tên nhóm quyền..." />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnSave" runat="server" Text="Thêm" CssClass="btn btn-success" OnClick="btnSave_Click" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                    </div>
                </div>
            </div>
        </div>
        <%----Modal Xoá Nhóm Quyền--%>
        <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content border-danger">
                    <div class="modal-header">
                        <h5 class="modal-title" id="deleteModalLabel">Xác nhận xoá</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Đóng"></button>
                    </div>
                    <div class="modal-body">
                        <p>Bạn có chắc muốn xoá nhóm quyền này không?</p>
                        <asp:HiddenField ID="hdDeleteId" runat="server" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Huỷ</button>
                        <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger" Text="Xoá" OnClick="btnConfirmDelete_Click" />
                    </div>
                </div>
            </div>
        </div>
        <!-- Modal SỬA NHÓM QUYỀN -->
        <div class="modal fade" id="editModal" tabindex="-1" aria-labelledby="editModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="editModalLabel">Sửa nhóm quyền</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
                    </div>
                    <div class="modal-body">

                        <!-- HiddenField lưu mã thật để postback -->
                        <asp:HiddenField ID="hdfMaNhomQuyen" runat="server" />

                        <div class="mb-3">
                            <label class="form-label">Mã nhóm quyền</label>
                            <asp:TextBox ID="txtEditMa" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Tên nhóm quyền</label>
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




        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    </div>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            var deleteModal = document.getElementById('deleteModal');
            deleteModal.addEventListener('show.bs.modal', function (event) {
                var button = event.relatedTarget;
                var id = button.getAttribute('data-id');

                // Gán id vào hidden field của ASP.NET
                var hidden = document.getElementById('<%= hdDeleteId.ClientID %>');
                hidden.value = id;
            });
        });

        var editModal = document.getElementById('editModal');
        editModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget; // Button đã click
            var ma = button.getAttribute('data-ma');
            var ten = button.getAttribute('data-ten');

            // Gán vào các input ASP.NET
            document.getElementById('<%= txtEditMa.ClientID %>').value = ma;
      document.getElementById('<%= txtEditTen.ClientID %>').value = ten;
      document.getElementById('<%= hdfMaNhomQuyen.ClientID %>').value = ma;
  });
    </script>
</asp:Content>
