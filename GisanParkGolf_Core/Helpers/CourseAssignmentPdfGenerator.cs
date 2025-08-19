using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;

public class CourseAssignmentRow
{
    public string? GameName { get; set; }
    public string? GameDate { get; set; }
    public string? StadiumName { get; set; }
    public string? TeamNumber { get; set; }
    public string? CourseName { get; set; }
    public string? HoleNumber { get; set; }
    public string? CourseOrder { get; set; }
    public string? ParticipantName { get; set; }
    public string? ParticipantID { get; set; }
    public string? Gender { get; set; }
    public string? Note { get; set; }
}

public class CourseAssignmentPdfGenerator
{
    public static void GeneratePdf(List<CourseAssignmentRow> rows, Stream outputStream)
    {
        // 데이터가 부족하면 빈 행 추가 (최소 20행)
        int minRows = 20;
        while (rows.Count < minRows)
        {
            rows.Add(new CourseAssignmentRow());
        }

        // 첫 행에서 대회 정보 추출
        var first = rows.FirstOrDefault();
        string gameName = first?.GameName ?? "대회명";
        string gameDate = first?.GameDate ?? "대회일자";
        string stadiumName = first?.StadiumName ?? "대회장소";

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);

                // 제목
                page.Header().Column(col =>
                {
                    col.Item()
                        .AlignCenter()
                        .PaddingBottom(8)
                        .Text(gameName)
                            .FontSize(22)
                            .Bold();

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"대회일자    {gameDate}").FontSize(12);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"대회장소    {stadiumName}").FontSize(12);
                    });
                });

                // 표
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(32); // No.
                        columns.ConstantColumn(60); // 팀번호
                        columns.ConstantColumn(50); // 코스명
                        columns.ConstantColumn(48); // 홀번호
                        columns.ConstantColumn(48); // 코스순번
                        columns.ConstantColumn(80); // 참가자명
                        columns.ConstantColumn(80); // 참가자ID
                        columns.ConstantColumn(40); // 성별
                        columns.ConstantColumn(60); // 비고
                    });

                    // 헤더
                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell)
                            .Text("No.").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("팀번호").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("코스명").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("홀번호").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("코스순번").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("참가자명").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("참가자ID").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("성별").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("비고").FontSize(11).Bold();
                    });

                    // 데이터 행
                    int no = 1;
                    foreach (var row in rows)
                    {
                        table.Cell().Element(CellStyle)
                            .Text(row.TeamNumber != null ? no.ToString() : "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.TeamNumber ?? "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.CourseName ?? "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.HoleNumber ?? "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.CourseOrder ?? "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.ParticipantName ?? "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.ParticipantID ?? "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.Gender ?? "").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.Note ?? "").FontSize(10);
                        no++;
                    }
                });
            });
        })
        .GeneratePdf(outputStream);
    }

    // 헤더 셀 스타일
    static IContainer HeaderCell(IContainer container)
    {
        return container
            .Border(1)
            .Background("#F5F5F5")
            .AlignCenter()
            .AlignMiddle()
            .PaddingVertical(4);
    }

    // 일반 셀 스타일
    static IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .AlignCenter()
            .AlignMiddle()
            .PaddingVertical(3);
    }
}