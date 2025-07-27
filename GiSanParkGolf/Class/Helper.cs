using GiSanParkGolf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Class
{
    public static class Helper
    {
        public static void RequireAdmin(Page page)
        {
            if (Helper.CurrentUser?.UserClass != 1)
            {
                page.Response.Redirect("~/Sites/Login/Admin Alert.aspx");
            }
        }

        public static string ConvertDate(DateTime datetime)
        {
            DateTime now = datetime;
            string formattedDate = now.ToString("yyyy-MM-dd");

            return formattedDate;
        }

        public static UserViewModel CurrentUser
        {
            get
            {
                return HttpContext.Current.Session["UserInfo"] as UserViewModel;
            }
        }

        // 나이 계산 유틸리티
        public static int CalculateAge(int userNumber, int userGender)
        {
            // 6자리 생년월일 입력 보정 (예: 851010 → "851010")
            string birthPart = userNumber.ToString().PadLeft(6, '0');

            // YYMMDD → YY만 추출
            if (!int.TryParse(birthPart.Substring(0, 2), out int yy))
                return 0;

            // 성별 코드 기반 세기 구분
            int birthYear;
            switch (userGender)
            {
                case 1:
                case 2:
                    birthYear = 1900 + yy;
                    break;
                case 3:
                case 4:
                    birthYear = 2000 + yy;
                    break;
                case 5:
                case 6: // 외국인 등록번호 (1900 또는 2000 구분 없음)
                    birthYear = (yy >= 25) ? 1900 + yy : 2000 + yy;
                    break;
                default:
                    return 0; // 알 수 없는 성별 코드
            }

            // 한국식 나이 계산: 올해 - 출생연도 + 1
            int currentYear = DateTime.Today.Year;
            int age = currentYear - birthYear + 1;

            // 현실적인 나이만 반환
            return (age >= 0 && age <= 130) ? age : 0;
        }

        public static List<BoundField> GetExportColumns(Dictionary<string, string> columnHeaders)
        {
            var columnList = new List<BoundField>();

            foreach (var entry in columnHeaders)
            {
                var column = new BoundField
                {
                    DataField = entry.Key,
                    HeaderText = entry.Value,
                    ItemStyle = { HorizontalAlign = HorizontalAlign.Center },
                    HeaderStyle = { HorizontalAlign = HorizontalAlign.Center }
                };
                columnList.Add(column);
            }

            return columnList;
        }

        public static void ExportGridViewToExcel(GridView gridView, string fileName, HttpResponse response)
        {
            response.Clear();
            response.Buffer = true;
            response.AddHeader("content-disposition", $"attachment;filename={HttpUtility.UrlEncode(fileName, Encoding.UTF8)}");
            response.Charset = "";
            response.ContentType = "application/vnd.ms-excel";
            response.ContentEncoding = Encoding.UTF8;

            using (var sw = new StringWriter())
            {
                using (var hw = new HtmlTextWriter(sw))
                {
                    foreach (GridViewRow row in gridView.Rows)
                    {
                        row.Attributes.Add("class", "textmode");
                    }

                    gridView.RenderControl(hw);

                    string style = @"<meta http-equiv='Content-Type' content='text/html; charset=utf-8' /> 
                                     <style> .textmode { mso-number-format:\@; } </style>";

                    response.Write(style);
                    response.Output.Write(sw.ToString());
                    response.Flush();
                    response.End();
                }
            }
        }
    }
}