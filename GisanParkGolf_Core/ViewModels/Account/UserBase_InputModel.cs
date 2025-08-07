using System.ComponentModel.DataAnnotations;

namespace GisanParkGolf_Core.ViewModels.Account
{
    public abstract class UserBase_InputModel
    {
        [Required(ErrorMessage = "이름은 필수입니다.")]
        [Display(Name = "이름")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "생년월일은 필수입니다.")]
        [Display(Name = "생년월일")]
        [ValidBirthdate] // 우리가 만든 똑똑한 검사 규칙을 여기에 딱!
        public string UserNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "성별은 필수입니다.")]
        [RegularExpression(@"^[1-4]$", ErrorMessage = "성별은 1~4 사이의 숫자로 입력해주세요.")]
        [Display(Name = "성별")]
        public string UserGender { get; set; } = string.Empty;

        [Required(ErrorMessage = "주소는 필수입니다.")]
        [Display(Name = "주소")]
        public string UserAddress { get; set; } = string.Empty;

        [Display(Name = "상세주소")]
        public string? UserAddress2 { get; set; }

        [Display(Name = "비고")]
        [DataType(DataType.MultilineText)]
        public string? UserNote { get; set; }
    }
}