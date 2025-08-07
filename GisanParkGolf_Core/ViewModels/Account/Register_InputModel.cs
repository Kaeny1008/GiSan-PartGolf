using System.ComponentModel.DataAnnotations;

namespace GisanParkGolf_Core.ViewModels.Account
{
    public class Register_InputModel : UserBase_InputModel
    {
        [Required(ErrorMessage = "아이디는 필수입니다.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "4~15자리여야 합니다.")]
        [RegularExpression(@"^[a-zA-Z][0-9a-zA-Z]{3,14}$", ErrorMessage = "문자로 시작하는 4~15자리 영문/숫자")]
        [Display(Name = "아이디")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호는 필수입니다.")]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "4~12자리여야 합니다.")]
        [DataType(DataType.Password)]
        [Display(Name = "비밀번호")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호 확인은 필수입니다.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다.")]
        [Display(Name = "비밀번호 확인")]
        public string PasswordConfirm { get; set; } = string.Empty;
    }
}
