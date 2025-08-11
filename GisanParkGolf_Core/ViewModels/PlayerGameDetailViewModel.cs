using System;

namespace GisanParkGolf_Core.ViewModels
{
    public class PlayerGameDetailViewModel
    {
        public string GameCode { get; set; }
        public string GameName { get; set; }
        public DateTime GameDate { get; set; }
        public string StadiumName { get; set; }
        public string GameHost { get; set; }
        public string PlayMode { get; set; }
        public int HoleMaximum { get; set; }
        public DateTime StartRecruiting { get; set; }
        public DateTime EndRecruiting { get; set; }
        public string GameNote { get; set; }
        public int ParticipantNumber { get; set; }
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public string CancelledBy { get; set; }
        public string AssignmentStatus { get; set; }
        public int IsCancelled { get; set; }
    }
}