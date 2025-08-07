using System.ComponentModel.DataAnnotations;

namespace GisanParkGolf_Core.Data
{
    public class PagingButtonViewModel
    {
        public string Text { get; set; } = string.Empty;
        public int PageIndex { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsCurrent { get; set; } = false;
    }

    // ===================================================================
    // ★★★ 레벨 1: 순수한 '개인 정보'만 담는 최상위 부모 ★★★
    // ===================================================================
    public abstract class UserBase_InputModel
    {
        [Required(ErrorMessage = "이름은 필수입니다.")]
        [Display(Name = "이름")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "생년월일은 필수입니다.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "생년월일은 6자리 숫자로 입력해주세요.")]
        [Display(Name = "생년월일")]
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

    // ===================================================================
    // ★★★ 레벨 2: '선택적 비밀번호 변경' 기능을 가진 중간 부모 ★★★
    // ===================================================================
    public abstract class UserUpdate_InputModel : UserBase_InputModel
    {
        [Required]
        [Display(Name = "아이디")]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "새 비밀번호")]
        [StringLength(12, MinimumLength = 4, ErrorMessage = "비밀번호는 4~12자리여야 합니다.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Display(Name = "새 비밀번호 확인")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "새 비밀번호가 일치하지 않습니다.")]
        public string? ConfirmPassword { get; set; }
    }

    // ===================================================================
    // ★★★ 최종 자식들: 각자 필요한 부모를 상속받아 완성! ★★★
    // ===================================================================

    /// <summary>
    /// 회원가입 모델: '개인정보' + '필수 ID/비밀번호'
    /// </summary>
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

    /// <summary>
    /// 내 정보 수정 모델: '선택적 비밀번호 변경' 기능이 필요
    /// </summary>
    public class MyInfo_InputModel : UserUpdate_InputModel
    {

    }

    /// <summary>
    /// 관리자용 회원 수정 모델: '선택적 비밀번호 변경' 기능이 필요
    /// </summary>
    public class Player_InputModel : UserUpdate_InputModel
    {
        // 관리자만 설정할 수 있는 추가 속성
        public bool IsApproved { get; set; }

        [Required(ErrorMessage = "회원등급을 선택해야 합니다.")]
        [Display(Name = "회원 등급")]
        public int UserClass { get; set; } = 3;
    }
}