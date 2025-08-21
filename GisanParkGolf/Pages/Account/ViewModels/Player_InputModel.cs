using GisanParkGolf.Data;
using System.ComponentModel.DataAnnotations;

namespace GisanParkGolf.ViewModels.Account
{
    public class Player_InputModel : UserUpdate_InputModel
    {
        // 관리자만 설정할 수 있는 추가 속성
        [Required(ErrorMessage = "가입 상태를 선택해주세요.")]
        [Display(Name = "가입 상태")]
        public string UserWClass { get; set; } = "승인대기";

        [Required(ErrorMessage = "회원등급을 선택해야 합니다.")]
        [Display(Name = "회원 등급")]
        public int UserClass { get; set; } = 3;
    }
}
