using BBS.Models;
using Dapper;
using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using T_Engine;

namespace GiSanParkGolf.Class
{
    public class DB_Management
    {
        // 공통으로 사용될 커넥션 개체
        private readonly SqlConnection DB_Connection;

        public DB_Management()
        {
            DB_Connection = new SqlConnection
            {
                ConnectionString = WebConfigurationManager.ConnectionStrings["ParkGolfDB"].ConnectionString
            };
        }

        /// <summary>
        /// SYS_Users 리스트: GetAll, FindAll 
        /// </summary>
        /// <param name="page">페이지 번호</param>
        public DataTable GetUserAll(int page, string readyUser)
        {
            SqlCommand sqlCMD = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = "sp_SYS_UserList",
                CommandType = CommandType.StoredProcedure
            };
            sqlCMD.Parameters.AddWithValue("@Page", page);
            sqlCMD.Parameters.AddWithValue("@ReadyUser", readyUser);

            DB_Connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCMD);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            DB_Connection.Close();

            return dataSet.Tables["Table"];

        }

        /// <summary>
        /// SYS_Users 검색 결과 리스트
        /// </summary>
        public DataTable GetUserSeachAll(int page, string readyUser, string searchField, string searchQuery)
        {
            SqlCommand sqlCMD = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = "sp_SYS_UserSearch",
                CommandType = CommandType.StoredProcedure
            };
            sqlCMD.Parameters.AddWithValue("@Page", page);
            sqlCMD.Parameters.AddWithValue("@SearchField", searchField);
            sqlCMD.Parameters.AddWithValue("@SearchQuery", searchQuery);
            sqlCMD.Parameters.AddWithValue("@ReadyUser", readyUser);

            DB_Connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCMD);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            DB_Connection.Close();

            return dataSet.Tables["Table"];
        }

        /// <summary>
        /// SYS_Users 테이블의 모든 레코드 수
        /// </summary>
        public int GetUserCountAll(string readyUser)
        {
            try
            {
                int userCount = 0;
                SqlCommand sqlCMD = new SqlCommand
                {
                    Connection = DB_Connection,
                    CommandText = "sp_SYS_UserCountALL",
                    CommandType = CommandType.StoredProcedure
                };
                sqlCMD.Parameters.AddWithValue("@ReadyUser", readyUser);

                DB_Connection.Open();

                SqlDataReader sqlDR = sqlCMD.ExecuteReader();

                while (sqlDR.Read())
                {
                    userCount = sqlDR.GetInt32(0);
                }

                DB_Connection.Close();
                return userCount;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// SYS_Users 검색 카운트
        /// </summary>
        public int GetUserCountBySearch(string searchField, string searchQuery, string readyUser)
        {
            try
            {
                int userCount = 0;
                SqlCommand sqlCMD = new SqlCommand
                {
                    Connection = DB_Connection,
                    CommandText = "sp_SYS_UserCount",
                    CommandType = CommandType.StoredProcedure
                };
                sqlCMD.Parameters.AddWithValue("@SearchField", searchField);
                sqlCMD.Parameters.AddWithValue("@SearchQuery", searchQuery);
                sqlCMD.Parameters.AddWithValue("@ReadyUser", readyUser);

                DB_Connection.Open();

                SqlDataReader sqlDR = sqlCMD.ExecuteReader();

                while (sqlDR.Read())
                {
                    userCount = sqlDR.GetInt32(0);
                }

                DB_Connection.Close();
                return userCount;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 대회 리스트
        /// </summary>
        /// <param name="page">페이지 번호</param>
        public DataTable GetGameALL(int page)
        {
            SqlCommand sqlCMD = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = "sp_GameList",
                CommandType = CommandType.StoredProcedure
            };
            sqlCMD.Parameters.AddWithValue("@Page", page);

            DB_Connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCMD);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            DB_Connection.Close();

            return dataSet.Tables["Table"];
        }

        /// <summary>
        /// 대회 검색 검색 리스트
        /// </summary>
        public DataTable GetGameSeachAll(int page, string searchField, string searchQuery)
        {
            SqlCommand sqlCMD = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = "sp_GameSearch",
                CommandType = CommandType.StoredProcedure
            };
            sqlCMD.Parameters.AddWithValue("@Page", page);
            sqlCMD.Parameters.AddWithValue("@SearchField", searchField);
            sqlCMD.Parameters.AddWithValue("@SearchQuery", searchQuery);

            DB_Connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCMD);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            DB_Connection.Close();

            return dataSet.Tables["Table"];
        }

        /// <summary>
        /// Game_List 테이블의 모든 레코드 수
        /// </summary>
        public int GetGameCountAll()
        {
            try
            {
                int gameCount = 0;
                SqlCommand sqlCMD = new SqlCommand
                {
                    Connection = DB_Connection,
                    CommandText = "sp_GameCountALL",
                    CommandType = CommandType.StoredProcedure
                };

                DB_Connection.Open();

                SqlDataReader sqlDR = sqlCMD.ExecuteReader();

                while (sqlDR.Read())
                {
                    gameCount = sqlDR.GetInt32(0);
                }

                DB_Connection.Close();
                return gameCount;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Game_List 검색 카운트
        /// </summary>
        public int GetGameCountBySearch(string searchField, string searchQuery)
        {
            try
            {
                int userCount = 0;
                SqlCommand sqlCMD = new SqlCommand
                {
                    Connection = DB_Connection,
                    CommandText = "sp_GameCountSearch",
                    CommandType = CommandType.StoredProcedure
                };
                sqlCMD.Parameters.AddWithValue("@SearchField", searchField);
                sqlCMD.Parameters.AddWithValue("@SearchQuery", searchQuery);

                DB_Connection.Open();

                SqlDataReader sqlDR = sqlCMD.ExecuteReader();

                while (sqlDR.Read())
                {
                    userCount = sqlDR.GetInt32(0);
                }

                DB_Connection.Close();
                return userCount;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 유저 검색 결과 리스트
        /// </summary>
        /// <param name="UserName">사용자명 검색</param>
        /// <param name="onlyReady">승인대기 중인 사용자만</param>
        public DataTable GetUserList(string userName, int onlyReady, int pageIndex)
        {
            SqlCommand sqlCMD = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = "sp_SYS_UserList",
                CommandType = CommandType.StoredProcedure
            };
            sqlCMD.Parameters.AddWithValue("@UserName", userName);
            sqlCMD.Parameters.AddWithValue("@OnlyReady", onlyReady);
            sqlCMD.Parameters.AddWithValue("@Page", pageIndex);

            DB_Connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(sqlCMD);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);

            DB_Connection.Close();

            return dataSet.Tables["Table"];
        }

        /// <summary>
        /// GameCode 결과 리스트
        /// </summary>
        /// <param name="gameCode">GameCode</param>
        public GameListModel GetGameInformation(string gameCode)
        {
            var parameters = new DynamicParameters(new { GameCode = gameCode });
            return DB_Connection.Query<GameListModel>("sp_GameInformation", parameters,
                commandType: CommandType.StoredProcedure).SingleOrDefault();
        }

        public UserViewModel GetUserByUserID(string userID)
        {

            string strSQL = "SELECT UserId, UserPassword, UserName, UserWClass, UserClass";
            strSQL += " FROM SYS_Users";
            strSQL += " WHERE UserId = @UserID";
            strSQL += ";";

            SqlCommand sqlCMD = new SqlCommand(strSQL, DB_Connection);
            sqlCMD.CommandType = CommandType.Text;

            sqlCMD.Parameters.AddWithValue("@UserID", userID);

            DB_Connection.Open();

            SqlDataReader sqlDR = sqlCMD.ExecuteReader();
            while (sqlDR.Read())
            {
                Global.uvm.UserID = sqlDR.GetString(0);
                Global.uvm.Password = sqlDR.GetString(1);
                Global.uvm.UserName = sqlDR.GetString(2);
                Global.uvm.UserWClass = sqlDR.GetString(3);
                Global.uvm.UserClass = sqlDR.GetInt32(4);
            }
            DB_Connection.Close();

            SetCookie(Global.uvm.UserID,
                Global.uvm.Password,
                Global.uvm.UserName,
                Global.uvm.UserWClass,
                Global.uvm.UserClass
                , 2);

            return Global.uvm;
        }

        public void SetCookie(string userID, string userPassword, string userName, string userWClass, int userClass, int expireDayAdd)
        {
            string strUserData = userID + ":" + userPassword + ":" + userName + ":" + userWClass + ":" + userClass;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userID, DateTime.Now, DateTime.Now.AddDays(expireDayAdd), false, strUserData, FormsAuthentication.FormsCookiePath);
            string hash = FormsAuthentication.Encrypt(ticket); //Encrypt ticket

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
            if (ticket.IsPersistent)
                cookie.Expires = ticket.Expiration;

            HttpContext.Current.Response.Cookies.Add(cookie); //Create cookie
            Debug.WriteLine("쿠기 생성완료.");
        }

        public SelectUserViewModel GetSelectUserByUserID(string userID)
        {
            string strSQL = "SELECT UserId, UserName, UserPassword, UserNumber";
            strSQL += ", UserGender, UserAddress, UserAddress2";
            strSQL += ", UserRegistrationDate, UserNote, UserWClass, UserClass";
            strSQL += " FROM SYS_Users";
            strSQL += " WHERE UserId = @UserID";
            strSQL += ";";

            SqlCommand sqlCMD = new SqlCommand(strSQL, DB_Connection);
            sqlCMD.CommandType = CommandType.Text;

            sqlCMD.Parameters.AddWithValue("@UserID", userID);

            DB_Connection.Open();

            SqlDataReader sqlDR = sqlCMD.ExecuteReader();
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
            DB_Connection.Close();

            return Global.suvm;
        }

        public string IsCorrectUser(string userID, string password, int onlyInsert)
        {
            string result = string.Empty;

            Cryptography newCrypt = new Cryptography();
            String cryptPassword = newCrypt.GetEncoding("ParkGolf", password);

            DB_Connection.Open();

            SqlCommand cmd = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = "sp_SYS_UserLogin",
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@Password", cryptPassword);
            cmd.Parameters.AddWithValue("@OnlyInsert", onlyInsert);

            SqlDataReader sqlDR = cmd.ExecuteReader();
            if (sqlDR.Read())
            {
                if (sqlDR.GetString(0).Equals("승인"))
                {
                    result = "OK";
                }
                else if (sqlDR.GetString(0).Equals("승인대기"))
                {
                    result = "Ready";
                }
                else if (sqlDR.GetString(0).Equals("Logged in"))
                {
                    result = "Logged in";
                }
                else
                {
                    result = string.Empty;
                }
            }
            sqlDR.Close();

            DB_Connection.Close();

            Debug.WriteLine("로그인 기록을 남긴다. 로그인 결과 : " + result);
            return result;
        }

        public void LogoutUser(string userID)
        {
            DB_Connection.Open();

            SqlCommand cmd = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = "sp_SYS_UserLogOut",
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.ExecuteNonQuery();

            DB_Connection.Close();
        }

        public string DB_Write(string strSQL)
        {
            try
            {
                DB_Connection.Open();

                SqlCommand cmd = new SqlCommand
                {
                    Connection = DB_Connection,
                    CommandText = strSQL,
                    CommandType = System.Data.CommandType.Text
                };
                cmd.ExecuteNonQuery();

                DB_Connection.Close();
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

            DB_Connection.Open();

            string strSQL = "SELECT * FROM SYS_Users WHERE UserID = @UserId";
            SqlCommand cmd = new SqlCommand
            {
                Connection = DB_Connection,
                CommandText = strSQL,
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@UserID", userID);

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
                result = false;

            dr.Close();
            DB_Connection.Close();

            return result;
        }

        /// <summary>
        /// 공지사항 최근 글 리스트(최근 글 5개 리스트)
        /// </summary>
        public List<Note> GetNoticeRecentPosts(string bbsId)
        {
            string sql = "SELECT TOP 5 [Id], [Title], [Name], [PostDate]"
                + ", ROW_NUMBER() Over (Order By Id) As 'RowNumber'"
                + " FROM BBS_Notes"
                + " Where Category = @Category Order By Id Desc";
            return DB_Connection.Query<Note>(sql, new { Category = bbsId }).ToList();
        }

        /// <summary>
        /// 최근 대회 리스트(최근 글 5개 리스트)
        /// </summary>
        /// <param name="topcount">최근글 몇개를 불러올지</param>
        public List<GameListModel> GetGameList(int topcount)
        {
            var parameters = new DynamicParameters(new { TopCount = topcount });
            return DB_Connection.Query<GameListModel>("sp_GameList_Recent", parameters,
                commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// 활성화된 대회 리스트(최근 글 5개 리스트)
        /// </summary>
        public List<GameListModel> GetGameReadyList()
        {
            return DB_Connection.Query<GameListModel>("sp_GameList_Ready", null,
                commandType: CommandType.StoredProcedure).ToList();
        }

        public string GetEarlyJoin(string gameCode, string userID)
        {
            string strSQL = "SELECT JoinStatus FROM Game_JoinUser WHERE UserId = @Id AND GameCode = @GameCode";
            var parameters = new DynamicParameters(new
            {
                Id = userID,
                GameCode = gameCode
            });

            return DB_Connection.Query<string>(strSQL, parameters, commandType:CommandType.Text).SingleOrDefault();
        }

        public string GameJoin(GameJoinUserModel n)
        {
            try
            {
                // 파라미터 추가
                var p = new DynamicParameters();

                //[a] 공통
                p.Add("@UserId", value: n.UserId, dbType: DbType.String);
                p.Add("@JoinIP", value: n.JoinIP, dbType: DbType.String);
                p.Add("@GameCode", value: n.GameCode, dbType: DbType.String);
                DB_Connection.Execute("sp_Player_GameJoin", p, commandType: CommandType.StoredProcedure);

                return "Success";
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
        }

        /// <summary>
        /// 내가 치룬 대회의 목록을 조회
        /// </summary>
        public List<GameListModel> GetMyGameList(string userID)
        {
            var parameters = new DynamicParameters(new { UserId = userID });
            return DB_Connection.Query<GameListModel>("sp_Player_MyGame", parameters,
                commandType: CommandType.StoredProcedure).ToList();
        }

        public string MyGameCancel(string gameCode, string userID)
        {
            string result = "Success";
            try
            {
                var parameters = new DynamicParameters(new
                {
                    UserId = userID,
                    GameCode = gameCode
                });
                DB_Connection.Query("sp_Player_GameCancel",
                    parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public List<GameListModel> GetReadyGame(int readyOnly)
        {
            string strSQL = "SELECT *";
            strSQL += " FROM Game_List";
            strSQL += " WHERE UserId = @Id AND GameCode = @GameCode";

            var parameters = new DynamicParameters(new
            {
                ReadyOnly = readyOnly
            });

            return DB_Connection.Query<GameListModel>(strSQL, parameters, commandType: CommandType.Text).ToList();
        }
    }
}