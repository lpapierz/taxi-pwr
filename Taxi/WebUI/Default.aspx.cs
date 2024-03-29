﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;
using DAL;
using System.Web.Security;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }
    /*
    protected void Button1_Click(object sender, EventArgs e)
    {
        GridView1.DataSource = Repository.getCoursesStatus();
        GridView1.DataBind();

        WebService x = new WebService();
        //List<TaxiDriver> a = x.GetAllTaxiDrivers(); // brak takiej 
    }
     */

    protected void LoginControl_Authenticate(object sender, AuthenticateEventArgs e)
    {
        try
        {
            Login log = (Login)lv_Login.FindControl("Login1");
            int userId = Repository.UserAuth(log.UserName, log.Password);
            if (userId >= 0)
            {
                e.Authenticated = true;
                Session["userId"] = userId;
                Session["userType"] = Repository.getEmployeeById(userId).employee_type_id;
                Session["userName"] = log.UserName;
                this.startThreads();
            }
        }
        catch (Exception exception)
        {

        }
    }

    protected void linkb_Logout_Click(object sender, EventArgs e)
    {
        Session.Abandon();
        FormsAuthentication.SignOut();
        Response.Redirect("Default.aspx");
    }

    private void startThreads()
    {

        CreatingDriverList.server = this.Server;
        CreatingDriverList.startThread();
        CreatingOrdersList.server = this.Server;
        CreatingOrdersList.startThread();
    }

}
