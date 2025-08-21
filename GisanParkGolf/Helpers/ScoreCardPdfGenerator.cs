using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

public class ScoreCardRow
{
    public string? PlayerName { get; set; }
    public string? PlayerID { get; set; }
    public string? TeamNumber { get; set; }
    public string? CourseOrder { get; set; }
}

public class CourseScoreCardData
{
    public string CourseName { get; set; } = "";
    public List<string> HoleNumbers { get; set; } = new();
    public List<ScoreCardRow> Players { get; set; } = new();
}

public class TeamScoreCardData
{
    public string TeamNumber { get; set; } = "";
    public List<ScoreCardRow> Players { get; set; } = new();
}

public class ScoreCardPdfGenerator
{
    public static void GeneratePdfByTeamLandscape(
        string gameName,
        string gameDate,
        string stadiumName,
        List<CourseScoreCardData> coursesTemplate,
        List<TeamScoreCardData> teams,
        Stream outputStream,
        string? assignmentCompletedAt = null)
    {
        string assignmentTimeText = !string.IsNullOrEmpty(assignmentCompletedAt) ? assignmentCompletedAt : "미등록";

        Document.Create(container =>
        {
            foreach (var team in teams)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(18);

                    // 상단 — 수정: PaddingTop은 Item() 컨테이너에 적용합니다.
                    page.Header().Column(col =>
                    {
                        col.Item()
                            .AlignCenter()
                            .PaddingBottom(6)
                            .Text($"{gameName} 스코어카드")
                            .FontFamily("NanumGothic")
                            .FontSize(20).Bold();

                        // 대회일자(왼쪽) / 코스배치 완료시각(오른쪽)
                        col.Item().Row(r =>
                        {
                            r.RelativeItem()
                                .Text($"대회일자: {gameDate}")
                                .FontFamily("NanumGothic")
                                .FontSize(11);

                            r.ConstantItem(220)
                                .AlignRight()
                                .Text($"코스배치 완료시각: {assignmentTimeText}")
                                .FontFamily("NanumGothic")
                                .FontSize(10);
                        });

                        // 여기서 PaddingTop은 컨테이너에 적용
                        col.Item().PaddingTop(2)
                            .Text($"대회장소: {stadiumName}")
                            .FontFamily("NanumGothic")
                            .FontSize(12).Bold();

                        // 팀 텍스트도 PaddingTop을 Item()에 적용
                        col.Item().PaddingTop(2)
                            .Text($"팀: {team.TeamNumber}")
                            .FontFamily("NanumGothic")
                            .FontSize(12).Bold();
                    });

                    page.Content().Column(contentCol =>
                    {
                        foreach (var courseTemplate in coursesTemplate)
                        {
                            contentCol.Item()
                                .PaddingTop(8)
                                .Text($"코스명: {courseTemplate.CourseName}")
                                .FontFamily("NanumGothic")
                                .FontSize(14)
                                .Bold();

                            contentCol.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(36);
                                    columns.ConstantColumn(56);
                                    columns.ConstantColumn(150);
                                    columns.ConstantColumn(100);
                                    foreach (var hole in courseTemplate.HoleNumbers)
                                        columns.RelativeColumn();
                                    columns.ConstantColumn(70);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderCell).Text("No.").FontFamily("NanumGothic").FontSize(11).Bold();
                                    header.Cell().Element(HeaderCell).Text("코스순번").FontFamily("NanumGothic").FontSize(11).Bold();
                                    header.Cell().Element(HeaderCell).Text("참가자명").FontFamily("NanumGothic").FontSize(11).Bold();
                                    header.Cell().Element(HeaderCell).Text("참가자ID").FontFamily("NanumGothic").FontSize(11).Bold();
                                    foreach (var hole in courseTemplate.HoleNumbers)
                                        header.Cell().Element(HeaderCell).Text($"{hole}홀").FontFamily("NanumGothic").FontSize(11).Bold();
                                    header.Cell().Element(HeaderCell).Text("합계").FontFamily("NanumGothic").FontSize(11).Bold();
                                });

                                int no = 1;
                                foreach (var player in team.Players)
                                {
                                    table.Cell().Element(CellStyle).Text(no.ToString()).FontFamily("NanumGothic").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(player.CourseOrder ?? "").FontFamily("NanumGothic").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(player.PlayerName ?? "").FontFamily("NanumGothic").FontSize(10);
                                    table.Cell().Element(CellStyle).Text(player.PlayerID ?? "").FontFamily("NanumGothic").FontSize(10);
                                    foreach (var hole in courseTemplate.HoleNumbers)
                                        table.Cell().Element(CellStyle).Text("");
                                    table.Cell().Element(CellStyle).Text("");
                                    no++;
                                }

                                if (!team.Players.Any())
                                {
                                    table.Cell().Element(CellStyle).Text("1");
                                    table.Cell().Element(CellStyle).Text("");
                                    table.Cell().Element(CellStyle).Text("참가자가 없습니다");
                                    table.Cell().Element(CellStyle).Text("");
                                    foreach (var hole in courseTemplate.HoleNumbers)
                                        table.Cell().Element(CellStyle).Text("");
                                    table.Cell().Element(CellStyle).Text("");
                                }
                            });
                        }
                    });
                });
            }
        })
        .GeneratePdf(outputStream);
    }

    static IContainer HeaderCell(IContainer container) =>
        container.Border(1).Background("#F5F5F5").AlignCenter().AlignMiddle().PaddingVertical(6);

    static IContainer CellStyle(IContainer container) =>
        container.Border(1).AlignCenter().AlignMiddle().PaddingVertical(6).PaddingHorizontal(4);
}