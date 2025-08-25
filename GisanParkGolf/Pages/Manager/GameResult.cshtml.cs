using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace GiSanParkGolf.Pages.Manager
{
    public class GameResultModel : PageModel
    {
        public List<TeamScoreCourse> TeamScoreCourses { get; set; } = new();

        public void OnGet()
        {
            // 예시 데이터: 코스별, 팀단위로 생성
            TeamScoreCourses = new List<TeamScoreCourse>
            {
                new TeamScoreCourse
                {
                    CourseName = "A 코스",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    TeamRows = new List<TeamScoreRow>
                    {
                        new TeamScoreRow { CourseOrder=1, TeamNumber="T1", ParticipantName="회원2", ParticipantId="A122" },
                        new TeamScoreRow { CourseOrder=2, TeamNumber="T1", ParticipantName="회원4", ParticipantId="A125" },
                        new TeamScoreRow { CourseOrder=3, TeamNumber="T1", ParticipantName="회원3", ParticipantId="A123" },
                        new TeamScoreRow { CourseOrder=4, TeamNumber="T1", ParticipantName="회원44", ParticipantId="A124" },
                        new TeamScoreRow { CourseOrder=5, TeamNumber="T1", ParticipantName="관리자", ParticipantId="1" }
                    }
                },
                new TeamScoreCourse
                {
                    CourseName = "B 코스",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    TeamRows = new List<TeamScoreRow>
                    {
                        new TeamScoreRow { CourseOrder=1, TeamNumber="T2", ParticipantName="회원2", ParticipantId="A122" },
                        new TeamScoreRow { CourseOrder=2, TeamNumber="T2", ParticipantName="회원4", ParticipantId="A125" },
                        new TeamScoreRow { CourseOrder=3, TeamNumber="T2", ParticipantName="회원3", ParticipantId="A123" },
                        new TeamScoreRow { CourseOrder=4, TeamNumber="T2", ParticipantName="회원44", ParticipantId="A124" },
                        new TeamScoreRow { CourseOrder=5, TeamNumber="T2", ParticipantName="관리자", ParticipantId="1" }
                    }
                }
            };
            // 점수는 처음엔 빈 값. 필요하면 TeamRows[i].Scores[h] = 점수; 형태로 채움
        }

        public class TeamScoreCourse
        {
            public string CourseName { get; set; }
            public List<int> HoleNumbers { get; set; } = new();
            public List<TeamScoreRow> TeamRows { get; set; } = new();
        }

        public class TeamScoreRow
        {
            public int CourseOrder { get; set; }
            public string TeamNumber { get; set; }
            public string ParticipantName { get; set; }
            public string ParticipantId { get; set; }
            public Dictionary<int, int> Scores { get; set; } = new(); // key: 홀번호, value: 점수
        }
    }
}