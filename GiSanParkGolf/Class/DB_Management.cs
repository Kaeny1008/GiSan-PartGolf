using BBS.Models;
using Dapper;
using GiSanParkGolf.Models;
using GiSanParkGolf.Sites.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.Security;
using System.Web.UI.WebControls;
using T_Engine;

namespace GiSanParkGolf.Class
{
    public class DB_Management
    {
        // 공통으로 사용될 커넥션 개체
        public readonly SqlConnection DB_Connection;

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
        public List<SelectUserViewModel> GetUserAll(int page, string readyUser)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Page", page);
            parameters.Add("@ReadyUser", readyUser);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<SelectUserViewModel>(
                    "sp_SYS_UserList",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserAll] 오류: {ex.Message}");
                return new List<SelectUserViewModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
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
        public List<SelectUserViewModel> GetUserSearchAll(int page, string readyUser, string searchField, string searchQuery)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Page", page);
            parameters.Add("@SearchField", searchField);
            parameters.Add("@SearchQuery", searchQuery);
            parameters.Add("@ReadyUser", readyUser);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<SelectUserViewModel>(
                    "sp_SYS_UserSearch",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserSearchAll] 오류: {ex.Message}");
                return new List<SelectUserViewModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// SYS_Users 검색 카운트
        /// </summary>
        public int GetUserCountBySearch(string searchField, string searchQuery, string readyUser)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SearchField", searchField);
            parameters.Add("@SearchQuery", searchQuery);
            parameters.Add("@ReadyUser", readyUser);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.ExecuteScalar<int>(
                    "sp_SYS_UserCount", parameters, commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserCountBySearch] 오류: {ex.Message}");
                return -1;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 대회 검색 검색 리스트
        /// </summary>
        public List<GameListModel> GetGameSearchAll(int page, string searchField, string searchQuery)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Page", page);
            parameters.Add("@SearchField", searchField);
            parameters.Add("@SearchQuery", searchQuery);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<GameListModel>(
                    "sp_GameSearch", parameters, commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGameSearchAll] 오류: {ex.Message}");
                return new List<GameListModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// Game_List 테이블의 모든 레코드 수
        /// </summary>
        public int GetGameCountAll()
        {
            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.ExecuteScalar<int>(
                    "sp_GameCountALL", null, commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGameCountAll] 오류: {ex.Message}");
                return -1;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// Game_List 검색 카운트
        /// </summary>
        public int GetGameCountBySearch(string searchField, string searchQuery)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SearchField", searchField);
            parameters.Add("@SearchQuery", searchQuery);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.ExecuteScalar<int>(
                    "sp_GameCountSearch", parameters, commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGameCountBySearch] 오류: {ex.Message}");
                return -1;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 유저 검색 결과 리스트
        /// </summary>
        /// <param name="UserName">사용자명 검색</param>
        /// <param name="onlyReady">승인대기 중인 사용자만</param>
        public List<SelectUserViewModel> GetUserList(string userName, int onlyReady, int pageIndex)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", userName);
            parameters.Add("@OnlyReady", onlyReady);
            parameters.Add("@Page", pageIndex);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<SelectUserViewModel>(
                    "sp_SYS_UserList", parameters, commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserList] 오류: {ex.Message}");
                return new List<SelectUserViewModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// GameCode 결과 리스트
        /// </summary>
        /// <param name="gameCode">GameCode</param>
        public Select_GameList GetGameInformation(string gameCode)
        {
            var query = "sp_Get_GameInformation";
            var parameters = new DynamicParameters(new { GameCode = gameCode });

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<Select_GameList>(
                    query, parameters, commandType: CommandType.StoredProcedure
                ).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGameInformation] 오류: {ex.Message}");
                return null;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        //public UserViewModel GetUserByUserID(string userID)
        //{

        //    string strSQL = "SELECT UserId, UserPassword, UserName, UserWClass, UserClass";
        //    strSQL += " FROM SYS_Users";
        //    strSQL += " WHERE UserId = @UserID";
        //    strSQL += ";";

        //    SqlCommand sqlCMD = new SqlCommand(strSQL, DB_Connection);
        //    sqlCMD.CommandType = CommandType.Text;

        //    sqlCMD.Parameters.AddWithValue("@UserID", userID);

        //    DB_Connection.Open();

        //    SqlDataReader sqlDR = sqlCMD.ExecuteReader();
        //    while (sqlDR.Read())
        //    {
        //        Helper.CurrentUser?.UserId = sqlDR.GetString(0);
        //        Helper.CurrentUser?.UserPassword = sqlDR.GetString(1);
        //        Helper.CurrentUser?.UserName = sqlDR.GetString(2);
        //        Global.uvm.UserWClass = sqlDR.GetString(3);
        //        Global.uvm.UserClass = sqlDR.GetInt32(4);
        //    }
        //    DB_Connection.Close();

        //    SetCookie(Helper.CurrentUser?.UserId,
        //        Helper.CurrentUser?.UserPassword,
        //        Helper.CurrentUser?.UserName,
        //        Global.uvm.UserWClass,
        //        Global.uvm.UserClass
        //        , 2);

        //    return Global.uvm;
        //}

        public UserViewModel GetUserByUserID(string userID)
        {
            string query = @"
                SELECT UserId, UserPassword, UserName, UserWClass, UserClass
                FROM SYS_Users
                WHERE UserId = @UserID
            ";

            var parameters = new { UserID = userID };

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                var user = DB_Connection.QueryFirstOrDefault<UserViewModel>(
                    query, parameters
                );

                if (user != null)
                {
                    SetCookie(user.UserId,
                              user.UserPassword,
                              user.UserName,
                              user.UserWClass,
                              user.UserClass,
                              2); // 2시간 유지 등

                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserByUserID] 오류: {ex.Message}");
                return null;
            }
            finally
            {
                DB_Connection.Close();
            }
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
            string query = @"
                SELECT UserId, UserName, UserPassword, UserNumber,
                       UserGender, UserAddress, UserAddress2,
                       UserRegistrationDate, UserNote, UserWClass, UserClass
                FROM SYS_Users
                WHERE UserId = @UserID
            ";

            var parameters = new { UserID = userID };

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.QueryFirstOrDefault<SelectUserViewModel>(
                    query, parameters
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetSelectUserByUserID] 오류: {ex.Message}");
                return null;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public string IsCorrectUser(string userID, string password, int onlyInsert)
        {
            string result = string.Empty;

            var crypt = new Cryptography();
            string encryptedPassword = crypt.GetEncoding("ParkGolf", password);

            var parameters = new DynamicParameters();
            parameters.Add("@UserID", userID);
            parameters.Add("@Password", encryptedPassword);
            parameters.Add("@OnlyInsert", onlyInsert);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                var loginStatus = DB_Connection.QueryFirstOrDefault<string>(
                    "sp_SYS_UserLogin", parameters, commandType: CommandType.StoredProcedure
                );

                switch (loginStatus)
                {
                    case "승인": result = "OK"; break;
                    case "승인대기": result = "Ready"; break;
                    case "Logged in": result = "Logged in"; break;
                    default: result = string.Empty; break;
                }

                Console.WriteLine($"[IsCorrectUser] 로그인 결과 : {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IsCorrectUser] 오류: {ex.Message}");
                return $"Error: {ex.Message}";
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public void LogoutUser(string userID)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserID", userID);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                DB_Connection.Execute("sp_SYS_UserLogOut", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LogoutUser] 오류: {ex.Message}");
                // 필요 시: 로그 저장 또는 경고 처리
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public string DB_Write(string strSQL)
        {
            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                DB_Connection.Execute(strSQL); // 텍스트 쿼리 직접 실행

                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB_Write] 오류: {ex.Message}");
                return $"Error: {ex.Message}";
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public bool ID_DuplicateCheck(string userID)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM SYS_Users 
                WHERE UserID = @UserId
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                int count = DB_Connection.ExecuteScalar<int>(query, new { UserId = userID });
                return count == 0; // 중복 없으면 true
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ID_DuplicateCheck] 오류: {ex.Message}");
                return false; // 실패 시 보호 처리
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 공지사항 최근 글 리스트(최근 글 5개 리스트)
        /// </summary>
        public List<Note> GetNoticeRecentPosts(string bbsId)
        {
            string sql = @"
                SELECT TOP 5 [Id], [Title], [Name], [PostDate],
                       ROW_NUMBER() OVER (ORDER BY Id) AS RowNumber
                FROM BBS_Notes
                WHERE Category = @Category
                ORDER BY Id DESC
            ";

            var parameters = new { Category = bbsId };

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<Note>(sql, parameters).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetNoticeRecentPosts] 오류: {ex.Message}");
                return new List<Note>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 최근 대회 리스트(최근 글 5개 리스트)
        /// </summary>
        /// <param name="topcount">최근글 몇개를 불러올지</param>
        public List<GameListModel> GetGameList(int topcount)
        {
            var parameters = new DynamicParameters(new { TopCount = topcount });

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<GameListModel>(
                    "sp_Get_GameList_Recent",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGameList] 오류: {ex.Message}");
                return new List<GameListModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 활성화된 대회 리스트(최근 글 5개 리스트)
        /// </summary>
        public List<GameListModel> GetGameReadyList(string field, string keyword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<GameListModel>(
                    "sp_Get_Game_ReadyList",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGameReadyList] 오류: {ex.Message}");
                return new List<GameListModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public string GetEarlyJoin(string gameCode, string userID)
        {
            string query = @"
                SELECT JoinStatus
                FROM Game_JoinUser
                WHERE UserId = @Id AND GameCode = @GameCode
            ";

            var parameters = new
            {
                Id = userID,
                GameCode = gameCode
            };

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<string>(
                    query, parameters, commandType: CommandType.Text
                ).SingleOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetEarlyJoin] 오류: {ex.Message}");
                return null; // 또는 "Unknown", "Error" 등 정의된 기본 값
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public string GameJoin(GameJoinUserModel n)
        {
            var p = new DynamicParameters();
            p.Add("@UserId", n.UserId, DbType.String);
            p.Add("@JoinIP", n.JoinIP, DbType.String);
            p.Add("@GameCode", n.GameCode, DbType.String);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                DB_Connection.Execute("sp_Player_GameJoin", p, commandType: CommandType.StoredProcedure);
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameJoin] 오류: {ex.Message}");
                return $"Error: {ex.Message}";
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 내가 치룬 대회의 목록을 조회
        /// </summary>
        public List<GameListModel> GetMyGameList(string userID)
        {
            var parameters = new DynamicParameters(new { UserId = userID });

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<GameListModel>(
                    "sp_Get_Player_MyGame",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetMyGameList] 오류: {ex.Message}");
                return new List<GameListModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public string MyGameCancel(string gameCode, string userID)
        {
            string result = "Success";

            var parameters = new DynamicParameters(new
            {
                UserId = userID,
                GameCode = gameCode
            });

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                DB_Connection.Query(
                    "sp_Player_GameCancel",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).SingleOrDefault();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                DB_Connection.Close();
            }

            return result;
        }

        //public List<GameListModel> GetReadyGame(int readyOnly)
        //{
        //    string strSQL = "SELECT *";
        //    strSQL += " FROM Game_List";
        //    strSQL += " WHERE UserId = @Id AND GameCode = @GameCode";

        //    var parameters = new DynamicParameters(new
        //    {
        //        ReadyOnly = readyOnly
        //    });

        //    return DB_Connection.Query<GameListModel>(strSQL, parameters, commandType: CommandType.Text).ToList();
        //}

        public List<GameJoinUserList> GetGameUserList(string gameCode)
        {
            var parameters = new DynamicParameters(new
            {
                GameCode = gameCode
            });

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<GameJoinUserList>(
                    "sp_Get_Game_JoinUser",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGameUserList] 오류: {ex.Message}");
                return new List<GameJoinUserList>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 지정 게임의 핸디캡 목록 조회 (검색어 옵션)
        /// </summary>
        public IEnumerable<PlayerHandicapViewModel> GetHandicaps(string gameCode, string searchTerm = null)
        {
            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<PlayerHandicapViewModel>(
                    "sp_GetGameHandicaps",
                    new { GameCode = gameCode, SearchTerm = searchTerm },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetHandicaps] 오류: {ex.Message}");
                return Enumerable.Empty<PlayerHandicapViewModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public List<UserWithHandicap> GetUserHandicaps(string field, string keyword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                var result = DB_Connection.Query<UserWithHandicap>(
                    "sp_Get_UserHandicaps",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                             // 나이 계산 후 모델에 주입
                foreach (var user in result)
                {
                    user.Age = Helper.CalculateAge(user.UserNumber, user.UserGender);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserHandicaps] 오류: {ex.Message}");
                return new List<UserWithHandicap>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // ✅ 핸디캡 정보 저장 (업데이트 또는 신규)
        public void UpdateHandicap(string userId, int handicap, string source, string updatedBy)
        {
            string sql = @"
                MERGE SYS_Handicap AS target
                USING (SELECT @UserId AS UserId) AS src
                ON target.UserId = src.UserId
                WHEN MATCHED THEN 
                    UPDATE SET AgeHandicap = @Handicap,
                               Source = @Source,
                               LastUpdated = GETDATE(),
                               LastUpdatedBy = @UpdatedBy
                WHEN NOT MATCHED THEN
                    INSERT (UserId, AgeHandicap, Source, LastUpdated, LastUpdatedBy)
                    VALUES (@UserId, @Handicap, @Source, GETDATE(), @UpdatedBy);
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                DB_Connection.Execute(sql, new
                {
                    UserId = userId,
                    Handicap = handicap,
                    Source = source,
                    UpdatedBy = updatedBy
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateHandicap] 오류: {ex.Message}");
                            // 필요한 경우: 로깅 or 에러 핸들링 추가 가능
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public void InsertHandicapChangeLog(HandicapChangeLog log)
        {
            string sql = @"
                INSERT INTO SYS_HandicapLog
                (UserId, Age, PrevHandicap, NewHandicap, PrevSource, NewSource, Reason, ChangedBy)
                VALUES (@UserId, @Age, @PrevHandicap, @NewHandicap, @PrevSource, @NewSource, @Reason, @ChangedBy);
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                DB_Connection.Execute(sql, log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InsertHandicapChangeLog] 오류: {ex.Message}");
                            // 필요 시: 에러 로그 DB 저장 or 재시도 로직
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public List<HandicapChangeLog> GetHandicapChangeLogs(string field, string keyword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<HandicapChangeLog>(
                    "sp_Get_HandicapChangeLogs",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetHandicapChangeLogs] 오류: {ex.Message}");
                return new List<HandicapChangeLog>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public List<UserViewModel> GetPlayers(string field, string keyword, bool readyOnly)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);
            parameters.Add("@ReadyOnly", readyOnly ? 1 : 0);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<UserViewModel>(
                    "sp_Get_SYS_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetPlayers] 오류: {ex.Message}");
                return new List<UserViewModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        /// <summary>
        /// 대회 리스트
        /// </summary>
        /// <param name="page">페이지 번호</param>
        public List<GameListModel> GetGames(string field, string keyword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<GameListModel>(
                    "sp_Get_GameList",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetGames] 오류: {ex.Message}");
                return new List<GameListModel>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // 경기장 목록 조회
        public List<StadiumList> GetStadiumList()
        {
            string query = @"
                SELECT StadiumCode AS Code, StadiumName AS Name, IsActive, Note, CreatedAt
                FROM SYS_StadiumList
                ORDER BY StadiumCode
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<StadiumList>(query).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetStadiumList] 오류: {ex.Message}");
                return new List<StadiumList>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // 경기장 등록
        public bool InsertStadium(StadiumList stadium)
        {
            string query = @"
                INSERT INTO SYS_StadiumList (StadiumCode, StadiumName, IsActive, Note)
                VALUES (@StadiumCode, @StadiumName, @IsActive, @Note)
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Execute(query, stadium) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InsertStadium] 오류: {ex.Message}");
                return false;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // 특정 경기장의 코스 목록 조회
        public List<CourseList> GetCourseListByStadium(string stadiumCode)
        {
            string query = @"
                SELECT CourseCode, StadiumCode, CourseName, HoleCount, IsActive, CreatedAt
                FROM SYS_CourseList
                WHERE StadiumCode = @StadiumCode
                ORDER BY CourseCode
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection
                    .Query<CourseList>(query, new { StadiumCode = stadiumCode })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetCourseListByStadium] 오류: {ex.Message}");
                return new List<CourseList>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // 코스 등록
        public int InsertCourseAndGetId(CourseList course)
        {
            string query = @"
                INSERT INTO SYS_CourseList (StadiumCode, CourseName, HoleCount, IsActive, CreatedAt)
                VALUES (@StadiumCode, @CourseName, @HoleCount, @IsActive, SYSDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.ExecuteScalar<int>(query, course);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InsertCourseAndGetId] 오류: {ex.Message}");
                return -1; // 실패 시 음수 반환
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // 홀 목록 저장 (신규 등록)
        public bool SaveHoleList(string courseCode, List<HoleList> holes)
        {
            string query = @"
                INSERT INTO SYS_HoleInfo (CourseCode, HoleName, Distance, Par)
                VALUES (@CourseCode, @HoleName, @Distance, @Par)
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                SqlTransaction tx = DB_Connection.BeginTransaction();

                try
                {
                    foreach (var hole in holes)
                    {
                        var param = new
                        {
                            CourseCode = courseCode,
                            HoleName = hole.HoleName,
                            Distance = hole.Distance,
                            Par = hole.Par
                        };

                        DB_Connection.Execute(query, param, tx);
                    }

                    tx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    Console.WriteLine($"[SaveHoleList] 오류: {ex.Message}");
                    return false;
                }
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // 홀 목록 수정
        public bool UpdateHoleList(string courseCode, List<HoleList> holes)
        {
            string updateQuery = @"
                UPDATE SYS_HoleInfo
                SET HoleName = @HoleName, Distance = @Distance, Par = @Par
                WHERE HoleId = @HoleId AND CourseCode = @CourseCode
            ";

            string insertQuery = @"
                INSERT INTO SYS_HoleInfo (CourseCode, HoleName, Distance, Par)
                VALUES (@CourseCode, @HoleName, @Distance, @Par)
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                SqlTransaction tx = DB_Connection.BeginTransaction();

                try
                {
                    foreach (var hole in holes)
                    {
                        var param = new
                        {
                            HoleId = hole.HoleId,
                            CourseCode = courseCode,
                            HoleName = hole.HoleName,
                            Distance = hole.Distance,
                            Par = hole.Par
                        };

                        if (hole.HoleId > 0)
                        {
                            DB_Connection.Execute(updateQuery, param, tx);
                        }
                        else
                        {
                            DB_Connection.Execute(insertQuery, param, tx);
                        }
                    }

                    tx.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    Console.WriteLine($"[UpdateHoleList] 오류: {ex.Message}");
                    return false;
                }
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        // 특정 코스의 홀 목록 조회
        public List<HoleList> GetHoleListByCourse(string courseCode)
        {
            string query = @"
                SELECT HoleId, CourseCode, HoleName, Distance, Par
                FROM SYS_HoleInfo
                WHERE CourseCode = @CourseCode
                ORDER BY HoleId
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection
                    .Query<HoleList>(query, new { CourseCode = courseCode })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetHoleListByCourse] 오류: {ex.Message}");
                return new List<HoleList>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public string CreateCode(string tableName, string columnName, string prefix, int digits)
        {
            string query = $@"
                SELECT TOP 1 {columnName}
                FROM {tableName}
                WHERE {columnName} LIKE @Pattern
                ORDER BY {columnName} DESC
            ";

            string pattern = $"{prefix}%";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                string lastCode = DB_Connection.QueryFirstOrDefault<string>(
                    query,
                    new { Pattern = pattern }
                );

                int number = 1;

                if (!string.IsNullOrEmpty(lastCode) && lastCode.Length > prefix.Length)
                {
                    string numericPart = lastCode.Substring(prefix.Length);
                    if (int.TryParse(numericPart, out int parsed))
                        number = parsed + 1;
                }

                return $"{prefix}{number.ToString().PadLeft(digits, '0')}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateCode] 오류: {ex.Message}");
                return $"{prefix}{new string('0', digits)}";
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public List<StadiumList> SearchStadiumList(string field, string keyword, bool readyOnly, int pageIndex, int pageSize)
        {
            string condition = "";
            var param = new DynamicParameters();

            int offset = pageIndex * pageSize;
            param.Add("Offset", offset);
            param.Add("PageSize", pageSize);

            if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(keyword))
            {
                condition += $" AND {field} LIKE @Keyword";
                param.Add("Keyword", $"%{keyword}%");
            }

            if (readyOnly)
                condition += " AND IsActive = 1";

            string query = $@"
                SELECT StadiumCode, StadiumName, IsActive, Note, CreatedAt
                FROM SYS_StadiumList
                WHERE 1 = 1 {condition}
                ORDER BY StadiumCode
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<StadiumList>(query, param).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SearchStadiumList] 오류: {ex.Message}");
                return new List<StadiumList>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public CourseList GetCourseByCode(string courseCode)
        {
            string query = @"
                SELECT CourseCode, StadiumCode, CourseName, HoleCount, IsActive, CreatedAt
                FROM SYS_CourseList
                WHERE CourseCode = @CourseCode
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.QueryFirstOrDefault<CourseList>(query, new { CourseCode = courseCode });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetCourseByCode] 오류: {ex.Message}");
                return null;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public bool UpdateHoleCount(string courseCode, int holeCount)
        {
            string query = @"
                UPDATE SYS_CourseList
                SET HoleCount = @HoleCount
                WHERE CourseCode = @CourseCode
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                int affectedRows = DB_Connection.Execute(query, new { HoleCount = holeCount, CourseCode = courseCode });
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateHoleCount] 오류: {ex.Message}");
                return false;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public bool DeleteHoleById(int holeId)
        {
            string query = @"
                DELETE FROM SYS_HoleInfo
                WHERE HoleId = @HoleId
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                int rows = DB_Connection.Execute(query, new { HoleId = holeId });
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteHoleById] 오류: {ex.Message}");
                return false;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public bool DeleteAllHolesByCourse(string courseCode)
        {
            if (string.IsNullOrWhiteSpace(courseCode))
                return false;

            string query = @"
                DELETE FROM SYS_HoleInfo
                WHERE CourseCode = @CourseCode
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                int rows = DB_Connection.Execute(query, new { CourseCode = courseCode });
                return rows > -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteAllHolesByCourse] 오류: {ex.Message}");
                return false;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public bool DeleteCourse(int courseCode)
        {
            string deleteHoles = "DELETE FROM SYS_HoleInfo WHERE CourseCode = @CourseCode";
            string deleteCourse = "DELETE FROM SYS_CourseList WHERE CourseCode = @CourseCode";

            DB_Connection.Open();

            try
            {
                // 1. 홀 먼저 삭제
                DB_Connection.Execute(deleteHoles, new { CourseCode = courseCode });

                // 2. 코스 삭제
                int rows = DB_Connection.Execute(deleteCourse, new { CourseCode = courseCode });

                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteCourse] 오류: {ex.Message}");
                return false;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public bool DeleteStadium(string stadiumCode)
        {
            if (string.IsNullOrWhiteSpace(stadiumCode))
                return false;

            string deleteHoles = @"
                DELETE FROM SYS_HoleInfo
                WHERE CourseCode IN (
                    SELECT CourseCode FROM SYS_CourseList WHERE StadiumCode = @StadiumCode
                )";

            string deleteCourses = @"
                DELETE FROM SYS_CourseList
                WHERE StadiumCode = @StadiumCode
            ";

            string deleteStadium = @"
                DELETE FROM SYS_StadiumList
                WHERE StadiumCode = @StadiumCode
            ";

            DB_Connection.Open();

            try
            {
                // 1. 해당 경기장에 속한 홀들 삭제
                DB_Connection.Execute(deleteHoles, new { StadiumCode = stadiumCode });

                // 2. 해당 경기장에 속한 코스들 삭제
                DB_Connection.Execute(deleteCourses, new { StadiumCode = stadiumCode });

                // 3. 경기장 자체 삭제
                int rows = DB_Connection.Execute(deleteStadium, new { StadiumCode = stadiumCode });

                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteStadium] 오류: {ex.Message}");
                return false;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public List<StadiumList> GetStadiums()
        {
            string query = @"
                SELECT StadiumCode, StadiumName, IsActive, Note, CreatedAt
                FROM SYS_StadiumList
                WHERE IsActive = 1
                ORDER BY StadiumName ASC
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<StadiumList>(query).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetStadiums] 오류: {ex.Message}");
                return new List<StadiumList>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public List<AssignedPlayer> GetAssignmentResult(string gameCode)
        {
            string query = @"
                SELECT A.GameCode, A.UserId, U.UserName, U.UserGender,
                       CASE U.UserGender 
                           WHEN 1 THEN '남성'
                           WHEN 2 THEN '여성'
                           WHEN 3 THEN '남성'
                           WHEN 4 THEN '여성'
                           ELSE '기타'
                       END AS GenderText,
                       A.CourseName, A.HoleNumber, A.TeamNumber,
                       A.GroupNumber, A.CourseOrder, A.AgeHandicap
                FROM Game_UserAssignment A
                INNER JOIN SYS_Users U ON A.UserId = U.UserId
                WHERE A.GameCode = @GameCode
                ORDER BY 
                    CASE 
                        WHEN A.TeamNumber LIKE 'M%' THEN 0
                        WHEN A.TeamNumber LIKE 'F%' THEN 1
                        ELSE 2
                    END,
                    A.TeamNumber ASC,
                    A.CourseOrder ASC
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                return DB_Connection.Query<AssignedPlayer>(query, new { GameCode = gameCode }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetAssignmentResult] 오류: {ex.Message}");
                return new List<AssignedPlayer>();
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public bool SaveAssignmentResult(List<AssignedPlayer> assignments)
        {
            if (assignments == null || assignments.Count == 0)
                return false;

            string gameCode = assignments.First().GameCode;

            string deleteQuery = "DELETE FROM Game_UserAssignment WHERE GameCode = @GameCode";

            string insertQuery = @"
                INSERT INTO Game_UserAssignment (
                    GameCode, UserId, CourseName, HoleNumber,
                    TeamNumber, GroupNumber, CourseOrder,
                    AgeHandicap, AssignedDate
                ) VALUES (
                    @GameCode, @UserId, @CourseName, @HoleNumber,
                    @TeamNumber, @GroupNumber, @CourseOrder,
                    @AgeHandicap, GETDATE()
                )
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                // 🧹 기존 기록 삭제
                DB_Connection.Execute(deleteQuery, new { GameCode = gameCode });

                // 📝 새 배정 저장
                foreach (var player in assignments)
                {
                    DB_Connection.Execute(insertQuery, new
                    {
                        GameCode = player.GameCode,
                        UserId = player.UserId,
                        CourseName = player.CourseName,
                        HoleNumber = player.HoleNumber,
                        TeamNumber = player.TeamNumber,
                        GroupNumber = player.GroupNumber,
                        CourseOrder = player.CourseOrder,
                        AgeHandicap = player.AgeHandicap
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveAssignmentResult] 저장 실패: {ex.Message}");
                return false;
            }
            finally
            {
                DB_Connection.Close();
            }
        }

        public void UpdateGameSetting(string gameCode, string settingCode)
        {
            string query = @"
                UPDATE Game_List
                SET GameSetting = @SettingCode
                WHERE GameCode = @GameCode
            ";

            try
            {
                if (DB_Connection.State != ConnectionState.Open)
                    DB_Connection.Open();

                DB_Connection.Execute(query, new
                {
                    SettingCode = settingCode,
                    GameCode = gameCode
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateGameSetting] 오류: {ex.Message}");
            }
            finally
            {
                DB_Connection.Close();
            }
        }

    }
}