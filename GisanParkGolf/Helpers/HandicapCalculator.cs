namespace GisanParkGolf_Core.Helpers
{
    public static class HandicapCalculator
    {
        public static int CalculateByAge(int age)
        {
            if (age >= 80) return 8;
            if (age >= 70) return 6;
            if (age >= 60) return 4;
            if (age >= 50) return 2;
            return 0;
        }
    }
}