<%@ Page Title="Nhập nội dung công văn"
    Language="C#"
    MasterPageFile="~/QLCV.Master"
    AutoEventWireup="true"
    CodeBehind="ThemCVKoDuyet.aspx.cs"
    Inherits="QLCVan.ThemCVKoDuyet" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
/* ==== FORM LAYOUT TINH CHỈNH (thay thế block <style> cũ) ==== */
:root{
  --ink:#0f172a; --red:#c00000; --blue:#0d6efd;
  --line:#e5e7eb; --line-2:#dcdcdc; --bg:#f6f7fb;
}
.cv-wrap{background:var(--bg)}
/* ===== Phần tiêu đề + thanh chạy chữ ===== */
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
  background: #c00;                  /* nền đỏ đậm */
  color: #fff;
  border-radius: 4px;                /* bo góc mềm */
  padding: 8px 0;                    /* cao vừa để chữ nằm giữa */
  margin: 0 auto 26px auto;
  font-weight: bold;                 /* in đậm */
  text-align: center;
  display: flex;
  align-items: center;               /* căn giữa theo chiều cao */
  justify-content: center;
  height: 15px;                      /* chiều cao cố định để đều */
  overflow: hidden;                  /* ẩn phần chữ thừa */
}

.welcome-bar marquee {
  font-size: 16px;                   /* chữ lớn hơn chút */
  font-weight: bold;
  color: #fff;
                
}

/* header nhỏ + marquee giữ nguyên */

.form-container{
  margin:16px auto 28px;
  padding:24px;                     /* cân đều hai bên */
  max-width:920px;                  /* hẹp hơn 1 chút để giống figma */
  border-radius:12px;
  background:#fff;
  box-shadow:0 4px 12px rgba(0,0,0,.08);
  border:1px solid #eef0f2;
}
.form-title{
  text-align:center; font-weight:700; font-size:22px; margin:0 0 18px; color:#222;
}

/* Lưới 2 cột có gutter phải/trái rõ ràng */
.form-grid{
  display:grid;
  grid-template-columns:1fr 1fr;
  column-gap:28px;                  /* khoảng cách 2 cột */
  row-gap:14px;
}

/* Mỗi dòng: label + control căn giữa theo chiều dọc */
.form-field{display:flex; align-items:center; gap:14px;}
.form-field-full{grid-column:1 / -1;}
.form-field-full .form-field-inner{display:flex; align-items:center; gap:14px; width:100%;}

/* Label: cùng chiều cao với input để không lệch baseline */
.form-label{
  min-width:150px;                  /* đồng nhất độ rộng label */
  text-align:right;
  color:#555; font-weight:600;
  display:flex; align-items:center; /* căn giữa theo chiều dọc */
  height:40px;                      /* = chiều cao input */
  margin:0;
}

/* Controls: chiều cao chuẩn 40px */
.form-input-control{flex:1; display:flex; flex-direction:column; width:100%;}
.form-input,.form-select{
  height:40px;                      /* chuẩn */
  padding:8px 12px;
  border:1px solid var(--line-2);
  border-radius:8px; background:#f9f9f9;
  transition:border-color .15s, box-shadow .15s, background .15s;
}
.form-textarea{
  min-height:92px; resize:vertical;
  padding:10px 12px; border:1px solid var(--line-2);
  border-radius:8px; background:#f9f9f9;
}
.form-input:focus,.form-select:focus,.form-textarea:focus{
  outline:none;border-color:#0b57d0;box-shadow:0 0 0 2px rgba(11,87,208,.18);background:#fff;
}

/* Select mũi tên native, giữ padding phải để không đè chữ */
.form-select{appearance:none; background-image:none;}

/* Radio: căn hàng giữa, không trôi thấp */
.radio-list{display:flex; align-items:center; gap:18px; height:40px;}
.radio-list input[type="radio"]{margin-right:6px}

/* Upload + đính kèm */
.file-upload-row{display:flex; align-items:center; width:100%; gap:12px}
.file-upload-main{display:flex; align-items:center; gap:10px}
.file-upload-custom{display:flex; align-items:center; border:1px solid var(--line-2); border-radius:8px; background:#f9f9f9}
.file-upload-custom input[type="file"]{display:none}
.file-upload-custom label{background:#e8e8e8; color:#202124; border-right:1px solid var(--line-2);
  padding:8px 16px; border-radius:8px 0 0 8px; cursor:pointer; font-weight:600}
.file-upload-custom label:hover{background:#d9d9d9}
.file-chosen-text{padding:0 12px; color:#777}
.file-list-row{display:flex; align-items:flex-start; gap:12px}
.form-listbox{width:100%; height:110px}

/* Buttons */
.btn{display:inline-flex; align-items:center; justify-content:center; min-width:94px; height:36px; padding:0 16px;
  border-radius:8px; border:0; font-weight:700; cursor:pointer; transition:background .15s, transform .02s}
.btn:active{transform:translateY(1px)}
.btn-primary{background:#0b57d0; color:#fff}
.btn-primary:hover{background:#0949ae}
.btn-gray{background:#dadce0; color:#202124}
.btn-gray:hover{background:#c0c4c8}
.form-buttons{grid-column:1 / -1; display:flex; justify-content:flex-end; gap:10px; margin-top:12px}

.note-red{color:#d32f2f; font-size:12px; margin-top:5px}

/* Mobile */
@media (max-width:760px){
  .form-grid{grid-template-columns:1fr}
  .form-label{text-align:left; min-width:120px}
}
/* ===== Khối tệp đính kèm ===== */
.file-list-row{
  display:flex; align-items:center; gap:12px; width:100%;
}
.file-list-control{
  flex:1;                           /* giãn hết chiều ngang */
  display:flex; align-items:flex-start; gap:10px; width:100%;
}
.form-listbox{
  display:block;
  width:100% !important;            /* ép full chiều ngang */
  min-height:120px;                 /* cùng tỉ lệ figma */
  box-sizing:border-box;
  padding:6px 8px;
  border:1px solid #e5e7eb; border-radius:8px; background:#fff;
}
.file-list-control .btn{            /* nút Xóa bám cạnh phải, thẳng hàng trên */
  align-self:center;
  height:36px;
}

    </style>

    <!-- datepicker (giống NhapNDCV) -->
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
            $('#<%= txtNgayBanHanh.ClientID %>').datepicker({ changeMonth:true, changeYear:true, yearRange:'2000:2040' });
        $('#<%= txtNgayGui.ClientID %>').datepicker({ changeMonth: true, changeYear: true, yearRange: '2000:2040' });
      });
    </script>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="cv-wrap">
    <div class="content-header">
      <h2 class="content-header-title">NHẬP NỘI DUNG CÔNG VĂN</h2>
    </div>
    <div class="welcome-bar">
      <marquee behavior="scroll" direction="left" scrollamount="6">
        Chào mừng bạn đến với hệ thống Quản lý Công văn điện tử.
      </marquee>
    </div>

    <div class="form-container">
      <h3 class="form-title">THÊM MỚI CÔNG VĂN</h3>

      <div class="form-grid">
        <!-- Tiêu đề FULL -->
        <div class="form-field-full">
          <div class="form-field-inner">
            <asp:Label ID="lblTieuDe" runat="server" Text="Tiêu đề:" CssClass="form-label"></asp:Label>
            <div class="form-input-control">
              <asp:TextBox ID="txtTieuDe" runat="server" CssClass="form-input" placeholder="Nhập vào tiêu đề" />
              <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTieuDe" ErrorMessage="* Nhập tiêu đề" CssClass="note-red" />
            </div>
          </div>
        </div>

        <!-- Số CV / Loại CV -->
        <div class="form-field">
          <asp:Label ID="lblSoCV" runat="server" Text="Số CV:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:TextBox ID="txtSoCV" runat="server" CssClass="form-input" placeholder="Nhập vào số công văn" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSoCV" ErrorMessage="* Nhập số công văn" CssClass="note-red" />
          </div>
        </div>
        <div class="form-field">
          <asp:Label ID="lblLoaiCV" runat="server" Text="Loại công văn:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:DropDownList ID="ddlLoaiCV" runat="server" CssClass="form-select">
              <asp:ListItem Text="-- Chọn loại công văn --" Value="" />
            </asp:DropDownList>
            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlLoaiCV" InitialValue="" ErrorMessage="* Chọn loại công văn" CssClass="note-red" />
          </div>
        </div>

        <!-- Ngày ban hành / Ngày gửi -->
        <div class="form-field">
          <asp:Label ID="lblNgayBanHanh" runat="server" Text="Ngày ban hành:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:TextBox ID="txtNgayBanHanh" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNgayBanHanh" ErrorMessage="* Nhập ngày ban hành" CssClass="note-red" />
          </div>
        </div>
        <div class="form-field">
          <asp:Label ID="lblNgayGui" runat="server" Text="Ngày gửi:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:TextBox ID="txtNgayGui" runat="server" CssClass="form-input" placeholder="dd/mm/yyyy" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNgayGui" ErrorMessage="* Nhập ngày gửi" CssClass="note-red" />
          </div>
        </div>

        <!-- Cơ quan ban hành / Đơn vị nhận -->
        <div class="form-field">
          <asp:Label ID="lblCQBH" runat="server" Text="Cơ quan ban hành:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:TextBox ID="txtCQBH" runat="server" CssClass="form-input" placeholder="Nhập cơ quan ban hành" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCQBH" ErrorMessage="* Nhập cơ quan ban hành" CssClass="note-red" />
          </div>
        </div>
        <div class="form-field">
          <asp:Label ID="lblDonViNhan" runat="server" Text="Đơn vị nhận:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:DropDownList ID="ddlDonViNhan" runat="server" CssClass="form-select">
              <asp:ListItem Text="-- Chọn đơn vị nhận --" Value="" />
            </asp:DropDownList>
          </div>
        </div>

        <!-- Người ký / Bảo mật -->
        <div class="form-field">
          <asp:Label ID="lblNguoiKy" runat="server" Text="Người ký:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:TextBox ID="txtNguoiKy" runat="server" CssClass="form-input" placeholder="Nhập người ký" />
          </div>
        </div>
        <div class="form-field">
          <asp:Label ID="lblBaoMat" runat="server" Text="Bảo mật:" CssClass="form-label"></asp:Label>
          <div class="form-input-control">
            <asp:RadioButtonList ID="rblBaoMat" runat="server" RepeatDirection="Horizontal" CssClass="radio-list">
              <asp:ListItem Selected="True" Value="1">Có</asp:ListItem>
              <asp:ListItem Value="0">Không</asp:ListItem>
            </asp:RadioButtonList>
          </div>
        </div>

        <!-- Trích yếu (full) -->
        <div class="form-field-full">
          <div class="form-field-inner">
            <asp:Label ID="lblTrichYeu" runat="server" Text="Trích yếu:" CssClass="form-label"></asp:Label>
            <div class="form-input-control">
              <asp:TextBox ID="txtTrichYeu" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="4" placeholder="Nhập nội dung trích yếu"></asp:TextBox>
              <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTrichYeu" ErrorMessage="* Nhập trích yếu" CssClass="note-red" />
            </div>
          </div>
        </div>

        <!-- Ghi chú (full) -->
        <div class="form-field-full">
          <div class="form-field-inner">
            <asp:Label ID="lblGhiChu" runat="server" Text="Ghi chú:" CssClass="form-label"></asp:Label>
            <div class="form-input-control">
              <asp:TextBox ID="txtGhiChu" runat="server" CssClass="form-textarea" TextMode="MultiLine" Rows="4" placeholder="Nhập ghi chú"></asp:TextBox>
            </div>
          </div>
        </div>

        <!-- Upload -->
        <div class="form-field-full">
          <div class="file-upload-row">
            <asp:Label ID="lblFile" runat="server" Text="File (nếu có):" CssClass="form-label"></asp:Label>
            <div class="file-upload-main">
              <div class="file-upload-custom">
                <asp:FileUpload ID="FileUpload1" runat="server" />
                <label for="<%= FileUpload1.ClientID %>">Choose File</label>
                <span class="file-chosen-text">No file chosen</span>
              </div>
              <asp:Button ID="btnUpload" runat="server" CssClass="btn btn-primary" Text="Upload" OnClick="btnUpload_Click" CausesValidation="False" />
            </div>
          </div>
        </div>

        <!-- Tệp đính kèm (textbox ListBox + Xóa) -->
        <div class="form-field-full">
          <div class="file-list-row">
            <asp:Label ID="lblTapDinhKem" runat="server" Text="Tệp đính kèm:" CssClass="form-label"></asp:Label>
            <div class="file-list-control">
              <asp:ListBox ID="ListBox1" runat="server" CssClass="form-listbox" />
              <asp:Button ID="btnRemove" runat="server" CssClass="btn btn-primary" Text="Xóa" OnClick="btnRemove_Click" CausesValidation="False" />
            </div>
          </div>
          <asp:Label ID="lblLoi" runat="server" Text="" CssClass="note-red" Style="display:block; margin-left:135px; margin-top:6px;"></asp:Label>
        </div>

        <!-- Buttons -->
        <div class="form-buttons">
          <asp:Button ID="btnQuayLai" runat="server" CssClass="btn btn-gray" Text="Quay lại" OnClick="btnQuayLai_Click" CausesValidation="False" />
          <asp:Button ID="btnThem" runat="server" CssClass="btn btn-primary" Text="Thêm" OnClick="btnThem_Click" />
        </div>
      </div>
    </div>
  </div>
</asp:Content>
