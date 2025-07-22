using GiSanParkGolf.Class;
using System;
using System.Diagnostics;
using System.Web.UI;
using T_Engine;

namespace GiSanParkGolf.Sites.UserManagement
{
    public partial class User_Modify : System.Web.UI.Page
    {
        static string prePage;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                prePage = Request.ServerVariables["HTTP_REFERER"];
                Debug.WriteLine("이전 위치 : " + prePage);
                UserInformationLoad(Global.uvm.UserId);
            }               
        }

        protected void UserInformationLoad(string userID)
        {
            DB_Management userRepo = new DB_Management();
            Global.suvm = userRepo.GetSelectUserByUserID(userID);

            txtID.Text = Global.suvm.UserID;
            txtName.Text = Global.suvm.UserName;
            txtBirthDay.Text = Global.suvm.UserNumber.ToString();
            txtGender.Text = Global.suvm.UserGender.ToString();
            txtAddress.Text = Global.suvm.UserAddress;
            txtAddress2.Text = Global.suvm.UserAddress2;
            txtMemo.Text = Global.suvm.UserNote;

        }

        protected void BTN_Register_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("수정 버튼이 눌렸다.");
            if (Page.IsValid)
            {
                Debug.WriteLine(txtName.Text);
                //패스워드가 입력되었다면 암호화 하기.
                string newPassword = txtPassword.Text;

                if (!newPassword.Equals(string.Empty))
                {
                    Cryptography newCrypt = new Cryptography();
                    newPassword = newCrypt.GetEncoding("ParkGolf", txtPassword.Text);
                }

                UserModify(newPassword);
            }
        }

        private void UserModify(string userPassword)
        {
            //String.Format("{0:yyyy-MM-dd}", DateTime.Parse(TextBox1.Text))
            string strSQL = "UPDATE SYS_Users SET";
            strSQL += " UserName = '" + txtName.Text + "'";
            strSQL += ", UserNumber = '" + txtBirthDay.Text + "'";
            strSQL += ", UserGender = '" + txtGender.Text + "'";
            strSQL += ", UserAddress = '" + txtAddress.Text + "'";
            strSQL += ", UserAddress2 = '" + txtAddress2.Text + "'";
            strSQL += ", UserNote = '" + txtMemo.Text + "'";

            if (!userPassword.Equals(string.Empty))
            {
                strSQL += ", UserPassword = '" + userPassword + "'";
            }

            strSQL += " WHERE UserId = '" + txtID.Text + "'";
            strSQL += ";";

            Debug.WriteLine(strSQL);

            DB_Management dbWrite = new DB_Management();
            string writeResult = dbWrite.DB_Write(strSQL);

            if (writeResult.Equals("Success"))
            {
                //모달을 닫을때 사용(근데 자동으로 닫히는데?)
                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "CloseModal();", true);
                Global.uvm.UserName = txtName.Text;
                Debug.WriteLine("이전 위치로 이동한다. : " + prePage);
                Response.Redirect(prePage);
                //string strJs = "<script>alert('수정 되었습니다.'); location.href='/Default';</script>";
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "goDefault", strJs);
            }
            else
            {
                string strAlarm = @"<script language='JavaScript'>window.alert('";
                strAlarm += writeResult;
                strAlarm += "');</script>";
                Response.Write(strAlarm);
            }
        }
    }
}