<%@ Page Title="Student Registration" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="Register.aspx.vb" Inherits="StudentInformationSystem_MB.Register" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .register-container {
            text-align: center;
            padding: 60px 20px;
        }

        .register-heading {
            font-size: 36px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .register-form {
            margin-top: 40px;
            max-width: 300px;
            margin-left: auto;
            margin-right: auto;
        }

        .register-link {
            margin-top: 20px;
            font-size: 16px;
        }

        .register-link a {
            text-decoration: none;
            color: #007bff;
        }
    </style>

    <div class="register-container">
        <div class="register-heading">Student Registration</div>
        
        <div class="register-form">
            <asp:TextBox ID="txtStudentID" runat="server" CssClass="form-control" Placeholder="Student ID" />
            <br />
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" Placeholder="First Name" />
            <br />
            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" Placeholder="Last Name" />
            <br />
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Placeholder="Email" />
            <br />
            <asp:TextBox ID="txtEnrollmentDate" runat="server" CssClass="form-control" Placeholder="Enrollment Date e. g. 01.01.2020" MaxLength="10" />
            <br />
            <!-- New Password Field -->
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Password" />
            <br />
            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" Placeholder="Confirm Password" />
            <br />
            
            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary" OnClick="btnRegister_Click" />
            <br /><br />
            <!-- Label for error/success message -->
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
        </div>
    </div>
</asp:Content>



