using GiSanParkGolf.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace GiSanParkGolf.Models
{
    /// <summary>
    /// GameListModel 클래스: Game_List 테이블과 일대일 매핑되는 ViewModel 클래스
    /// </summary>
    [Serializable]
    public class GameListModel
    {
        [Display(Name = "GameCode")]
        [Required(ErrorMessage = "* GameCode를 생성해 주십시오.")]
        public string GameCode { get; set; }

        [Display(Name = "대회일자")]
        [Required(ErrorMessage = "* 대회일자를 입력하여 주십시오.")]
        public DateTime GameDate { get; set; }

        [Display(Name = "경기장")]
        [Required(ErrorMessage = "* 경기장를 입력하여 주십시오.")]
        public string StadiumName { get; set; }

        public string StadiumCode { get; set; }

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
        public string GameSetting { get; set; }
        public int IsCancelled { get; set; } // 참가 취소 여부

        public string IsCancelledText
        {
            get
            {
                switch (IsCancelled)
                {
                    case 0:
                        return "참가";
                    case 1:
                        return "취소";
                    default:
                        return "확인불가";
                }
            }
        }
    }

    public class Select_GameList
    {
        [Display(Name = "GameCode")]
        [Required(ErrorMessage = "* GameCode를 생성해 주십시오.")]
        public string GameCode { get; set; }

        [Display(Name = "대회일자")]
        [Required(ErrorMessage = "* 대회일자를 입력하여 주십시오.")]
        public DateTime GameDate { get; set; }

        [Display(Name = "경기장")]
        [Required(ErrorMessage = "* 경기장를 입력하여 주십시오.")]
        public string StadiumName { get; set; }

        public string StadiumCode { get; set; }

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
        public string GameSetting { get; set; }
        public string PlayMode { get; set; }

        public string PlayModeToText
        {
            get
            {
                if (PlayMode == "Stroke") return "스트로크";
                if (PlayMode == "Match") return "매치";
                return "미입력";
            }
        }

        public int IsCancelled { get; set; }

        public DateTime? CancelDate { get; set; } // 취소 일자
        public string CancelReason { get; set; } // 취소 사유
        public string CancelledBy { get; set; } // 취소한 사용자 ID
        public string AssignmentStatus { get; set; }
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
        public string UserAddress { get; set; }
        public string UserAddress2 { get; set; }
        public DateTime UserRegistrationDate { get; set; }

        public int UserNumber { get; set; }     // YYMMDD
        public int UserGender { get; set; }     // 주민번호 뒷자리 성별코드

        public string FormattedBirthDate => UserNumber.ToString().PadLeft(6, '0');

        public string GenderText
        {
            get
            {
                switch (UserGender)
                {
                    case 1:
                    case 3:
                        return "남자";
                    case 2:
                    case 4:
                        return "여자";
                    default:
                        return "확인불가";
                }
            }
        }
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

    [Serializable]
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
        public int UserNumber { get; set; }

        public int UserGender { get; set; }
        public string SuggestedSlotText { get; set; }
        public string GameCode { get; set; }
        public string GameName { get; set; }

        public string FormattedBirthDate => UserNumber.ToString().PadLeft(6, '0');
        public int AgeHandicap { get; set; }    // 핸디캡 점수

        public string AwardType { get; set; }       // 수상 종류 (예: 최우수상)
        public string AwardLevel { get; set; }      // 수상 등급 (Gold, Silver 등)
        public DateTime? AwardDate { get; set; }    // 수상 날짜
        public string AwardNote { get; set; }       // 심사평 혹은 비고
        public string AwardsSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(AwardType) && !string.IsNullOrEmpty(AwardLevel))
                    return $"{AwardType} ({AwardLevel})";
                else if (!string.IsNullOrEmpty(AwardType))
                    return AwardType;
                else
                    return "없음";
            }
        }
        public string GenderText
        {
            get
            {
                switch (UserGender)
                {
                    case 1:
                    case 3:
                        return "남자";
                    case 2:
                    case 4:
                        return "여자";
                    default:
                        return "확인불가";
                }
            }
        }
        public string AgeText
        {
            get
            {
                int age = Helper.CalculateAge(UserNumber, UserGender);
                return (age > 0) ? age + "세" : "정보없음";
            }
        }

        public string TeamNumber { get; set; } // 팀 번호 (optional)
        public int IsCancelled { get; set; } // 참가 취소 여부
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
        public int UserNumber { get; set; }

        public int UserGender { get; set; }

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

    public class StadiumList
    {
        public string StadiumCode { get; set; }          // STD0001
        public string StadiumName { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Serializable]
    public class CourseList
    {
        public string CourseCode { get; set; }          // CO001
        public string StadiumCode { get; set; }
        public string CourseName { get; set; }
        public int HoleCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ActiveStatus => IsActive ? "사용" : "미사용";
    }

    [Serializable]
    public class HoleList
    {
        public int HoleId { get; set; }               // PK
        public string CourseCode { get; set; }
        public string HoleName { get; set; }
        public int Distance { get; set; }
        public int Par { get; set; }

        public int HoleNo
        {
            get
            {
                // HoleName이 "1홀", "10홀" 등일 때 숫자 부분만 추출
                if (!string.IsNullOrEmpty(HoleName))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(HoleName, @"^\d+");
                    if (match.Success && int.TryParse(match.Value, out int holeNumber))
                    {
                        return holeNumber;
                    }
                }
                return 0; // 파싱 실패 시 0 반환
            }
        }
    }

    [Serializable]
    public class AssignedPlayer
    {
        public int RowNumber { get; set; }         //그리드 번호
        public string UserId { get; set; }           // 참가자 ID
        public string UserName { get; set; }         // 참가자 이름
        public int AgeHandicap { get; set; }         // 핸디캡
        public string GameCode { get; set; }         // 대회 코드
        public string CourseName { get; set; }       // 배정된 코스명
        public int CourseOrder { get; set; }         // 코스 내 순서
        public int GroupNumber { get; set; }         // 그룹 번호 (optional)
        public string HoleNumber { get; set; }
        public string TeamNumber { get; set; }
        public int UserNumber { get; set; }
        public int UserGender { get; set; }
        public string FormattedBirthDate => UserNumber.ToString().PadLeft(6, '0');
        public string GenderText
        {
            get
            {
                switch (UserGender)
                {
                    case 1:
                    case 3:
                        return "남자";
                    case 2:
                    case 4:
                        return "여자";
                    default:
                        return "확인불가";
                }
            }
        }
        public string AgeText
        {
            get
            {
                int age = Helper.CalculateAge(UserNumber, UserGender);
                return (age > 0) ? age + "세" : "정보없음";
            }
        }
        public string AwardType { get; set; }       // 수상 종류 (예: 최우수상)
        public string AwardLevel { get; set; }      // 수상 등급 (Gold, Silver 등)
        public DateTime? AwardDate { get; set; }    // 수상 날짜
        public string AwardNote { get; set; }       // 심사평 혹은 비고
        public string AwardsSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(AwardType) && !string.IsNullOrEmpty(AwardLevel))
                    return $"{AwardType} ({AwardLevel})";
                else if (!string.IsNullOrEmpty(AwardType))
                    return AwardType;
                else
                    return "없음";
            }
        }
        public string AssignmentStatus { get; set; } // 배정 상태 (예: "배정완료", "대기중" 등)
        public string Summary =>
            $"{UserName} (핸디캡 {AgeHandicap}) → {CourseName} {CourseOrder}번";
        public int CancelPlayer { get; set; } = 0; // 취소 참가자 여부 (0: 기존 참가자, 1: 취소 참가자)
    }

    public class PlayerAssignmentOptions
    {
        public bool UseHandicap { get; set; }
        public bool UseGender { get; set; }
        public bool UseAgeGroup { get; set; }
        public bool UseAwards { get; set; }
        public bool UseSeparateHolePerTeam { get; set; }
        public string PlayMode { get; set; }  // "Stroke", "Match" 등
    }
}