using System;

namespace GisanParkGolf.Pages.AdminPage.ViewModels
{
    /// <summary>
    /// 대회 참가자 명단(및 미배정 인원 등)에 사용되는 참가자 ViewModel
    /// </summary>
    public class GameJoinUserListViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int? UserNumber { get; set; }
        public int? UserGender { get; set; }
        public int? AgeHandicap { get; set; } 
        public string? AwardsSummary { get; set; }
        public bool? IsCancelled { get; set; }
        public DateTime? CancelDate { get; set; }
        public string? JoinStatus { get; set; }

        public string FormattedBirthDate
        {
            get
            {
                if (UserNumber.HasValue)
                {
                    var str = UserNumber.Value.ToString();
                    if (str.Length == 6) // 예: 850412
                        return $"{str.Substring(0, 2)}-{str.Substring(2, 2)}-{str.Substring(4, 2)}";
                    else
                        return str; // 길이가 8이 아니면 그냥 숫자 반환
                }
                return string.Empty;
            }
        }

        public string UserGenderText
        {
            get
            {
                if (UserGender == 1 || UserGender == 3)
                    return "남자";
                else if (UserGender == 2 || UserGender == 4)
                    return "여자";
                else
                    return "알수없음";
            }
        }

        public string AgeText
        {
            get
            {
                if (UserNumber.HasValue && UserGender.HasValue)
                {
                    return Helpers.PersonInfoCalculator.CalculateAge(UserNumber.Value, UserGender.Value) + "세";
                }
                else
                {
                    return string.Empty; // 또는 "미상", "정보 없음" 등 원하는 문구
                }
            }
        }
    }
}