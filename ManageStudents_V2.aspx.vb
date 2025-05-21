Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class ManageStudents_V2
    Inherits System.Web.UI.Page

    ' Azure SQL-Verbindungszeichenfolge
    Private ReadOnly connectionString As String = "Server=tcp:medibergmann.database.windows.net,1433;Initial Catalog=studentinformationsystem_v3;Persist Security Info=False;User ID=medibergmann;Password=Visselhoevede1409_;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            txtEnrollmentDate.Text = Date.Today.ToString("yyyy-MM-dd")
            LoadStudents()
        End If
    End Sub

    ' Daten aus Azure laden und GridView binden
    Private Sub LoadStudents()
        Dim dt As New DataTable()
        Using con As New SqlConnection(connectionString)
            Using cmd As New SqlCommand("SELECT id, first_name, last_name, email, enrollment_date FROM Students", con)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        gvStudents.DataSource = dt
        gvStudents.DataBind()
    End Sub

    ' Neuer Eintrag
    Protected Sub btnCreate_Click(sender As Object, e As EventArgs)
        Using con As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO Students (first_name, last_name, email, enrollment_date) VALUES (@FirstName, @LastName, @Email, @EnrollmentDate)"
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text)
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text)
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text)
                cmd.Parameters.AddWithValue("@EnrollmentDate", Date.Parse(txtEnrollmentDate.Text))

                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadStudents()
        ClearFields()
        lblMessage.ForeColor = Drawing.Color.Green
        lblMessage.Text = "✅ Student added."
    End Sub

    ' Bestehenden Eintrag aktualisieren
    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        Dim idToUpdate As Integer
        If Integer.TryParse(txtStudentID.Text, idToUpdate) Then
            Using con As New SqlConnection(connectionString)
                Dim query As String = "UPDATE Students SET first_name = @FirstName, last_name = @LastName, email = @Email, enrollment_date = @EnrollmentDate WHERE id = @ID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text)
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text)
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text)

                    Dim parsedDate As Date
                    If Date.TryParse(txtEnrollmentDate.Text, parsedDate) Then
                        cmd.Parameters.AddWithValue("@EnrollmentDate", parsedDate)
                    Else
                        cmd.Parameters.AddWithValue("@EnrollmentDate", Date.Today)
                    End If

                    cmd.Parameters.AddWithValue("@ID", idToUpdate)

                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            LoadStudents()
            ClearFields()
            lblMessage.ForeColor = Drawing.Color.Green
            lblMessage.Text = "✅ Student updated."
        End If
    End Sub

    ' Delete the student
    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Dim idToDelete As Integer
        If Integer.TryParse(txtStudentID.Text, idToDelete) Then
            Try
                Using con As New SqlConnection(connectionString)
                    con.Open()

                    ' First we need to delete the to dos of the student from the oter tabel because otherwise we get a error 
                    Dim deleteTodosQuery As String = "DELETE FROM todos WHERE student_id = @ID"
                    Using cmdDeleteTodos As New SqlCommand(deleteTodosQuery, con)
                        cmdDeleteTodos.Parameters.AddWithValue("@ID", idToDelete)
                        cmdDeleteTodos.ExecuteNonQuery()
                    End Using

                    ' Then we can delete the student with the matching student id from our student table
                    Dim deleteStudentQuery As String = "DELETE FROM students WHERE id = @ID"
                    Using cmdDeleteStudent As New SqlCommand(deleteStudentQuery, con)
                        cmdDeleteStudent.Parameters.AddWithValue("@ID", idToDelete)
                        cmdDeleteStudent.ExecuteNonQuery()
                    End Using
                End Using

                LoadStudents()
                ClearFields()
                lblMessage.ForeColor = Drawing.Color.Green
                lblMessage.Text = "✅ Student deleted successfully."
            Catch ex As Exception
                lblMessage.ForeColor = Drawing.Color.Red
                lblMessage.Text = "❌ Error while deleting: " & ex.Message
            End Try
        Else
            lblMessage.ForeColor = Drawing.Color.Red
            lblMessage.Text = "❌ Invalid Student ID."
        End If
    End Sub


    ' Zeile im GridView auswählen
    Protected Sub gvStudents_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim row As GridViewRow = gvStudents.SelectedRow
        txtStudentID.Text = row.Cells(0).Text
        txtFirstName.Text = row.Cells(1).Text
        txtLastName.Text = row.Cells(2).Text
        txtEmail.Text = row.Cells(3).Text

        Dim rawDate As String = row.Cells(4).Text.Trim()
        Dim parsedDate As DateTime

        If DateTime.TryParseExact(rawDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, parsedDate) Then
            txtEnrollmentDate.Text = parsedDate.ToString("yyyy-MM-dd")

        Else
            txtEnrollmentDate.Text = ""
        End If
    End Sub

    ' Eingabefelder zurücksetzen
    Protected Sub btnClear_Click(sender As Object, e As EventArgs)
        ClearFields()
        lblMessage.Text = ""
    End Sub

    Private Sub ClearFields()
        txtStudentID.Text = ""
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtEmail.Text = ""
        txtEnrollmentDate.Text = Date.Today.ToString("yyyy-MM-dd")
    End Sub
End Class