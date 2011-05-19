﻿<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="MapView.aspx.cs" Inherits="Default2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script src="Scripts/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="Scripts/MapView.js" type="text/javascript"></script>
    <script src="Scripts/MapHandler.js" type="text/javascript"></script>
    <script src="http://www.openlayers.org/api/OpenLayers.js" type="text/javascript"></script> <!--TODO wczytywać mapkę z dysku, tylko trzeba ikonki pościągać i powprowadzać odpowiednie ścieżki do obrazków-->
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
<div id="itemlist" class="rightmenu orders">
    <div class="list">
        <div class="title"></div>
        <div class="content">
            <div class="content2">
                <ul></ul>
                <br />
            </div>
        </div>
        <div class="bottom"></div>
    </div>
    <div class="new">
        <button>Nowe zgłoszenie</button>
    </div>
</div>

<div id="dialog_change_orders">
    <div class="close">X</div>
    <div class="title">Zgłoszenia</div>
    <input type="hidden" id="tb_id_order" />
    <input type="hidden" id="tb_order_lon" />
    <input type="hidden" id="tb_order_lat" />
    <label for="tb_order_course_date">Data: </label><input type="text" id="tb_order_course_date" />
    <label for="tb_order_startpoint_name">Adres: </label><input type="text" id="tb_order_startpoint_name" />
    <label for="tb_order_client_name">Imię i nazwisko: </label><input type="text" id="tb_order_client_name" />
    <label for="tb_order_client_phone">Telefon: </label><input type="text" id="tb_order_client_phone" />
    <div class="submitbutton">
        <button class="add">Dodaj zgłoszenie</button>
        <button class="edit">Zmień zgłoszenie</button>
    </div>
</div>

<div class="main">
    <div class="maintop"></div>
    <div class="content">
    <div class="content2">

        <div id="map_view"></div>

    </div>
    </div>
    <div class="mainbottom"></div>
</div>


</asp:Content>
