namespace GisanParkGolf.Helpers
{
    public static class StringHelper
    {
        // 한글, 영문 모두 안전하게 자르는 함수 (유니코드 고려, 필요시 개선)
        public static string CutStringUnicode(string? input, int length)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Length <= length ? input : input.Substring(0, length) + "...";
        }
    }
}