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

namespace GiSanParkGolf.Repositories
{
    public class UserRepository
    {
        // 공통으로 사용될 커넥션 개체
        private OleDbConnection con;

        public UserRepository()
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
            UserViewModel r = new UserViewModel();

            string strSQL = "SELECT UserId, UserPassword, UserName FROM User_Information WHERE UserId = @UserID";
            OleDbCommand sqlCmd = new OleDbCommand(strSQL, con);
            sqlCmd.CommandType = CommandType.Text;
            
            sqlCmd.Parameters.AddWithValue("@UserID", userID);

            con.Open();

            OleDbDataReader sqlDR = sqlCmd.ExecuteReader();
            while (sqlDR.Read())
            {
                r.UserID = sqlDR.GetString(0);
                r.Password = sqlDR.GetString(1);
                r.UserName = sqlDR.GetString(2);
            }
            con.Close();

            return r;
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

        public bool IsCorrectUser(string userID, string password)
        {
            bool result = false;

            con.Open();

            string strSql = "SELECT * FROM User_Information WHERE UserID = @UserId AND UserPassword = @Password";
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = con;
            cmd.CommandText = strSql;
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@Password", password);

            OleDbDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
                result = true;

            dr.Close();
            con.Close();

            return result;
        }
    }
}