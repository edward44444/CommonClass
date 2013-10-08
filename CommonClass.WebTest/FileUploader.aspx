<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileUploader.aspx.cs" EnableViewState="false" Inherits="CommonClass.WebTest.FileUploader" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" action="FileUploader.ashx">
    <div>
        <asp:FileUpload ID="file1" runat="server" /><br />
        <asp:FileUpload ID="file2" runat="server" /><br />
        <asp:Button ID="btnUpload" runat="server" Text="Upload" 
            onclick="btnUpload_Click" />
    </div>
    </form>
</body>
</html>
