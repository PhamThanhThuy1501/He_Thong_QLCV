<%@ Page Title="Gán Quyền" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="GanQuyen.aspx.cs" Inherits="QLCVan.GanQuyen" EnableEventValidation="false" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <!-- Bootstrap 5 CSS -->
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

        <!-- Font Awesome (nếu có icon) -->
        <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
       <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
        <style>
             .toaster-wrap { position: fixed; top: 1rem; right: 1rem; z-index: 1080; }
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
                background: #c00;
                /* nền đỏ */
                color: #fff;
                border-radius: 4px;
                /* bo góc */
                padding: 6px 10px;
                /* khoảng cách trong */
                margin: 0 auto 20px auto;
                /* cách dưới */
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
                justify-content: center;
                /* căn giữa ngang */
                gap: 30px;
                /* 🔹 tăng khoảng cách giữa các phần tử */
                margin: 0 auto 25px auto;
                /* cách dưới thêm một chút */
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
                /* 🔹 tăng độ rộng ô nhập */
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
                text-decoration: none !important;
                /* xoá gạch chân */
                outline: none !important;
                /* bỏ viền khi focus */
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
                background-color: #c00;
                /* nền đỏ đậm */
                color: #fff;
                /* chữ trắng */
                font-weight: 600;
                text-transform: uppercase;
                border-bottom: 2px solid #900;
                /* viền đỏ đậm phía dưới */
            }



           /* ===== PHÂN TRANG NGOÀI BẢNG – GIỐNG QLNGUOIDUNG ===== */
.pager-out {
  width: 70%;
  margin: 22px auto 0 auto;
  text-align: center;
}

.pager-out a,
.pager-out span {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 42px;
  height: 42px;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  background: #fff;
  color: #4b5563;
  font-size: 16px;
  font-weight: 500;
  text-decoration: none;
  margin: 0 12px;
}

.pager-out span {
  background: #fff;
  color: #4b5563;
  border: 1px solid #d1d5db;
}

.pager-out a:hover { border-color: #d1d5db; }
/* Bỏ gạch dưới cho LinkButton & giữ không có viền/đổ bóng */
.search-bar a,
.search-bar a:link,
.search-bar a:visited,
.search-bar a:hover,
.search-bar a:focus {
  text-decoration: none !important;
  box-shadow: none !important;
  outline: none !important;
}

/* Nút tìm kiếm đỏ */
.btn-search {
  background: #c00 !important;   /* đỏ */
  color: #fff !important;
  border: none !important;
  padding: 8px 18px;
  font-weight: 600;
  border-radius: 6px;
  cursor: pointer;
  display: inline-flex;           /* để icon căn giữa */
  align-items: center;
  justify-content: center;
}

/* Hover/Focus màu đỏ đậm hơn */
.btn-search:hover,
.btn-search:focus {
  background: #a00 !important;
  color: #fff !important;
  text-decoration: none !important;  /* phòng trường hợp browser thêm underline */
}

/* (tuỳ chọn) đảm bảo icon trắng */
.btn-search i { color: #fff; }
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

/* ===== PHÂN TRANG NGOÀI BẢNG – GIỐNG QLNHOMQUYEN ===== */
.pager-out {
  width:70%;
 margin:22px auto 600px auto;
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
  background:#C62828; color:#fff; border-color:#C62828;
  box-shadow:0 1px 2px rgba(0,0,0,.06);
}
.pager-out a:hover {
  background:#C62828; color:#fff; border-color:#C62828;
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
                <h2 class="content-header-title">QUẢN LÝ NGƯỜI DÙNG</h2>
            </div>

            <div class="welcome-bar">
                <marquee behavior="scroll" direction="left" scrollamount="6">
                    Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
                </marquee>
            </div>

            <h3 class="page-title">
                GÁN QUYỀN CHO NHÓM QUYỀN
                <asp:Label ID="lblTenNhom" runat="server"></asp:Label>
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

         <div class="table-wrapper">
  <asp:GridView ID="gvGanQuyen" runat="server" AutoGenerateColumns="False"
      CssClass="table table-bordered gridview"
      AllowPaging="True" PageSize="5"
      OnPageIndexChanging="gvGanQuyen_PageIndexChanging"
      PagerStyle-CssClass="pagination pagination-source"
      BorderStyle="None">
      <Columns>
        <asp:BoundField DataField="MaQuyen" HeaderText="Mã quyền" />
        <asp:BoundField DataField="TenQuyen" HeaderText="Tên quyền" />
        <asp:TemplateField HeaderText="Thao tác">
          <ItemTemplate>
            <asp:Button ID="btnGan" runat="server"
                CommandName="ToggleQuyen"
                CommandArgument='<%# Eval("MaQuyen") %>'
                Text='<%# (bool)Eval("DaGan") ? "Đã gán" : "Gán" %>'
                CssClass='<%# (bool)Eval("DaGan") ? "btn btn-outline-primary" : "btn btn-primary" %>' />
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
  </asp:GridView>
</div>

<!-- ✅ Phân trang ngoài bảng -->
<div id="pagerOutside" class="pager-out"></div>


        <script>
    // type: 'success' | 'error' | 'info' | 'warning'
    window.showToast = function (message, type) {
        var bg =
            type === 'error' ? 'bg-danger' :
                type === 'warning' ? 'bg-warning text-dark' :
                    type === 'info' ? 'bg-info text-dark' :
                        'bg-success';

        var id = 't' + Date.now();
        var html =
            '<div id="' + id + '" class="toast align-items-center text-white ' + bg + ' border-0 mb-2" role="alert" aria-live="assertive" aria-atomic="true">' +
            '<div class="d-flex">' +
            '<div class="toast-body">' + message + '</div>' +
            '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>' +
            '</div>' +
            '</div>';

        var wrap = document.getElementById('toaster');
        wrap.insertAdjacentHTML('beforeend', html);

        var el = document.getElementById(id);
        var t = new bootstrap.Toast(el, { delay: 2200 });
        t.show();
        el.addEventListener('hidden.bs.toast', function () { el.remove(); });
            };
           
          
                (function () {
                    function clonePager() {
                        var grid = document.getElementById('<%= gvGanQuyen.ClientID %>');
                        if (!grid) return;

                        // Tìm pager gốc trong GridView
                        var src = grid.querySelector('.pagination') || grid.querySelector('.grid-pager') || grid.querySelector('tr td > table');
                        var out = document.getElementById('pagerOutside');
                        if (!src || !out) return;

                        out.innerHTML = '';
                        src.querySelectorAll('a, span').forEach(el => out.appendChild(el.cloneNode(true)));
                        src.style.display = 'none'; // ẩn pager gốc trong bảng
                    }

  if (document.readyState === 'loading')
                document.addEventListener('DOMContentLoaded', clonePager);
                else
                clonePager();

                if (typeof(Sys) !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager)
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(clonePager);
})();
    


        </script>
    </asp:Content>