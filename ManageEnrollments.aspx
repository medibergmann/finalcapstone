<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageEnrollments.aspx.vb" 
    Inherits="StudentInformationSystem_MB.ManageEnrollments" MasterPageFile="~/Site.Master" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Admin View: Visible only to users with "Admin" role -->
    <asp:PlaceHolder ID="phAdminView" runat="server">
        <h3>Admin View</h3>

        <!-- Chart showing number of students enrolled per course using Chart.js -->
        <canvas id="enrollmentChart" width="800" height="400" class="mb-5"></canvas>

        <!-- Chart.js script to visualize student enrollment per course -->
        <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
        <script type="text/javascript">
            window.onload = function () {
                var ctx = document.getElementById('enrollmentChart').getContext('2d');
                var chart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: [<%= ChartLabels %>], // Course names
                        datasets: [{
                            label: 'Students per Course',
                            data: [<%= ChartData %>], // Number of students
                            backgroundColor: 'rgba(54, 162, 235, 0.5)',
                            borderColor: 'rgba(54, 162, 235, 1)',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        responsive: true,
                        scales: {
                            y: {
                                beginAtZero: true,
                                precision: 0
                            }
                        }
                    }
                });
            };
        </script>

        <!-- Repeater to display each course and the students enrolled in it -->
        <asp:Repeater ID="rptCourses" runat="server" OnItemDataBound="rptCourses_ItemDataBound">
            <ItemTemplate>
                <div class="card mb-4">
                    <!-- Course header -->
                    <div class="card-header bg-primary text-white">
                        <strong><%# Eval("course_name") %></strong> (ID: <%# Eval("course_id") %>)
                    </div>

                    <!-- Nested Repeater for students in each course -->
                    <div class="card-body">
                        <asp:Repeater ID="rptStudents" runat="server">
                            <HeaderTemplate>
                                <table class="table table-bordered table-sm">
                                    <thead>
                                        <tr>
                                            <th>Student Name</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>

                            <ItemTemplate>
                                <tr>
                                    <td><%# Eval("full_name") %></td>
                                    <td>
                                        <!-- Button to remove student from course -->
                                        <asp:Button ID="btnRemoveStudent" runat="server" Text="Remove" CssClass="btn btn-sm btn-danger"
                                            CommandArgument='<%# Eval("student_id") %>'
                                            CommandName='<%# Eval("course_id") %>'
                                            OnClick="RemoveStudent_Click" />
                                    </td>
                                </tr>
                            </ItemTemplate>

                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:PlaceHolder>

    <!-- Student View: Visible only to users with "Student" role -->
    <asp:PlaceHolder ID="phStudentView" runat="server">
        <h2 class="mb-4">
            <!-- Dynamic label displaying student's name -->
            <asp:Label ID="lblStudentTitle" runat="server" CssClass="fw-bold fs-3" />
        </h2>

        <!-- Display total ECTS and hours from student's enrolled courses -->
        <div class="mb-4">
            <h4>Total ECTS: <asp:Label ID="lblTotalECTS" runat="server" CssClass="badge bg-primary" Text="0" /></h4>
            <h4>Total Hours: <asp:Label ID="lblTotalHours" runat="server" CssClass="badge bg-primary" Text="0" /></h4>
        </div>

        <!-- GridView showing the student's enrolled courses -->
        <asp:GridView ID="gvStudentEnrollments" runat="server" CssClass="table table-bordered table-striped table-hover table-responsive" AutoGenerateColumns="False" DataKeyNames="course_id">
            <Columns>
                <asp:BoundField DataField="course_id" HeaderText="Course ID" />
                <asp:BoundField DataField="course_name" HeaderText="Course Name" />
                <asp:BoundField DataField="ects" HeaderText="ECTS" />
                <asp:BoundField DataField="hours" HeaderText="Hours" />
                <asp:BoundField DataField="format" HeaderText="Format" />
                <asp:BoundField DataField="instructor" HeaderText="Instructor" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <!-- Drop button to allow students to drop a course -->
                        <asp:Button ID="btnDrop" runat="server" Text="Drop" CssClass="btn btn-outline-danger btn-sm"
                            CommandArgument='<%# Eval("course_id") %>' OnClick="DropCourse_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- To-Do List Section -->
        <div class="mt-5">
            <h4 class="mb-3">To-Do List</h4>

            <!-- Repeater to list all tasks for the student -->
            <asp:Repeater ID="rptTodos" runat="server">
                <ItemTemplate>
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <!-- Task description -->
                        <span><%# Eval("todo") %></span>
                        <!-- Button to mark task as done and remove it -->
                        <asp:Button ID="btnDone" runat="server"
                                    Text="Done"
                                    CssClass="btn btn-sm btn-success"
                                    CommandArgument='<%# Eval("todo_id") %>'
                                    OnClick="DeleteTodo_Click" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <!-- Input field to add a new task -->
            <asp:TextBox ID="txtNewTodo" runat="server" CssClass="form-control mt-3" Placeholder="Add a new task" />
            <asp:Button ID="btnAddTask" runat="server" CssClass="btn btn-success mt-2" Text="Add Task" OnClick="btnAddTodo_Click" />
        </div>
    </asp:PlaceHolder>
</asp:Content>