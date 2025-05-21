<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageStudents_V2.aspx.vb" Inherits="StudentInformationSystem_MB.ManageStudents_V2" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Manage Students</h2>

    <asp:Label ID="lblMessage" runat="server" ForeColor="Green" />

    <div class="form-group">
        <label>Student ID:</label>
        <asp:TextBox ID="txtStudentID" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <label>First Name:</label>
        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <label>Last Name:</label>
        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <label>Email:</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <label>Enrollment Date:</label>
        <asp:TextBox ID="txtEnrollmentDate" runat="server" TextMode="Date" CssClass="form-control" />
    </div>

    <asp:Button ID="btnCreate" runat="server" Text="Create"
        CssClass="btn btn-success"
        OnClick="btnCreate_Click"
        OnClientClick="return confirm('Are you sure you want to add this student?');" />

    <asp:Button ID="btnUpdate" runat="server" Text="Update"
        CssClass="btn btn-warning"
        OnClick="btnUpdate_Click" />

    <asp:Button ID="btnDelete" runat="server" Text="Delete"
        CssClass="btn btn-danger"
        OnClick="btnDelete_Click"
        OnClientClick="return confirm('Are you sure you want to delete this student?');" />

    <asp:Button ID="btnClear" runat="server" Text="Clear"
        CssClass="btn btn-secondary"
        OnClick="btnClear_Click" />

    <br /><br />

    <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered" OnSelectedIndexChanged="gvStudents_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="id" HeaderText="ID" />
           <asp:BoundField DataField="first_name" HeaderText="First Name" />
           <asp:BoundField DataField="last_name" HeaderText="Last Name" />
           <asp:BoundField DataField="email" HeaderText="Email" />
            <asp:BoundField DataField="enrollment_date" HeaderText="Enrollment Date" DataFormatString="{0:dd.MM.yyyy}" HtmlEncode="False" />
            <asp:CommandField ShowSelectButton="True" SelectText="Select" />
        </Columns>
    </asp:GridView>
</asp:Content>


