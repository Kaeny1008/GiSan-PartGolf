using System;

namespace GisanParkGolf.Pages.Admin.ViewModels
{
    // 핸디캡 변경 로그를 화면에 보여주기 위한 전용 모델
    public class HandicapChangeLogViewModel
    {
        public int LogId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Age { get; set; }
        public int PrevHandicap { get; set; }
        public int NewHandicap { get; set; }
        public string PrevSource { get; set; } = string.Empty;
        public string NewSource { get; set; } = string.Empty;
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Reason { get; set; }
    }
}