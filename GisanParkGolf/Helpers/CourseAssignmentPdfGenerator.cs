using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    // 변경: assignmentCompletedAt 파라미터 추가
    public static void GeneratePdf(List<CourseAssignmentRow> rows, Stream outputStream, string? assignmentCompletedAt = null)
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

        // assignmentCompletedAt 포맷/대체문구
        string assignmentTimeText = !string.IsNullOrEmpty(assignmentCompletedAt)
            ? assignmentCompletedAt
            : "미등록";

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);

                // 제목 및 상단 정보 (대회정보 + 코스배치 완료시각)
                page.Header().Column(col =>
                {
                    col.Item()
                        .AlignCenter()
                        .PaddingBottom(8)
                        .Text(gameName)
                        .FontFamily("NanumGothic")
                        .FontSize(22)
                        .Bold();

                    // 한 행에 '대회일자'와 '코스배치 완료시각'을 배치. 완료시각은 오른쪽 정렬로 표시.
                    col.Item().Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"대회일자    {gameDate}")
                            .FontFamily("NanumGothic")
                            .FontSize(12);
                    });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"대회장소    {stadiumName}")
                            .FontFamily("NanumGothic")
                            .FontSize(12);

                        row.ConstantItem(220)
                            .AlignRight()
                            .Text($"코스배치 완료시각: {assignmentTimeText}")
                            .FontFamily("NanumGothic")
                            .FontSize(10);
                    });
                });

                // 표
                page.Content().Table(table =>
                {
                    // 컬럼 정의 (헤더/데이터와 동일)
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
                            .Text("No.").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("팀번호").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("코스명").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("홀번호").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("코스순번").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("참가자명").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("참가자ID").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("성별").FontFamily("NanumGothic").FontSize(11).Bold();
                        header.Cell().Element(HeaderCell)
                            .Text("비고").FontFamily("NanumGothic").FontSize(11).Bold();
                    });

                    // 데이터 행 (팀이 바뀔 때마다 '빈 행' 삽입)
                    int no = 1;
                    string? prevTeam = null;

                    foreach (var row in rows)
                    {
                        var currTeam = row.TeamNumber ?? "";

                        // 팀 변경 감지: 이전 팀이 비어있지 않고 현재 팀과 다르면 구분 빈줄 삽입
                        if (!string.IsNullOrEmpty(prevTeam) && currTeam != prevTeam)
                        {
                            // 각 컬럼마다 빈셀을 만들어서, 좌우 외곽(border)는 유지하고 내부 세로선은 보이지 않게 함
                            InsertSeparatorRow(table);
                        }

                        // 실제 데이터 행 출력
                        table.Cell().Element(CellStyle)
                            .Text(row.TeamNumber != null ? no.ToString() : "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.TeamNumber ?? "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.CourseName ?? "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.HoleNumber ?? "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.CourseOrder ?? "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.ParticipantName ?? "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.ParticipantID ?? "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.Gender ?? "").FontFamily("NanumGothic").FontSize(10);
                        table.Cell().Element(CellStyle)
                            .Text(row.Note ?? "").FontFamily("NanumGothic").FontSize(10);

                        no++;
                        prevTeam = currTeam;
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

    // Separator 행을 개별 셀로 삽입: 좌우 외곽선만 보이게, 내부는 경계선 제거
    // TableDescriptor 타입은 QuestPDF.Fluent.TableDescriptor
    static void InsertSeparatorRow(QuestPDF.Fluent.TableDescriptor table)
    {
        // 빈행 높이: 패딩값으로 조절 (작게)
        float verticalPadding = 3f;

        // 1) 첫번째 셀: 왼쪽 외곽선 유지, 내부선 제거
        table.Cell().Element(container =>
        {
            // 왼쪽 외곽선만 그리기, 상하경계는 제거
            container.BorderLeft(1).BorderTop(0).BorderBottom(0).BorderRight(0)
                     .PaddingVertical(verticalPadding)
                     .Background("#FFFFFF");
        });

        // 2) 중간 셀들: 경계(테두리) 모두 제거하여 내부 세로선이 보이지 않게 함
        for (int i = 0; i < 7; i++) // 총 9컬럼 중 첫/마지막 제외 -> 7개
        {
            table.Cell().Element(container =>
            {
                container.Border(0).PaddingVertical(verticalPadding).Background("#FFFFFF");
            });
        }

        // 3) 마지막 셀: 오른쪽 외곽선 유지
        table.Cell().Element(container =>
        {
            container.BorderRight(1).BorderTop(0).BorderBottom(0).BorderLeft(0)
                     .PaddingVertical(verticalPadding)
                     .Background("#FFFFFF");
        });
    }
}