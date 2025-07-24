using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Diagnostics;
using System.Web;
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
                UserInformationLoad(Helper.CurrentUser?.UserId);
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

            DB_Management dbWrite = new DB_Management();
            string writeResult = dbWrite.DB_Write(strSQL);

            if (writeResult.Equals("Success"))
            {
                var updatedUser = new UserViewModel
                {
                    UserId = txtID.Text,
                    UserName = txtName.Text,
                    UserNumber = int.Parse(txtBirthDay.Text),
                    UserGender = int.Parse(txtGender.Text),
                    UserAddress = txtAddress.Text,
                    UserAddress2 = txtAddress2.Text,
                    UserNote = txtMemo.Text,
                    UserWClass = Helper.CurrentUser.UserWClass,
                    UserClass = Helper.CurrentUser.UserClass
                };

                Global.dbManager.SetCookie(txtID.Text, userPassword, txtName.Text, Helper.CurrentUser.UserWClass, Helper.CurrentUser.UserClass, 2);
                Session["UserInfo"] = updatedUser;

                Debug.WriteLine("세션 이름 업데이트 후: " + ((UserViewModel)Session["UserInfo"]).UserName);
                Debug.WriteLine("이전 위치로 이동한다. : " + prePage);
                Response.Redirect(prePage);
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