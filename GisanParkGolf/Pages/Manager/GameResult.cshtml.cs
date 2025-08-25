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
            // ���� ������: �ڽ���, �������� ����
            TeamScoreCourses = new List<TeamScoreCourse>
            {
                new TeamScoreCourse
                {
                    CourseName = "A �ڽ�",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    TeamRows = new List<TeamScoreRow>
                    {
                        new TeamScoreRow { CourseOrder=1, TeamNumber="T1", ParticipantName="ȸ��2", ParticipantId="A122" },
                        new TeamScoreRow { CourseOrder=2, TeamNumber="T1", ParticipantName="ȸ��4", ParticipantId="A125" },
                        new TeamScoreRow { CourseOrder=3, TeamNumber="T1", ParticipantName="ȸ��3", ParticipantId="A123" },
                        new TeamScoreRow { CourseOrder=4, TeamNumber="T1", ParticipantName="ȸ��44", ParticipantId="A124" },
                        new TeamScoreRow { CourseOrder=5, TeamNumber="T1", ParticipantName="������", ParticipantId="1" }
                    }
                },
                new TeamScoreCourse
                {
                    CourseName = "B �ڽ�",
                    HoleNumbers = Enumerable.Range(1,9).ToList(),
                    TeamRows = new List<TeamScoreRow>
                    {
                        new TeamScoreRow { CourseOrder=1, TeamNumber="T2", ParticipantName="ȸ��2", ParticipantId="A122" },
                        new TeamScoreRow { CourseOrder=2, TeamNumber="T2", ParticipantName="ȸ��4", ParticipantId="A125" },
                        new TeamScoreRow { CourseOrder=3, TeamNumber="T2", ParticipantName="ȸ��3", ParticipantId="A123" },
                        new TeamScoreRow { CourseOrder=4, TeamNumber="T2", ParticipantName="ȸ��44", ParticipantId="A124" },
                        new TeamScoreRow { CourseOrder=5, TeamNumber="T2", ParticipantName="������", ParticipantId="1" }
                    }
                }
            };
            // ������ ó���� �� ��. �ʿ��ϸ� TeamRows[i].Scores[h] = ����; ���·� ä��
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
            public Dictionary<int, int> Scores { get; set; } = new(); // key: Ȧ��ȣ, value: ����
        }
    }
}