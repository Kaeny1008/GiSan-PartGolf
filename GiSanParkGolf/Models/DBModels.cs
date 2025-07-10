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
        public string UserID { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string UserWClass { get; set; }
        public int UserClass { get; set; }
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
}