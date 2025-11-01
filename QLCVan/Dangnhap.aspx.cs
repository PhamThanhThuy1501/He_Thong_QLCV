using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QLCVan
{
    public partial class Dangnhap : System.Web.UI.Page
    {
        InfoDataContext db = new InfoDataContext();
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {

        }
        // mã hóa
        //private string encryptpass(string pass)
        //{
        //    System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
        //    string hashade = System.Convert.ToBase64String(sha.ComputeHash(System.Text.UnicodeEncoding.Unicode.GetBytes(pass)));
        //    return hashade.Length > 49 ? hashade.Substring(0, 49) : hashade;
        //}

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            tblNguoiDung acc = db.tblNguoiDungs.SingleOrDefault(a => a.TenDN == Username.Text && a.MatKhau == (Password.Text));
            if (acc != null)
            {
                Session["MaNguoiDung"] = acc.MaNguoiDung;
                Session["TenDN"] = Username.Text;
                Session["Matkhau"] = (Password.Text);
                Session["QuyenHan"] = acc.QuyenHan.ToString();
                PermissionHelper.ReSyncPermission();

                // Thêm quyền tạm cho test
                var list = Session["ListQuyen"] as List<string> ?? new List<string>();
                if (!list.Contains("Q003")) list.Add("Q003");
                Session["ListQuyen"] = list;

                /// Lấy danh sách quyền 
                var listQuyen = (
                    from nd in db.tblNguoiDungs
                    join cv in db.tblChucVus on nd.MaChucVu equals cv.MaChucVu
                    join cvnq in db.tblChucVu_tblNhomQuyens on cv.MaChucVu equals cvnq.MaChucVu
                    join nq in db.tblNhomQuyens on cvnq.MaNhomQuyen equals nq.MaNhomQuyen
                    join nqq in db.tblNhomQuyen_tblQuyens on nq.MaNhomQuyen equals nqq.MaNhomQuyen
                    join q in db.tblQuyens on nqq.MaQuyen equals q.MaQuyen
                    where nd.MaNguoiDung == acc.MaNguoiDung
                    select q.MaQuyen

                    ).Distinct().ToList();

                /// nạp danh sách quyền của user vào list 
                Session["ListQuyen"] = listQuyen;
                Response.Redirect("Trangchu.aspx");


            }
            else
            {
                lbThongbaoLoi.Text = "Tài khoản hoặc mật khẩu không đúng! Yêu cầu nhập lại!";
            }
        }
    }
}