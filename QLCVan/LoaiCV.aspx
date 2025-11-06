<%@ Page Title="Quản lý Loại Công văn" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="LoaiCV.aspx.cs" Inherits="QLCVan.LoaiCV" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <style>
        /* ===== Theme đồng bộ QL Chức vụ (CSS-only) ===== */
        :root {
            --red: #c00; /* dải đỏ & header bảng */
            --red-dark: #a00; /* hover */
            --primary: #0d6efd; /* nút thêm / xanh bootstrap */
            --ink: #1f2937;
            --line: #e5e7eb;
        }

        /* Base */
        html, body {
            background: #fff;
            color: #111;
            font-family: "Segoe UI", Arial, sans-serif;
        }

        * {
            box-sizing: border-box;
        }

        /* ===== Header + ribbon (giữ marquee, chỉ style giống QL Chức vụ) ===== */
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

        /* Dải đỏ như QL Chức vụ */
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
            height: 30px;
            overflow: hidden;
        }

            .welcome-bar marquee {
                font-size: 16px;
                font-weight: bold;
                color: #fff;
            }

        /* ===== Tiêu đề trang ===== */
        .page-title {
            font-size: 20px;
            font-weight: bold;
            text-align: center;
            color: #111;
            margin: 25px 0 20px 0;
            text-transform: uppercase;
            font-weight: 400;
        }

        /* ===== Thanh tìm kiếm & nút thêm (match QL Chức vụ) ===== */
        .action-bar-container {
            width: 70%;
            margin: 0 auto 25px auto;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 30px;
        }

        /* bóc phần bên trái (nhãn + 2 ô) thành hàng ngang */
        .search-container {
            display: flex;
            align-items: center;
            gap: 14px;
        }

            .search-container .search-label {
                font-weight: 600;
                color: #111;
            }

            .search-container .search-input {
                border: 1px solid #ccc;
                border-radius: 4px;
                padding: 8px 10px;
                height: 34px;
                width: 280px;
                font-size: 14px;
            }

                .search-container .search-input:focus {
                    border-color: #b7c6ff;
                    box-shadow: 0 0 0 2px rgba(13,110,253,.12);
                    outline: none;
                }

        /* nút kính lúp đỏ vuông 36px */
        .btn-search {
            background: var(--red) !important;
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
                background: var(--red-dark) !important;
            }

        /* nút Thêm giống QL Chức vụ */
        .btn-add {
            background: var(--primary);
            color: #fff;
            padding: 8px 14px;
            font-size: 14px;
            font-weight: 600;
            border: none;
            border-radius: 6px;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: background-color .2s ease, box-shadow .2s ease, transform .05s ease;
        }

            .btn-add:hover {
                filter: brightness(.97);
            }

        /* ===== Bảng danh sách (đỏ header, width 70%) ===== */
        .table {
            width: 70%;
            margin: 0 auto;
            border-collapse: collapse;
            background: #fff;
            table-layout: fixed;
        }

            .table th, .table td {
                border: 1px solid #ddd;
                padding: 8px 10px;
                text-align: center;
                font-size: 14px;
            }

            .table tr th {
                background-color: var(--red) !important;
                color: #fff !important;
                font-weight: 600;
                text-transform: uppercase;
                border-bottom: 2px solid #900;
            }

            .table tr:nth-child(even) {
                background: #fafafa;
            }

            /* Cột: giống bố cục QL Chức vụ */
            .table th:nth-child(1), .table td:nth-child(1) {
                width: 20% !important;
                text-align: center;
            }

            .table th:nth-child(2), .table td:nth-child(2) {
                width: auto !important;
                text-align: center;
                padding-left: 14px;
            }

            .table th:nth-child(3), .table td:nth-child(3) {
                width: 18% !important;
                text-align: center;
            }

            .table th:nth-child(4), .table td:nth-child(4) {
                width: 22% !important;
                text-align: center;
                white-space: nowrap;
                overflow: hidden;
            }

        /* Nút icon thao tác giống tone QL Chức vụ */
        .icon-btn {
            width: 30px;
            height: 30px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            background: #fff;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            margin: 0 6px;
            line-height: 1;
            transition: background .15s;
        }

            .icon-btn:hover {
                background: #f3f4f6;
            }

            .icon-btn i {
                font-size: 12px;
            }

        /* ===== Pager ngoài (giống grid-pager của QL Chức vụ) ===== */
        .pagination-source {
            display: none !important;
        }
        /* ẩn pager trong GridView (nguồn để clone) */

        .pager-out {
            width: 70%;
            margin: 25px auto 0 auto;
            text-align: center;
        }

            .pager-out a, .pager-out span {
                border: none;
                background: none;
                padding: 6px 12px;
                border-radius: 4px;
                font-weight: 500;
                color: #111;
                text-decoration: none;
                transition: all .2s ease;
                display: inline-flex;
                align-items: center;
                justify-content: center;
            }

                .pager-out a:hover {
                    color: var(--red);
                }

            .pager-out span {
                background: var(--red);
                color: #fff;
            }

        /* ===== Modal (giữ nguyên HTML, đồng bộ bo góc/khung) ===== */
        .modalBackground {
            background: rgba(0,0,0,.7);
            position: fixed;
            inset: 0;
            z-index: 10000;
        }

        .modalPopup {
            width: 520px;
            max-width: 92vw;
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 14px 40px rgba(0,0,0,.28);
            overflow: hidden;
            padding: 0;
            animation: fadeInScale .22s ease-out;
            position: fixed !important; /* cố định trên màn hình */
            top: 5% !important; /* điều chỉnh khoảng cách từ trên xuống, có thể đổi 8–12% */
            left: 50% !important;
            transform: translateX(-50%) !important; /* canh giữa theo chiều ngang */
            margin: 0 !important;
        }

        .modal-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 16px 20px 10px 20px;
        }

        .modal-title {
            font-size: 18px;
            font-weight: 500;
            color: #222;
            width: 100%;
            text-align: left;
            position: relative;
            padding-bottom: 10px;
        }

            .modal-title::after {
                content: "";
                display: block;
                height: 2px;
                width: 100%;
                background: var(--line);
                border-radius: 1px;
                position: absolute;
                left: 0;
                bottom: 0;
            }

        .modal-close {
            border: none;
            background: transparent;
            cursor: pointer;
            font-size: 22px;
            line-height: 1;
            color: #6b7280;
            margin-left: 12px;
        }

            .modal-close:hover {
                color: #111;
            }

        .modal-body {
            padding: 18px 20px 6px 20px;
        }

            .modal-body .form-control {
                width: 100%;
                height: 44px;
                border: 1px solid #D0D5DD;
                border-radius: 8px;
                padding: 10px 12px;
                font-size: 14px;
                color: #111;
                outline: none;
                margin-bottom: 12px;
            }

                .modal-body .form-control:focus {
                    border-color: #89b4ff;
                    box-shadow: 0 0 0 2px rgba(13,110,253,.12);
                }

        .form-group-radio {
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 12px;
            padding: 6px 0;
        }

            .form-group-radio .radio-label {
                font-size: 14px;
                font-weight: 600;
                color: #333;
            }

        .modal-footer {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            padding: 12px 20px 20px 20px;
        }

            .modal-footer .btn {
                padding: 9px 16px;
                border: none;
                border-radius: 8px;
                font-weight: 700;
                cursor: pointer;
                transition: filter .15s;
            }

            .modal-footer .btn-success {
                background: #22c55e;
                color: #fff;
            }

                .modal-footer .btn-success:hover {
                    filter: brightness(.95);
                }

            .modal-footer .btn-secondary {
                background: #6b7280;
                color: #fff;
            }

                .modal-footer .btn-secondary:hover {
                    filter: brightness(.95);
                }

            .modal-footer .btn-danger {
                background: #dc3545;
                color: #fff;
            }

                .modal-footer .btn-danger:hover {
                    filter: brightness(.95);
                }
        /* Chỉnh lại màu nút "Cập nhật" trong modal */
        .btn-success {
            background-color: #198754 !important; /* màu xanh Bootstrap */
            border-color: #198754 !important;
            color: #fff !important;
        }

            .btn-success:hover {
                background-color: #157347 !important; /* xanh đậm hơn khi hover */
                border-color: #146c43 !important;
            }


        @keyframes fadeInScale {
            from {
                opacity: 0;
                transform: scale(.96)
            }

            to {
                opacity: 1;
                transform: scale(1)
            }
        }
        /* Bỏ gạch chân dưới icon của nút tìm kiếm & nút thêm */
        .btn-search,
        .btn-search i {
            text-decoration: none !important;
            outline: none;
        }
        /* Bỏ gạch chân cho icon sửa và các link thao tác */
        .icon-btn,
        .icon-btn i,
        .icon-btn:link,
        .icon-btn:visited,
        .icon-btn:hover,
        .icon-btn:focus,
        .icon-btn:active {
            text-decoration: none !important;
            outline: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hfEditID" runat="server" />
    <asp:HiddenField ID="hfDeleteID" runat="server" />

    <ajaxToolkit:ModalPopupExtender ID="mpeEdit" runat="server" TargetControlID="hfEditID" PopupControlID="pnlEditPopup" BackgroundCssClass="modalBackground" CancelControlID="btnCancelEdit" DropShadow="true" />
    <ajaxToolkit:ModalPopupExtender ID="mpeDelete" runat="server" TargetControlID="hfDeleteID" PopupControlID="pnlDeletePopup" BackgroundCssClass="modalBackground" CancelControlID="btnCancelDelete" DropShadow="true" />

    <div class="content-header">
        <h2 class="content-header-title">QUẢN LÝ LOẠI CÔNG VĂN</h2>
    </div>

    <div class="welcome-bar">
        <marquee behavior="scroll" direction="left" scrollamount="6">
            Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
        </marquee>
    </div>

    <h3 class="page-title"><b>DANH SÁCH LOẠI CÔNG VĂN</b></h3>

    <div class="action-bar-container">
        <div class="search-container">
            <asp:Label runat="server" Text="Tìm kiếm" CssClass="search-label"></asp:Label>
            <asp:TextBox ID="txtSearchMaLoai" runat="server" CssClass="search-input" placeholder="Nhập mã loại công văn"></asp:TextBox>
            <asp:TextBox ID="txtSearchTenLoai" runat="server" CssClass="search-input" placeholder="Nhập tên loại công văn"></asp:TextBox>
            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn-search" OnClick="btnSearch_Click" ToolTip="Tìm kiếm">
                <i class="fa fa-search"></i>
            </asp:LinkButton>
        </div>
        <div>
            <asp:LinkButton ID="btnOpenAdd" runat="server" CssClass="btn-add"
                OnClientClick="openAddModal(); return false;" CausesValidation="false">
                <i class="fa fa-plus"></i> Thêm loại công văn
            </asp:LinkButton>
        </div>
    </div>

    <asp:Button ID="btnShowPopupTarget" runat="server" Style="display: none" />
    <ajaxToolkit:ModalPopupExtender ID="mpeAdd" runat="server" TargetControlID="btnShowPopupTarget" PopupControlID="pnlPopup" BackgroundCssClass="modalBackground" CancelControlID="btnHuy" DropShadow="true" />

    <center>
        <asp:GridView ID="grvLoaiCV" runat="server" ShowFooter="False" AutoGenerateColumns="False"
            CssClass="table" DataKeyNames="MaLoaiCV"
            AllowPaging="True" PageSize="5"
            OnPageIndexChanging="grvLoaiCV_PageIndexChanging"
            OnRowCommand="grvLoaiCV_RowCommand"
            PagerStyle-CssClass="pagination pagination-source">
            <Columns>
                <asp:TemplateField HeaderText="Mã loại công văn">
                    <ItemTemplate>
                        <asp:Label ID="lblMaLoai" runat="server" Text='<%# Eval("MaLoaiCV") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Tên loại công văn">
                    <ItemTemplate>
                        <asp:Label ID="lblTenLoai" runat="server" Text='<%# Eval("TenLoaiCV") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Phê duyệt">
                    <ItemTemplate>
                        <asp:Label ID="lblPheDuyet" runat="server"
                            Text='<%# Eval("PheDuyet") != null && Eval("PheDuyet").ToString() == "1" ? "Có" : "Không" %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Thao tác">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="icon-btn" CommandName="ShowEditPopup" CommandArgument='<%# Eval("MaLoaiCV") %>' ToolTip="Sửa" CausesValidation="False">
                            <i class="fa fa-pen" style="color:#0d6efd;"></i>
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="icon-btn" CommandName="ShowDeletePopup" CommandArgument='<%# Eval("MaLoaiCV") %>' ToolTip="Xóa" CausesValidation="False">
                            <i class="fa fa-trash" style="color:#dc3545;"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </center>

    <!-- Phân trang bên ngoài bảng -->
    <div id="pagerOutside" class="pager-out"></div>

    <!-- Popup Thêm -->
    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none;">
        <div class="modal-header">
            <div class="modal-title">Thêm mới loại công văn</div>
            <button type="button" class="modal-close" onclick="closeAddModal()" aria-label="Đóng">×</button>
        </div>
        <div class="modal-body">
            <asp:TextBox ID="txtMaLoaiCV" runat="server" CssClass="form-control" placeholder="Nhập mã loại công văn" />
            <asp:TextBox ID="txtTenLoaiCV" runat="server" CssClass="form-control" placeholder="Nhập tên loại công văn" />
            <div class="form-group-radio">
                <span class="radio-label">Phê duyệt:</span>
                <asp:RadioButton ID="rbPheDuyetCo" runat="server" Text="Có" GroupName="pheduyet_add" Checked="true" />
                <asp:RadioButton ID="rbPheDuyetKhong" runat="server" Text="Không" GroupName="pheduyet_add" />
            </div>
        </div>
        <div class="modal-footer">
            <asp:Button ID="btnLuu" runat="server" Text="Thêm" CssClass="btn btn-success" OnClick="btnAdd_Click" UseSubmitBehavior="true" />
            <asp:Button ID="btnHuy" runat="server" Text="Đóng" CssClass="btn btn-secondary" />
        </div>
    </asp:Panel>

    <!-- Popup Sửa -->
    <asp:Panel ID="pnlEditPopup" runat="server" CssClass="modalPopup" Style="display: none;">
        <div class="modal-header">
            <div class="modal-title">Chỉnh sửa loại công văn</div>
            <asp:LinkButton ID="lnkCloseEdit" runat="server" CssClass="modal-close" OnClientClick="$find('mpeEdit').hide(); return false;">×</asp:LinkButton>
        </div>
        <div class="modal-body">
            <asp:TextBox ID="txtEditMaLoai" runat="server" CssClass="form-control" Enabled="false" />
            <asp:TextBox ID="txtEditTenLoaiCV" runat="server" CssClass="form-control" />
            <div class="form-group-radio">
                <span class="radio-label">Phê duyệt:</span>
                <asp:RadioButton ID="rbEditPheDuyetCo" runat="server" Text="Có" GroupName="pheduyet_edit" />
                <asp:RadioButton ID="rbEditPheDuyetKhong" runat="server" Text="Không" GroupName="pheduyet_edit" />
            </div>
        </div>
        <div class="modal-footer">
            <asp:Button ID="btnUpdate" runat="server" Text="Cập nhật" CssClass="btn btn-success" OnClick="btnUpdate_Click" />
            <asp:Button ID="btnCancelEdit" runat="server" Text="Đóng" CssClass="btn btn-secondary" />
        </div>
    </asp:Panel>

    <!-- Popup Xóa -->
    <asp:Panel ID="pnlDeletePopup" runat="server" CssClass="modalPopup" Style="display: none;">
        <div class="modal-header">
            <div class="modal-title">Xác nhận xóa loại công văn</div>
            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="modal-close" OnClientClick="$find('mpeDelete').hide(); return false;">×</asp:LinkButton>
        </div>
        <div class="modal-body modal-body-delete">
            Bạn có chắc chắn muốn xóa loại công văn này không?
        </div>
        <div class="modal-footer">
            <asp:Button ID="btnCancelDelete" runat="server" Text="Hủy" CssClass="btn btn-secondary" />
            <asp:Button ID="btnConfirmDelete" runat="server" Text="Xóa" CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" />
        </div>
    </asp:Panel>

    <script type="text/javascript">
        function openAddModal() {
            var mpe = $find('<%= mpeAdd.ClientID %>');
            if (mpe) { mpe.show(); }
        }
        function closeAddModal() {
            var mpe = $find('<%= mpeAdd.ClientID %>');
            if (mpe) { mpe.hide(); }
        }

        // Clone pager mặc định của GridView ra ngoài bảng
        (function () {
            function clonePager() {
                // tìm pager nằm trong GridView
                var grid = document.getElementById('<%= grvLoaiCV.ClientID %>');
                if (!grid) return;
                var src = grid.querySelector('.pagination'); // pager mặc định
                var out = document.getElementById('pagerOutside');
                if (!src || !out) return;

                out.innerHTML = '';
                // clone tất cả a, span (trang hiện tại render là <span>)
                var items = src.querySelectorAll('a, span');
                items.forEach(function (el) {
                    out.appendChild(el.cloneNode(true));
                });

                // Ẩn nguồn (phòng trường hợp CSS chưa áp)
                src.style.display = 'none';
            }

            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', clonePager);
            } else {
                clonePager();
            }

            // Nếu có partial postback (MS AJAX), sau khi cập nhật lại grid thì clone lại pager
            if (typeof (Sys) !== 'undefined' &&
                Sys.WebForms && Sys.WebForms.PageRequestManager) {
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(clonePager);
            }
        })();
    </script>
    <!-- Toast thông báo -->
    <div class="position-fixed top-0 end-0 p-3" style="z-index: 11000">
        <div id="liveToast" class="toast align-items-center text-bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div id="toastBody" class="toast-body">Thao tác thành công</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    </div>

    <script>
        // Hàm hiển thị Toast (chuẩn màu xanh lá - vàng - đỏ)
        function showToast(message, type) {
            var toastEl = document.getElementById('liveToast');
            var toastBody = document.getElementById('toastBody');
            toastBody.textContent = message || 'Thao tác thành công!';

            // reset class nền
            toastEl.classList.remove('text-bg-success', 'text-bg-warning', 'text-bg-danger');

            switch (type) {
                case 'success':
                    toastEl.classList.add('text-bg-success'); // xanh lá
                    break;
                case 'warning':
                    toastEl.classList.add('text-bg-warning'); // vàng
                    break;
                case 'error':
                    toastEl.classList.add('text-bg-danger');  // đỏ
                    break;
                default:
                    toastEl.classList.add('text-bg-success');
                    break;
            }

            var toast = new bootstrap.Toast(toastEl, { delay: 2000 });
            toast.show();
        }
    </script>
    <!-- Container -->
    <div id="toastContainer" class="position-fixed top-0 end-0 p-3" style="z-index: 1100"></div>

    <script>
        function showToast(message, type) {
            var container = document.getElementById('toastContainer');
            var id = 't' + Date.now();
            var bg = (type === 'success') ? 'text-bg-success'
                : (type === 'error') ? 'text-bg-danger'
                    : 'text-bg-secondary';

            container.insertAdjacentHTML('beforeend',
                '<div id="' + id + '" class="toast align-items-center ' + bg + ' border-0" role="alert" aria-live="assertive" aria-atomic="true">' +
                '<div class="d-flex">' +
                '<div class="toast-body">' + message + '</div>' +
                '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>' +
                '</div>' +
                '</div>'
            );
            var toast = new bootstrap.Toast(document.getElementById(id), { delay: 2000 });
            toast.show();
        }
    </script>
</asp:Content>
