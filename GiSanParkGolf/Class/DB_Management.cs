using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using T_Engine;

namespace GiSanParkGolf.Class
{
    public class DB_Management
    {
        // 공통으로 사용될 커넥션 개체
        private OleDbConnection con;

        public DB_Management()
        {
            con = new OleDbConnection();
            con.ConnectionString = WebConfigurationManager.ConnectionStrings["MDB_ConnectionString"].ConnectionString;
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

        public UserViewModel GetUserByUserID(string userID)
        {

            string strSQL = "SELECT UserId, UserPassword, UserName, UserWClass FROM User_Information WHERE UserId = @UserID";
            OleDbCommand sqlCmd = new OleDbCommand(strSQL, con);
            sqlCmd.CommandType = CommandType.Text;
            
            sqlCmd.Parameters.AddWithValue("@UserID", userID);

            con.Open();

            OleDbDataReader sqlDR = sqlCmd.ExecuteReader();
            while (sqlDR.Read())
            {
                Global.uvm.UserID = sqlDR.GetString(0);
                Global.uvm.Password = sqlDR.GetString(1);
                Global.uvm.UserName = sqlDR.GetString(2);
                Global.uvm.UserWClass = sqlDR.GetString(3);
            }
            con.Close();

            return Global.uvm;
        }

        //public void ModifyUser(int UID, string userID, string password)
        //{
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = con;
        //    cmd.CommandText = "ModifyUsers";
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@UserID", userID);
        //    cmd.Parameters.AddWithValue("@Password", password);
        //    cmd.Parameters.AddWithValue("@UID", UID);

        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //    con.Close();
        //}

        public string IsCorrectUser(string userID, string password)
        {
            string result = string.Empty;

            Cryptography newCrypt = new Cryptography();
            String cryptPassword = newCrypt.GetEncoding("ParkGolf", password);

            con.Open();

            string strSql = "SELECT UserWClass FROM User_Information WHERE UserID = @UserId AND UserPassword = @Password";
            OleDbCommand cmd = new OleDbCommand
            {
                Connection = con,
                CommandText = strSql,
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@Password", cryptPassword);

            OleDbDataReader sqlDR = cmd.ExecuteReader();
            if (sqlDR.Read())
            {
                if (sqlDR.GetString(0).Equals("승인"))
                {
                    result = "OK";
                } else if (sqlDR.GetString(0).Equals("승인대기"))
                {
                    result = "Ready";
                } else
                {
                    result = string.Empty;
                }
            }
            sqlDR.Close();
            con.Close();

            return result;
        }

        public string DB_Write(string strSQL)
        {
            try
            {
                con = new OleDbConnection();
                con.ConnectionString = WebConfigurationManager.ConnectionStrings["MDB_ConnectionString"].ConnectionString;
                con.Open();

                OleDbCommand cmd = new OleDbCommand
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

            string strSql = "SELECT * FROM User_Information WHERE UserID = @UserId";
            OleDbCommand cmd = new OleDbCommand
            {
                Connection = con,
                CommandText = strSql,
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@UserID", userID);

            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
                result = false;

            dr.Close();
            con.Close();

            return result;
        }
    }
}