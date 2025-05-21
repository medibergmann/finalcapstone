Imports System.Data
Imports System.Data.SqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.Globalization

Partial Class Register
    Inherits System.Web.UI.Page

    ' Azure SQL connection string
    Private ReadOnly connectionString As String = "Server=tcp:medibergmann.database.windows.net,1433;Initial Catalog=studentinformationsystem_v3;Persist Security Info=False;User ID=medibergmann;Password=Visselhoevede1409_;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

    ' Event triggered when "Register" button is clicked
    Protected Sub btnRegister_Click(sender As Object, e As EventArgs)
        ' Check if passwords match
        If txtPassword.Text <> txtConfirmPassword.Text Then
            lblMessage.Text = "❌ Passwords do not match."
            lblMessage.ForeColor = Drawing.Color.Red
            Exit Sub
        End If

        ' Try to parse the enrollment date in dd.MM.yyyy format
        Dim enrollmentRaw As String = txtEnrollmentDate.Text.Trim()
        Dim enrollmentParsed As DateTime

        If Not DateTime.TryParseExact(enrollmentRaw, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, enrollmentParsed) Then
            lblMessage.Text = "❌ Please enter the date in the format DD.MM.YYYY (e.g., 01.01.2020)."
            lblMessage.ForeColor = Drawing.Color.Red
            Exit Sub
        End If

        Using con As New SqlConnection(connectionString)
            con.Open()

            ' Check if the student ID already exists in "registration" table
            Dim checkQuery As String = "SELECT COUNT(*) FROM registration WHERE student_id = @StudentID"
            Using cmdCheck As New SqlCommand(checkQuery, con)
                cmdCheck.Parameters.AddWithValue("@StudentID", txtStudentID.Text)
                Dim count As Integer = Convert.ToInt32(cmdCheck.ExecuteScalar())

                If count > 0 Then
                    lblMessage.Text = "❌ This student ID already exists."
                    lblMessage.ForeColor = Drawing.Color.Red
                    Exit Sub
                End If
            End Using

            ' Insert new student into the "registration" table
            Dim insertRegistrationQuery As String = "INSERT INTO registration (student_id, first_name, last_name, email, enrollment_date, password) 
                                                     VALUES (@StudentID, @FirstName, @LastName, @Email, @EnrollmentDate, @Password)"
            Using cmd As New SqlCommand(insertRegistrationQuery, con)
                cmd.Parameters.AddWithValue("@StudentID", txtStudentID.Text)
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text)
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text)
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text)
                cmd.Parameters.AddWithValue("@EnrollmentDate", enrollmentParsed)
                cmd.Parameters.AddWithValue("@Password", txtPassword.Text) ' Plain text password
                cmd.ExecuteNonQuery()
            End Using

            ' Check if student already exists in "students" table
            Dim checkStudentQuery As String = "SELECT COUNT(*) FROM students WHERE id = @StudentID"
            Using checkCmd As New SqlCommand(checkStudentQuery, con)
                checkCmd.Parameters.AddWithValue("@StudentID", txtStudentID.Text)
                Dim exists As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())

                If exists = 0 Then
                    ' Insert student into "students" table
                    Dim insertStudentQuery As String = "INSERT INTO students (id, first_name, last_name, email, enrollment_date) 
                                                        VALUES (@Id, @FirstName, @LastName, @Email, @EnrollDate)"
                    Using insertCmd As New SqlCommand(insertStudentQuery, con)
                        insertCmd.Parameters.AddWithValue("@Id", txtStudentID.Text)
                        insertCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text)
                        insertCmd.Parameters.AddWithValue("@LastName", txtLastName.Text)
                        insertCmd.Parameters.AddWithValue("@Email", txtEmail.Text)
                        insertCmd.Parameters.AddWithValue("@EnrollDate", enrollmentParsed)
                        insertCmd.ExecuteNonQuery()
                    End Using
                End If
            End Using
        End Using

        ' Show success message
        lblMessage.ForeColor = Drawing.Color.Green
        lblMessage.Text = "✅ Student registered successfully."

        ' Clear all input fields
        ClearFields()
    End Sub


    ' Clears all input fields on the form
    Private Sub ClearFields()
        txtStudentID.Text = ""
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtEmail.Text = ""
        txtEnrollmentDate.Text = ""
        txtPassword.Text = ""
        txtConfirmPassword.Text = ""
    End Sub
End Class