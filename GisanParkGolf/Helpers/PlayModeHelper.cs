namespace GisanParkGolf_Core.Helpers
{
    public static class PlayModeHelper
    {
        public static string ToKorDisplay(string? playMode)
        {
            return playMode switch
            {
                "Stroke" => "스트로크",
                "Match" => "매치",
                "Stableford" => "스테이블포드",
                "Skins" => "스킨스",
                "Foursome" => "포섬",
                "Fourball" => "포볼",
                _ => playMode ?? "(미지정)"
            };
        }

        public static string ToEngDisplay(string? playMode)
        {
            return playMode switch
            {
                "스트로크" => "Stroke",
                "매치" => "Match",
                "스테이블포드" => "Stableford",
                "스킨스" => "Skins",
                "포섬" => "Foursome",
                "포볼" => "Fourball",
                _ => playMode ?? "(Not Assigned)"
            };
        }
    }
}
