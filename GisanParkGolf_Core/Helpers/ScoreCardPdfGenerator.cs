using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;

public class ScoreCardRow
{
    public string? PlayerName { get; set; }
    public string? PlayerID { get; set; }
    public string? TeamNumber { get; set; }
}

public class CourseScoreCardData
{
    public string CourseName { get; set; } = "";
    public List<string> HoleNumbers { get; set; } = new();
    public List<ScoreCardRow> Players { get; set; } = new();
}

public class ScoreCardPdfGenerator
{
    public static void GeneratePdf(
    string gameName,
    string gameDate,
    string stadiumName,
    List<CourseScoreCardData> courses, // 코스별 데이터
    Stream outputStream)
    {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);

                // 상단 대회 정보
                page.Header().Column(col =>
                {
                    col.Item()
                        .AlignCenter()
                        .PaddingBottom(8)
                        .Text($"{gameName} 스코어카드").FontSize(22).Bold();

                    col.Item().Text($"대회일자    {gameDate}").FontSize(12);
                    col.Item().Text($"대회장소    {stadiumName}").FontSize(12);
                });

                // **한 번만 사용!**
                page.Content().Column(contentCol =>
                {
                    foreach (var course in courses)
                    {
                        // 코스명
                        contentCol.Item()
                            .PaddingTop(10)
                            .Text($"코스명: {course.CourseName}").FontSize(16).Bold();

                        // 스코어카드 표
                        contentCol.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(32); // No.
                                columns.ConstantColumn(80); // 참가자명
                                columns.ConstantColumn(60); // 참가자ID
                                foreach (var hole in course.HoleNumbers)
                                    columns.ConstantColumn(35); // 홀별 스코어
                                columns.ConstantColumn(50); // 합계
                            });

                            // 헤더
                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("No.").FontSize(11).Bold();
                                header.Cell().Element(HeaderCell).Text("참가자명").FontSize(11).Bold();
                                header.Cell().Element(HeaderCell).Text("참가자ID").FontSize(11).Bold();
                                foreach (var hole in course.HoleNumbers)
                                    header.Cell().Element(HeaderCell).Text($"{hole}홀").FontSize(11).Bold();
                                header.Cell().Element(HeaderCell).Text("합계").FontSize(11).Bold();
                            });

                            // 데이터 행(빈칸)
                            int no = 1;
                            foreach (var player in course.Players)
                            {
                                table.Cell().Element(CellStyle).Text(no.ToString()).FontSize(10);
                                table.Cell().Element(CellStyle).Text(player.PlayerName ?? "").FontSize(10);
                                table.Cell().Element(CellStyle).Text(player.PlayerID ?? "").FontSize(10);
                                foreach (var hole in course.HoleNumbers)
                                    table.Cell().Element(CellStyle).Text(""); // 경기 중 직접 입력
                                table.Cell().Element(CellStyle).Text(""); // 합계
                                no++;
                            }
                        });
                    }
                });
            });
        })
        .GeneratePdf(outputStream);
    }

    static IContainer HeaderCell(IContainer container) =>
        container.Border(1).Background("#F5F5F5").AlignCenter().AlignMiddle().PaddingVertical(4);

    static IContainer CellStyle(IContainer container) =>
        container.Border(1).AlignCenter().AlignMiddle().PaddingVertical(3);
}