namespace GisanParkGolf_Core.Helpers
{
    public static class GameStatusHelper
    {
        // 한글 → 영문 변환
        public static string? ToStatusCode(string? display)
        {
            if (string.IsNullOrWhiteSpace(display)) return null;
            return display.Trim() switch
            {
                "준비중" => "Ready",
                "모집중" => "Recruiting",
                "모집완료" => "Recruiting End",
                "완료" => "Completed",
                "취소됨" => "Cancelled",
                _ => display // 이미 영문일 수도 있음
            };
        }

        // 영문 → 한글 변환 (표시용)
        public static string ToDisplay(string? status)
        {
            return status switch
            {
                "Ready" => "준비중",
                "Recruiting" => "모집중",
                "Recruiting End" => "모집완료",
                "Completed" => "완료",
                "Cancelled" => "취소됨",
                _ => status ?? ""
            };
        }
    }
}