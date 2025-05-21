<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageCourses.aspx.vb" Inherits="StudentInformationSystem_MB.ManageCourses" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Page heading -->
    <h2 class="mb-4">Manage Courses</h2>

    <!-- Bubble Chart: Shown only for students -->
    <asp:PlaceHolder ID="phStudentChart" runat="server" Visible="false">
        <div class="mt-5">
            <h4 class="mb-3">Course Fun vs. Difficulty</h4>
            <canvas id="ratingChart" width="800" height="400"></canvas>
        </div>

        <!-- Load Chart.js for chart rendering -->
        <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

        <!-- Script to render the bubble chart -->
        <script type="text/javascript">
            window.onload = function () {
                var ctx = document.getElementById('ratingChart').getContext('2d');
                new Chart(ctx, {
                    type: 'bubble', // Bubble chart type
                    data: {
                        datasets: [<%= BubbleChartData %>] // Server-generated chart data
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: {
                                position: 'top'
                            },
                            tooltip: {
                                callbacks: {
                                    // Tooltip displays course name and both ratings
                                    label: function (context) {
                                        return context.dataset.label + ": Fun " + context.raw.x + ", Difficulty " + context.raw.y;
                                    }
                                }
                            }
                        },
                        scales: {
                            x: {
                                beginAtZero: true,
                                max: 10,
                                title: {
                                    display: true,
                                    text: 'Fun Rating'
                                }
                            },
                            y: {
                                beginAtZero: true,
                                max: 10,
                                title: {
                                    display: true,
                                    text: 'Difficulty Rating'
                                }
                            }
                        }
                    }
                });
            };
        </script>
    </asp:PlaceHolder>

    <!-- GridView for listing all available courses -->
    <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False"
                  CssClass="table table-bordered table-striped table-hover table-responsive"
                  DataKeyNames="course_id" OnRowDeleting="gvCourses_RowDeleting">
        <Columns>
            <asp:BoundField DataField="course_id" HeaderText="Course ID" />
            <asp:BoundField DataField="course_name" HeaderText="Course Name" />
            <asp:BoundField DataField="ects" HeaderText="ECTS" />
            <asp:BoundField DataField="hours" HeaderText="Hours" />
            <asp:BoundField DataField="format" HeaderText="Format" />
            <asp:BoundField DataField="instructor" HeaderText="Instructor" />

            <asp:TemplateField>
                <ItemTemplate>
                    <!-- "Enroll" button: only shown to students -->
                    <asp:Button ID="btnEnroll" runat="server" Text="Enroll"
                        CssClass="btn btn-primary btn-sm"
                        OnClick="Enroll_Click"
                        CommandArgument='<%# Eval("course_id") %>'
                        Visible='<%# Not IsAdmin() %>' />

                    <!-- "Delete" button: only shown to admins -->
                    <asp:Button ID="btnDelete" runat="server" Text="Delete"
                        CssClass="btn btn-danger btn-sm"
                        OnClick="DeleteCourse_Click"
                        CommandArgument='<%# Eval("course_id") %>'
                        Visible='<%# IsAdmin() %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <!-- Admin-only section: create new course -->
    <div class="mt-5">
        <asp:Panel ID="pnlAdminControls" runat="server" Visible="False">

            <!-- Input fields for course creation -->
            <div class="form-group">
                <asp:TextBox ID="txtCourseID" runat="server" Placeholder="Course ID" CssClass="form-control" />
            </div>

            <div class="form-group">
                <asp:TextBox ID="txtCourseName" runat="server" Placeholder="Course Name" CssClass="form-control" />
            </div>
            <div class="form-group">
                <asp:TextBox ID="txtECTS" runat="server" Placeholder="ECTS" CssClass="form-control" />
            </div>
            <div class="form-group">
                <asp:TextBox ID="txtHours" runat="server" Placeholder="Hours" CssClass="form-control" />
            </div>
            <div class="form-group">
                <asp:TextBox ID="txtFormat" runat="server" Placeholder="Format (online, campus, blended)" CssClass="form-control" />
            </div>
            <div class="form-group">
                <asp:TextBox ID="txtInstructor" runat="server" Placeholder="Instructor" CssClass="form-control" />
            </div>

            <!-- Button to trigger course creation -->
            <asp:Button ID="btnCreate" runat="server" Text="Add Course" CssClass="btn btn-success btn-lg btn-block" OnClick="btnCreate_Click" />
        </asp:Panel>
    </div>

    <!-- Area to display success or error messages -->
    <asp:Label ID="lblMessage" runat="server" ForeColor="Green" />
</asp:Content>