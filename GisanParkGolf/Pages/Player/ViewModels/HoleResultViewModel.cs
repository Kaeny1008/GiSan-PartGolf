namespace GisanParkGolf.Pages.Player.ViewModels
{
    public class HoleResultViewModel
    {
        public string CourseName { get; set; } = string.Empty; // �ڽ� �̸�
        public int HoleId { get; set; } // Ȧ ID
        public string HoleName { get; set; } = string.Empty; // Ȧ �̸�
        public int Score { get; set; } // ����
    }
}