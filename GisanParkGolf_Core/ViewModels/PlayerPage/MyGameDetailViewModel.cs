using GisanParkGolf_Core.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace GisanParkGolf_Core.ViewModels.PlayerPage
{
    public class MyGameDetailViewModel
    {
        public string? GameCode { get; set; }
        public string? GameName { get; set; }
        public DateTime GameDate { get; set; }
        public string? StadiumName { get; set; }
        public string? PlayMode { get; set; }

        [NotMapped]
        public string? PlayModeDisplay
        {
            get
            {
                return PlayMode switch
                {
                    PlayModeType.Stroke => "스트로크",
                    PlayModeType.Match => "매치",
                    PlayModeType.Stableford => "스테이블포드",
                    PlayModeType.Skins => "스킨스",
                    PlayModeType.FourBall => "포볼",
                    PlayModeType.Foursome => "4인조",
                    _ => PlayMode
                };
            }
        }
        public string? GameHost { get; set; }
        public int HoleMaximum { get; set; }
        public DateTime StartRecruiting { get; set; }
        public DateTime EndRecruiting { get; set; }
        public string? GameNote { get; set; }
        public int ParticipantNumber { get; set; }
        public int? IsCancelled { get; set; } // 0:참가중, 1:취소, null:기록없음
        public DateTime? CancelDate { get; set; }
        public string? CancelReason { get; set; }
        public string? AssignmentStatus { get; set; }
        public string? Approval { get; set; } // 관리자 승인값
    }
    public class PlayModeType
    {
        public const string Stroke = "Stroke";
        public const string Match = "Match";
        public const string Stableford = "Stableford";
        public const string Skins = "Skins";
        public const string FourBall = "FourBall";
        public const string Foursome = "Foursome";
    }
}
