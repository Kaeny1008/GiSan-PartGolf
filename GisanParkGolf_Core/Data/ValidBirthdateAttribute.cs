using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

// ★★★ 우리가 직접 만드는 "생년월일 유효성 검사" 규칙! ★★★
public class ValidBirthdateAttribute : ValidationAttribute
{
    // 이 메서드가 실제 검사를 수행하는 핵심 로직이야.
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // 1. 입력값이 문자열(string)인지 확인
        if (value is string birthdateString)
        {
            // 2. 길이가 정확히 6자리인지 확인
            if (birthdateString.Length != 6)
            {
                // IsValid 메서드 안에서는 ErrorMessage를 직접 쓰지 않고,
                // 이렇게 ValidationResult를 반환하는 게 정석이야.
                return new ValidationResult("생년월일은 6자리 숫자로 입력해야 합니다.");
            }

            try
            {
                // 3. 월(MM)과 일(DD) 부분만 잘라내기
                var month = int.Parse(birthdateString.Substring(2, 2));
                var day = int.Parse(birthdateString.Substring(4, 2));

                // 4. 월과 일이 유효한 범위에 있는지 확인
                if (month < 1 || month > 12)
                {
                    return new ValidationResult("태어난 월(月)은 1월부터 12월 사이여야 합니다.");
                }

                if (day < 1 || day > 31)
                {
                    return new ValidationResult("태어난 일(日)은 1일부터 31일 사이여야 합니다.");
                }
            }
            catch (FormatException)
            {
                // Substring이나 int.Parse에서 에러가 나면, 숫자가 아니라는 뜻.
                return new ValidationResult("생년월일은 숫자로만 입력해야 합니다.");
            }
        }

        // 모든 검사를 통과하면, 성공!
        return ValidationResult.Success;
    }
}