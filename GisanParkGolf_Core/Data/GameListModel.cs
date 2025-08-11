using System;

namespace GisanParkGolf_Core.Data
{
    public class MyGameListModel
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
        public string? StadiumName { get; set; }
        public string? GameHost { get; set; }
        public DateTime GameDate { get; set; }
        public DateTime StartRecruiting { get; set; }
        public DateTime EndRecruiting { get; set; }
        public string? GameStatus { get; set; }
        public string? IsCancelledText { get; set; }
    }
}