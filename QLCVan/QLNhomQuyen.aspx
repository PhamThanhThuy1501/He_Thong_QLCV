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
            height: 30px; /* chiều cao cố định để đều */
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

/* ✅ Bỏ gạch chân dưới icon tìm kiếm */
.btn-search,
.btn-search i {
  text-decoration: none !important;   /* xoá gạch chân */
  outline: none !important;           /* bỏ viền khi focus */
}

/* Giữ nguyên màu và hiệu ứng như cũ */
.btn-search {
  background: #C62828;        
  color: #fff;
  border: none;
  height: 36px;
  width: 36px;
  cursor: pointer;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  transition: background-color .25s ease, transform .1s ease;
}

.btn-search:hover {
  background: #BB0000;           /* đỏ khi hover */

}

/* Nếu dùng <asp:LinkButton> thì thêm rule này để chắc chắn không bị gạch chân */
.btn-search:link,
.btn-search:visited,
.btn-search:active,
.btn-search:hover {
  text-decoration: none !important;
  color: #fff !important;     /* luôn trắng */
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
            /* ===== Chỉ sửa bằng CSS (fix cho GridView) ===== */

/* 1) Ép layout cố định cột */
.table { table-layout: fixed !important; width: 100%; }

/* 2) Hai cột ngoài bằng nhau; dùng tr thay vì thead/tbody để chắc chắn match GridView */
/*.table tr th, .table tr td { box-sizing: border-box; }*/

/* Cột 1: Mã chức vụ */
.table tr th:nth-child(1),
.table tr td:nth-child(1) {
  width: 20% !important;
  text-align: center;
}

/* Cột 2: Chức vụ (ăn phần còn lại, dài nhất) */
.table tr th:nth-child(2),
.table tr td:nth-child(2) {
  width: auto !important;
  text-align: center;
  padding-left: 14px;
}

/* Cột 3: Thao tác */
.table tr th:nth-child(3),
.table tr td:nth-child(3) {
  width: 27% !important;
  text-align: center;
  white-space: nowrap;      /* giữ 1 dòng */
  overflow: hidden;         /* tránh đẩy nở bảng nếu cần */
}

/* 3) Nút trong cột Thao tác: cách đều & kích thước ổn định */
.table tr td:nth-child(3) > a,
.table tr td:nth-child(3) > button,
.table tr td:nth-child(3) > span > a,
.table tr td:nth-child(3) > span > button {
  display: inline-flex !important;
  align-items: center;
  justify-content: center;
  height: 30px;
  margin: 0 6px;
  border-radius: 6px;
  text-decoration: none;
  font-weight: 600;
  border: 1px solid transparent;
  line-height: 1;
}

/* Gán quyền (nút chữ xanh) */
.table tr td:nth-child(3) > *:first-child {
  min-width: 88px;
  padding: 0 12px;
  background: #0d6efd; color: #fff; border-color: #0d6efd;
}
.table tr td:nth-child(3) > *:first-child:hover { background:#0b5ed7; }

/* Sửa (icon viền xám) */
.table tr td:nth-child(3) > *:nth-child(2) {
  width: 30px; padding: 0;
  background:#fff; color:#0B57D0; border-color:#d1d5db;
}
.table tr td:nth-child(3) > *:nth-child(2):hover { background:#f3f4f6; }

/* Xóa (icon viền đỏ) */
.table tr td:nth-child(3) > *:last-child {
  width: 30px; padding: 0;
  background:#fff; color:#DC2626; border-color:#d1d5db;
}
.table tr td:nth-child(3) > *:last-child:hover { background:#fee2e2; }

/* Header đỏ giữ nguyên nhưng thêm !important để thắng Bootstrap */
.table tr th { background-color:#c00 !important; color:#fff !important; }
    </style>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page">
        <div class="content-header">
            <h2 class="content-header-title">QUẢN LÝ NHÓM QUYỀN</h2>
        </div>

        <div class="welcome-bar">
            <marquee behavior="scroll" direction="left" scrollamount="6">
                Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
            </marquee>
        </div>

        <h3 class="page-title">DANH SÁCH NHÓM QUYỀN</h3>

        <!-- ✅ Thanh tìm kiếm bên trái -->
        <div class="search-bar">
            <label>Tìm kiếm</label>
            <asp:TextBox ID="txtTenQuyenSR" runat="server" placeholder="Nhập tên quyền" />
            <asp:TextBox ID="txtMaQuyenSR" runat="server" placeholder="Nhập mã quyền" />
            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click">
        <i class="fa fa-search"></i>
            </asp:LinkButton>
            <button type="button"
    class="btn btn-primary"
    data-bs-toggle="modal"
    data-bs-target="#addModal">
    <i class="fa fa-plus"></i> Thêm nhóm quyền
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
                                <a type="button" class="btn btn-primary btn-sm"
                                    href="<%# "GanQuyen.aspx?ma=" +Eval("MaNhomQuyen")+"&ten=" + Eval("TenNhomQuyen") %>"
                                    >
                                    Gán Quyền
                                </a>
                            <button
                                type="button"
                                class="btn btn-warning btn-sm"
                               
                                data-bs-toggle="modal"
                                data-bs-target="#editModal"
                                data-ma='<%# Eval("MaNhomQuyen") %>'
                                data-ten="<%# Eval("TenNhomQuyen") %>">
                                 <i class="fa fa-pen"></i>
                            </button>
                            <button
                                type="button"
                                class="btn btn-danger btn-sm"
                                aria-hidden="true"                            
                                data-bs-toggle="modal"
                                data-bs-target="#deleteModal"
                                data-id='<%#Eval("MaNhomQuyen") %>'>
                                <i class="fa fa-trash"></i>
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
        <!-- Toast container (fixed ở góc trên bên phải) -->
<div class="position-fixed top-0 end-0 p-3" style="z-index:1080">
  <div id="liveToast" class="toast align-items-center text-bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
    <div class="d-flex">
      <div id="toastBody" class="toast-body">Đã xoá thành công</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
  </div>
</div>

<script>
    // Hàm hiển thị toast với message + màu
    function showToast(message, bsBgClass) {
        var toastEl = document.getElementById('liveToast');
        var bodyEl = document.getElementById('toastBody');

        // đổi nội dung + màu nền (success / danger / info ...)
        bodyEl.textContent = message || 'Thành công';
        toastEl.classList.remove('text-bg-success', 'text-bg-danger', 'text-bg-info', 'text-bg-warning');
        toastEl.classList.add(bsBgClass || 'text-bg-success');

        var toast = new bootstrap.Toast(toastEl, { delay: 2000 });
        toast.show();
    }
</script>
</asp:Content>
