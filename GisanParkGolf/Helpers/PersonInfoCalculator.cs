using System;

namespace GisanParkGolf.Helpers
{
    public static class PersonInfoCalculator
    {
        /// <summary>
        /// 주민등록번호 앞 6자리와 성별 코드를 기반으로 만 나이를 계산합니다.
        /// </summary>
        /// <param name="birth">생년월일 6자리 (예: 990101)</param>
        /// <param name="genderNum">성별 코드 (1,2,5,6: 1900년대 / 3,4,7,8: 2000년대)</param>
        /// <returns>계산된 만 나이</returns>
        public static int CalculateAge(int birth, int genderNum)
        {
            // 유효하지 않은 생년월일 형식은 0을 반환
            if (birth < 100000) return 0;
            string birthStr = birth.ToString("D6"); // 6자리로 만듦 (예: 990101)

            try
            {
                // 성별 코드를 기반으로 출생년도 앞자리를 결정
                int yearPrefix = (genderNum == 1 || genderNum == 2 || genderNum == 5 || genderNum == 6) ? 1900 : 2000;
                int year = yearPrefix + int.Parse(birthStr.Substring(0, 2));
                int month = int.Parse(birthStr.Substring(2, 2));
                int day = int.Parse(birthStr.Substring(4, 2));

                DateTime birthDate = new DateTime(year, month, day);

                // 만 나이 계산
                int age = DateTime.Today.Year - birthDate.Year;
                // 생일이 지나지 않았으면 1살을 뺀다.
                if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;

                return age;
            }
            catch
            {
                // 날짜 형식이 잘못된 경우 (예: 2월 30일) 0을 반환
                return 0;
            }
        }
    }
}