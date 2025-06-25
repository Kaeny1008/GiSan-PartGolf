using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using T_Engine;

namespace GiSanParkGolf.Sites.UserManagement
{
    public partial class UserRepositories : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BTN_Register_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //패스워드 암호화 txtPassword.Text
                Cryptography newCrypt = new Cryptography();
                String cryptPassword = newCrypt.GetEncoding("ParkGolf", txtPassword.Text);

                UserWrite(cryptPassword);
            } 
            else
            {

            }
        }

        protected void UserWrite(string userPassword)
        {
            //String.Format("{0:yyyy-MM-dd}", DateTime.Parse(TextBox1.Text))
            //데이터 저장
            string strSQL = "INSERT INTO SYS_Users(UserName, UserGender, UserNumber, UserAddress, UserAddress2" +
                    ", UserNote, UserId, UserPassword, UserWClass" +
                    ") VALUES(" +
                    "'" + txtName.Text + "'" +
                    ",'" + txtGender.Text + "'" +
                    ",'" + txtBirthDay.Text + "'" +
                    ",'" + txtAddress.Text + "'" +
                    ",'" + txtAddress2.Text + "'" +
                    ",'" + txtMemo.Text + "'" +
                    ",'" + txtID.Text + "'" +
                    ",'" + userPassword + "'" +
                    ",'승인대기'" +
                    ");";

            DB_Management dbWrite = new DB_Management();
            string writeResult = dbWrite.DB_Write(strSQL);

            if (writeResult.Equals("Success"))
            {
                string strJs = "<script>alert('가입 승인대기 되었습니다.'); location.href='/Default.aspx';</script>";
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

        protected void Btn_IDCheck_Click(object sender, EventArgs e)
        {
            txtPassword.Attributes.Add("value", txtPassword.Text);
            txtReCheck.Attributes.Add("value", txtReCheck.Text);

            DB_Management IDCheck = new DB_Management();
            Boolean checkResult = IDCheck.ID_DuplicateCheck(txtID.Text);

            if (checkResult == false)
            {
                IDResult.Text = "ID를 사용 할 수 없습니다.";
                IDResult.ForeColor = Color.Red;
                TextBox2.Text = "Ready";
            }
            else
            {
                IDResult.Text = "ID를 사용 할 수 있습니다.";
                IDResult.ForeColor = Color.Black;
                TextBox2.Text = "OK";
            }
        }
    }
}