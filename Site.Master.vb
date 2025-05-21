Imports Microsoft.AspNet.Identity

Public Class SiteMaster
    Inherits MasterPage

    Private Const AntiXsrfTokenKey As String = "__AntiXsrfToken"
    Private Const AntiXsrfUserNameKey As String = "__AntiXsrfUserName"
    Private _antiXsrfTokenValue As String

    Protected Sub Page_Init(sender As Object, e As EventArgs)
        Dim requestCookie = Request.Cookies(AntiXsrfTokenKey)
        Dim requestCookieGuidValue As Guid
        If requestCookie IsNot Nothing AndAlso Guid.TryParse(requestCookie.Value, requestCookieGuidValue) Then
            _antiXsrfTokenValue = requestCookie.Value
            Page.ViewStateUserKey = _antiXsrfTokenValue
        Else
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N")
            Page.ViewStateUserKey = _antiXsrfTokenValue

            Dim responseCookie = New HttpCookie(AntiXsrfTokenKey) With {
                .HttpOnly = True,
                .Value = _antiXsrfTokenValue
            }
            If FormsAuthentication.RequireSSL AndAlso Request.IsSecureConnection Then
                responseCookie.Secure = True
            End If
            Response.Cookies.[Set](responseCookie)
        End If

        AddHandler Page.PreLoad, AddressOf master_Page_PreLoad
    End Sub

    Protected Sub master_Page_PreLoad(sender As Object, e As EventArgs)
        If Not IsPostBack Then
            ViewState(AntiXsrfTokenKey) = Page.ViewStateUserKey
            ViewState(AntiXsrfUserNameKey) = If(Context.User.Identity.Name, String.Empty)
        Else
            If DirectCast(ViewState(AntiXsrfTokenKey), String) <> _antiXsrfTokenValue OrElse
               DirectCast(ViewState(AntiXsrfUserNameKey), String) <> If(Context.User.Identity.Name, String.Empty) Then
                Throw New InvalidOperationException("Fehler bei der Überprüfung des Anti-XSRF-Tokens.")
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Standardmäßig alles verstecken
            phLoggedIn.Visible = False
            phAnonymous.Visible = True
            navDashboard.Visible = False

            ' Wenn jemand eingeloggt ist
            If Session("Role") IsNot Nothing Then
                phLoggedIn.Visible = True
                phAnonymous.Visible = False
                navDashboard.Visible = True

                ' Benutzernamen anzeigen
                If Session("Role").ToString() = "Admin" Then
                    lblLoggedInUser.Text = "Admin"
                ElseIf Session("Role").ToString() = "Student" Then
                    lblLoggedInUser.Text = Session("FirstName") & " " & Session("LastName")
                End If
            End If
        End If
    End Sub

    Protected Sub btnLogout_Click(sender As Object, e As EventArgs)
        Session.Clear()
        Session.Abandon()
        Response.Redirect("~/Default.aspx")
    End Sub
End Class