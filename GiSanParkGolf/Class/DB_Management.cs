using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using T_Engine;

namespace GiSanParkGolf.Class
{
    public class DB_Management
    {
        // 공통으로 사용될 커넥션 개체
        private SqlConnection con;

        public DB_Management()
        {
            con = new SqlConnection();
            con.ConnectionString = WebConfigurationManager.ConnectionStrings["ParkGolfDB"].ConnectionString;
        }

        //public void AddUser(string userID, string password)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = con;
        //    cmd.CommandText = "WriteUsers";
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@UserID", userID);
        //    cmd.Parameters.AddWithValue("@Password", password);

        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //    con.Close();
        //}

        protected void SetCookie(string userID, string userPassword, string userName, string userWClass, int userClass)
        {
            string strUserData = userID + ":" + userPassword + ":" + userName + ":" + userWClass + ":" + userClass;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userID, DateTime.Now, DateTime.Now.AddDays(365), true, strUserData, FormsAuthentication.FormsCookiePath);
            string hash = FormsAuthentication.Encrypt(ticket); //Encrypt ticket

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
            if (ticket.IsPersistent)
                cookie.Expires = ticket.Expiration;

            HttpContext.Current.Response.Cookies.Add(cookie); //Create cookie
        }

        public UserViewModel GetUserByUserID(string userID)
        {

            string strSQL = "SELECT UserId, UserPassword, UserName, UserWClass, UserClass";
            strSQL += " FROM SYS_Users";
            strSQL += " WHERE UserId = @UserID";
            strSQL += ";";

            SqlCommand sqlCmd = new SqlCommand(strSQL, con);
            sqlCmd.CommandType = CommandType.Text;
            
            sqlCmd.Parameters.AddWithValue("@UserID", userID);

            con.Open();

            SqlDataReader sqlDR = sqlCmd.ExecuteReader();
            while (sqlDR.Read())
            {
                Global.uvm.UserID = sqlDR.GetString(0);
                Global.uvm.Password = sqlDR.GetString(1);
                Global.uvm.UserName = sqlDR.GetString(2);
                Global.uvm.UserWClass = sqlDR.GetString(3);
                Global.uvm.UserClass = sqlDR.GetInt32(4);
            }
            con.Close();

            SetCookie(Global.uvm.UserID,
                Global.uvm.Password,
                Global.uvm.UserName,
                Global.uvm.UserWClass,
                Global.uvm.UserClass);

            return Global.uvm;
        }

        public SelectUserViewModel GetSelectUserByUserID(string userID)
        {

            string strSQL = "SELECT UserId, UserName, UserPassword, UserNumber";
            strSQL += ", UserGender, UserAddress, UserAddress2";
            strSQL += ", UserRegistrationDate, UserNote, UserWClass, UserClass";
            strSQL += " FROM SYS_Users";
            strSQL += " WHERE UserId = @UserID";
            strSQL += ";";

            SqlCommand sqlCmd = new SqlCommand(strSQL, con);
            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.Parameters.AddWithValue("@UserID", userID);

            con.Open();

            SqlDataReader sqlDR = sqlCmd.ExecuteReader();
            while (sqlDR.Read())
            {
                Global.suvm.UserID = sqlDR.GetString(0);
                Global.suvm.UserName = sqlDR.GetString(1);
                Global.suvm.UserPassword = sqlDR.GetString(2);
                Global.suvm.UserNumber = sqlDR.GetInt32(3);
                Global.suvm.UserGender = sqlDR.GetInt32(4);
                Global.suvm.UserAddress = sqlDR.GetString(5);
                Global.suvm.UserAddress2 = sqlDR.GetString(6);
                Global.suvm.UserRegistrationDate = sqlDR.GetDateTime(7);
                Global.suvm.UserNote = sqlDR.GetString(8);
                Global.suvm.UserWClass = sqlDR.GetString(9);
                Global.suvm.UserClass = sqlDR.GetInt32(10);
            }
            con.Close();

            return Global.suvm;
        }

        public string IsCorrectUser(string userID, string password)
        {
            string result = string.Empty;

            Cryptography newCrypt = new Cryptography();
            String cryptPassword = newCrypt.GetEncoding("ParkGolf", password);

            con.Open();

            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = "SYS_UserLogin",
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@Password", cryptPassword);

            SqlDataReader sqlDR = cmd.ExecuteReader();
            if (sqlDR.Read())
            {
                if (sqlDR.GetString(0).Equals("승인"))
                {
                    result = "OK";
                } else if (sqlDR.GetString(0).Equals("승인대기"))
                {
                    result = "Ready";
                }
                else if (sqlDR.GetString(0).Equals("Logged in"))
                {
                    result = "Logged in";
                } else
                {
                    result = string.Empty;
                }
            }
            sqlDR.Close();
            con.Close();

            return result;
        }

        public void LogoutUser(string userID)
        {
            con.Open();

            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = "SYS_UserLogOut",
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.ExecuteNonQuery();

            con.Close();
        }

        public string DB_Write(string strSQL)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand
                {
                    Connection = con,
                    CommandText = strSQL,
                    CommandType = System.Data.CommandType.Text
                };
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (OleDbException ex)
            {
                return ex.ToString();
            }

            return "Success";
        }

        public Boolean ID_DuplicateCheck(string userID)
        {
            bool result = true;

            con.Open();

            string strSql = "SELECT * FROM SYS_Users WHERE UserID = @UserId";
            SqlCommand cmd = new SqlCommand
            {
                Connection = con,
                CommandText = strSql,
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@UserID", userID);

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
                result = false;

            dr.Close();
            con.Close();

            return result;
        }
    }
}