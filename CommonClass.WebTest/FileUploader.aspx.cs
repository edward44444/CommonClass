using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace CommonClass.WebTest
{
    public partial class FileUploader : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (file1.HasFile)
            {
                file1.SaveAs(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file1.FileName)));
            }
            if (file2.HasFile)
            {
                file2.SaveAs(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(file2.FileName)));
            }
        }
    }
}