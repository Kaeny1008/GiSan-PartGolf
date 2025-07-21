using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GiSanParkGolf.Controls
{
    public partial class NewPagingControl : System.Web.UI.UserControl
    {
        public event EventHandler<int> PageChanged;

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int MaxButtons { get; set; } = 10;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RenderPagerButtons();
        }

        private void RenderPagerButtons()
        {
            int start = Math.Max(0, CurrentPage - MaxButtons / 2);
            int end = Math.Min(TotalPages, start + MaxButtons);
            if (end - start < MaxButtons && start > 0)
                start = Math.Max(0, end - MaxButtons);

            var buttons = new List<PagerButton>();

            buttons.Add(new PagerButton { Text = "처음", Index = 0, CssClass = "btn btn-sm btn-outline-secondary me-1" });

            if (CurrentPage > 0)
                buttons.Add(new PagerButton { Text = "◀", Index = CurrentPage - 1, CssClass = "btn btn-sm btn-outline-secondary me-1" });

            for (int i = start; i < end; i++)
            {
                string css = (i == CurrentPage) ? "btn btn-sm btn-primary me-1" : "btn btn-sm btn-outline-secondary me-1";
                buttons.Add(new PagerButton { Text = (i + 1).ToString(), Index = i, CssClass = css });
            }

            if (CurrentPage < TotalPages - 1)
                buttons.Add(new PagerButton { Text = "▶", Index = CurrentPage + 1, CssClass = "btn btn-sm btn-outline-secondary me-1" });

            buttons.Add(new PagerButton { Text = "끝", Index = TotalPages - 1, CssClass = "btn btn-sm btn-outline-secondary" });

            rptButtons.DataSource = buttons;
            rptButtons.DataBind();
        }

        protected void rptButtons_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            PageChanged?.Invoke(this, Convert.ToInt32(e.CommandArgument));
        }

        public class PagerButton
        {
            public string Text { get; set; }
            public int Index { get; set; }
            public string CssClass { get; set; }
        }
    }
}
