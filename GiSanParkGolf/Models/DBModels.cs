using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GiSanParkGolf.Models
{
    /// <summary>
    /// GameListModel 클래스: Game_List 테이블과 일대일 매핑되는 ViewModel 클래스
    /// </summary>
    public class GameListModel
    {
        [Display(Name = "GameCode")]
        [Required(ErrorMessage = "* GameCode를 생성해 주십시오.")]
        public string GameCode { get; set; }

        [Display(Name = "대회일자")]
        [Required(ErrorMessage = "* 대회일자를 입력하여 주십시오.")]
        public DateTime GameDate { get; set; }

        [Display(Name = "개최지")]
        [Required(ErrorMessage = "* 개최지를 입력하여 주십시오.")]
        public string StadiumName { get; set; }

        [Display(Name = "개최주관")]
        [Required(ErrorMessage = "* 개최주관을 입력하여 주십시오.")]
        public string GameHost { get; set; }

        [Display(Name = "모집시작 일자")]
        [Required(ErrorMessage = "* 모집 일자를 입력하여 주십시오.")]
        public DateTime StartRecruiting { get; set; }

        [Display(Name = "모집종료 일자")]
        [Required(ErrorMessage = "* 모집 일자를 입력하여 주십시오.")]
        public DateTime EndRecruiting { get; set; }

        [Display(Name = "1홀당 최대 참가자")]
        [Required(ErrorMessage = "* 최대 참가자를 입력하여 주십시오.")]
        public int HoleMaximum { get; set; }

        public DateTime PostDate { get; set; }
        public string PostIP { get; set; }
        public string PostUser { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifyIP { get; set; }
        public string ModifyUser { get; set; }

        [Display(Name = "대회명")]
        [Required(ErrorMessage = "* 대회명을 입력하여 주십시오.")]
        public string GameName { get; set; }
        public string GameNote { get; set; }
        public string GameStatus { get; set; }
        public int RowNumber { get; set; }

        [Display(Name = "참가인원 수")]
        public int ParticipantNumber { get; set; }
    }

    /// <summary>
    /// UserViewModel 클래스: SYS_Users 테이블과 일대일 매핑되는 ViewModel 클래스
    /// </summary>
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserPassword { get; set; }
        public string UserName { get; set; }
        public string UserWClass { get; set; }
        public int UserClass { get; set; }
        public string UserNote { get; set; }
        public DateTime UserRegistrationDate { get; set; }
        public int UserNumber { get; set; }
    }

    /// <summary>
    /// SelectUserViewModel 클래스: ...
    /// </summary>
    public class SelectUserViewModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public int UserNumber { get; set; }
        public int UserGender { get; set; }
        public string UserAddress { get; set; }
        public string UserAddress2 { get; set; }
        public DateTime UserRegistrationDate { get; set; }
        public string UserNote { get; set; }
        public string UserWClass { get; set; }
        public int UserClass { get; set; }
    }

    /// <summary>
    /// GameJoinUserModel 클래스: Game_JoinUser 테이블과 매핑
    /// </summary>
    public class GameJoinUserModel
    {
        [Display(Name = "JoinId")]
        public string JoinId { get; set; }

        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Display(Name = "JoinDate")]
        public DateTime JoinDate { get; set; }

        [Display(Name = "JoinIP")]
        public string JoinIP { get; set; }

        [Display(Name = "JoinStatus")]
        public string JoinStatus { get; set; }

        [Display(Name = "GameCode")]
        public string GameCode { get; set; }
    }

    public class  GameJoinUserList
    {
        public int RowNumber { get; set; }

        [Display(Name = "JoinId")]
        public string JoinId { get; set; }

        [Display(Name = "UserId")]
        public string UserId { get; set; }
        
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "UserNumber")]
        public string UserNumber { get; set; }
    }

    public class PlayerHandicapViewModel
    {
        public string UserId { get; set; }    // 참가자 ID (SYS_Users.UserId)
        public string UserName { get; set; }    // 참가자 이름
        public string GameCode { get; set; }    // 대회 코드
        public string GameName { get; set; }    // 대회 이름
        public int Handicap { get; set; }    // 핸디캡 값
        public string Source { get; set; }    // Manual / Auto
        public DateTime LastUpdated { get; set; }    // 마지막 수정일
    }

    //핸디캡 테이블 (SYS_Handicap)
    public class SYS_Handicap
    {
        public string UserId { get; set; }        // NVARCHAR(15)
        public int AgeHandicap { get; set; }      // 나이 기반 핸디캡 점수
        public string Source { get; set; }        // "자동" or "수동"
        public DateTime LastUpdated { get; set; } // 마지막 갱신일
        public string LastUpdatedBy { get; set; } // 마지막 갱신자 ID
    }

     // 화면 출력용 뷰 모델
    public class UserWithHandicap
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserNumber { get; set; }

        public int Age { get; set; }              // 생년월일 → 계산된 나이
        public int AgeHandicap { get; set; }      // 핸디캡 점수
        public string Source { get; set; }
        public DateTime LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; } // 마지막 갱신자 ID
    }

    public class HandicapChangeLog
    {
        public int LogId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }   // 사용자 이름 속성 추가됨
        public int Age { get; set; }
        public int PrevHandicap { get; set; }
        public int NewHandicap { get; set; }
        public string PrevSource { get; set; }
        public string NewSource { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string Reason { get; set; }
    }
}