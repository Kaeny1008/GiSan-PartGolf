using System;
using System.ComponentModel.Design;

namespace Dul
{
    public static class StringLibrary
    {
        /// <summary>
        /// 주어진 문자열을 주어진 길이만큼만 잘라서 반환, 나머지 부분은 '...'을 붙임
        /// </summary>
        /// <param name="cut">원본 문자열</param>
        /// <param name="length">잘라낼 길이</param>
        /// <returns>안녕하세요. => 안녕...</returns>
        public static string CutString(
            this string cut, int length, string suffix = "...")
        {
            if (cut.Length > (length - 3))
            {
                return cut.Substring(0, length - 3) + "" + suffix;
            }
            return cut;
        }

        /// <summary>
        /// 유니코드 이모티콘을 포함한 문자열 자르기
        /// </summary>
        /// <param name="str">한글, 영문, 유니코드 문자열</param>
        /// <param name="length">자를 문자열의 길이</param>
        /// <returns>잘라진 문자열: 안녕하세요. => 안녕...</returns>
        public static string CutStringUnicode(
            this string str, int length, string suffix = "...")
        {
            string result = str;

            if (length > 4) // 마이너스 값 들어오는 경우 제외 
            {
                var si = new System.Globalization.StringInfo(str);
                var l = si.LengthInTextElements;

                if (l > (length - 3))
                {
                    result = si.SubstringByTextElements(0, length - 3) + "" + suffix;
                }
            }

            return result;
        }

        /// <summary>
        /// 경기상태 문자변환
        /// </summary>
        /// <param name="str">DB의 경기상태</param>
        /// <param name="startdate">모집시작 일시</param>
        /// <param name="startdate">모집종료 일시</param>
        /// <returns>준비중, 모집중, 종료</returns>
        public static string StatusStringConvert(string str, DateTime startdate, DateTime enddate)
        {
            string result;
            switch (str)
            {
                case "End":
                    result = "대회종료";
                    break;
                default:
                    DateTime nowDate = DateTime.Now;
                    DateTime newStartDate = new DateTime(startdate.Year, startdate.Month, startdate.Day, 0, 0, 0);
                    DateTime newEndDate = new DateTime(enddate.Year, enddate.Month, enddate.Day, 23, 59, 59);
                    if (nowDate >= newStartDate && nowDate <= newEndDate)
                    {
                        result = "모집중";
                    } 
                    else
                    {
                        result = "준비중";
                    }
                    break;
            }

            return result;
        }
    }
}
