<%@ Page Title="Quản lý chức vụ" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="QLChucVu.aspx.cs" Inherits="QLCVan.QLChucVu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />
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
            overflow: hidden;
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
            background: #c00 !important;
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
                background: #a00 !important;
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

  /* ✅ Phân trang ra giữa, KHÔNG VIỀN */
  .grid-pager {
      display: flex;
      justify-content: center;
      /* 🔹 căn giữa ngang */
      align-items: center;
      gap: 10px;
      margin-top: 25px;
  }

  .grid-pager a,
  .grid-pager span {
      border: none;
      /* ❌ bỏ viền */
      background: none;
      /* ❌ bỏ nền trắng */
      padding: 6px 12px;
      border-radius: 4px;
      font-weight: 500;
      color: #111;
      text-decoration: none;
      transition: all 0.2s ease;
  }

  .grid-pager a:hover {
      color: #c00;
      /* 🔹 khi hover chuyển sang đỏ */
  }

  .grid-pager span {
      background: #c00;
      /* 🔹 trang hiện tại tô đỏ */
      color: #fff;
  }
        .btn-search {
            background: #0d6efd; /* xanh primary */
            color: #fff;
            border: none;
            padding: 8px 18px;
            font-weight: 600;
            border-radius: 6px;
            text-decoration: none;
            transition: background-color .2s ease, box-shadow .2s ease, transform .05s ease;
        }


        /* ===== Chỉ sửa bằng CSS (fix cho GridView) ===== */

        /* 1) Ép layout cố định cột */
        .table {
            table-layout: fixed !important;
            width: 100%;
        }

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
                white-space: nowrap; /* giữ 1 dòng */
                overflow: hidden; /* tránh đẩy nở bảng nếu cần */
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
                    background: #0d6efd;
                    color: #fff;
                    border-color: #0d6efd;
                }

                    .table tr td:nth-child(3) > *:first-child:hover {
                        background: #0b5ed7;
                    }

                /* Sửa (icon viền xám) */
                .table tr td:nth-child(3) > *:nth-child(2) {
                    width: 30px;
                    padding: 0;
                    background: #fff;
                    color: #0B57D0;
                    border-color: #d1d5db;
                }

                    .table tr td:nth-child(3) > *:nth-child(2):hover {
                        background: #f3f4f6;
                    }

                /* Xóa (icon viền đỏ) */
                .table tr td:nth-child(3) > *:last-child {
                    width: 30px;
                    padding: 0;
                    background: #fff;
                    color: #DC2626;
                    border-color: #d1d5db;
                }

                    .table tr td:nth-child(3) > *:last-child:hover {
                        background: #fee2e2;
                    }

            /* Header đỏ giữ nguyên nhưng thêm !important để thắng Bootstrap */
            .table tr th {
                background-color: #c00 !important;
                color: #fff !important;
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

        <h3 class="page-title">DANH SÁCH CHỨC VỤ</h3>

        <!-- ✅ Thanh tìm kiếm -->
        <div class="search-bar">
            <label>Tìm kiếm</label>
            <asp:TextBox ID="txtTenChucVuSR" runat="server" placeholder="Nhập tên chức vụ" />
            <asp:TextBox ID="txtMaChucVuSR" runat="server" placeholder="Nhập mã chức vụ" />
            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click">
                <i class="fa fa-search"></i>
            </asp:LinkButton>
            <button type="button"
                class="btn btn-primary"
                data-bs-toggle="modal"
                data-bs-target="#addModal">
                <i class="fa fa-plus"></i> Thêm chức vụ
            </button>

  <!-- Bảng danh sách -->
  <div class="table-wrapper">
    <asp:GridView ID="gvChucVu" runat="server" AutoGenerateColumns="False"
      CssClass="table table-bordered gridview"
      AllowPaging="True" PageSize="5"
      OnPageIndexChanging="gvChucVu_PageIndexChanging"
      PagerStyle-CssClass="pagination pagination-source"
      BorderStyle="None">

      <Columns>
        <asp:BoundField DataField="MaChucVu" HeaderText="Mã chức vụ" />
        <asp:BoundField DataField="TenChucVu" HeaderText="Tên chức vụ" />
        <asp:TemplateField HeaderText="Thao tác">
          <ItemTemplate>
            <a type="button" class="btn btn-primary btn-sm"
               href='<%# "GanNhomQuyen.aspx?ma=" + Eval("MaChucVu") + "&ten=" + Eval("TenChucVu") %>'>
               Gán quyền
            </a>

            <button type="button" class="btn btn-warning btn-sm"
                    data-bs-toggle="modal" data-bs-target="#editModal"
                    data-ma='<%# Eval("MaChucVu") %>'
                    data-ten="<%# Eval("TenChucVu") %>">
              <i class="fa fa-pen"></i>
            </button>

            <button type="button" class="btn btn-danger btn-sm"
                    data-bs-toggle="modal" data-bs-target="#deleteModal"
                    data-id='<%# Eval("MaChucVu") %>'>
              <i class="fa fa-trash"></i>
            </button>
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
    </asp:GridView>
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
                                href='<%# "GanNhomQuyen.aspx?ma=" + Eval("MaChucVu") + "&ten=" + Eval("TenChucVu") %>'>Gán Quyền
                            </a>

                            <!-- Nút sửa -->
                            <button type="button"
                                class="btn btn-warning btn-sm"
                                data-bs-toggle="modal"
                                data-bs-target="#editModal"
                                data-ma='<%# Eval("MaChucVu") %>'
                                data-ten="<%# Eval("TenChucVu") %>">
                                <i class="fa fa-pen"></i>
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

                </Columns>
            </asp:GridView>
        </div>

  <!-- Modal thêm -->
  <div class="modal fade" id="addModal" tabindex="-1" aria-labelledby="addModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="addModalLabel">Thêm mới chức vụ</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
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

  <!-- Modal xóa -->
  <div class="modal fade" id="deleteModal" tabindex="-1" aria-labelledby="deleteModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content border-danger">
        <div class="modal-header">
          <h5 class="modal-title" id="deleteModalLabel">Xác nhận xoá</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
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

  <!-- Modal sửa -->
  <div class="modal fade" id="editModal" tabindex="-1" aria-labelledby="editModalLabel" aria-hidden="true">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Sửa chức vụ</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
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
    <!-- Toast container (fixed ở góc trên bên phải) -->
    <div class="position-fixed top-0 end-0 p-3" style="z-index: 1080">
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

    // Clone phân trang giống QLNgườiDùng
    (function () {
      function clonePager() {
        var grid = document.getElementById('<%= gvChucVu.ClientID %>');
              if (!grid) return;
              var src = grid.querySelector('.pagination');
              var out = document.getElementById('pagerOutside');
              if (!src || !out) return;
              out.innerHTML = '';
              src.querySelectorAll('a, span').forEach(function (el) {
                  out.appendChild(el.cloneNode(true));
              });
              src.style.display = 'none';
          }
          if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', clonePager);
          else clonePager();
          if (typeof (Sys) !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager)
              Sys.WebForms.PageRequestManager.getInstance().add_endRequest(clonePager);
      })();
  </script>
</div>
</asp:Content>
