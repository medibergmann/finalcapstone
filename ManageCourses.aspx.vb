Imports System.Data.SqlClient
Imports System.Net.Mail

Partial Class ManageCourses
    Inherits System.Web.UI.Page


    ' These properties store chart-related data to be rendered in the Chart.js component on the frontend.
    Public Property ChartLabels As String
    Public Property FunData As String
    Public Property DifficultyData As String
    Public Property BubbleChartData As String

    ' SQL Server connection string for connecting to the Azure-hosted student information database
    Private ReadOnly connectionString As String = "Server=tcp:medibergmann.database.windows.net,1433;Initial Catalog=studentinformationsystem_v3;Persist Security Info=False;User ID=medibergmann;Password=Visselhoevede1409_;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

    ' Executes once when the page is loaded (only on first request, not on postbacks)
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadCourses() ' Load course list into GridView
            CheckAdminRole() ' Show/hide admin UI elements based on session role

            ' If the logged-in user is a student, generate the ratings chart
            If Not IsAdmin() Then
                LoadCourseRatingsChart()
            End If
        End If

        ' Toggle visibility of the bubble chart placeholder for students only
        phStudentChart.Visible = Not IsAdmin()
    End Sub

    ' Checks whether the current session is for an admin user
    Protected Function IsAdmin() As Boolean
        Return Session("Role") IsNot Nothing AndAlso Session("Role").ToString() = "Admin"
    End Function

    ' Loads all courses from the database and binds them to the GridView
    Private Sub LoadCourses()
        Dim dt As New DataTable()
        Using con As New SqlConnection(connectionString)
            Using cmd As New SqlCommand("SELECT course_id, course_name, ects, hours, format, instructor FROM Courses", con)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using
        gvCourses.DataSource = dt
        gvCourses.DataBind()
    End Sub

    ' Reveals admin controls (form for creating courses) only if the session role is Admin
    Private Sub CheckAdminRole()
        If IsAdmin() Then
            pnlAdminControls.Visible = True
        End If
    End Sub

    ' Handles adding a new course by inserting it into the database
    Protected Sub btnCreate_Click(sender As Object, e As EventArgs)
        Dim courseId As Integer
        If Not Integer.TryParse(txtCourseID.Text.Trim(), courseId) Then
            lblMessage.ForeColor = Drawing.Color.Red
            lblMessage.Text = "❌ Please enter a valid numeric Course ID."
            Return
        End If

        ' Check if Course ID already exists
        Using con As New SqlConnection(connectionString)
            Dim checkQuery As String = "SELECT COUNT(*) FROM Courses WHERE course_id = @CourseID"
            Using checkCmd As New SqlCommand(checkQuery, con)
                checkCmd.Parameters.AddWithValue("@CourseID", courseId)
                con.Open()
                Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                con.Close()

                If count > 0 Then
                    lblMessage.ForeColor = Drawing.Color.Red
                    lblMessage.Text = "❌ Course ID already exists. Please choose a different one."
                    Return
                End If
            End Using
        End Using

        ' If ID is unique, insert new course
        Using con As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO Courses (course_id, course_name, ects, hours, format, instructor) VALUES (@CourseID, @CourseName, @ECTS, @Hours, @Format, @Instructor)"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@CourseID", courseId)
                cmd.Parameters.AddWithValue("@CourseName", txtCourseName.Text)
                cmd.Parameters.AddWithValue("@ECTS", txtECTS.Text)
                cmd.Parameters.AddWithValue("@Hours", txtHours.Text)
                cmd.Parameters.AddWithValue("@Format", txtFormat.Text)
                cmd.Parameters.AddWithValue("@Instructor", txtInstructor.Text)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadCourses()
        ClearFields()
        lblMessage.ForeColor = Drawing.Color.Green
        lblMessage.Text = "✅ Course added."
    End Sub


    ' Handles course deletion via GridView row action (Admin only)
    Protected Sub gvCourses_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Dim courseID As Integer = Convert.ToInt32(gvCourses.DataKeys(e.RowIndex).Value)
        Using con As New SqlConnection(connectionString)
            Dim query As String = "DELETE FROM Courses WHERE course_id = @CourseID"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@CourseID", courseID)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadCourses() ' Refresh course list
        lblMessage.ForeColor = Drawing.Color.Green
        lblMessage.Text = "✅ Course deleted."
    End Sub

    ' Handles course deletion from a button click (Admin only)
    Protected Sub DeleteCourse_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim courseId As Integer = Convert.ToInt32(btn.CommandArgument)
        Using con As New SqlConnection(connectionString)
            Dim query As String = "DELETE FROM Courses WHERE course_id = @CourseID"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@CourseID", courseId)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadCourses()
        lblMessage.ForeColor = Drawing.Color.Green
        lblMessage.Text = "✅ Course deleted."
    End Sub

    ' Handles student course enrollment and triggers confirmation email
    Protected Sub Enroll_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim courseId As Integer = Convert.ToInt32(btn.CommandArgument)
        Dim studentId As Integer = Convert.ToInt32(Session("UserId"))

        ' Get course name for email
        Dim courseName As String = ""
        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand("SELECT course_name FROM Courses WHERE course_id = @CourseId", con)
            cmd.Parameters.AddWithValue("@CourseId", courseId)
            con.Open()
            Dim result = cmd.ExecuteScalar()
            If result IsNot Nothing Then courseName = result.ToString()
        End Using

        ' Add entry to enrollments table
        Using con As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO enrollments (student_id, course_id, enrollment_date) VALUES (@StudentId, @CourseId, GETDATE())"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@StudentId", studentId)
                cmd.Parameters.AddWithValue("@CourseId", courseId)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Send confirmation email to student
        SendEnrollmentConfirmation(Session("Email").ToString(), Session("FirstName").ToString(), courseName)

        lblMessage.Text = "✅ You have successfully enrolled and a confirmation email has been sent!"
        lblMessage.ForeColor = Drawing.Color.Green
    End Sub

    ' Sends confirmation email using SMTP after successful course enrollment
    Private Sub SendEnrollmentConfirmation(studentEmail As String, studentName As String, courseName As String)
        Dim smtpClient As New SmtpClient("smtp.office365.com", 587)
        smtpClient.EnableSsl = True
        smtpClient.Credentials = New System.Net.NetworkCredential("mn.bergmann@t-online.de", "lwxbpydqfwlskdbb")

        Dim mailMessage As New MailMessage()
        mailMessage.From = New MailAddress("mn.bergmann@t-online.de", "Student Information System")
        mailMessage.To.Add(studentEmail)
        mailMessage.Subject = "Enrollment Confirmation"
        mailMessage.IsBodyHtml = True
        mailMessage.Body = $"<h3>Hello {studentName},</h3><p>You have successfully enrolled in the course <strong>{courseName}</strong>.</p><p>Thank you for registering!</p>"

        Try
            smtpClient.Send(mailMessage)
        Catch ex As Exception
            ' Optional: log error or notify failure
        End Try
    End Sub

    ' Clears the input fields in the course creation form
    Private Sub ClearFields()
        txtCourseID.Text = ""        ' 👈 Diese Zeile fehlte!
        txtCourseName.Text = ""
        txtECTS.Text = ""
        txtHours.Text = ""
        txtFormat.Text = ""
        txtInstructor.Text = ""
    End Sub

    ' Loads ratings for all courses and prepares them as JSON for Chart.js bubble chart
    Private Sub LoadCourseRatingsChart()
        Dim dataItems As New List(Of String)()
        Dim random As New Random()

        Using con As New SqlConnection(connectionString)
            Dim query As String = "SELECT c.course_name, r.fun_rating, r.difficulty_rating FROM course_ratings r JOIN courses c ON r.course_id = c.course_id"
            Using cmd As New SqlCommand(query, con)
                con.Open()
                Dim reader As SqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    ' Escape course name and extract ratings
                    Dim name = reader("course_name").ToString().Replace("'", "\'")
                    Dim fun = reader("fun_rating").ToString()
                    Dim difficulty = reader("difficulty_rating").ToString()

                    ' Assign a random color for each bubble
                    Dim color = $"rgba({random.Next(50, 255)},{random.Next(50, 255)},{random.Next(50, 255)},0.6)"

                    ' Build chart data entry
                    dataItems.Add($"{{ label: '{name}', data: [{{ x: {fun}, y: {difficulty}, r: 12 }}], backgroundColor: '{color}' }}")
                End While
            End Using
        End Using

        ' Join all data items into a JSON array string for Chart.js
        BubbleChartData = String.Join(",", dataItems)
    End Sub
End Class
