﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Security.Cryptography;

namespace BLL
{
    public class Repository
    {

        /* USER */

        /// <summary>Funkcja autoryzująca użytkownika
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pswd"></param>
        /// <returns>Id autoryzowanego użytkownika</returns>
        /// <exception cref="UserNotExistException">UserNotExistException</exception>
        /// <exception cref="WrongPasswordException">WrongPasswordException</exception>
        public static int UserAuth(string login, string pswd)
        {

            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            var query = from u in ctx.Employees
                        where u.login == login
                        select u;

            Employee user = query.SingleOrDefault();

            if (user == null) throw new UserNotExistException();
            String pswdSalt = pswd + user.salt;
            String password = CalculateSHA1(pswdSalt, Encoding.ASCII);

            if (user.password != password) throw new WrongPasswordException("Wrong password");

            return user.id;
        }


        /// <summary>Funkcja pobierająca użytkownika o podanym id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Employee</returns>
        /// <exception cref="UserNotExistException">UserNotExistException</exception>
        public static Employee GetUserById(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Employee e = ctx.Employees.SingleOrDefault(u => u.id == id);

            if (e == null)
            {
                throw new UserNotExistException();
            }

            return e;
        }

        /// <summary>Usuń uzytkownika o podanym Id
        /// </summary>
        /// <param name="userId"></param>
        /// <exception cref="UserNotExistException">UserNotExistException</exception>
        public static void DeleteUser(int userId)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            var x = from u in ctx.Employees where u.id == userId select u;

            Employee user = x.SingleOrDefault();

            if (user == null)
            {
                throw new UserNotExistException();
            }

            ctx.Employees.DeleteOnSubmit(user);
            ctx.SubmitChanges();
        }

        /// <summary> Funkcja zmiany hasła użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <exception cref="UserNotExistException"></exception>
        public static void ChangeUserPassword(int userId, String newPassword)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            Employee e = ctx.Employees.SingleOrDefault( em => em.id == userId );

            if (e == null)
            {
                throw new UserNotExistException();
            }

            String NewPassword = Repository.CalculateSHA1(newPassword + e.salt, Encoding.ASCII);

            e.password = NewPassword;

            ctx.SubmitChanges();

        }

        /// <summary>
        /// Funkcja dodająca użytkownika o podanych parametrach
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="email"></param>
        /// <param name="houseNr"></param>
        /// <param name="street"></param>
        /// <param name="pesel"></param>
        /// <param name="postalCode"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns>Employee - Dodany użytkownik</returns>
        /// <exception cref="UserExistException">UserExistException</exception>
        private static Employee AddNewUser(
            String name, 
            String surname, 
            String city, 
            String email, 
            String houseNr,  
            String street,
            String pesel, 
            String postalCode, 
            String login, 
            String password,
            String telephone)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Employee e = new Employee();

            e.name = name;
            e.surname = surname;
            e.city = city;
            e.e_mail = email;
            e.house_nr = houseNr;
            e.login = login;
            e.pesel = pesel;
            e.postal_code = postalCode;
            e.street = street;
            e.telephone = telephone;

            Employee check = ctx.Employees.SingleOrDefault(u => u.login == e.login);

            if  (check != null)
            {
                throw new UserExistException();
            }

            e.salt = CreateRandomString(8);

            String pwd;

            if (password != null)
            {
                pwd = Repository.CalculateSHA1(password + e.salt, Encoding.ASCII);
            }
            else
            {
                string randomPwd = CreateRandomString(8);
                pwd = Repository.CalculateSHA1(randomPwd + e.salt, Encoding.ASCII);
            }

            e.password = pwd;

            return e;

        }

        /// <summary>Funkcja zwraca listę wszystkich pracowników
        /// </summary>
        /// <returns>List<Employee></returns>
        public static List<Employee> GetAllEmployees()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from e in ctx.Employees select e;
            return x.ToList();
        }

        /// <summary>Funckja edytująca dane użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="street"></param>
        /// <param name="house_nr"></param>
        /// <param name="postal_code"></param>
        /// <param name="e_mail"></param>
        /// <param name="pesel"></param>
        /// <param name="telephone"></param>
        /// <exception cref="UserExistException">UserExistException</exception>
        /// <exception cref="UserNotExistException">UserNotExistException</exception>
        private static Employee EditUserData(
            int userId,
            String login,
            String name,
            String surname,
            String city,
            String street,
            String house_nr,
            String postal_code,
            String e_mail,
            String pesel,
            String telephone)
        {

            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            Employee e = ctx.Employees.FirstOrDefault(em => em.id == userId);

            if (e == null)
            {
                throw new UserNotExistException();
            }

            //check if login is OK!
            if (e.login != login)
            {
                Employee check = ctx.Employees.FirstOrDefault(em => em.login == login);

                if (check != null)
                {
                    throw new UserExistException();
                }
            }

            e.login = login;
            e.name = name;
            e.surname = surname;
            e.city = city;
            e.street = street;
            e.house_nr = house_nr;
            e.postal_code = postal_code;
            e.e_mail = e_mail;
            e.pesel = pesel;
            e.telephone =  telephone;
            e.street = street;

            ctx.SubmitChanges();

            return e;

        }

        // CRUD
        /* TAXI DRIVER */

        /// <summary> Funkcja zwracająca listę wszystkich taksówkarzy
        /// </summary>
        /// <returns>List<TaxiDriver></returns>
        public static List<TaxiDriver> GetAllTaxiDrivers()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var query = from c in ctx.Employees.OfType<TaxiDriver>() select c;
            return query.ToList();
        }

        // <summary>Funkja dodająca nowego taksówkarza
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="email"></param>
        /// <param name="houseNr"></param>
        /// <param name="pesel"></param>
        /// <param name="licenceNr"></param>
        /// <param name="postalCode"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="UserExistException">UserExistException</exception>
        public static void AddNewTaxiDriver(String name, String surname, String city, String email, 
            String houseNr, String street, String pesel, String licenceNr, String postalCode, String login, String password, String telephone,
            String carBrand, String carModel, String productionYear, String seatPlaces, String registrationNumber, String taxiNumber, int carType)
        {
            
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            int carModelId = Repository.addNewCarModel(carBrand, carModel, productionYear, seatPlaces);

            int taxiId = AddNewTaxi(taxiNumber, registrationNumber, carModelId, carType);

            Employee e = Repository.AddNewUser( name, surname, city, email, houseNr, street,  pesel,  postalCode, login, password, telephone);

            TaxiDriver td = new TaxiDriver(e);

            td.licence_number = licenceNr;
            td.taxi_id = taxiId;
            ctx.Employees.InsertOnSubmit(td);
            ctx.SubmitChanges();

        }

       
        /// <summary>Funkcja zmieniająca dane taksówkarza
        /// </summary>
        /// <param name="taxiDriveId"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="email"></param>
        /// <param name="houseNr"></param>
        /// <param name="pesel"></param>
        /// <param name="licenceNr"></param>
        /// <param name="postalCode"></param>
        /// <exception cref="UserNotExistException"></exception>
        /// <exception cref="UserExistException"></exception>

        public static void EditTaxiDriver(int idDriver, String name, String surname, String city, String email, 
            String houseNr, String street, String pesel, String licenceNr, String postalCode, String telephone, String login,
            String carBrand, String carModel, String productionYear, String seatPlaces, String registrationNumber, String taxiNumber, int carType)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            TaxiDriver taxiDriver = ctx.Employees.OfType<TaxiDriver>().SingleOrDefault(u => u.id == idDriver);
            if (taxiDriver == null) { throw new UserNotExistException(); }
            Repository.EditUserData(idDriver, login, name, surname, city, street, houseNr, postalCode, email, pesel, telephone); 
           
            taxiDriver.licence_number = licenceNr;
            taxiDriver.Taxi.registration_number = registrationNumber;
            taxiDriver.Taxi.taxi_number = taxiNumber;

            taxiDriver.Taxi.Car_model.producer = carBrand;
            taxiDriver.Taxi.Car_model.production_year = int.Parse(productionYear);
            taxiDriver.Taxi.Car_model.seats = int.Parse(seatPlaces);
            taxiDriver.Taxi.Car_model.model = carModel;

            taxiDriver.Taxi.id_car_type = carType;

            ctx.SubmitChanges();
        }

        /// <summary>
        /// Funkcja pobiera liste taksowkarzy (podana ich liczbę) najbliższych podanemu miejscu.
        /// Dodatkowo mają okreslony typ samochodu o określonej liczbi miejsc
        /// </summary>
        /// <param name="course"></param>
        /// <param name="carTypeId"></param>
        /// <param name="seats"></param>
        /// <param name="maxResultCount"></param>
        /// <returns></returns>
        public static List<TaxiDriver> GetTaxiDriversByCourseAndTaxiType(Course course, int carTypeId, int seats, int maxResultCount)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            var x = from td in ctx.Employees.OfType<TaxiDriver>()
                    where
                    td.Taxi.Car_model.seats == seats
                    && td.Taxi.Car_type.id == carTypeId
                    select td;


            List<TaxiDriver> tdList = x.ToList();

            tdList = tdList.OrderBy(t => t, new DistanceComparer(course)).Take(maxResultCount).ToList();

            return tdList;

        }

        public class DistanceComparer : IComparer<TaxiDriver>
        {
            Course c;

            public DistanceComparer(Course c)
            {
                this.c = c;
            }

            public int Compare(TaxiDriver x, TaxiDriver y)
            {
                double odl1 = Math.Sqrt((double)((x.position_lon - c.startpoint_lon) * (x.position_lon - c.startpoint_lon) + (x.position_lat - c.startpoint_lat) * (x.position_lat - c.startpoint_lat)));
                double odl2 = Math.Sqrt((double)((y.position_lon - c.startpoint_lon) * (y.position_lon - c.startpoint_lon) + (y.position_lat - c.startpoint_lat) * (y.position_lat - c.startpoint_lat)));

                if (odl1 == odl2) return 0;
                if (odl1 < odl2) return 1;
                else return -1;
            }
        }

        /// <summary>Funkcja pobiera listę taksówkarzy o podanym statusie
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns>List<TaxiDriver></returns>
        public static List<TaxiDriver> GetTaxiDriversByStatus(int statusId)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            var x =
                from td in ctx.Employees.OfType<TaxiDriver>()
                where td.Driver_status.id == statusId
                select td;

            return x.ToList();
        }

        /// Funkcja pobierz 5 najblizszych taksowek o podanym statusie i okreslonej minimalnej liczbie miejsc
        /// Konczy kurs.
        /// </summary>
        /// <param name="idDriver"></param>
        /// <returns></returns>
        public static List<TaxiDriver> GetNearestTaxi(decimal lon, decimal lat, int status)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x =
                (from td in ctx.Employees.OfType<TaxiDriver>()
                let dist = Math.Sqrt((Double)((td.position_lon-lon)*(td.position_lon-lon))+(Double)((td.position_lat-lat)*(td.position_lat-lat)))
                where td.driver_status_id==status
                orderby dist
                select td).Take(5);
            return x.ToList();
        }

        /// <summary>
        /// Funkcja koncząca kurs taksówkarza 
        /// </summary>
        /// <param name="idDriver"></param>
        /// <returns></returns>
        public static Boolean FinishCourse( int idDriver ) {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Course course = ctx.Courses.FirstOrDefault(c => c.taxidriver_id==idDriver && c.course_status_id == 3);
            course.course_status_id = 4;
            course.taxidriver_id = null;
            var x = from i in ctx.Employees.OfType<TaxiDriver>() where i.id == idDriver select i;
            TaxiDriver td = x.SingleOrDefault();
            td.Courses = null;
            td.Courses1 = null;
            ctx.SubmitChanges();
            return true;
        }

        /// <summary>
        /// Funckcja zwaracająca informacje do loginu (WebService)
        /// </summary>
        /// <param name="idDriver"></param>
        /// <returns></returns>
        public static String getLoginInfo(int idDriver)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from i in ctx.Employees.OfType<TaxiDriver>() where i.id == idDriver select i;
            TaxiDriver td = x.SingleOrDefault();
            return td.name + " " + td.surname;
        }

        /// <summary>
        /// Funkcja pobierz 5 najblizszych taksowek o podanym statusie i okreslonej minimalnej liczbie miejsc
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public static List<TaxiDriver> GetNearestTaxiBySeats(decimal lon, decimal lat, int status, int seats)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x =
                (from td in ctx.Employees.OfType<TaxiDriver>()
                 let dist = Math.Sqrt((Double)((td.position_lon - lon) * (td.position_lon - lon)) + (Double)((td.position_lat - lat) * (td.position_lat - lat)))
                 where td.driver_status_id == status && td.Taxi.Car_model.seats>=seats
                 orderby dist 
                 select td).Take(5);
            
            return x.ToList();
        }


        /// <summary>Funkcja pobiera listę taksówkarzy o podanym statusie
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static List<TaxiDriver> GetTaxiDriversByStatus(Driver_status ds)
        {
            return GetTaxiDriversByStatus(ds.id);
        }

        /// <summary> funkcja zwraca listę taksówkarzy, do których przypisana jest taksówka o określonym typie
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List<TaxiDriver></returns>
        public static List<TaxiDriver> GetTaxiDriverByCarType(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            var x =
                from td in ctx.Employees.OfType<TaxiDriver>()
                where td.Taxi.Car_type.id == id
                select td;

            return x.ToList();
        }

        /// <summary>Pobiera taksówkarza o podanym typie samochodu
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>List<TaxiDriver></returns>
        public static List<TaxiDriver> GetTaxiDriverByCarType(Car_type ct)
        {
            return GetTaxiDriverByCarType(ct.id);
        }

        /// <summary> Funkcja pobiera taksówkarza o podanym statusie i typie samochodu
        /// </summary>
        /// <param name="driverStatusId"></param>
        /// <param name="carTypeId"></param>
        /// <returns>List<TaxiDriver></returns>
        public static List<TaxiDriver> GetTaxiDriverByStatusAndCarType(int driverStatusId, int carTypeId)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            var x = from td in ctx.Employees.OfType<TaxiDriver>()
                    where
                    td.Driver_status.id == driverStatusId
                    &&
                    td.Taxi.Car_type.id == carTypeId
                    select td;

            return x.ToList();
            

        }

        /* ADMIN */

        /// <summary>Funkcja dodająca administratora
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="email"></param>
        /// <param name="houseNr"></param>
        /// <param name="street"></param>
        /// <param name="pesel"></param>
        /// <param name="postalCode"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <exception cref="UserExistException">UserExistException</exception>
        public static void AddNewAdmin(String name, String surname, String city, String email, String houseNr, String street, String pesel,  String postalCode, String login, String password, String telephone)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Employee e = Repository.AddNewUser(name, surname, city, email, houseNr, street, pesel, postalCode, login, password, telephone);
            Admin a = new Admin(e);

            ctx.Employees.InsertOnSubmit(a);
            ctx.SubmitChanges();

        }

        /// <summary>Funkcja edytująca dane administratora
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="login"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="street"></param>
        /// <param name="email"></param>
        /// <param name="houseNr"></param>
        /// <param name="pesel"></param>
        /// <param name="postalCode"></param>
        /// <param name="phone"></param>
        /// <exception cref="UserNotExistException">UserNotExistException</exception>
        /// <exception cref="UserExistException">UserExistException</exception>
        public static void EditAdminData(int adminId, String login, String name, String surname, String city, String street, String email, String houseNr, String pesel, String postalCode, String phone)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Admin a = ctx.Employees.OfType<Admin>().SingleOrDefault(u => u.id == adminId);

            if (a == null) { throw new UserNotExistException(); }
            Repository.EditUserData(a.id, login, name, surname, city, street, houseNr, postalCode, email, pesel, phone);

            ctx.SubmitChanges();
        }
        

        /* DISPATCHER */

        /// <summary>Funkcja dodająca dyspozytora
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="email"></param>
        /// <param name="houseNr"></param>
        /// <param name="street"></param>
        /// <param name="pesel"></param>
        /// <param name="postalCode"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <exception cref="UserExistException">UserExistException</exception>
        public static void AddNewDispatcher(String name, String surname, String city, String email, String houseNr, String street, String pesel, String postalCode, String login, String password, String telephone)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Employee e = Repository.AddNewUser(name, surname, city, email, houseNr, street, pesel, postalCode, login, password, telephone);
            Dispatcher d = new Dispatcher(e);

            ctx.Employees.InsertOnSubmit(d);
            ctx.SubmitChanges();

        }

        /// <summary>Funkcja edytująca dane dyspozytora
        /// </summary>
        /// <param name="disId"></param>
        /// <param name="login"></param>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="city"></param>
        /// <param name="street"></param>
        /// <param name="email"></param>
        /// <param name="houseNr"></param>
        /// <param name="pesel"></param>
        /// <param name="postalCode"></param>
        /// <param name="phone"></param>
        /// <exception cref="UserNotExistException">UserNotExistException</exception>
        /// <exception cref="UserExistException">UserExistException</exception>
        public static void EditDispatcheDatar(int disId, String login, String name, String surname, String city, String street, String email, String houseNr, String pesel, String postalCode, String phone)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Dispatcher d = ctx.Employees.OfType<Dispatcher>().SingleOrDefault(u => u.id == disId);

            if (d == null) { throw new UserNotExistException(); }
            Repository.EditUserData(d.id, login, name, surname, city, street, houseNr, postalCode, email, pesel, phone);

            ctx.SubmitChanges();
        }
        /* COURS */

        /// <summary>Funkcja dodająca nowy kurs
        /// </summary>
        /// <param name="taxidriver_id"></param>
        /// <param name="depositor_id"></param>
        /// <param name="client_phone"></param>
        /// <param name="course_date"></param>
        /// <param name="course_status_id"></param>
        /// <param name="client_name"></param>
        /// <param name="startpoint_name"></param>
        /// <param name="startpoint_lon"></param>
        /// <param name="startpoint_lat"></param>
        /// <param name="endpoint_lon"></param>
        /// <param name="endpoint_lat"></param>
        public static void addNewCourse(
            int? taxidriver_id, 
            int? depositor_id, 
            String client_phone, 
            DateTime? course_date, 
            int? course_status_id, 
            String client_name,
            String startpoint_name, 
            Decimal startpoint_lon, 
            Decimal startpoint_lat, 
            Decimal endpoint_lon, 
            Decimal endpoint_lat,
            int car_type_id,
            int seats)
        {
            /*
             * paramatry zostały zmienione na nullowalne
             * przy dodawaniu nowego kursu pewnie nie będzie wiadomo, jakie jest id taxsówkarza
             * i w funkcji będzi emozna podac null, to samo poleci do bazy
             */

            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Course course = new Course();

            /* course_date jest po to, jakby ktos sobie zamawiał taksówka na pozniej, 
             * jak nic nie jest podane to znaczy ze zamowienie ma byc jak najszybciej
             * i jest przypisywana data zgłoszenia
             */

            if (course_date == null)
            {
                course_date = DateTime.Now;
            }

            course.client_phone = client_phone;
            course.date = DateTime.Now;
            course.course_date = course_date;
            course.client_name = client_name;
            course.course_status_id = course_status_id;
            course.depositor_id = depositor_id;
            course.taxidriver_id = taxidriver_id;
            course.startpoint_lat = startpoint_lat;
            course.startpoint_lon = startpoint_lon;
            course.endpoint_lat = endpoint_lat;
            course.endpoint_lon = endpoint_lon;
            course.startpoint_name = startpoint_name;
            course.car_type_id = car_type_id;
            course.seats = seats;


            ctx.Courses.InsertOnSubmit(course);
            ctx.SubmitChanges();

        }


        /// <summary>Funkcja edytująca dane kursu
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="taxidriver_id"></param>
        /// <param name="depositor_id"></param>
        /// <param name="client_phone"></param>
        /// <param name="course_date"></param>
        /// <param name="course_status_id"></param>
        /// <param name="client_name"></param>
        /// <param name="startpoint_name"></param>
        /// <param name="startpoint_lon"></param>
        /// <param name="startpoint_lat"></param>
        /// <param name="endpoint_lon"></param>
        /// <param name="endpoint_lat"></param>
        public static void EditCourse(
            int courseId, 
            int taxidriver_id, 
            int depositor_id, 
            String client_phone, 
            DateTime? course_date, 
            int course_status_id, 
            String client_name,
            String startpoint_name, 
            Decimal startpoint_lon, 
            Decimal startpoint_lat, 
            Decimal endpoint_lon, 
            Decimal endpoint_lat,
            int car_type_id,
            int seats)        
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Course course = ctx.Courses.SingleOrDefault(c => c.id == courseId);

            if (course == null)
            {
                throw new CourseNotExistException();
            }

            if (course_date == null)
            {
                course_date = DateTime.Now;
            }

            course.client_phone = client_phone;
            course.course_date = course_date;
            course.client_name = client_name;
            course.course_status_id = course_status_id;
            course.depositor_id = depositor_id;
            course.taxidriver_id = taxidriver_id;
            course.startpoint_lat = startpoint_lat;
            course.startpoint_lon = startpoint_lon;
            course.endpoint_lat = endpoint_lat;
            course.endpoint_lon = endpoint_lon;
            course.car_type_id = car_type_id;
            course.seats = seats;

            ctx.SubmitChanges();
        }

        /// <summary>Funkcja wiążąca kurs z taksówkarzem
        /// </summary>
        /// <param name="idTaxiDriver"></param>
        /// <param name="idCours"></param>
        /// <exception cref="UserNotExistException"></exception>
        /// <exception cref="CourseNotExistException"></exception>
        public static void BindCoursToTaxiDriver(int idTaxiDriver, int idCours)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            TaxiDriver td = ctx.Employees.OfType<TaxiDriver>().SingleOrDefault(t => t.id == idTaxiDriver);

            if (td == null)
            {
                throw new UserNotExistException();
            }

            Course course = ctx.Courses.SingleOrDefault( c => c.id == idCours );

            if (course == null)
            {
                throw new CourseNotExistException();
            }

            course.taxidriver_id = idTaxiDriver;
            course.course_status_id = 3;
            td.driver_status_id = 1;

            ctx.SubmitChanges();
        }

        /* TAXI */

        /// <summary>Funkcja pobierająca listętaksówek
        /// </summary>
        /// <returns>List<Taxi></returns>
        public static List<Taxi> GetTaxiList()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Taxis select c;
            return x.ToList();
        }

        /// <summary>
        /// Funkcja dodająca nową taksówkę
        /// </summary>
        /// <param name="taxiNumber"></param>
        /// <param name="registrationNumber"></param>
        /// <param name="carModelId"></param>
        /// <param name="carTypeId"></param>
        /// <exception cref="TaxiExistException"></exception>
        /// <exception cref="CarModelNotExistException"></exception>
        /// <exception cref="CarTypeNotExistException"></exception>
        public static int AddNewTaxi(String taxiNumber, String registrationNumber, int carModelId, int carTypeId)
        {

            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            Taxi t1 = ctx.Taxis.SingleOrDefault(t => t.taxi_number == taxiNumber);
            if (t1 != null) throw new TaxiExistException("Taxi with given taxi number already exist");
            Taxi t2 = ctx.Taxis.SingleOrDefault(t => t.registration_number == registrationNumber);
            if (t2 != null) throw new TaxiExistException("Taxi with given registration number arleady exist");

            Car_model cm = ctx.Car_models.SingleOrDefault( c => c.id == carModelId);
            if (cm == null) throw new CarModelNotExistException();

            Car_type ct = ctx.Car_types.SingleOrDefault( c => c.id == carTypeId);
            if (ct == null) throw new CarTypeNotExistException();

            Taxi taxi = new Taxi()
            {
                car_model_id = carModelId,
                id_car_type = carTypeId,
                registration_number = registrationNumber,
                taxi_number = taxiNumber
            };

            ctx.Taxis.InsertOnSubmit( taxi );
            ctx.SubmitChanges();
            return taxi.id;
        }

        /// <summary> Funkcja edytująca dane taksówki
        /// </summary>
        /// <param name="taxiId"></param>
        /// <param name="taxiNumber"></param>
        /// <param name="registrationNumber"></param>
        /// <param name="carModelId"></param>
        /// <param name="carTypeId"></param>
        /// <exception cref="TaxiNotExistException"></exception>
        public static void EditTaxiData(int taxiId, String taxiNumber, String registrationNumber, int carModelId, int carTypeId)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            Taxi taxi = ctx.Taxis.SingleOrDefault( t => t.id == taxiId);

            if (taxi == null) throw new TaxiNotExistException();

            taxi.taxi_number = taxiNumber;
            taxi.registration_number = registrationNumber;
            taxi.car_model_id = carModelId;
            taxi.id_car_type = carTypeId;

            ctx.SubmitChanges();
        }

        /// <summary>Funkcja wiążąca taksówkarza z taksówką
        /// </summary>
        /// <param name="idTaxiDriver"></param>
        /// <param name="idTaxi"></param>
        /// <exception cref="TaxiNotExistException">TaxiNotExistException</exception>
        /// <exception cref="UserNotExistException">UserNotExistException</exception>
        public static void BindTaxiToTaxiDriver(int idTaxiDriver, int? idTaxi)
        {
            /*
             * jak chcemu odczepić taksówke od taksówkarza, jako drugi parametr podajemy null
             */

            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            
            if (idTaxi != null)
            {
                Taxi taxi = ctx.Taxis.SingleOrDefault(t => t.id == idTaxi);

                if (taxi == null) throw new TaxiNotExistException();
            }

            TaxiDriver td = ctx.Employees.OfType<TaxiDriver>().SingleOrDefault( t => t.id == idTaxiDriver );
            if (td == null) throw new UserNotExistException();

            td.taxi_id = idTaxi;

            ctx.SubmitChanges();
        }

        /// <summary>Funkcja zwrająca listę typów samochodów
        /// </summary>
        /// <returns>List<Car_type> </returns>
        public static List<Car_type> GetCarModelsList()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Car_types select c;
            return x.ToList();
        }

        /*  */

        /// <summary> Funkcja dodająca typ samochodu o podanej nazwie
        /// </summary>
        /// <param name="name"></param>
        public static void AddNewCarType(String name)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();

            Car_type ct = new Car_type();
            ct.name = name;

            ctx.Car_types.InsertOnSubmit(ct);
            ctx.SubmitChanges();
        }

        /// <summary>Funkcja usuwająca typ taksówki o podanym id
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteCarType(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Car_type ct = ctx.Car_types.SingleOrDefault( c => c.id == id );

            ctx.Car_types.DeleteOnSubmit(ct);
            ctx.SubmitChanges();
        }

        /// <summary>Funkcja zwraca Listę Typów użytkownika
        /// </summary>
        /// <returns>List<Employee_type> </returns>
        public static List<Employee_type> GetAllEmployeeTypes()
        {
            //get data context
            TaxiDataClassesDataContext dc = new TaxiDataClassesDataContext();

            var x = from i
                    in dc.Employee_types
                    select i;

            return x.ToList();
        }

        /// <summary>Funkcja aktualizująca położenie taksówkarza
        /// </summary>
        /// <returns></returns>
        public static bool setTaxiPosition(Decimal lon, Decimal lat,int idTaxi)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from i in ctx.Employees.OfType<TaxiDriver>() where i.id == idTaxi && i.employee_type_id == 1 select i;
            TaxiDriver td = x.SingleOrDefault();
            td.position_lon = lon;
            td.position_lat = lat;
            ctx.SubmitChanges();
            //na razie zwraca zawsze true ;), moze wypadaloby zwrocic stan taksowkarza?
            return true;
        }

        /// <summar> Funkcja zmieniająca status taksówkarza
        /// </summary>
        /// <param name="state"></param>
        /// <param name="idTaxi"></param>
        /// <returns></returns>
        public static bool setTaxiState(int state, int idTaxi)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from i in ctx.Employees.OfType<TaxiDriver>() where i.id == idTaxi && i.employee_type_id == 1 select i;
            TaxiDriver td = x.SingleOrDefault();
            var y = from i in ctx.Driver_status where i.id == state select i;
            td.Driver_status = y.SingleOrDefault();
            //na razie zwraca zawsze true ;), moze wypadaloby zwrocic stan taksowkarza?
            ctx.SubmitChanges();
            return true;
        }

        /// <summary>
        /// Funkcja zwraca skróconą informację dla taksowkarza o aktualnych kursach przypisanych do niego (WebService)
        /// </summary>
        /// <returns></returns>
        public static CourseData GetCourseData(int idDriver)
        {
            CourseData coursedata = new CourseData();
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from i
                    in ctx.Courses where i.taxidriver_id == idDriver && i.course_status_id == 3
                    select i;
            Course course = x.FirstOrDefault();
            coursedata.LocationName = course.startpoint_name;
            coursedata.ClientName = course.client_name;
            coursedata.ClientPhone = course.client_phone;
            coursedata.IdCourse = course.id;
            return coursedata;
        }

        /// <summary>
        /// Metoda usuwająca przypisanie kursu do taksowkarza (WebService)
        /// </summary>
        /// <param name="idDriver"></param>
        /// <returns></returns>
        public static bool DeclineCourse(int idDriver)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from i in ctx.Employees.OfType<TaxiDriver>() where i.id == idDriver select i;
            TaxiDriver td = x.SingleOrDefault();
            td.Courses = null;
            td.Courses1 = null;
            Course course = ctx.Courses.FirstOrDefault(c => c.taxidriver_id == idDriver && c.course_status_id == 3);
            course.taxidriver_id = null;
            course.course_status_id = 5;
            ctx.SubmitChanges();
            return true;
        }

        /// <summary>
        /// Funkcja prawdza, czy dla danego taksowkarza przypisane zostaly kursy (WebService)
        /// </summary>
        /// <param name="idDriver"></param>
        /// <returns></returns>
        public static bool isCourseAvailable(int idDriver)
        {
            CourseData coursedata = new CourseData();
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from i
                    in ctx.Courses
                    where i.taxidriver_id == idDriver && i.course_status_id == 1
                    select i;
            if (x.Count() > 0)
            {
                Course course = x.FirstOrDefault();
                course.taxidriver_id = idDriver;
                course.course_status_id = 3;
                ctx.SubmitChanges();
                return true;
            }
            else return false;
        }

        /* Marcina */
        /* Marcina */

        /// <summary>
        /// Funkcja pobiera kursy, któe nie sąprzypisane do żadnego taksówkarza
        /// </summary>
        /// <returns>List<Course></returns>
        public static List<Course> getAllUnsignedCourses()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Courses
                    where c.course_status_id == 1
                    select c;
            return x.ToList();
        }

        /// <summary>
        /// Funkcja pobiera wszystkie kursy
        /// </summary>
        /// <returns>List<Course></returns>
        public static List<Course> getAllCourses()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Courses
                    select c;
            return x.ToList();
        }

        /// <summary>
        /// Funkcja pobiera listę możliwych statusów kursów 
        /// </summary>
        /// <returns>List<Course_status></returns>
        public static List<Course_status> getCoursesStatus()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Course_status
                    select c;
            return x.ToList();
        }

        /// <summary>
        /// Funkcja pobiera listę kursów o określonym statusie
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List<Course></returns>
        public static List<Course> getCoursesByStatusId(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Courses
                    where c.course_status_id == id
                    orderby c.course_date ascending
                    select c;
            return x.ToList();
        }

        /// <summary>
        /// Funkcja pobiera listędostępnych typów samochodów
        /// </summary>
        /// <returns>List<Car_type></returns>
        public static List<Car_type> getAllCarTypes()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Car_types
                    select c;
            return x.ToList();
        }


        /// <summary>
        /// Funkcja dodająca nowy model samochodu
        /// </summary>
        /// <param name="brand"></param>
        /// <param name="model"></param>
        /// <param name="productionYear"></param>
        /// <param name="seatPlaces"></param>
        /// <returns>Identyfikator modelu samochodu</returns>
        public static int addNewCarModel(String brand, String model, String productionYear, String seatPlaces)
        {
            Car_model car_model = new Car_model();
            car_model.producer = brand;
            car_model.model = model;
            car_model.production_year = int.Parse(productionYear);
            car_model.seats = int.Parse(seatPlaces);

            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            ctx.Car_models.InsertOnSubmit(car_model);
            ctx.SubmitChanges();

            return car_model.id;
        }

        /// <summary>
        /// Funkcja pobierająca wszystkich administratorów systemu
        /// </summary>
        /// <returns>List<Employee> - lista administratorów</returns>
        public static List<Employee> getAllAdmins()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Employees
                    where c.employee_type_id == 3
                    select c;

            return x.ToList();
        }

        /// <summary>
        /// Funkcja pobierająca wszystkich dyspozytorów
        /// </summary>
        /// <returns>List<Employee> - lista dyspozytorów</returns>
        public static List<Employee> getAllDispatchers()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Employees
                    where c.employee_type_id == 2
                    select c;

            return x.ToList();
        }

        /// <summary>
        /// Funkcja pobiera listękursów o statusie "oczekujący"
        /// </summary>
        /// <returns> List<Course></returns>
        public static List<Course> getWaitingOrders()
        {
            return getCoursesByStatusId(1);
        }

        /// <summary>
        /// Funkcja pobiera listę zaakceptowanych kursów
        /// </summary>
        /// <returns>List<Course></returns>
        public static List<Course> getAcceptedOrders()
        {
            return getCoursesByStatusId(2);
        }

        /// <summary>
        /// Funkcja pobiera kursy o statusie "w trakcie wykonywania"
        /// </summary>
        /// <returns>List<Course></returns>
        public static List<Course> getInProgressOrders()
        {
            return getCoursesByStatusId(3);
        }

        /// <summary>
        /// Funkcja pobiera kursy o statusie "zakończony"
        /// </summary>
        /// <returns>List<Course></returns>
        public static List<Course> getDoneOrders()
        {
            return getCoursesByStatusId(4);
        }

        /// <summary>
        /// Funkcja pobiera kursy o statusie "Anulowany"
        /// </summary>
        /// <returns>List<Course></returns>
        public static List<Course> getCanceledOrders()
        {
            return getCoursesByStatusId(5);
        }

        /// <summary>
        /// Funkcja pobierająca kurs o określonym id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Course getCourseById(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Courses
                    where c.id == id
                    select c;


            return x.SingleOrDefault();
        }

        /// <summary>
        /// Funkcja edytująca dane kursu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="client"></param>
        /// <param name="phone"></param>
        /// <param name="startpoint"></param>
        /// <param name="date"></param>
        public static void editCourse(int id, string client, string phone, string startpoint, string date)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Courses
                    where c.id == id
                    select c;

            Course course = x.SingleOrDefault();
            course.client_name = client;
            course.client_phone = phone;
            course.startpoint_name = startpoint;
            course.date = DateTime.Parse(date);


            ctx.SubmitChanges();
        }

        /// <summary>
        /// Funkcja edytująca dane kursu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="destination"></param>
        /// <param name="date"></param>
        /// <param name="client"></param>
        /// <param name="clientPhone"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        public static void editCourse(int id, String destination, String date, String client, String clientPhone, Decimal lon, Decimal lat)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Courses
                    where c.id == id
                    select c;

            Course course = x.SingleOrDefault();
            course.client_name = client;
            course.client_phone = clientPhone;
            course.startpoint_name = destination;
            course.date = DateTime.Parse(date);
            course.startpoint_lon = lon;
            course.startpoint_lat = lat;

            ctx.SubmitChanges();
        }

        /// <summary>
        /// Funkcja usuwająca kurs o podanym id
        /// </summary>
        /// <param name="id"></param>
        public static void deleteCourse(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Course course = ctx.Courses.SingleOrDefault(c => c.id == id);

            ctx.Courses.DeleteOnSubmit(course);
            ctx.SubmitChanges();
        }

        /// <summary>
        /// Funkcja anulująca dany kurs
        /// </summary>
        /// <param name="id"></param>
        public static void cancelCourse(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Course course = ctx.Courses.SingleOrDefault(c => c.id == id);
            TaxiDriver driver = ctx.Employees.OfType<TaxiDriver>().SingleOrDefault(t => t.id == course.taxidriver_id);
            driver.driver_status_id = 0;
            course.course_status_id = 5;
            ctx.SubmitChanges();
        }

        public static List<CourseAccepedView> getWaitingOrdersView()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.CourseAccepedViews
                    orderby c.course_date ascending
                    select c;

            return x.ToList();
        }

        public static List<CourseInProgressView> getAcceptedOrdersView()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.CourseInProgressViews
                    where c.course_status_id == 2
                    orderby c.course_date ascending
                    select c;

            return x.ToList();
        }

        public static List<CourseInProgressView> getInProgressOrdersView()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.CourseInProgressViews
                    where c.course_status_id == 3
                    orderby c.course_date ascending
                    select c;

            return x.ToList();
        }

        public static List<CourseInProgressView> getDoneOrdersView()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.CourseInProgressViews
                    where c.course_status_id == 4
                    orderby c.course_date ascending
                    select c;

            return x.ToList();
        }

        public static List<CourseInProgressView> getCanceledOrdersView()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.CourseInProgressViews
                    where c.course_status_id == 5
                    orderby c.course_date ascending
                    select c;

            return x.ToList();
        }

        public static List<EmployeeView> getEmployeeView()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.EmployeeViews
                    select c;

            return x.ToList();
        }

        public static List<TaxiDriverView> getTaxiDriversView()
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.TaxiDriverViews
                    select c;

            return x.ToList();
        }



        public static Employee getEmployeeById(int id)
        {
            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            var x = from c in ctx.Employees
                    where c.id == id
                    select c;
            return x.SingleOrDefault();
        }
        public static void editCourse(int id, String destination, String date, String client, String clientPhone, Decimal lon, Decimal lat, int car_type_id, int seats)
        {

            TaxiDataClassesDataContext ctx = new TaxiDataClassesDataContext();
            Course course = ctx.Courses.SingleOrDefault(c => c.id == id);

            course.client_name = client;
            course.client_phone = clientPhone;
            course.startpoint_name = destination;
            course.course_date = DateTime.Parse(date);
            course.startpoint_lon = lon;
            course.startpoint_lat = lat;
            course.seats = seats;
            course.car_type_id = car_type_id;
            ctx.SubmitChanges();

        }


        /* PRIVATE MEMBER */

        #region Losowy ciąg znaków o zadanej długości
        #endregion
        private static string CreateRandomString(int length)
        {

            Random random = new Random();

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(random.Next().ToString()));
            string password = Convert.ToBase64String(hash).Substring(0, length);
            string newPass = "";

            // Uppercase at random 
            random = new Random();
            for (int i = 0; i < password.Length; i++)
            {
                if (random.Next(0, 2) == 1)
                    newPass += password.Substring(i, 1).ToUpper();
                else
                    newPass += password.Substring(i, 1);
            }
            return newPass;
        }

        #region Suma SHA1
        #endregion
        private static string CalculateSHA1(string text, Encoding enc)
        {
            byte[] buffer = enc.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).ToLower().Replace("-", "");
            return hash;
        }

      
    }
}
