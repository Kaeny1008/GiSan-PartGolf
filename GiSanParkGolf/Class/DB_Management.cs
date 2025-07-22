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
                Global.uvm.UserId = sqlDR.GetString(0);
                Global.uvm.UserPassword = sqlDR.GetString(1);
                Global.uvm.UserName = sqlDR.GetString(2);
                Global.uvm.UserWClass = sqlDR.GetString(3);
                Global.uvm.UserClass = sqlDR.GetInt32(4);
            }
            DB_Connection.Close();

            SetCookie(Global.uvm.UserId,
                Global.uvm.UserPassword,
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
        public List<GameListModel> GetGameReadyList(string field, string keyword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);

            return DB_Connection.Query<GameListModel>(
                "sp_Get_Game_ReadyList",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public string GetEarlyJoin(string gameCode, string userID)
        {
            string strSQL = "SELECT JoinStatus FROM Game_JoinUser WHERE UserId = @Id AND GameCode = @GameCode";
            var parameters = new DynamicParameters(new
            {
                Id = userID,
                GameCode = gameCode
            });

            return DB_Connection.Query<string>(strSQL, parameters, commandType: CommandType.Text).SingleOrDefault();
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
            return DB_Connection.Query<GameListModel>("sp_Get_Player_MyGame", parameters,
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

        public List<GameJoinUserList> GetGameUserList(string gameCode)
        {
            var parameters = new DynamicParameters(new
            {
                GameCode = gameCode
            });

            return DB_Connection.Query<GameJoinUserList>("sp_GameJoinUser", 
                parameters, commandType: CommandType.StoredProcedure).ToList();
        }

        /// <summary>
        /// 지정 게임의 핸디캡 목록 조회 (검색어 옵션)
        /// </summary>
        public IEnumerable<PlayerHandicapViewModel> GetHandicaps(
            string gameCode,
            string searchTerm = null)
        {
            return DB_Connection.Query<PlayerHandicapViewModel>(
                "sp_GetGameHandicaps",
                new { GameCode = gameCode, SearchTerm = searchTerm },
                commandType: CommandType.StoredProcedure);
        }

        public List<UserWithHandicap> GetUserHandicaps(string field, string keyword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);

            var result = DB_Connection.Query<UserWithHandicap>(
                "sp_Get_UserHandicaps",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            // 나이 계산 후 모델에 주입
            foreach (var user in result)
            {
                user.Age = CalculateAge(user.UserNumber.ToString());
            }

            return result;
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
                    VALUES (@UserId, @Handicap, @Source, GETDATE(), @UpdatedBy);";

            DB_Connection.Execute(sql, new
            {
                UserId = userId,
                Handicap = handicap,
                Source = source,
                UpdatedBy = updatedBy
            });
        }

        // ✅ 나이 계산 유틸리티
        public int CalculateAge(string userNumber)
        {
            if (string.IsNullOrWhiteSpace(userNumber) || userNumber.Length != 6)
                return 0;

            int yy = int.Parse(userNumber.Substring(0, 2));
            int mm = int.Parse(userNumber.Substring(2, 2));
            int dd = int.Parse(userNumber.Substring(4, 2));

            // 현재 연도와 기준 생년 조합
            DateTime today = DateTime.Today;
            int base1900 = 1900 + yy;
            int base2000 = 2000 + yy;

            // 두 후보 생년 중 어떤 것이 실제 생일에 가까운지 선택
            DateTime birth1900, birth2000;
            try
            {
                birth1900 = new DateTime(base1900, mm, dd);
                birth2000 = new DateTime(base2000, mm, dd);
            }
            catch
            {
                return 0;
            }

            int age1900 = today.Year - birth1900.Year;
            if (birth1900 > today.AddYears(-age1900)) age1900--;

            int age2000 = today.Year - birth2000.Year;
            if (birth2000 > today.AddYears(-age2000)) age2000--;

            // ✅ 나이가 0~130세인 정상 범위일 경우 선택
            if (age1900 >= 0 && age1900 <= 130) return age1900;
            if (age2000 >= 0 && age2000 <= 130) return age2000;

            return 0; // 둘 다 비정상일 경우
        }

        public void InsertHandicapChangeLog(HandicapChangeLog log)
        {
            string sql = @"
                INSERT INTO SYS_HandicapLog
                (UserId, Age, PrevHandicap, NewHandicap, PrevSource, NewSource, Reason, ChangedBy)
                VALUES (@UserId, @Age, @PrevHandicap, @NewHandicap, @PrevSource, @NewSource, @Reason, @ChangedBy);";

            DB_Connection.Execute(sql, log);
        }

        public List<HandicapChangeLog> GetHandicapChangeLogs(string field, string keyword)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);

            return DB_Connection.Query<HandicapChangeLog>(
                "sp_Get_HandicapChangeLogs",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public List<UserViewModel> GetPlayers(string field, string keyword, bool readyOnly)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Field", field);
            parameters.Add("@Keyword", keyword);
            parameters.Add("@ReadyOnly", readyOnly ? 1 : 0);

            return DB_Connection.Query<UserViewModel>(
                "sp_Get_SYS_Users",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
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

            return DB_Connection.Query<GameListModel>(
                "sp_Get_GameList",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public List<StadiumDTO> GetStadiumList()
        {
            string query = @"
        SELECT StadiumCode, StadiumName, IsActive
        FROM SYS_StadiumList
        ORDER BY StadiumCode";  // 필요에 따라 StadiumName 정렬 가능

            return DB_Connection.Query<StadiumDTO>(query).ToList();
        }

        public string GenerateNextCode(string table, string column, string prefix, int digits)
        {
            string sql = $"SELECT MAX({column}) FROM {table}";

            string maxCode = DB_Connection.QueryFirstOrDefault<string>(sql);
            int nextNumber = 1;

            if (!string.IsNullOrEmpty(maxCode) && maxCode.StartsWith(prefix))
            {
                string numericPart = maxCode.Substring(prefix.Length);
                if (int.TryParse(numericPart, out int current))
                {
                    nextNumber = current + 1;
                }
            }

            return $"{prefix}{nextNumber.ToString($"D{digits}")}";
        }

        public bool InsertStadium(string stadiumCode, string stadiumName, bool isActive)
        {
            string query = @"
        INSERT INTO SYS_StadiumList (StadiumCode, StadiumName, IsActive)
        VALUES (@StadiumCode, @StadiumName, @IsActive)";

            var param = new DynamicParameters();
            param.Add("@StadiumCode", stadiumCode);
            param.Add("@StadiumName", stadiumName);
            param.Add("@IsActive", isActive);

            try
            {
                int rowsAffected = DB_Connection.Execute(query, param);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                // 예외 로깅이나 오류 메시지 전달 처리 가능
                Console.WriteLine($"InsertStadium 오류: {ex.Message}");
                return false;
            }
        }

        public bool InsertCourse(string stadiumCode, string courseName, int holeCount, bool isActive)
        {
            string query = @"
        INSERT INTO SYS_CourseList (StadiumCode, CourseName, HoleCount, IsActive)
        VALUES (@StadiumCode, @CourseName, @HoleCount, @IsActive)";

            var param = new DynamicParameters();
            param.Add("@StadiumCode", stadiumCode);
            param.Add("@CourseName", courseName);
            param.Add("@HoleCount", holeCount);
            param.Add("@IsActive", isActive);

            try
            {
                int rowsAffected = DB_Connection.Execute(query, param);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InsertCourse 오류: {ex.Message}");
                return false;
            }
        }

        public bool InsertHoleList(int courseCode, List<HoleDTO> holeList)
        {
            string query = @"
        INSERT INTO SYS_HoleInfo (CourseCode, HoleName, Distance, Par)
        VALUES (@CourseCode, @HoleName, @Distance, @Par)";

            try
            {
                foreach (var hole in holeList)
                {
                    var param = new DynamicParameters();
                    param.Add("@CourseCode", courseCode);
                    param.Add("@HoleName", hole.HoleName);
                    param.Add("@Distance", hole.Distance);
                    param.Add("@Par", hole.Par);

                    DB_Connection.Execute(query, param);  // 개별 실행
                }

                return true;  // 전체 성공 처리
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InsertHoleList 오류: {ex.Message}");
                return false;
            }
        }

        public List<CourseDTO> GetCourseListByStadium(string stadiumCode)
        {
            string query = @"
        SELECT CourseCode,
               CourseName,
               HoleCount,
               CASE WHEN IsActive = 1 THEN N'사용' ELSE N'미사용' END AS UseStatus,
               StadiumCode
        FROM SYS_CourseList
        WHERE StadiumCode = @StadiumCode
        ORDER BY CourseCode";

            var param = new DynamicParameters();
            param.Add("@StadiumCode", stadiumCode);

            return DB_Connection.Query<CourseDTO>(query, param).ToList();
        }

        public List<HoleDTO> GetHoleListByCourse(int courseCode)
        {
            string query = @"
        SELECT HoleId, HoleName, Distance, Par, CourseCode
        FROM SYS_HoleInfo
        WHERE CourseCode = @CourseCode
        ORDER BY HoleId";

            var param = new DynamicParameters();
            param.Add("@CourseCode", courseCode);

            return DB_Connection.Query<HoleDTO>(query, param).ToList();
        }

        public int GetHoleCountByCourse(int courseCode)
        {
            string query = "SELECT HoleCount FROM SYS_CourseList WHERE CourseCode = @CourseCode";

            var param = new DynamicParameters();
            param.Add("@CourseCode", courseCode);

            using (var connection = new SqlConnection(WebConfigurationManager.ConnectionStrings["ParkGolfDB"].ConnectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<int>(query, param);
            }
        }

        public int CountStadiums(string field, string keyword)
        {
            // ⚠ 필드명은 반드시 사전 검증된 값이어야 해! (SQL Injection 방지)
            var allowedFields = new HashSet<string> { "StadiumName", "StadiumCode" };
            if (!allowedFields.Contains(field))
                field = "StadiumName";  // 기본 필드로 fallback

            string query = $@"
        SELECT COUNT(*)
        FROM SYS_StadiumList
        WHERE {field} LIKE @Keyword";

            var param = new DynamicParameters();
            param.Add("@Keyword", "%" + keyword + "%");

            return DB_Connection.QueryFirstOrDefault<int>(query, param);
        }

        public List<StadiumDTO> SearchStadiumsPaged(string field, string keyword, int pageIndex, int pageSize)
        {
            // ⚠ 필드명 검증: SQL Injection 방지
            var allowedFields = new HashSet<string> { "StadiumName", "StadiumCode" };
            if (!allowedFields.Contains(field))
                field = "StadiumName";  // 기본 필드 설정

            string query = $@"
        SELECT StadiumCode, StadiumName
        FROM SYS_StadiumList
        WHERE {field} LIKE @Keyword
        ORDER BY StadiumCode
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var param = new DynamicParameters();
            param.Add("@Keyword", "%" + keyword + "%");
            param.Add("@Offset", pageIndex * pageSize);  // 페이지 시작 위치
            param.Add("@PageSize", pageSize);            // 한 페이지당 항목 수

            return DB_Connection.Query<StadiumDTO>(query, param).ToList();
        }

        public bool UpdateHoleList(int courseCode, List<HoleDTO> holes)
        {
            string query = @"
        UPDATE SYS_HoleInfo
        SET HoleName = @HoleName, Distance = @Distance, Par = @Par
        WHERE HoleId = @HoleId AND CourseCode = @CourseCode";

            var connection = DB_Connection;

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transaction = connection.BeginTransaction();

            try
            {
                foreach (var hole in holes)
                {
                    var parameters = new
                    {
                        HoleName = hole.HoleName,
                        Distance = hole.Distance,
                        Par = hole.Par,
                        HoleId = hole.HoleId,
                        CourseCode = courseCode
                    };

                    connection.Execute(query, parameters, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}