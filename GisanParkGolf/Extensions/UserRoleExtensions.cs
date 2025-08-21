namespace GisanParkGolf.Extensions
{
    public static class UserRoleExtensions
    {
        public static string ToRoleName(this int userClass)
        {
            return userClass switch
            {
                0 => "Super Admin",
                1 => "Administrator ",
                2 => "Manager ",
                3 => "Member",
                _ => "알수없음"
            };
        }
    }
}
