<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeFile="SuaCV.aspx.cs" Inherits="QLCVan.SuaCV" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
 <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <style type="text/css">
        :root { --ink:#0f172a; --red:#c00000; --blue:#0d6efd; --line:#e5e7eb; }
        .content-header{background:transparent;padding:0;border-bottom:none;margin:0 auto 6px}
        .content-header-title{text-transform:uppercase;font-weight:700;font-size:20px;color:#444;margin:0 0 6px}
        .welcome-bar{background:#c00;color:#fff;border-radius:4px;padding:8px 0;margin:0 auto 26px;font-weight:bold;text-align:center;display:flex;align-items:center;justify-content:center;height:15px;overflow:hidden}
        .welcome-bar marquee{font-size:16px;font-weight:bold;color:#fff}
        .form-container{margin:40px auto;padding:30px;max-width:900px;border-radius:12px;background:#fff;box-shadow:0 4px 12px rgba(0,0,0,.08)}
        .form-title{text-align:center;font-weight:700;font-size:24px;margin-bottom:25px;color:#222}
    


        .form-grid{display:grid;grid-template-columns:repeat(2,1fr);gap:20px 30px}
        .form-field{display:flex;align-items:center;gap:15px}
        .form-field-full-width{grid-column:1/-1}
        .form-field-full-width .form-field-inner{display:flex;align-items:center;width:100%;gap:15px}
        .form-label{font-weight:600;color:#555;flex-shrink:0;width:120px;text-align:right}
        .form-input-control{flex-grow:1;display:flex;flex-direction:column;width:100%}
        .form-input,.form-select,.form-textarea{width:100%;padding:12px 14px;border:1px solid #dcdcdc;border-radius:8px;box-sizing:border-box;background:#f9f9f9;transition:all .2s}
        .form-input:focus,.form-select:focus,.form-textarea:focus{outline:none;border-color:#0b57d0;box-shadow:0 0 0 2px rgba(11,87,208,.2);background:#fff}
        .form-textarea{resize:vertical;min-height:100px}
        .form-select{-webkit-appearance:none;-moz-appearance:none;appearance:none;background-image:url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="%23333" viewBox="0 0 16 16"><path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/></svg>');background-repeat:no-repeat;background-position:right 14px center}
        .radio-group{display:flex;align-items:center;gap:20px}
        .radio-list td{padding:0;white-space:nowrap}
        .radio-list label{margin-right:20px}
        .file-upload-row{display:flex;align-items:center;width:100%;gap:15px}
        .file-upload-main-container{display:flex;align-items:center;gap:10px}
        .file-upload-custom{display:flex;align-items:center;border:1px solid #dcdcdc;border-radius:8px;background:#f9f9f9}
        .file-upload-custom input[type="file"]{display:none}
        .file-upload-label{background:#e8e8e8;color:#202124;border-right:1px solid #dcdcdc;padding:8px 16px;border-radius:8px 0 0 8px;cursor:pointer;font-weight:600}
        .file-list-row{display:flex;align-items:flex-start;width:100%;gap:15px}
        .file-list-control-container{display:flex;flex-grow:1;align-items:flex-end;gap:10px}
 .form-static {
    background: #f9f9f9;
    border: 1px solid #dcdcdc;
    border-radius: 8px;
    padding: 8px;
    color: #555;
    font-size: 14px;
    display: flex;
    align-items: center;
    height: 23px;
    justify-content: flex-start;
}


        .btn{display:inline-block;padding:10px 24px;border-radius:8px;cursor:pointer;font-size:16px;font-weight:600;text-decoration:none;transition:all .2s;border:none}
        .btn-primary{background:#0b57d0;color:#fff}.btn-primary:hover{background:#0949ae}
        .btn-quaylai{background:#dadce0;color:#202124}.btn-quaylai:hover{background:#c0c4c8}
        .form-buttons{grid-column:1/-1;display:flex;justify-content:flex-end;gap:10px;margin-top:25px}
        .note-red{color:#d32f2f;font-size:12px;margin-top:5px}
        @media (max-width:768px){
            .form-grid{grid-template-columns:1fr}
            .form-field,.form-field-full-width .form-field-inner,.file-upload-row,.file-list-row{flex-direction:column;align-items:stretch}
            .form-label{text-align:left;width:auto}
        }
    </style>

    <!-- Datepicker -->
    <script src="Scripts/datepicker/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="Scripts/datepicker/jquery-ui.js" type="text/javascript"></script>
    <link href="Scripts/datepicker/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        jQuery(function ($) {
            $.datepicker.regional['vi'] = {
                closeText: 'Đóng', prevText: '&#x3c;Trước', nextText: 'Tiếp&#x3e;', currentText: 'Hôm nay',
                monthNames: ['Tháng Một', 'Tháng Hai', 'Tháng Ba', 'Tháng Tư', 'Tháng Năm', 'Tháng Sáu', 'Tháng Bảy', 'Tháng Tám', 'Tháng Chín', 'Tháng Mười', 'Th.Mười Một', 'Th.Mười Hai'],
                monthNamesShort: ['Th1', 'Th2', 'Th3', 'Th4', 'Th5', 'Th6', 'Th7', 'Th8', 'Th9', 'Th10', 'Th11', 'Th12'],
                dayNames: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
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
        <h2 class="content-header-title">SỬA NỘI DUNG CÔNG VĂN</h2>
    </div>

    <div class="welcome-bar">
        <marquee behavior="scroll" direction="left" scrollamount="6">
            Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
        </marquee>
    </div>

    <div class="form-container">
        <h3 class="form-title">CHỈNH SỬA CÔNG VĂN</h3>

        <div class="form-grid">
            <!-- Tiêu đề -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label ID="lblTieuDe" runat="server" Text="Tiêu đề:" CssClass="form-label"></asp:Label>
                    <div class="form-input-control">
                        <asp:TextBox ID="txttieude" CssClass="form-input" runat="server" placeholder="Nhập vào tiêu đề" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txttieude" ErrorMessage="* Nhập tiêu đề" CssClass="note-red" />
                    </div>
                </div>
            </div>

            <!-- Số CV -->
            <div class="form-field">
                <asp:Label ID="lblSoCV" runat="server" Text="Số CV:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtsocv" CssClass="form-input" runat="server" placeholder="Nhập vào số công văn" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtsocv" ErrorMessage="* Nhập số công văn" CssClass="note-red" />
                </div>
            </div>

            <!-- Loại công văn (readonly) -->
            <div class="form-field">
                <asp:Label ID="lblLoaiCV" runat="server" Text="Loại công văn:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <asp:DropDownList ID="ddlLoaiCV" runat="server" CssClass="form-select" Enabled="false">
                        <asp:ListItem Text="-- Chọn loại công văn  --" Value="" />
                    </asp:DropDownList>
                </div>
            </div>

            <!-- Ngày ban hành -->
            <div class="form-field">
                <asp:Label ID="lblNgayBanHanh" runat="server" Text="Ngày ban hành:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtngaybanhanh" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" />
                </div>
            </div>

            <!-- Ngày gửi -->
            <div class="form-field">
                <asp:Label ID="lblNgayGui" runat="server" Text="Ngày gửi:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtngaygui" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" />
                </div>
            </div>

            <!-- Cơ quan ban hành -->
            <div class="form-field">
                <asp:Label ID="lblCoQuanBanHanh" runat="server" Text="Cơ quan ban hành:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtcqbh" CssClass="form-input" runat="server" placeholder="Chọn cơ quan ban hành" />
                </div>
            </div>

<!-- Đơn vị nhận -->
<div class="form-field">
    <asp:Label runat="server" Text="Đơn vị nhận:" CssClass="form-label" />
    <div class="form-input-control">
        <asp:Label ID="lblDonViNhan" runat="server" CssClass="form-static" />
    </div>
</div>


            <!-- Người ký -->
            <div class="form-field">
                <asp:Label ID="lblNguoiKy" runat="server" Text="Người ký:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <asp:TextBox ID="txtNguoiKy" CssClass="form-input" runat="server" placeholder="Nhập người ký" />
                </div>
            </div>

            <!-- Bảo mật -->
            <div class="form-field">
                <asp:Label ID="lblBaoMat" runat="server" Text="Bảo mật:" CssClass="form-label"></asp:Label>
                <div class="form-input-control">
                    <div class="radio-group">
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal" CssClass="radio-list" CellSpacing="20">
                            <asp:ListItem Selected="True" Value="Có">Có</asp:ListItem>
                            <asp:ListItem Value="Không">Không</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>

            <!-- Trích yếu -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label ID="lblTrichYeu" runat="server" Text="Trích yếu:" CssClass="form-label"></asp:Label>
                    <div class="form-input-control">
                        <asp:TextBox ID="txttrichyeu" CssClass="form-textarea" runat="server" TextMode="MultiLine" Rows="4" placeholder="Nhập nội dung trích yếu"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txttrichyeu" ErrorMessage="* Nhập trích yếu" CssClass="note-red" />
                    </div>
                </div>
            </div>

            <!-- Ghi chú -->
            <div class="form-field-full-width">
                <div class="form-field-inner">
                    <asp:Label ID="lblGhiChu" runat="server" Text="Ghi chú:" CssClass="form-label"></asp:Label>
                    <div class="form-input-control">
                        <asp:TextBox ID="txtGhiChu" CssClass="form-textarea" runat="server" TextMode="MultiLine" Rows="4" placeholder="Nhập ghi chú"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Upload (giống NhapNDCV) -->
            <div class="form-field-full-width">
                <div class="file-upload-row">
                    <asp:Label ID="lblFile" runat="server" Text="File (nếu có):" CssClass="form-label"></asp:Label>
                    <div class="file-upload-main-container">
                        <div class="file-upload-custom">
                            <asp:FileUpload ID="FileUpload1" runat="server" />
                            <label for="<%= FileUpload1.ClientID %>" class="file-upload-label">Choose File</label>
                            <span class="file-chosen-text">No file chosen</span>
                        </div>
                        <asp:Button ID="Button1" runat="server"
                            CssClass="btn btn-primary"
                            Text="Upload"
                            OnClick="btnUp_Click"
                            CausesValidation="False" />
                    </div>
                </div>
            </div>

            <!-- Danh sách tệp (giống NhapNDCV) -->
            <div class="form-field-full-width">
                <div class="file-list-row">
                    <asp:Label ID="lblTapDinhKem" runat="server" Text="Tệp đính kèm:" CssClass="form-label"></asp:Label>
                    <div class="file-list-control-container">
                        <asp:ListBox ID="ListBox1" runat="server" Width="100%" Height="100px" CssClass="form-listbox" />
                        <asp:Button ID="btnRemove" runat="server"
                            CssClass="btn btn-primary"
                            Text="Xóa"
                            OnClick="btnRemove_Click"
                            CausesValidation="False" />
                    </div>
                </div>
                <asp:Label ID="lblloi" runat="server" Text="" CssClass="note-red" Style="display:block;margin-left:135px;margin-top:6px;"></asp:Label>
            </div>

            <!-- Buttons -->
            <div class="form-buttons">
                <asp:Button ID="btnBack" runat="server" CssClass="btn btn-quaylai" Text="Quay lại" OnClick="btnBack_Click" CausesValidation="False" />
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Lưu" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
      <!-- Toast container (fixed ở góc trên bên phải) -->
<div class="position-fixed top-0 end-0 p-3" style="z-index:1080">
  <div id="liveToast" class="toast align-items-center text-bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
    <div class="d-flex">
      <div id="toastBody" class="toast-body">Đã xoá thành công</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
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
