<%@ Page Title="" Language="C#" MasterPageFile="~/QLCV.Master" AutoEventWireup="true"
    CodeBehind="Trangchu.aspx.cs" Inherits="QLCVan.Trangchu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --red: #c00;
            --red-600: #a60d0d;
            --ink: #222;
            --muted: #6b7280;
            --line: #e5e7eb;
            --bg: #f7f7f7;
            --white: #fff;
            --content-w: 1100px; /* Đổi 1 chỗ này để tăng/giảm bề rộng Search + Bảng */
        }

        body {
            background: var(--bg);
            color: var(--ink);
            font-family: Arial, sans-serif
        }

        .cv {
            max-width: 1800px;
            margin: 0 auto;
            padding: 12px
        }

        /* ✅ Căn giữa phần danh sách công văn */
        .cv-list-title {
            width: 100% !important;
            max-width: var(--content-w) !important;
            margin: 16px auto 10px !important;
            padding: 8px 18px !important; /* trùng padding 2 bên với bảng */
            box-sizing: border-box !important;
            text-align: center !important;
        }

        .gridwrap {
            width: 100% !important;
            max-width: var(--content-w) !important;
            margin-left: auto !important;
            margin-right: auto !important;
            padding: 0 18px !important; /* trùng 18px hai bên để thẳng hàng */
            box-sizing: border-box !important;
            display: block !important; /* bỏ flex để bảng không lệch */
        }

        /* ✅ Đảm bảo bảng căn giữa trong khung */
        .gridview {
            margin: 0 auto;
        }

        /* ===== ĐỒNG BỘ CHIỀU RỘNG SEARCH + BẢNG + TIÊU ĐỀ ===== */
        .cv-box, .gridwrap, .cv-list-title {
            width: 100%;
            max-width: var(--content-w);
            margin-left: auto;
            margin-right: auto;
            box-sizing: border-box;
            padding: 16px 18px;
        }

        .cv-list-title {
            text-align: center;
            font-weight: 700;
            font-size: 20px;
            color: #0f172a;
            margin: 12px 0 8px;
            letter-spacing: .6px;
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
            height: 13px;
            overflow: hidden;
        }

            .welcome-bar marquee {
                font-size: 16px;
                font-weight: bold;
                color: #fff;
            }

        /* TÌM KIẾM */
        .cv-box {
            background: #f3f4f6 !important;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
        }

        .cv-box-title {
            font-weight: bold;
            font-size: 18px;
            color: #003366;
            text-align: center;
            margin-bottom: 16px;
            letter-spacing: 1px;
        }

        .cv-box .cv-form {
            display: grid !important;
            grid-template-columns: 1fr 1fr 1fr !important;
            grid-template-areas: "socv tieude loai" "from to actions";
            gap: 14px 24px !important;
        }

            .cv-box .cv-form > .field:nth-child(1) {
                grid-area: socv;
            }

            .cv-box .cv-form > .field:nth-child(2) {
                grid-area: tieude;
            }

            .cv-box .cv-form > .field:nth-child(3) {
                grid-area: loai;
            }

            .cv-box .cv-form > .field:nth-child(4) {
                grid-area: from;
            }

            .cv-box .cv-form > .field:nth-child(5) {
                grid-area: to;
            }

            .cv-box .cv-form > .field:nth-child(6) {
                grid-area: actions;
            }

            .cv-box .cv-form .field {
                display: flex !important;
                flex-direction: column !important;
                gap: 6px !important;
            }

            .cv-box .cv-form label {
                margin: 0 !important;
                font-weight: 200 !important;
                font-size: 13px !important;
                color: #1f2937 !important;
            }

            .cv-box .cv-form .input, .cv-box .cv-form .select {
                background: #fff !important;
                border: 1px solid #d1d5db !important;
                border-radius: 8px !important;
                padding: 8px 12px !important;
                min-height: 10px;
                font-size: 14px;
            }

            .cv-box .cv-form .btn, .cv-box .cv-form input.btn {
                background: var(--red) !important;
                color: #fff !important;
                border: none !important;
                border-radius: 6px !important;
                padding: 6px 12px !important;
                font-size: 13px !important;
                font-weight: 500 !important;
                justify-self: start;
                cursor: pointer;
                transition: all .2s ease-in-out;
            }

                .cv-box .cv-form .btn:hover, .cv-box .cv-form input.btn:hover {
                    transform: scale(1.03);
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

        .cv-note {
            color: #ef4444;
            font-size: 13px;
            margin: 6px 0
        }

        /* Bảng danh sách */
        .gridview {
            width: 100%;
            border-collapse: collapse;
            font-family: Tahoma, sans-serif;
            font-size: 13px;
            table-layout: fixed;
        }

            .gridview th {
                background: var(--red);
                color: #fff;
                font-weight: bold;
                padding: 8px;
                border: 1px solid #ddd;
                text-align: left;
            }

            .gridview td {
                border: 1px solid #ddd;
                padding: 10px 12px;
                color: #000;
                vertical-align: middle;
            }

            .gridview tr:nth-child(even) {
                background: #f9f9f9
            }

            .gridview a {
                color: #0066cc;
                text-decoration: none
            }

                .gridview a:hover {
                    text-decoration: underline
                }

        .cell-trichyeu a {
            display: -webkit-box;
            -webkit-box-orient: vertical;
            -webkit-line-clamp: 2;
            overflow: hidden;
            line-height: 1.35;
        }

        .status-cell {
            text-align: center;
            white-space: nowrap;
        }

        .badge {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 999px;
            font-size: 12px;
            font-weight: 700;
            line-height: 1;
            border: 1px solid transparent;
        }

        .badge--success {
            background: #22c55e;
            color: #fff;
            border-color: #22c55e;
        }

        .badge--danger {
            background: #fff;
            color: #ef4444;
            border-color: #ef4444;
        }

        .badge--warning {
            background: #fff;
            color: #d97706;
            border-color: #f59e0b;
        }

        .actions {
            display: flex;
            gap: 8px;
            justify-content: center;
            align-items: center;
            white-space: nowrap;
        }

        .action-pill {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            padding: 6px 10px;
            font-size: 12px;
            font-weight: 600;
            border-radius: 8px;
            min-width: auto;
            text-decoration: none;
            border: 1px solid rgba(0,0,0,.06);
            box-shadow: 0 1px 2px rgba(0,0,0,.06);
        }

        .action-view {
            background: #28a745;
            color: #fff;
        }

        .action-edit {
            background: #ffc107;
            color: #111;
        }

        .action-del {
            background: #dc3545;
            color: #fff;
        }

        .pager {
            text-align: center;
            padding: 10px;
            background: #f1f1f1
        }

            .pager a, .pager span {
                display: inline-block;
                margin: 0 4px;
                padding: 4px 8px;
                border-radius: 3px;
                color: #0066cc;
                text-decoration: none;
                border: 1px solid transparent;
            }

                .pager a:hover {
                    border: 1px solid var(--red);
                    color: var(--red)
                }

            .pager span {
                border: 1px solid var(--red);
                background: var(--red);
                color: #fff;
                font-weight: bold
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

    <!-- TÌM KIẾM VĂN BẢN -->
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
                <asp:DropDownList ID="ddlLoai" runat="server" CssClass="select"></asp:DropDownList>
            </div>
            <div class="field">
                <label for="txtFromDate">Từ ngày:</label>
                <asp:TextBox ID="txtFromDate" runat="server" CssClass="input" TextMode="Date" placeholder="mm/dd/yyyy" />
            </div>
            <div class="field">
                <label for="txtToDate">Đến ngày:</label>
                <asp:TextBox ID="txtToDate" runat="server" CssClass="input" TextMode="Date" placeholder="mm/dd/yyyy" />
            </div>
            <div class="field">
                <label></label>
                <asp:Button ID="Button1" runat="server" Text="Tìm kiếm" CssClass="btn" OnClick="btnSearch_Click" />
            </div>
        </div>
    </div>

    <div class="cv-list-title">DANH SÁCH CÔNG VĂN</div>

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
                                        OnClientClick="return confirm('Bạn có chắc chắn muốn xóa công văn này không?')" />
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
</asp:Content>
