using GiSanParkGolf.Class;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using T_Engine;

namespace GiSanParkGolf.Sites.Admin
{
    public partial class Player_Information : System.Web.UI.Page
    {
        private string selectID = Global.uvm.SelectID;
        protected void Page_Load(object sender, EventArgs e)
        {
            //string strJs = @"<script language='JavaScript'>window.alert('" + Global.uvm.SelectID + "');</script>";
            //Response.Write(strJs);
            UserInformationLoad(Global.uvm.SelectID);
        }
        protected void UserInformationLoad(string userID)
        {
            DB_Management userRepo = new DB_Management();
            string userInfo = userRepo.GetSelectUserByUserID2(userID);
            string[] user = userInfo.Split('!');

            txtID.Text = userID;
            txtName.Text = user[1];
            txtBirthDay.Text = user[3];
            txtGender.Text = user[4];
            txtAddress.Text = user[5];
            txtAddress2.Text = user[6];
            txtMemo.Text = user[8];
            //txtID.Text = Global.suvm.UserWClass;

        }

        protected void BTN_Register_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
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
            string strSQL = "UPDATE User_Information SET";
            strSQL += " UserName = '" + txtName.Text + "'";
            strSQL += ", UserNumber = '" + txtBirthDay.Text + "'";
            strSQL += ", UserGender = '" + txtGender.Text + "'";
            strSQL += ", UserAddress = '" + txtAddress.Text + "'";
            strSQL += ", UserAddress2 = '" + txtAddress2.Text + "'";
            strSQL += ", UserNote = '" + txtMemo.Text + "'";

            if (!userPassword.Equals(string.Empty))
            {
                strSQL += ", UserPassword = '" + txtPassword.Text + "'";
            }
            //strSQL += " UserWClass = '" + txtName.Text + "'";
            strSQL += " WHERE UserId = '" + txtID.Text + "'";
            strSQL += ";";

            Debug.WriteLine(strSQL);

            DB_Management dbWrite = new DB_Management();
            string writeResult = dbWrite.DB_Write(strSQL);

            if (writeResult.Equals("Success"))
            {
                string strJs = "<script>alert('수정 되었습니다.'); location.href='/Sites/Admin/Player Management.aspx';</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "goDefault", strJs);
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