using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace CommonClass.WebTest
{
    public partial class Wininet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TridentAgent.InternetSetCookie("http://www.edward44444.com", "JSESSIONID", "Name=edward;Expires=" + DateTime.UtcNow.AddMinutes(1).ToString("R"));
                int dataSize = 1000;
                StringBuilder sbCookie = new StringBuilder(1000);
                TridentAgent.InternetGetCookie("http://www.edward44444.com", "JSESSIONID", sbCookie, ref dataSize);
                Response.Write(sbCookie.ToString());
            }
        }
    }
}