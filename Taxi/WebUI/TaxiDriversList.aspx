﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="TaxiDriversList.aspx.cs" Inherits="TaxiDriversList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<div class="main">
    <div class="maintop"></div>
    <div class="content">
    <div class="content2">
        <br class="clear noheight" />

        <div>
    
    <h1 id="h1_Title">Lista Taksówkarzy</h1>

        <br />

        <asp:Panel ID="p_EmployeesList" runat="server">
            <asp:GridView ID="gv_Employees" runat="server" AutoGenerateColumns="False" 
                CellPadding="4" DataSourceID="ds_Employees" ForeColor="#333333" 
                GridLines="None" AllowSorting="True" DataKeyNames="id"
                onselectedindexchanged="gv_Employees_SelectedIndexChanged">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="name" HeaderText="name" SortExpression="Imie" />
                    <asp:BoundField DataField="surname" HeaderText="Nazwisko" 
                        SortExpression="surname" />
                    <asp:BoundField DataField="telephone" HeaderText="Telefon" 
                        SortExpression="street" />
                    <asp:BoundField DataField="taxi_number" HeaderText="Nr taksówki" 
                        SortExpression="house_nr" />
                    <asp:BoundField DataField="producer" HeaderText="Samochód" 
                        SortExpression="postal_code" />
                    <asp:BoundField DataField="model" HeaderText="Model" 
                        SortExpression="city" />
                    <asp:BoundField DataField="car_type" HeaderText="Typ samochodu" 
                        SortExpression="e_mail" />
                    <asp:BoundField DataField="driver_status" HeaderText="Status" 
                        SortExpression="driver_status" />
                    <asp:CommandField HeaderText="Edytuj" ShowSelectButton="True" 
                        SelectText="Wybierz"/>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />

    <SortedAscendingCellStyle BackColor="#E9E7E2"></SortedAscendingCellStyle>

    <SortedAscendingHeaderStyle BackColor="#506C8C"></SortedAscendingHeaderStyle>

    <SortedDescendingCellStyle BackColor="#FFFDF8"></SortedDescendingCellStyle>

    <SortedDescendingHeaderStyle BackColor="#6F8DAE"></SortedDescendingHeaderStyle>
            </asp:GridView>
    

            <asp:ObjectDataSource ID="ds_Employees" runat="server" 
                SelectMethod="getTaxiDriversView" TypeName="BLL.Repository">
            </asp:ObjectDataSource>
    
        </asp:Panel>

        </div>

    </div>
    </div>
    <div class="mainbottom"></div>
</div>


</asp:Content>

