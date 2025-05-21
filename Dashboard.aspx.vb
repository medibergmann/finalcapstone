' Import required namespace for SQL operations (not directly used here but good practice in web apps)
Imports System.Data.SqlClient

' Code-behind for the Dashboard.aspx page
Partial Class Dashboard
    Inherits System.Web.UI.Page

    ' This method runs every time the page is loaded
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' Check if the session contains a role; if not, redirect user to login page (Default.aspx)
        If Session("Role") Is Nothing Then
            Response.Redirect("Default.aspx") ' User is not authenticated, send to login
        End If

        ' Get the user's role from session and display it in the welcome message
        Dim role As String = Session("Role").ToString()
        welcomeMessage.InnerText = $"You are logged in as {role}."
    End Sub
End Class
