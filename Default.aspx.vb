' Import SQL Server namespace for database operations
Imports System.Data.SqlClient

' This class handles the logic for the login page
Partial Class _Default
    Inherits System.Web.UI.Page

    ' Connection string to access the Azure SQL database
    Private ReadOnly connectionString As String =
        "Server=tcp:medibergmann.database.windows.net,1433;" &
        "Initial Catalog=studentinformationsystem_v3;" &
        "Persist Security Info=False;User ID=medibergmann;" &
        "Password=Visselhoevede1409_;" &
        "MultipleActiveResultSets=False;Encrypt=True;" &
        "TrustServerCertificate=False;Connection Timeout=30;"

    ' This method is executed when the login button is clicked on the page
    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        ' Retrieve and sanitize input values from the form
        Dim username As String = txtUsername.Text.Trim().ToLower() ' Normalize username
        Dim password As String = txtPassword.Text.Trim()
        Dim selectedRole As String = ddlRole.SelectedValue

        ' ----------------------------------------
        ' ADMIN LOGIN LOGIC
        ' ----------------------------------------
        If selectedRole = "Admin" Then
            ' Only allow login with hardcoded admin credentials
            If username = "admin" AndAlso password = "admin123" Then
                ' Store admin role in session and redirect to dashboard
                Session("Role") = "Admin"
                Response.Redirect("Dashboard.aspx")
            Else
                ' Show error message for invalid admin credentials
                lblLoginMessage.Text = "❌ Only the real admin can log in as Admin."
                lblLoginMessage.ForeColor = Drawing.Color.Red
            End If
            Exit Sub ' Prevent further logic from executing
        End If

        ' ----------------------------------------
        ' STUDENT LOGIN LOGIC
        ' ----------------------------------------
        If selectedRole = "Student" Then
            ' SQL query to validate user credentials using the registration table (by email)
            Dim query As String = "SELECT student_id, password FROM registration WHERE LOWER(email) = @Email"

            Using con As New SqlConnection(connectionString)
                con.Open()
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@Email", username) ' username ist jetzt die Email (already .ToLower())

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim storedPassword As String = reader("password").ToString()
                            Dim studentId As String = reader("student_id").ToString()

                            If password = storedPassword Then
                                reader.Close()

                                ' Jetzt das Profil aus students laden
                                Dim studentQuery As String = "SELECT * FROM students WHERE id = @Id"
                                Using studentCmd As New SqlCommand(studentQuery, con)
                                    studentCmd.Parameters.AddWithValue("@Id", studentId)

                                    Using studentReader As SqlDataReader = studentCmd.ExecuteReader()
                                        If studentReader.Read() Then
                                            Session("Role") = "Student"
                                            Session("UserId") = studentId
                                            Session("FirstName") = studentReader("first_name").ToString()
                                            Session("LastName") = studentReader("last_name").ToString()
                                            Session("Email") = studentReader("email").ToString()
                                            Session("EnrollmentDate") = Convert.ToDateTime(studentReader("enrollment_date")).ToString("yyyy-MM-dd")

                                            Response.Redirect("Dashboard.aspx")
                                        Else
                                            lblLoginMessage.Text = "⚠️ Student not found in profile table."
                                            lblLoginMessage.ForeColor = Drawing.Color.Red
                                        End If
                                    End Using
                                End Using
                            Else
                                lblLoginMessage.Text = "❌ Incorrect password."
                                lblLoginMessage.ForeColor = Drawing.Color.Red
                            End If
                        Else
                            lblLoginMessage.Text = "❌ Email not found."
                            lblLoginMessage.ForeColor = Drawing.Color.Red
                        End If
                    End Using
                End Using
            End Using
        End If

    End Sub

End Class