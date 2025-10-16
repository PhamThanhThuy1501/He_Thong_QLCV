<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeFile="CTCV.aspx.cs" Inherits="QLCVan.SuaCV" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        :root{--ink:#0f172a;--red:#c00000;--blue:#0d6efd;--line:#e5e7eb}

.content-header{background:transparent;padding:0;border-bottom:none;margin:0 auto 6px}
.content-header-title{text-transform:uppercase;font-weight:700;font-size:20px;color:#444;margin:0 0 6px}

.welcome-bar{background:#c00;color:#fff;border-radius:4px;padding:8px 0;margin:0 auto 26px;font-weight:bold;text-align:center;display:flex;align-items:center;justify-content:center;height:15px;overflow:hidden}
.welcome-bar marquee{font-size:16px;font-weight:bold;color:#fff}

.form-container{margin:40px auto;padding:30px;max-width:900px;border-radius:12px;background:#fff;box-shadow:0 4px 12px rgba(0,0,0,.08)}
.form-title{text-align:center;font-weight:700;font-size:22px;margin-bottom:22px;color:#222}
.no-select {
    user-select: none;
    -webkit-user-select: none; /* Safari */
    -moz-user-select: none;    /* Firefox */
    -ms-user-select: none;     /* IE/Edge */
}

.bottom-action-bar {
    grid-column: 1 / -1;
    display: flex;
    justify-content: flex-end;  Đẩy các nút sang phải 
    align-items: center;
    gap: 12px;
    margin-top: 28px;
    padding-top: 14px;
    border-top: 1px solid #f0f0f0;
    width: 100%;  Đảm bảo chiếm toàn bộ chiều ngang 
}
        .btn-action{ padding:10px 20px; border-radius:6px; font-weight:700; text-decoration:none; border:none; cursor:pointer; display:inline-flex; align-items:center; }
        .btn-edit{ background:#0b5ed7; color:#fff; } .btn-edit:hover{ background:#084c9e; }
        .btn-back{ background:#6c757d; color:#fff; } .btn-back:hover{ background:#5a6268; }


/* Lưới 2 cột: 120px (label) + phần còn lại (input) */
.form-grid{display:grid;grid-template-columns:repeat(2,1fr);gap:20px 30px}
.form-field{display:grid;grid-template-columns:120px minmax(0,1fr);align-items:center;gap:15px}
.form-field-full-width{grid-column:1/-1}
.form-field-full-width .form-field-inner{display:grid;grid-template-columns:120px minmax(0,1fr);align-items:center;gap:15px}

/* Label luôn cùng bề rộng, căn phải trên desktop */
.form-label{font-weight:600;color:#555;text-align:right}

/* Ô nhập chiếm phần còn lại, không tràn */
.form-input-control{min-width:0}

/* Input/Select/Textarea đồng bộ chiều cao – không chồng chéo */
.form-input,.form-select,.form-textarea{
  width:100%;padding:12px 14px;border:1px solid #dcdcdc;border-radius:8px;
  box-sizing:border-box;background:#f9f9f9;transition:all .2s;line-height:1.4
}
.form-input:focus,.form-select:focus,.form-textarea:focus{
  outline:none;border-color:#0b57d0;box-shadow:0 0 0 2px rgba(11,87,208,.2);background:#fff
}
.form-textarea{resize:vertical;min-height:100px}

/* Select có mũi tên, chừa padding phải để không đè chữ */
.form-select{
  -webkit-appearance:none;-moz-appearance:none;appearance:none;
  background-image:url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="%23333" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/></svg>');
  background-repeat:no-repeat;background-position:right 12px center;padding-right:36px
}

/* Radio cùng hàng, không vỡ layout */
.radio-group{display:flex;align-items:center;gap:20px}
.radio-list td{padding:0;white-space:nowrap}
.radio-list label{margin-right:20px}

/* Hàng upload/file list cũng theo 2 cột 120px + 1fr */
.file-upload-row,.file-list-row{display:grid;grid-template-columns:120px minmax(0,1fr);align-items:center;gap:15px}
.file-upload-main-container{display:flex;align-items:center;gap:10px}
.file-upload-custom{display:flex;align-items:center;border:1px solid #dcdcdc;border-radius:8px;background:#f9f9f9;height:40px}
.file-input-hidden{position:absolute;left:-9999px}
.file-upload-label{display:inline-flex;align-items:center;height:40px;padding:0 16px;background:#e8e8e8;border-right:1px solid #dcdcdc;border-radius:8px 0 0 8px;font-weight:600;cursor:pointer}
.file-chosen-text{display:inline-flex;align-items:center;height:40px;padding:0 12px;color:#777}

.file-list-control-container{display:flex;gap:10px;align-items:flex-start}
.form-listbox{width:100%;height:100px}

/* Nút */
.btn{display:inline-block;padding:10px 24px;border-radius:8px;cursor:pointer;font-size:16px;font-weight:600;text-decoration:none;transition:all .2s;border:none}
.btn-primary{background-color:#0b57d0;color:#fff}
.btn-primary:hover{background:#0949ae}
.btn-quaylai{background:#dadce0;color:#202124}
.btn-quaylai:hover{background:#c0c4c8}

.form-buttons{grid-column:1/-1;display:flex;justify-content:flex-end;gap:10px;margin-top:25px}
.note-red{color:#d32f2f;font-size:12px;margin-top:5px}

/* Mobile: 1 cột, label căn trái để tránh chồng */
@media (max-width:768px){
  .form-grid{grid-template-columns:1fr}
  .form-field,.form-field-full-width .form-field-inner,
  .file-upload-row,.file-list-row{grid-template-columns:1fr}
  .form-label{text-align:left}
}

    </style>

    <!-- jQuery UI datepicker (giống NhậpNDCV) -->
    <script src="Scripts/datepicker/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Scripts/datepicker/jquery-ui.js" type="text/javascript"></script>
    <link href="Scripts/datepicker/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        jQuery(function ($) {
            $.datepicker.regional['vi'] = {
                closeText: 'Đóng', prevText: '&#x3c;Trước', nextText: 'Tiếp&#x3e;', currentText: 'Hôm nay',
                monthNames: ['Tháng Một', 'Tháng Hai', 'Tháng Ba', 'Tháng Tư', 'Tháng Năm', 'Tháng Sáu', 'Tháng Bảy', 'Tháng Tám', 'Tháng Chín', 'Tháng Mười', 'Th.Mười Một', 'Th.Mười Hai'],
                monthNamesShort: ['Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'],
                dayNames: ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'],
                dayNamesShort: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                dayNamesMin: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                weekHeader: 'Tu', dateFormat: 'dd/mm/yy', firstDay: 0, isRTL: false, showMonthAfterYear: false, yearSuffix: ''
            };
            $.datepicker.setDefaults($.datepicker.regional['vi']);
            $('#<%= txtngaybanhanh.ClientID %>').datepicker({ changeMonth: true, changeYear: true, yearRange: '2000:2040' });
            $('#<%= txtngaygui.ClientID %>').datepicker({ changeMonth: true, changeYear: true, yearRange: '2000:2040' });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="content-header">
        <h2 class="content-header-title">Xem Chi Tiết NỘI DUNG CÔNG VĂN</h2>
    <div class="welcome-bar">
        <marquee behavior="scroll" direction="left" scrollamount="6">
            Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
        </marquee>
    </div>

    <div class="form-container">
        <h3 class="form-title">Chi Tiết Công Văn</h3>

        <div class="form-grid">
            <!-- Tiêu đề -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label ID="lblTieuDe" runat="server" Text="Tiêu đề:" CssClass="form-label" ></asp:Label>
                    <div class="form-input-control">
                        <asp:TextBox ID="txttieude" CssClass="form-input" runat="server" placeholder="Nhập vào tiêu đề" ReadOnly="true" />
                        <asp:RequiredFieldValidator ID="rfvTieuDe" runat="server"
                            ControlToValidate="txttieude" ErrorMessage="* Nhập tiêu đề" CssClass="note-red" />
                    </div>
                </div>
            </div>

            <!-- Số CV -->
            <div class="form-field">
                <asp:Label ID="lblSoCV" runat="server" Text="Số CV:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtsocv" CssClass="form-input" runat="server" placeholder="Nhập vào số công văn" ReadOnly="true" />
                    <asp:RequiredFieldValidator ID="rfvSoCV" runat="server"
                        ControlToValidate="txtsocv" ErrorMessage="* Nhập số công văn" CssClass="note-red" />
                </div>
            </div>

            <!-- Loại CV -->
            <div class="form-field">
                <asp:Label ID="lblLoaiCV" runat="server" Text="Loại công văn:" CssClass="form-label" Enabled="false"></asp:Label>
                <div class="form-input-control">
                    <asp:DropDownList ID="ddlLoaiCV" runat="server" CssClass="form-select" Enabled="false">
                        <asp:ListItem Text="-- Chọn loại công văn  --" Value="" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvLoaiCV" runat="server"
                        ControlToValidate="ddlLoaiCV" InitialValue=""
                        ErrorMessage="* Chọn công văn" CssClass="note-red" />
                </div>
            </div>

            <!-- Ngày ban hành -->
            <div class="form-field">
                <asp:Label ID="lblNgayBanHanh" runat="server" Text="Ngày ban hành:" CssClass="form-label" Enabled="false"></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtngaybanhanh" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" />
                </div>
            </div>

            <!-- Ngày gửi -->
            <div class="form-field">
                <asp:Label ID="lblNgayGui" runat="server" Text="Ngày gửi:" CssClass="form-label" ></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtngaygui" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" ReadOnly="true" />
                </div>
            </div>

            <!-- Cơ quan ban hành -->
            <div class="form-field">
                <asp:Label ID="lblCoQuanBanHanh" runat="server" Text="Cơ quan ban hành:" CssClass="form-label" ></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtcqbh" CssClass="form-input" runat="server" placeholder="Chọn cơ quan ban hành" ReadOnly="true" />
                </div>
            </div>

            <!-- Đơn vị nhận -->
            <div class="form-field">
                <asp:Label ID="lblDonViNhan" runat="server" Text="Đơn vị nhận:" CssClass="form-label" ></asp:Label>
                <div class="form-input-control">
                    <asp:DropDownList ID="ddlDonViNhan" runat="server" CssClass="form-select" Enabled="false">
                        <asp:ListItem Text="-- Chọn đơn vị nhận --" Value="" />
                    </asp:DropDownList>
                </div>
            </div>

            <!-- Người ký -->
            <div class="form-field">
                <asp:Label ID="lblNguoiKy" runat="server" Text="Người ký:" CssClass="form-label" ></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtNguoiKy" CssClass="form-input" runat="server" placeholder="Nhập người ký" ReadOnly="true"/>
                </div>
            </div>

            <!-- Bảo mật -->
            <div class="form-field">
                <asp:Label ID="lblGuiNhan" runat="server" Text="Bảo mật:" CssClass="form-label" ></asp:Label>
                <div class="form-input-control">
                    <div class="radio-group">
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="radio-list" CellSpacing="20" Enabled="false">
                            <asp:ListItem Selected="True" Value="Gui">Có</asp:ListItem>
                            <asp:ListItem Value="Nhan">Không</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>

            <!-- Trích yếu -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label ID="lblTrichYeu" runat="server" Text="Trích yếu:" CssClass="form-label" ReadOnly="true"></asp:Label>
                    <div class="form-input-control">
                        <asp:TextBox ID="txttrichyeu" CssClass="form-textarea" runat="server" TextMode="MultiLine" Rows="4" placeholder="Nhập nội dung trích yếu" Enabled="false"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvTrichYeu" runat="server"
                            ControlToValidate="txttrichyeu" ErrorMessage="* Nhập trích yếu" CssClass="note-red" />
                    </div>
                </div>
            </div>

            <!-- Ghi chú -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label ID="lblGhiChu" runat="server" Text="Ghi chú:" CssClass="form-label" ReadOnly="true"></asp:Label>
                    <div class="form-input-control">
                        <asp:TextBox ID="txtGhiChu" CssClass="form-textarea" runat="server" TextMode="MultiLine" Rows="4" placeholder="Nhập ghi chú" Enabled="false"></asp:TextBox>
                    </div>
                </div>
            </div>


            <!-- Danh sách tệp -->
            <div class="form-field-full-width">
                <div class="file-list-row">
                    <asp:Label ID="lblTapDinhKem" runat="server" Text="Tệp đính kèm:" CssClass="form-label" ></asp:Label>
                    <div class="file-list-control-container">
                        <asp:ListBox ID="ListBox1" runat="server" CssClass="form-listbox" SelectionMode="Multiple" Enabled="false" ReadOnly="true" />
                        <asp:Button ID="btnDelete" runat="server"
                            CssClass="btn btn-primary" Text="Xóa"
                            OnClick="btnDelete_Click"
                            OnClientClick="return confirm('Xóa mục đã chọn (nếu không chọn sẽ xóa tất cả)?');"
                            CausesValidation="False" Style="display: none;" />

                    </div>
                </div>
            </div>
            
            <!-- Buttons -->
<div class="bottom-action-bar">
    <div class="button-group">
                        <a class="btn-action btn-back" href="Trangchu.aspx">Quay lại</a>
                        <a class="btn-action btn-edit" href='<%= "SuaCV.aspx?id=" + Request.QueryString["id"] %>'>Chỉnh sửa</a>
    </div>
</div>
        </div>
    </div>

</asp:Content>
