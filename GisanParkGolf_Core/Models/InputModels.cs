using System.ComponentModel.DataAnnotations;

namespace GisanParkGolf_Core.Models
{
    public class InputModels
    {
    }

    public class Register_InputModel
    {
        [Required(ErrorMessage = "아이디는 필수입니다.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "4~15자리여야 합니다.")]
        [RegularExpression(@"^[a-zA-Z][0-9a-zA-Z]{3,14}$", ErrorMessage = "문자로 시작하여 4~15자리의 문자/숫자")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호는 필수입니다.")]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "4~12자리여야 합니다.")]
        [RegularExpression(@".*[!@#$%^&*/].*", ErrorMessage = "특수문자(!@#$%^&*/) 포함")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "비밀번호 확인은 필수입니다.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다.")]
        public string PasswordConfirm { get; set; } = string.Empty;

        [Required(ErrorMessage = "이름은 필수입니다.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "생년월일은 필수입니다.")]
        [RegularExpression(@"^\d{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "YYMMDD형식, 월(01~12), 일(01~31)만 입력")]
        public string BirthDay { get; set; } = string.Empty;

        [Required(ErrorMessage = "성별은 필수입니다.")]
        [RegularExpression(@"^[1-4]{1}$", ErrorMessage = "남자(1,3) 여자(2,4) 중 하나를 입력 하세요.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "주소는 필수입니다.")]
        public string Address { get; set; } = string.Empty;

        // string을 string?으로 변경하여 Null을 허용하도록 수정
        public string? Address2 { get; set; }

        // string을 string?으로 변경하여 Null을 허용하도록 수정
        public string? Memo { get; set; }
    }
}
