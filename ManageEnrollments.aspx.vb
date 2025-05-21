Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls

Public Class ManageEnrollments
    Inherits System.Web.UI.Page

    ' Properties to hold chart data for admin view
    Public Property ChartLabels As String
    Public Property ChartData As String
    Public Property StudentChartLabels As String
    Public Property StudentChartData As String

    ' Connection string for Azure SQL database
    Private ReadOnly connectionString As String = "Server=tcp:medibergmann.database.windows.net,1433;Initial Catalog=studentinformationsystem_v3;Persist Security Info=False;User ID=medibergmann;Password=Visselhoevede1409_;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

    ' Page Load: Decide what to show based on user role
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If IsAdmin() Then
                phAdminView.Visible = True
                phStudentView.Visible = False
                LoadAdminEnrollments()
            Else
                phAdminView.Visible = False
                phStudentView.Visible = True
                LoadStudentEnrollments()
            End If
        End If

        ' Display student name in title if available
        If Not IsAdmin() Then
            Dim userName As String = Session("UserName")?.ToString()
            If Not String.IsNullOrEmpty(userName) Then
                lblStudentTitle.Text = $"{userName}'s Courses"
            Else
                lblStudentTitle.Text = "Your Courses"
            End If
        End If

        ' Always load the student's to-do list
        LoadStudentTodoList()
    End Sub

    ' Check whether current user is an admin
    Protected Function IsAdmin() As Boolean
        Return Session("Role") IsNot Nothing AndAlso Session("Role").ToString() = "Admin"
    End Function

    ' Load all courses with student counts for admin view
    Private Sub LoadAdminEnrollments()
        Dim dtCourses As New DataTable()
        Using con As New SqlConnection(connectionString)
            Dim query As String = "SELECT c.course_id, c.course_name, COUNT(e.student_id) AS student_count FROM courses c LEFT JOIN enrollments e ON c.course_id = e.course_id GROUP BY c.course_id, c.course_name"
            Using cmd As New SqlCommand(query, con)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dtCourses)
                End Using
            End Using
        End Using

        rptCourses.DataSource = dtCourses
        rptCourses.DataBind()

        ' Prepare chart data
        Dim labels As New List(Of String)()
        Dim data As New List(Of Integer)()
        For Each row As DataRow In dtCourses.Rows
            labels.Add("'" & row("course_name").ToString() & "'")
            data.Add(Convert.ToInt32(row("student_count")))
        Next
        ChartLabels = String.Join(",", labels)
        ChartData = String.Join(",", data)
    End Sub

    ' When binding the student list inside each course card
    Protected Sub rptCourses_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim courseId As Integer = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "course_id"))
            Dim rptStudents As Repeater = CType(e.Item.FindControl("rptStudents"), Repeater)

            Dim dtStudents As New DataTable()
            dtStudents.Columns.Add("course_id", GetType(Integer))

            ' Fetch all students for the current course
            Using con As New SqlConnection(connectionString)
                Dim query As String = "SELECT s.id AS student_id, s.first_name + ' ' + s.last_name AS full_name FROM students s INNER JOIN enrollments e ON s.id = e.student_id WHERE e.course_id = @CourseId"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@CourseId", courseId)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dtStudents)
                    End Using
                End Using
            End Using

            ' Ensure course_id column is filled
            For Each row As DataRow In dtStudents.Rows
                row("course_id") = courseId
            Next

            rptStudents.DataSource = dtStudents
            rptStudents.DataBind()
        End If
    End Sub

    ' Admin removes a student from a course
    Protected Sub RemoveStudent_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim studentId As Integer = Convert.ToInt32(btn.CommandArgument)
        Dim courseId As Integer = Convert.ToInt32(btn.CommandName)

        Using con As New SqlConnection(connectionString)
            Dim query As String = "DELETE FROM enrollments WHERE student_id = @StudentId AND course_id = @CourseId"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@StudentId", studentId)
                cmd.Parameters.AddWithValue("@CourseId", courseId)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadAdminEnrollments()
    End Sub

    ' Load all courses that the student is enrolled in
    Private Sub LoadStudentEnrollments()
        Dim studentId As Integer = Convert.ToInt32(Session("UserId"))
        Dim dt As New DataTable()
        Using con As New SqlConnection(connectionString)
            Dim query As String = "SELECT c.course_id, c.course_name, c.ects, c.hours, c.format, c.instructor FROM dbo.courses c JOIN dbo.enrollments e ON c.course_id = e.course_id WHERE e.student_id = @StudentId"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@StudentId", studentId)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        gvStudentEnrollments.DataSource = dt
        gvStudentEnrollments.DataBind()

        ' Calculate totals for ECTS and Hours
        Dim totalECTS As Integer = 0
        Dim totalHours As Integer = 0
        For Each row As DataRow In dt.Rows
            totalECTS += Convert.ToInt32(row("ects"))
            totalHours += Convert.ToInt32(row("hours"))
        Next

        lblTotalECTS.Text = totalECTS.ToString()
        lblTotalHours.Text = totalHours.ToString()
    End Sub

    ' Load to-do list entries for the student
    Private Sub LoadStudentTodoList()
        Dim studentId As Integer = Convert.ToInt32(Session("UserId"))
        Dim dt As New DataTable()

        Using con As New SqlConnection(connectionString)
            Dim query As String = "SELECT todo_id, todo FROM todos WHERE student_id = @StudentId"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@StudentId", studentId)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        rptTodos.DataSource = dt
        rptTodos.DataBind()
    End Sub

    ' Add new to-do task for the current student
    Protected Sub btnAddTodo_Click(sender As Object, e As EventArgs)
        Dim newTask As String = txtNewTodo.Text.Trim()
        Dim studentId As Integer = Convert.ToInt32(Session("UserId"))

        If Not String.IsNullOrEmpty(newTask) Then
            Using con As New SqlConnection(connectionString)
                Dim query As String = "INSERT INTO todos (student_id, todo) VALUES (@StudentId, @Todo)"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@StudentId", studentId)
                    cmd.Parameters.AddWithValue("@Todo", newTask)
                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            txtNewTodo.Text = String.Empty
            LoadStudentTodoList()
        End If
    End Sub

    ' Remove to-do task by ID
    Protected Sub DeleteTodo_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim todoId As Integer = Convert.ToInt32(btn.CommandArgument)

        Using con As New SqlConnection(connectionString)
            Dim query As String = "DELETE FROM todos WHERE todo_id = @TodoId"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@TodoId", todoId)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadStudentTodoList()
    End Sub

    ' Student clicks "Drop" to leave a course
    Protected Sub DropCourse_Click(sender As Object, e As EventArgs)
        Dim btn As Button = CType(sender, Button)
        Dim studentId As Integer = Convert.ToInt32(Session("UserId"))
        Dim courseId As Integer = Convert.ToInt32(btn.CommandArgument)

        Using con As New SqlConnection(connectionString)
            Dim query As String = "DELETE FROM enrollments WHERE student_id = @StudentId AND course_id = @CourseId"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@StudentId", studentId)
                cmd.Parameters.AddWithValue("@CourseId", courseId)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadStudentEnrollments()
    End Sub

End Class