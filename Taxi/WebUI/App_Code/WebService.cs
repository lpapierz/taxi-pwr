﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BLL;
using DAL;
using System.Xml.Serialization;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://localhost/TestWebService/")]//"http://taxi.yasio.pl/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService {

    public WebService () 
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public bool SetTaxiCoord(double longitude, double latitude)
    {
        //odczytanie z kontekstu sesyjnego użytkownika zalogowanego użytkownika
        decimal lon = (decimal)longitude;//latwiejsze rozwiazanie, bo w javie nie ma decimala
        decimal lat = (decimal)latitude;
        int idDriver;
        if (Session["idDriver"] == null)
            return false;
        else
            idDriver = (int)Session["idDriver"];
        return Repository.setTaxiPosition(lon,lat,idDriver); // jak na razie 1
    }

    [WebMethod(EnableSession = true)]
    public bool SetTaxiStatus(int status)
    {
        //mozliwe rozdzielenie na kilka metod:
        // setTaxiStatusOnCourse
        // setTaxiStatusFree
        // setTaxiStatusBusy
        //itp itd - DO USTALENIA
        int idDriver;
        if (Session["idDriver"] == null)
            return false;
        else
            idDriver = (int)Session["idDriver"];
        return Repository.setTaxiState(status,idDriver); //zmieniamy ryska ;)
    }

    [WebMethod(EnableSession = true)]
    public bool LoginDriver(String login, String password)
    {
        try
        {
            Session["idDriver"]=Repository.UserAuth(login, password);
        }
        catch
        {
            return false;
        }
        return true;
    }

    [WebMethod(EnableSession = true)]
    public bool LogoutDriver()
    {
        Session.RemoveAll();
        return true;
    }

    [WebMethod(EnableSession = true)]
    public bool CheckCourse() {
        int idDriver;
        if (Session["idDriver"] == null)
            return false;
        else
            idDriver = (int)Session["idDriver"];
        return Repository.isCourseAvailable(idDriver);
    }

    [WebMethod(EnableSession = true)]
    public CourseData GetCourseData() {
        int idDriver;
        if (Session["idDriver"] == null)
            return null;
        else
            idDriver = (int)Session["idDriver"];
        return Repository.GetCourseData(idDriver);
    }

    [WebMethod(EnableSession = true)]
    public bool DeclineCourse()
    {
        return true;
    }

    [WebMethod(EnableSession = true)]
    public bool AcceptCourse()
    {
        return true;
    }
}
