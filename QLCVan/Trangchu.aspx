<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="Trangchu.aspx.cs" Inherits="QLCVan.Trangchu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
 <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <style type="text/css">
        :root {
            --red: #c00;
            --red-600: #a60d0d;
            --ink: #222;
            --muted: #6b7280;
            --line: #e5e7eb;
            --bg: #f7f7f7;
            --white: #fff;
            --content-w: 1100px;
        }
        body { background: var(--bg); color: var(--ink); font-family: Arial, sans-serif }
        .cv { max-width: 1800px; margin: 0 auto; padding: 12px }

        .cv-box, .gridwrap, .cv-list-title {
            width: 100%; max-width: var(--content-w);
            margin-left: auto; margin-right: auto;
            box-sizing: border-box; padding: 16px 18px;
        }

        .cv-list-title { text-align: center; font-weight: 700; font-size: 20px; color: #0f172a; margin: 12px 0 8px; letter-spacing: .6px; }

        .content-header { background: transparent; padding: 0; border-bottom: none; margin: 0 auto 6px auto; }
        .content-header-title { text-transform: uppercase; font-weight: 700; font-size: 20px; color: #444; margin: 0 0 6px 0; letter-spacing: 0; }

        .welcome-bar { background: #c00; color: #fff; border-radius: 4px; padding: 8px 0; margin: 0 auto 26px auto;
            font-weight: bold; text-align: center; display: flex; align-items: center; justify-content: center; height: 30px; overflow: hidden; }
        .welcome-bar marquee { font-size: 16px; font-weight: bold; color: #fff; }

        .cv-box { background: #f3f4f6 !important; border: 1px solid #e5e7eb; border-radius: 10px; }
        .cv-box-title { font-weight: bold; font-size: 18px; color: #003366; text-align: center; margin-bottom: 16px; letter-spacing: 1px; }

        .cv-box .cv-form {
            display: grid !important; grid-template-columns: 1fr 1fr 1fr !important;
            grid-template-areas: "socv tieude loai" "from to actions";
            gap: 14px 24px !important;
        }
        .cv-box .cv-form > .field:nth-child(1) { grid-area: socv; }
        .cv-box .cv-form > .field:nth-child(2) { grid-area: tieude; }
        .cv-box .cv-form > .field:nth-child(3) { grid-area: loai; }
        .cv-box .cv-form > .field:nth-child(4) { grid-area: from; }
        .cv-box .cv-form > .field:nth-child(5) { grid-area: to; }
        .cv-box .cv-form > .field:nth-child(6) { grid-area: actions; }

        .cv-box .cv-form .field { display: flex !important; flex-direction: column !important; gap: 6px !important; }
        .cv-box .cv-form label { margin: 0 !important; font-weight: 200 !important; font-size: 13px !important; color: #1f2937 !important; }
        .cv-box .cv-form .input, .cv-box .cv-form .select {
            background: #fff !important; border: 1px solid #d1d5db !important; border-radius: 8px !important; padding: 8px 12px !important; min-height: 10px; font-size: 14px;
        }

        .cv-box .cv-form .btn, .cv-box .cv-form input.btn {
            background: var(--red) !important; color: #fff !important; border: none !important; border-radius: 6px !important;
            padding: 6px 12px !important; font-size: 13px !important; font-weight: 500 !important; justify-self: start; cursor: pointer; transition: all .2s ease-in-out;
        }
        .cv-box .cv-form .btn:hover, .cv-box .cv-form input.btn:hover { transform: scale(1.03); }

        .btn-outline { background:#fff !important; color:var(--red) !important; border:1px solid var(--red) !important; }
        .btn-gray { background:#fff !important; color:#111 !important; border:1px solid #d1d5db !important; }

   .cv-box .cv-form .actions {
    display: flex;
    justify-content: center; /* Căn giữa */
    align-items: center;
    gap: 10px;
    width: 100%; /* Đảm bảo chiếm toàn bộ chiều rộng */
}


        @media (max-width:700px) {
            .cv-box .cv-form {
                grid-template-columns: 1fr 1fr !important;
                grid-template-areas: "socv socv" "tieude loai" "from to" "actions actions";
            }
        }
        @media (max-width:500px) {
            .cv-box .cv-form {
                grid-template-columns: 1fr !important;
                grid-template-areas: "socv" "tieude" "loai" "from" "to" "actions";
            }
        }

        .gridview { width: 100%; border-collapse: collapse; font-family: Tahoma, sans-serif; font-size: 13px; table-layout: fixed; }
        .gridview th { background: var(--red); color: #fff; font-weight: bold; padding: 8px; border: 1px solid #ddd; text-align: left; }
        .gridview td { border: 1px solid #ddd; padding: 10px 12px; color: #000; vertical-align: middle; }
        .gridview tr:nth-child(even) { background: #f9f9f9 }
        .gridview a { color: #0066cc; text-decoration: none }
        .gridview a:hover { text-decoration: underline }

        .cell-trichyeu a { display: -webkit-box; -webkit-box-orient: vertical; -webkit-line-clamp: 2; overflow: hidden; line-height: 1.35; }

        .status-cell { text-align: center; white-space: nowrap; }
        .badge { display: inline-block; padding: 6px 12px; border-radius: 999px; font-size: 12px; font-weight: 700; line-height: 1; border: 1px solid transparent; }
        .badge--success { background:#22c55e; color:#fff; border-color:#22c55e; }
        .badge--danger  { background:#fff; color:#ef4444; border-color:#ef4444; }
        .badge--warning { background:#fff; color:#d97706; border-color:#f59e0b; }

        .actions-row { display: flex; gap: 8px; justify-content: center; align-items: center; white-space: nowrap; }
        .action-pill { display: inline-flex; align-items: center; justify-content: center; padding: 6px 10px; font-size: 12px; font-weight: 600; border-radius: 8px;
                       min-width: auto; text-decoration: none; border: 1px solid rgba(0,0,0,.06); box-shadow: 0 1px 2px rgba(0,0,0,.06); }
        .action-view { background:#28a745; color:#fff; }
        .action-edit { background:#ffc107; color:#111; }
        .action-del  { background:#dc3545; color:#fff; }

        .pager { text-align: center; padding: 10px; background: #f1f1f1 }
        .pager a, .pager span { display: inline-block; margin: 0 4px; padding: 4px 8px; border-radius: 3px; color: #0066cc; text-decoration: none; border: 1px solid transparent; }
        .pager a:hover { border: 1px solid var(--red); color: var(--red) }
        .pager span { border: 1px solid var(--red); background: var(--red); color: #fff; font-weight: bold }

        /* ===== PHƯƠNG ÁN 1: Style riêng cho nút trong .cv-actions ===== */
        .cv-actions{
          width:100%;
          max-width: var(--content-w);
          margin: 10px auto 16px auto;
          padding: 0 18px;
          box-sizing: border-box;
        }
        .cv-actions a.btn,
        .cv-actions a.btn:link,
        .cv-actions a.btn:visited{
          display: inline-flex !important;
          align-items: center !important;
          gap: 6px !important;
          text-decoration: none !important;
          background: #fff !important;
          border: 1px solid var(--red) !important;
          color: var(--red) !important;
          padding: 8px 14px !important;
          border-radius: 8px !important;
          font-size: 13px !important;
          font-weight: 600 !important;
          line-height: 1 !important;
        }
        .cv-actions a.btn:hover,
        .cv-actions a.btn:focus{
          background: var(--red) !important;
          color: #fff !important;
          outline: none !important;
          text-decoration: none !important;
        }
        /* Thanh top chung: nút + tiêu đề trên cùng một hàng */
.cv-topbar{
  position: relative;
  width: 100%;
  max-width: var(--content-w);
  margin: 10px auto 12px auto;
  padding: 12px 18px;
  box-sizing: border-box;
}

/* Nút đỏ chữ trắng */
.cv-topbar a.btn,
.cv-topbar a.btn:link,
.cv-topbar a.btn:visited{
  position: absolute;
  left: 18px;
  top: 50%;
  transform: translateY(-50%);
  display: inline-flex;
  align-items: center;
  gap: 6px;
  text-decoration: none !important;
  background: var(--red) !important;
  border: 1px solid var(--red) !important;
  color: #fff !important;
  padding: 8px 14px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 600;
  line-height: 1;
}

.cv-topbar a.btn:hover,
.cv-topbar a.btn:focus{
  background: #a60d0d !important; /* hơi đậm hơn */
  border-color: #a60d0d !important;
  color: #fff !important;
  text-decoration: none !important;
}

/* Tiêu đề căn giữa */
.cv-topbar-title{
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%,-50%);
  margin: 0;
  text-align: center;
  font-weight: 700;
  font-size: 20px;
  color: #0f172a;
  letter-spacing: .6px;
}

@media (max-width: 520px){
  .cv-topbar{ padding: 10px 12px; }
  .cv-topbar a.btn{ left: 12px; padding: 7px 12px; font-size: 12px; }
  .cv-topbar-title{ font-size: 18px; }
}
       /* ===== PHÂN TRANG MÀU & FONT GIỐNG 100% TRANG QLNGUOIDUNG ===== */
.gridview .pager {
    text-align: center;
    padding: 10px 0 0 0 !important;
    margin-top: 0 !important;
    background: #fff;
}

.gridview .pager table {
    margin: 0 auto !important;
    border-collapse: separate !important;
    border-spacing: 8px !important;
    border: none !important;
    padding: 0 !important;
}

.gridview .pager tr,
.gridview .pager td {
    border: none !important;
    padding: 0 !important;
    margin: 0 !important;
    background: transparent !important;
}

/* Nút số trang */
.gridview .pager a,
.gridview .pager span {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 25px;
    height: 25px;
    border: 1px solid #d1d5db;
    border-radius: 4px;
    background: #fff;
    color: #6b7280;          /* 👈 màu xám giống hệt QLNgườiDùng */
    font-size: 15px;
    font-weight: 400;        /* 👈 nhẹ hơn một chút cho đúng tông */
    text-decoration: none;
    margin: 0 2px;
    transition: all 0.2s ease-in-out;
}

/* Trang hiện tại (được chọn) */
.gridview .pager span {
    background: #fff;
    color: #6b7280;
    border: 1px solid #d1d5db;
}

/* Hover: chỉ sáng viền, không đổi màu chữ */
.gridview .pager a:hover {
    border-color: #9ca3af;
    background: #f9fafb;
    color: #6b7280;
}
/* ===== Hiệu ứng phân trang: hover đỏ + trang hiện tại đỏ ===== */
.gridview .pager a,
.gridview .pager span{
  display:inline-flex; align-items:center; justify-content:center;
  width:25px; height:35px;
  border:1px solid #d1d5db; border-radius:4px;
  background:#fff; color:#6b7280;
  font-size:15px; font-weight:500; text-decoration:none;
  transition:background-color .18s ease, color .18s ease, border-color .18s ease, transform .06s ease;
}

/* Trang hiện tại */
.gridview .pager span{
  background:#c00 !important;     /* đỏ chủ đạo */
  color:#fff !important;
  border-color:#c00 !important;
  box-shadow:0 1px 2px rgba(0,0,0,.06);
}

/* Hover số trang */
.gridview .pager a:hover{
  background:#c00 !important;
  color:#fff !important;
  border-color:#c00 !important;
  transform:translateY(-1px);
}

/* Active (nhấn giữ) */
.gridview .pager a:active{ transform:translateY(0); }

/* Hỗ trợ focus bằng bàn phím */
.gridview .pager a:focus-visible{
  outline:2px solid rgba(13,110,253,.35);
  outline-offset:2px;
}
/* ===== Tạo khoảng cách giữa bảng và phân trang mà KHÔNG co bảng ===== */

/* Không đụng vào width bảng; chỉ tăng khoảng trống phía trên pager */
.gridview .pager {
  background:#fff !important;
  padding:0 !important;               /* không thêm padding tổng */
}

.gridview .pager td{
  border:none !important;
  padding:16px 0 0 0 !important;      /* 👈 khoảng cách mong muốn (sửa số tùy ý) */
}

/* Giữ nguyên kích thước nội dung pager, tránh làm co bảng */
.gridview .pager table{
  margin:0 !important;                 /* bỏ margin-top gây cảm giác co bảng */
  width:auto !important;               /* chắc chắn không chiếm rộng bất thường */
}
/* Căn giữa pager của GridView */
.gridview .pager { 
  text-align: center !important;           /* td chứa pager */
}

.gridview .pager td {
  text-align: center !important;           /* đề phòng inline style của GridView */
}

.gridview .pager table {
  display: inline-table !important;        /* để chịu tác dụng text-align:center */
  margin-left: auto !important;
  margin-right: auto !important;           /* đảm bảo luôn giữa */
}
.btn-danger { background-color: #dc3545; }
.btn-danger:hover { background-color: #bb2d3b; }
.btn-secondary { background-color: #6c757d; }
.modal-title {
    font-weight: 540 !important;
    color: #222;
}

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-header">
        <h2 class="content-header-title">XEM CÔNG VĂN</h2>
    </div>

    <div class="welcome-bar">
        <marquee behavior="scroll" direction="left" scrollamount="6">
            Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
        </marquee>
    </div>

    <div class="cv-box">
        <div class="cv-box-title">TÌM KIẾM VĂN BẢN</div>
        <div class="cv-form">
            <div class="field">
                <label for="TextBox1">Số công văn:</label>
                <asp:TextBox ID="TextBox1" runat="server" CssClass="input" placeholder="Nhập số công văn" />
            </div>
            <div class="field">
                <label for="txtTieuDe">Tiêu đề:</label>
                <asp:TextBox ID="txtTieuDe" runat="server" CssClass="input" placeholder="Nhập tiêu đề" />
            </div>
            <div class="field">
                <label for="ddlLoai">Loại công văn:</label>
                <asp:DropDownList ID="ddlLoai" runat="server" CssClass="select">
                    <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                   
                </asp:DropDownList>
            </div>
            <div class="field">
                <label for="txtFromDate">Từ ngày:</label>
                <asp:TextBox ID="txtFromDate" runat="server" CssClass="input" TextMode="Date" placeholder="mm/dd/yyyy" />
            </div>
            <div class="field">
                <label for="txtToDate">Đến ngày:</label>
                <asp:TextBox ID="txtToDate" runat="server" CssClass="input" TextMode="Date" placeholder="mm/dd/yyyy" />
            </div>

            <!-- Hai nút ẩn + Tìm kiếm (giữ nguyên) -->
            <div class="field" style="display:flex;align-items:end;">
                <label></label>
                <div class="actions">
                    <asp:Button ID="btnViewAll"
                        runat="server"
                        Text="↺ Xem toàn bộ công văn"
                        CssClass="btn btn-outline"
                        OnClick="btnViewAll_Click" 
                        Style="display:none"/>
                    <asp:Button ID="btnMyOnly"
                        runat="server"
                        Text="↩ Xem công văn của tôi"
                        CssClass="btn btn-gray"
                        OnClick="btnMyOnly_Click" 
                        Style="display:none"/>
                    <asp:Button ID="Button1"
                        runat="server"
                        Text="Tìm kiếm"
                        CssClass="btn"
                        OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>
    </div>

<div class="cv-topbar">
    <asp:HyperLink ID="lnkGoAll"
        runat="server"
        NavigateUrl="~/AllCongVan.aspx"
        CssClass="btn"
        Text="↺ Xem toàn bộ công văn" />
    <div class="cv-topbar-title">DANH SÁCH CÔNG VĂN</div>
</div>


    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="gridwrap">
                <asp:GridView ID="GridView1" runat="server" CssClass="gridview" AutoGenerateColumns="False"
                    Width="100%" AllowPaging="True" PageSize="5"
                    OnPageIndexChanging="GridView1_PageIndexChanging1"
                    ShowFooter="False" GridLines="None">
                    <Columns>
                        <%-- Số công văn (190px) --%>
                        <asp:TemplateField SortExpression="SoCV" HeaderText="Số công văn">
                            <ItemTemplate>
                                <%# Eval("SoCV") %>
                            </ItemTemplate>
                            <HeaderStyle Width="190px" />
                            <ItemStyle Width="190px" />
                        </asp:TemplateField>

                        <%-- Tiêu đề (260px) - mới thêm --%>
                        <asp:TemplateField SortExpression="TieuDeCV" HeaderText="Tiêu đề">
                            <ItemTemplate>
                                <%# Eval("TieuDeCV") %>
                            </ItemTemplate>
                            <HeaderStyle Width="260px" />
                            <ItemStyle Width="260px" />
                        </asp:TemplateField>

                        <%-- Ngày gửi (110px) --%>
                        <asp:BoundField DataField="NgayGui" HeaderText="Ngày gửi" SortExpression="Ngaygui" DataFormatString="{0:dd/MM/yyyy}">
                            <HeaderStyle Width="110px" />
                            <ItemStyle Width="110px" />
                        </asp:BoundField>

                        <%-- Trích yếu: tự giãn + ellipsis 2 dòng --%>
                        <asp:TemplateField SortExpression="TrichYeuND" HeaderText="Trích yếu nội dung">
                            <ItemTemplate>
                                <%# Eval("TrichYeuND") %>
                            </ItemTemplate>
                            <ItemStyle CssClass="cell-trichyeu" />
                        </asp:TemplateField>

                        <%-- TRẠNG THÁI (120px – cố định) --%>
                        <asp:TemplateField HeaderText="Trạng thái">
                            <ItemTemplate>
                                <%# Eval("TrangThai") %>
                            </ItemTemplate>
                            <HeaderStyle Width="120px" />
                            <ItemStyle CssClass="status-cell" Width="120px" HorizontalAlign="Center" />
                        </asp:TemplateField>

                        <%-- Thao tác (200px) --%>
                        <asp:TemplateField HeaderText="Thao tác">
                            <ItemTemplate>
                                <div class="actions">
                                    <asp:LinkButton
                                        ID="lnk_Xem"
                                        runat="server"
                                        CssClass="action-pill action-view"
                                        Text="Xem"
                                        CommandName="ViewCV"
                                        CommandArgument='<%# Eval("MaCV") %>'
                                        OnCommand="lnk_Command" />
                                    <asp:LinkButton
                                        ID="lnk_Sua"
                                        runat="server"
                                        CssClass="action-pill action-edit"
                                        Text="Sửa"
                                        CommandName="EditCV"
                                        CommandArgument='<%# Eval("MaCV") %>'
                                        OnCommand="lnk_Command" />
 <asp:LinkButton ID="lnk_Xoa" runat="server"
    CssClass="action-pill action-del"
    Text="Xóa"
    CommandName="DeleteCV"
    CommandArgument='<%# Eval("MaCV") %>'
    OnCommand="lnk_Command"
    data-bs-toggle="modal"
    data-bs-target="#confirmDeleteModal"
    OnClientClick='<%# "setDeleteId(\"" + Eval("MaCV") + "\"); return false;" %>'>

</asp:LinkButton>

                                </div>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" />
                            <ItemStyle Width="200px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pager" />
                </asp:GridView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
        </Triggers>
    </asp:UpdatePanel>




<!-- Modal xác nhận xóa -->
<div class="modal fade" id="confirmDeleteModal" tabindex="-1" aria-labelledby="confirmDeleteLabel" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content border-danger">
      <div class="modal-header">
        <h5 class="modal-title" id="confirmDeleteLabel">Xác nhận xoá</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Đóng"></button>
      </div>
      <div class="modal-body">
        <p>Bạn có chắc muốn xoá công văn này không?</p>
        <asp:HiddenField ID="hdDeleteId" runat="server" />
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Huỷ</button>
        <asp:Button ID="btnConfirmDelete" runat="server" Text="Xoá" CssClass="btn btn-danger" OnClick="btnConfirmDelete_Click" />
      </div>
    </div>
  </div>
</div>
    <script>
        function setDeleteId(maCV) {
            document.getElementById('<%= hdDeleteId.ClientID %>').value = maCV;
            var modal = new bootstrap.Modal(document.getElementById('confirmDeleteModal'));
            modal.show();
        }
    </script>
    <script>
        // Khi modal xác nhận xóa bị đóng, loại bỏ lớp nền còn sót
        document.addEventListener('hidden.bs.modal', function (event) {
            document.querySelectorAll('.modal-backdrop').forEach(function (el) {
                el.remove();
            });
            document.body.classList.remove('modal-open');
            document.body.style.overflow = ''; // tránh cuộn bị khóa
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
