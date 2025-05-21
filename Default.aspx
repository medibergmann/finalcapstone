<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="StudentInformationSystem_MB._Default" MasterPageFile="~/Site.Master" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- Embedded CSS for styling the login page -->
    <style>
        /* Container for all welcome content, vertically and horizontally centered */
        .welcome-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 60px 20px;
        }

        /* Styling for the main heading */
        .welcome-heading {
            font-size: 36px;
            font-weight: bold;
            margin-bottom: 20px;
            text-align: center;
        }

        /* Styling for the subheading below the main heading */
        .welcome-subheading {
            font-size: 20px;
            color: #555;
            margin-bottom: 30px;
            text-align: center;
        }

        /* Responsive image styling with shadow and rounded corners */
        .welcome-image {
            max-width: 100%;
            height: auto;
            border-radius: 12px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
            margin-bottom: 40px;
        }

        /* Container for the login form, centered and sized */
        .login-form {
            width: 100%;
            max-width: 400px;
            display: flex;
            flex-direction: column;
            align-items: center;
        }

        /* Styling shared by both textboxes and buttons */
        .login-form .form-control,
        .login-form .btn {
            width: 100%;
            max-width: 400px;
            padding: 14px;
            margin: 10px 0;
            font-size: 16px;
            border-radius: 8px;
            box-sizing: border-box;
            border: 1px solid #ccc;
            transition: all 0.3s ease;
        }

        /* Highlight on focus and hover for input fields */
        .login-form .form-control:hover,
        .login-form .form-control:focus {
            border-color: #007bff;
            box-shadow: 0 0 5px rgba(0, 123, 255, 0.25);
            outline: none;
        }

        /* Primary login button styling */
        .login-form .btn {
            background-color: #007bff;
            color: white;
            border: none;
            cursor: pointer;
        }

        /* Hover effect for the login button */
        .login-form .btn:hover {
            background-color: #0056b3;
        }

        /* Label styling for role dropdown */
        .form-label {
            font-weight: bold;
            margin-top: 10px;
            text-align: left;
            width: 100%;
            max-width: 400px;
        }

        /* Styling for the account registration link */
        .register-link {
            text-align: center;
            margin-top: 10px;
        }

        /* Link color for "create your account here" */
        .register-link a {
            color: #007bff;
        }
    </style>

    <!-- Full login section with image, form, and account link -->
    <div class="welcome-container">

        <!-- Page heading -->
        <div class="welcome-heading">Welcome to the Student Information System</div>

        <!-- Brief subheading under the title -->
        <div class="welcome-subheading">Manage your students efficiently and easily.</div>

        <!-- Visual banner image -->
        <img src="https://images.unsplash.com/photo-1590650046871-92c887180603?auto=format&fit=crop&w=1000&q=80"
             alt="Students working together" class="welcome-image" />

        <!-- The login form block -->
        <div class="login-form">

            <!-- Input for student or admin username -->
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" Placeholder="Username (e-mail adress)" />

            <!-- Password input field -->
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Password" />

            <!-- Dropdown list for selecting user role (Admin or Student) -->
            <asp:Label Text="Select Role:" runat="server" CssClass="form-label" />
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                <asp:ListItem Text="Select Role" Value="" />
                <asp:ListItem Text="Admin" Value="Admin" />
                <asp:ListItem Text="Student" Value="Student" />
            </asp:DropDownList>

            <!-- Login button that triggers VB login method -->
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn" OnClick="btnLogin_Click" />

            <!-- Label to show login errors like wrong password -->
            <asp:Label ID="lblLoginMessage" runat="server" ForeColor="Red" />

            <!-- Navigation link to account creation page -->
            <div class="register-link">
                If you do not have an account, <a href="Register.aspx">create your account here</a>.
            </div>
        </div>
    </div>
</asp:Content>