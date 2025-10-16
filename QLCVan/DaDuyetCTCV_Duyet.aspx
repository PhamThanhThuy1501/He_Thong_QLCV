<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/QLCV.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <!-- Bootstrap + Font Awesome -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" />

  <style>
    :root {
      --ink:#0f172a;
      --red:#c00000;
      --blue:#0d6efd;
      --line:#e5e7eb;
    }

    body { background:#fff; font-family:"Segoe UI", Arial, sans-serif; }

    /* ===== Tiêu đề ===== */
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

    /* ===== Thanh chạy chữ ===== */
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

    /* ===== Thẻ công văn ===== */
    .cv-wrap{max-width:980px;margin:24px auto 48px auto;}
    .cv-card{background:#fff;border:1px solid #e5e7eb;border-radius:10px;}
    .cv-head{position:relative;padding:18px 22px 0 22px}
    .cv-title{font-weight:700;text-align:center;margin:0 0 14px}
    .cv-badge{
      position:absolute;
      right:22px;top:14px;
      background:green;
      color:#fff;
      border-radius:20px;
      padding:8px 16px;
      font-weight:700;
    }
    .cv-body{padding:18px 22px 22px}
    .grid2{display:grid;grid-template-columns:1fr 1fr;gap:14px 24px}
    @media(max-width:768px){.grid2{grid-template-columns:1fr}}
    .field{display:flex;align-items:center;gap:12px}
    .field .lbl{width:160px;min-width:160px;color:#0f172a;font-weight:600;text-align:left}
    .field .ctl{flex:1}
    .row-full{grid-column:1/-1}
    .ipt,.sel,.ta{
      width:100%;box-sizing:border-box;height:38px;padding:6px 10px;border:1px solid #d1d5db;
      border-radius:6px;background:#fff;outline:none
    }
    .ta{min-height:92px;height:auto;resize:none;padding-top:8px;line-height:1.4}
    .ipt[readonly],.ta[readonly]{background:#f8fafc;color:#111827}

    /* ===== Select hiển thị mũi tên chuẩn kể cả khi disabled ===== */
    .select-wrap {
      position: relative;
    }
    .select-wrap::after {
      content: "";
      position: absolute;
      right: 14px;
      top: 50%;
      transform: translateY(-50%);
      width: 14px;
      height: 14px;
      pointer-events: none;
      background-image: url("data:image/svg+xml;utf8,\
        <svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='none' stroke='%236b7280' stroke-width='1.8' stroke-linecap='round' stroke-linejoin='round'>\
          <path d='M4 6l4 4 4-4'/>\
        </svg>");
      background-repeat: no-repeat;
      background-size: 14px 14px;
    }

    .sel {
      appearance: none;
      -webkit-appearance: none;
      -moz-appearance: none;
      background-color: #fff;
      border: 1px solid #d1d5db;
      border-radius: 6px;
      height: 38px;
      width: 100%;
      padding: 6px 40px 6px 10px;
      color: #111827;
    }
    .sel:disabled {
      background-color: #fff;
      opacity: 1;
      color: #111827;
      cursor: default;
    }

    /* ===== Nút hành động ===== */
    .actions {
      display: flex;
      justify-content: flex-end;
      gap: 10px;
      margin-top: 18px;
    }
    .btn {
      border: none;
      border-radius: 6px;
      padding: 0.45rem 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.2s ease;
    }
    .btn-ghost {
      border: 1px solid #cbd5e1;
      background: #f8fafc;
      color: #475569;
    }
    .btn-ghost:hover { background: #e2e8f0; }
    .btn-info {
      background: #0dcaf0;  /* xanh nhạt */
      color: #000;
      border: 1px solid #0bb5d8;
    }
    .btn-info:hover { background: #31d2f2; }
    .btn-primary {
      background: #0d6efd;  /* xanh đậm */
      color: #fff;
      border: 1px solid #0b5ed7;
    }
    .btn-primary:hover { background: #0b5ed7; }

  </style>

  <!-- ===== Tiêu đề + Thanh chạy chữ ===== -->
  <div class="content-header">
    <h2 class="content-header-title">NHẬP NỘI DUNG CÔNG VĂN</h2>
  </div>

  <div class="welcome-bar">
    <marquee behavior="scroll" direction="left" scrollamount="6">
      Chào mừng bạn đến với hệ thống Quản lý Công Văn điện tử.
    </marquee>
  </div>

  <!-- ===== Nội dung công văn ===== -->
  <div class="cv-wrap">
    <div class="cv-card">
      <div class="cv-head">
        <h5 class="cv-title">CHI TIẾT CÔNG VĂN</h5>
        <span class="cv-badge">Đã duyệt</span>
      </div>

      <div class="cv-body">
        <div class="grid2">
          <div class="row-full field">
            <div class="lbl">Tiêu đề:</div>
            <div class="ctl"><input class="ipt" type="text" value="Công văn chiến lược" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Số CV:</div>
            <div class="ctl"><input class="ipt" type="text" value="334-TB/QSQK2" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Loại công văn:</div>
            <div class="ctl select-wrap">
              <select class="sel" disabled>
                <option selected>Chiến lược</option>
                <option>Hành chính</option>
              </select>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Ngày ban hành:</div>
            <div class="ctl"><input class="ipt" type="text" value="10/09/2025" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Ngày gửi:</div>
            <div class="ctl"><input class="ipt" type="text" value="10/09/2025" readonly></div>
          </div>

          <div class="field">
            <div class="lbl">Cơ quan ban hành:</div>
            <div class="ctl select-wrap">
              <select class="sel" disabled>
                <option selected>Tiểu đoàn 2</option>
              </select>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Đơn vị nhận:</div>
            <div class="ctl select-wrap">
              <select class="sel" disabled>
                <option selected>Ban giám hiệu</option>
              </select>
            </div>
          </div>
        <div class="field">
          <div class="lbl">Người ký:</div>
          <div class="ctl">
            <select class="sel no-arrow" disabled>
              <option selected>Tiểu đoàn trưởng</option>
            </select>
          </div>
        </div>

        <div class="field">
            <div class="lbl">Bảo mật:</div>
            <div class="ctl radio-wrap">
              <label><input type="radio" checked disabled> Có</label>
              <label><input type="radio" disabled> Không</label>
            </div>
          </div>

          <div class="field">
            <div class="lbl">Người nhận:</div>
            <div class="ctl select-wrap">
              <select class="sel" disabled>
                <option selected>Dương Anh Tuấn</option>
              </select>
            </div>
          </div>

          

          <div class="row-full field">
            <div class="lbl">Trích yếu:</div>
            <div class="ctl">
              <textarea class="ta" readonly>1. Khoa thông báo kế hoạch đồ án đến các Bộ môn...</textarea>
            </div>
          </div>

          <div class="row-full field">
            <div class="lbl">Ghi chú:</div>
            <div class="ctl">
              <textarea class="ta" readonly>Không có ghi chú</textarea>
            </div>
          </div>

          <div class="row-full field">
            <div class="lbl">Tệp đính kèm:</div>
            <div class="ctl file-line">
              <i><a href="#" target="_blank">ISO-IT13.01Lap ke hoach do an tot nghiep (2016).doc</a></i>
              <span class="muted">38400 MB</span>
            </div>
          </div>
        </div>

        <!-- Nút hành động -->
        <div class="actions">
          <button type="button" class="btn btn-ghost">Quay lại</button>
          
          
        </div>
      </div>
    </div>
  </div>
</asp:Content>
