using System.ComponentModel.DataAnnotations;

namespace GisanParkGolf.ViewModels.Account
{
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
}
