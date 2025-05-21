<%@ Page Title="Dashboard" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Dashboard.aspx.vb" Inherits="StudentInformationSystem_MB.Dashboard" %>

<asp:Content ID="DashboardContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Dashboard Title -->
    <h2>Welcome to your Dashboard</h2>
    <p id="welcomeMessage" runat="server" />

    <!-- Admin-specific section (can be made visible dynamically in code-behind if needed) -->
    <div id="adminLinks" runat="server" visible="false">
        <h4>Admin Tools</h4>
    </div>

    <!-- CSS styles for dashboard layout and cards -->
    <style>
        .dashboard-header {
            text-align: center;
            padding: 30px;
        }

        .card-container {
            display: flex;
            justify-content: center;
            gap: 20px;
            flex-wrap: wrap;
            margin-top: 30px;
        }

        .card {
            background-color: #f8f9fa;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            padding: 20px;
            width: 280px;
            transition: transform 0.2s, box-shadow 0.3s;
            margin-bottom: 20px;
        }

        .card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
        }

        .card-title {
            font-size: 22px;
            font-weight: bold;
            margin-bottom: 15px;
        }

        .card-text {
            margin-bottom: 15px;
        }

        .card a {
            text-decoration: none;
            color: #0d6efd;
            font-weight: bold;
        }

        .student-card {
            background-color: #f1f3f5;
        }

        .admin-card {
            background-color: #e9ecef;
        }

        .card-container .admin-card {
            width: 280px;
        }

        .card-container .student-card {
            width: 280px;
        }

        .dashboard-header h2 {
            font-size: 32px;
            font-weight: 600;
            color: #343a40;
        }

        .dashboard-header p {
            font-size: 18px;
            color: #6c757d;
        }
    </style>

    <!-- Personalized welcome message -->
    <div class="dashboard-header">
        <!-- Show user's first name from session -->
        <h2>Hi <% = Session("FirstName") %> 👋</h2>
        <p>Welcome to your dashboard. What would you like to do today?</p>
    </div>

    <!-- Card-based navigation, rendered based on role -->
    <div class="card-container">
        <% If Session("Role") = "Student" Then %>
            <!-- Student Dashboard Cards -->

            <!-- Card for browsing and enrolling in courses -->
            <div class="card student-card">
                <div class="card-title">Course Enrollment</div>
                <div class="card-text">Browse open courses and enroll in the ones that interest you.</div>
                <a href="ManageCourses.aspx">Manage Courses</a>
            </div>

            <!-- Card for viewing current course enrollments -->
            <div class="card student-card">
                <div class="card-title">My Enrollments</div>
                <div class="card-text">View and manage the courses you're currently enrolled in.</div>
                <a href="ManageEnrollments.aspx">Manage Enrollments</a>
            </div>

        <% ElseIf Session("Role") = "Admin" Then %>
            <!-- Admin Dashboard Cards -->

            <!-- Card for managing student accounts -->
            <div class="card admin-card">
                <div class="card-title">Manage Students</div>
                <div class="card-text">View, edit, or delete student records in the system.</div>
                <a href="ManageStudents_V2.aspx">Go to Student Management</a>
            </div>

            <!-- Card for creating/editing/deleting courses -->
            <div class="card admin-card">
                <div class="card-title">Manage Courses</div>
                <div class="card-text">Create, edit, and view courses available to students.</div>
                <a href="ManageCourses.aspx">Manage Courses</a>
            </div>

            <!-- Card for managing student-course enrollments -->
            <div class="card admin-card">
                <div class="card-title">Manage Enrollments</div>
                <div class="card-text">Track and manage student enrollments in courses.</div>
                <a href="ManageEnrollments.aspx">Manage Enrollments</a>
            </div>
        <% End If %>
    </div>
</asp:Content>
