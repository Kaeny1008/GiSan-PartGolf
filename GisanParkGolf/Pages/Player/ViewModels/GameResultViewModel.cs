namespace GisanParkGolf.Pages.Player.ViewModels
{
    public class GameResultViewModel
    {
        public string UserId { get; set; } = string.Empty; // ������ ID
        public string UserName { get; set; } = string.Empty; // ������ �̸�
        public int TotalScore { get; set; } // �� ����
        public int Rank { get; set; } // ����
        public string? Award { get; set; } // ���� ���� (���� ����)
    }
}