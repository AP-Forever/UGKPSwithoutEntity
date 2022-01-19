using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UGKPSwithoutEntity.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using UGKPSwithoutEntity.Controllers;
using System.Dynamic;
using UGKPSwithoutEntity.Attributes;
using Newtonsoft.Json;
using PagedList;

namespace UGKPSwithoutEntity.Controllers
{
    public class HomeController : AccountController
    {
        GeneralFunctions functions = new GeneralFunctions(); //new instace of general functions to use all functions within

        //Connection string from web.config
        readonly string _mainconnString = ConfigurationManager.ConnectionStrings["ugkpsConnectionString"].ConnectionString; 

        // GET: Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // POST: Home/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ContactUs contactusdetails)
        {
            
            if (ModelState.IsValid)
            {
                contactusdetails.IsMember = functions.CheckUser("EmailID", contactusdetails.EmailID) != null ? true : false;
                int _queryResult = functions.InsertInquiryDetails(contactusdetails, contactusdetails.IsMember);
                if (_queryResult != 0)
                {
                    ModelState.Clear();
                    ViewBag.Message = "Success! Thank you for writing to us. We will get back to you soon!";
                }
                else
                {
                    ViewBag.Message = "Oops! Something went wrong. Your form was not submitted!";
                }
            }
            return View();
        }
        
        // GET: Home/Users
        [HttpGet]
        [CustomAuthorize(UserRole = "Admin")]
        public ActionResult Users(string sortBy, int? page, int? FetchCode)
        {
            //Sorting Implementation 12-05-2020
            #region sort Parameters Viewbag insert/update

            if (!string.IsNullOrEmpty(sortBy))
            {
                TempData["SortParameterName"] = sortBy;
            }
            else
            {
                TempData["SortParameterName"] = "UserIDDesc";
            }

            ViewBag.SortByParameter = sortBy;

            #endregion
            string _query = "";
            switch (FetchCode)
            {
                case 0:
                    _query = "SELECT * FROM [dbo].[Users] Where IsEmailVerified='true'";
                    ViewBag.BtnClicked = "ShowUsers_" + FetchCode;
                    break;
                case 1:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsNewRequest = 'true' AND IsEmailVerified='true'";
                    ViewBag.BtnClicked = "ShowUsers_"+FetchCode;
                    break;
                case 2:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsUserActivated = 'true' AND IsEmailVerified='true'";
                    ViewBag.BtnClicked = "ShowUsers_" + FetchCode;
                    break;
                case 3:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsUserActivated = 'false' " +
                        "AND IsNewRequest = 'false' AND IsDeactivated = 'false' AND IsEmailVerified='true'";
                    ViewBag.BtnClicked = "ShowUsers_" + FetchCode;
                    break;
                case 4:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsDeactivated = 'true' AND IsEmailVerified='true'";
                    ViewBag.BtnClicked = "ShowUsers_" + FetchCode;
                    break;
                default:
                    _query = "SELECT * FROM [dbo].[Users] Where IsEmailVerified='true'";
                    ViewBag.BtnClicked = "ShowUsers_0";
                    break;
            }

            DataTable dtblUsers = new DataTable();
            using(SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daUsers = new SqlDataAdapter(_query, _mainconnString);
                daUsers.Fill(dtblUsers);
            }

            List<User> List = new List<User>();

            for (int i = 0; i < dtblUsers.Rows.Count; i++)
            {
                User user = new User();
                for (int j = 0; j < dtblUsers.Columns.Count; j++)
                {  
                    string property = dtblUsers.Columns[j].ColumnName.ToString();
                    if ((property != "PWResetCode") && (property != "ActivationCode") && (property != "DateOfBirth"))
                    {
                        user.GetType().GetProperty(property).SetValue(user, dtblUsers.Rows[i][j]);
                    }
                }
                user.UserFamily = functions.GetUserFamilyMembers(user);

                foreach(FamilyMember member in user.UserFamily)
                {
                    int ageDiffTillNow = DateTime.Now.Year - member.CreatedDate.Year;
                    if (DateTime.Now.DayOfYear < member.CreatedDate.DayOfYear) ageDiffTillNow = ageDiffTillNow - 1;
                    member.Age += ageDiffTillNow;
                }

                InvestmentAccess UserInvestmentAccessDetails = functions.GetUsersInvestmentAccessDetails(user.UserID);
                user.InvestAccess = UserInvestmentAccessDetails;

                

                List.Add(user);
            }
            

            foreach(User user in List)
            {
                int ageDiffTillNow = DateTime.Now.Year - user.CreatedDate.Year;
                if (DateTime.Now.DayOfYear < user.CreatedDate.DayOfYear) ageDiffTillNow = ageDiffTillNow - 1;
                user.Age += ageDiffTillNow;

                List<string> listOfAccountAction = new List<string> { "Approve", "Disapprove", "Deactivate" };
                if (user.IsNewRequest) listOfAccountAction.Remove("Deactivate");
                else if (user.IsUserActivated)
                {
                    listOfAccountAction.Remove("Approve");
                    listOfAccountAction.Remove("Disapprove");
                }
                else if (user.IsDeactivated)
                {
                    listOfAccountAction.Clear();
                    listOfAccountAction.Add("Activate");
                }
                else
                {
                    listOfAccountAction.Remove("Deactivate");
                    listOfAccountAction.Remove("Disapprove");
                }

                ViewData["ListOfAccountAction" + user.UserID.ToString()] = new SelectList(listOfAccountAction, "Account Action");
            }

            var model = new ListOfUsers
            {
                UsersList = List
            };

            //Sorting Implementation 12-05-2020
            #region Implement Sorting through OrderBy in List
            var sortByParameter = (string)TempData["SortParameterName"];

            if (sortByParameter.IndexOf("Desc") >= 0)
            {
                sortByParameter = sortByParameter.Substring(0, sortByParameter.IndexOf("Desc"));
            }

            var propertyInfo = typeof(User).GetProperty(sortByParameter);

            if (((string)TempData["SortParameterName"]).Contains("Desc"))
            {
                List = List.OrderByDescending(user => propertyInfo.GetValue(user, null)).ToList();
            }
            else
            {
                List = List.OrderBy(user => propertyInfo.GetValue(user, null)).ToList();
            }
            model.UsersList = List;
            #endregion

            return View(List.ToPagedList(page ?? 1, 10));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Users(List<User> users)
        {
            TempData.Remove("Message");
            int IsUserActivated = 0;
            int IsDeactivated = 0;
            int _queryResult = 0;
            for (int i=0; i<users.Count; i++)
            {
                if (users[i].IsAdmin == false)
                {
                    InvestmentAccess UnchangedUserInvestmentAccessDetails = functions.GetUsersInvestmentAccessDetails(users[i].UserID);
                    if (users[i].AccountAction != null)
                    {
                        if ((users[i].AccountAction == "Approve") || (users[i].AccountAction == "Activate"))
                        {
                            IsUserActivated = 1; IsDeactivated = 0;
                        }
                        if (users[i].AccountAction == "Disapprove") IsUserActivated = 0;
                        if (users[i].AccountAction == "Deactivate")
                        {
                            IsUserActivated = 0; IsDeactivated = 1;
                        }


                        using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                        {
                            using (SqlCommand sqlcmd = new SqlCommand("sp_ApproveDisapproveUser", sqlCon))
                            {
                                sqlcmd.CommandType = CommandType.StoredProcedure;
                                sqlCon.Open();
                                sqlcmd.Parameters.AddWithValue("@IsUserActivated", IsUserActivated);
                                sqlcmd.Parameters.AddWithValue("@IsDeactivated", IsDeactivated);
                                sqlcmd.Parameters.AddWithValue("@UserID", users[i].UserID);
                                _queryResult = sqlcmd.ExecuteNonQuery();
                                if (_queryResult > 0)
                                {
                                    TempData["Message"] = TempData["Message"] + users[i].FirstName + " " + users[i].LastName 
                                        + " - Command successully executed for Account Action!<br/>";
                                    string body = "";
                                    string subject = "";
                                    if (users[i].AccountAction == "Approve" || users[i].AccountAction == "Activate")
                                    {
                                        body = "Dear " + users[i].FirstName + " " + users[i].LastName + ",<br/><br/>"
                                                        + "Upon reviewing your registration details, we are glad to inform you that your " +
                                                        "account has been activated.<br/><br/>"
                                                        + "You can login to the UGKPS website using your registration credentials. <br/><br/>";
                                        subject = "Account Activated";
                                    }
                                    if (users[i].AccountAction == "Disapprove")
                                    {
                                        body = "Dear " + users[i].FirstName + " " + users[i].LastName + ",<br/><br/>"
                                                        + "Upon reviewing your registration details, we regret to inform you that you " +
                                                        "do not match the criteria for UGKPS website usage.<br/><br/>"
                                                        + "If you need to reach out to us, please feel free to use the UGKPS website " +
                                                        "Contact Us form to write to us. <br/><br/>";
                                        subject = "Account Status";
                                    }
                                    if (users[i].AccountAction == "Deactivate")
                                    {
                                        body = "Dear " + users[i].FirstName + " " + users[i].LastName + ",<br/><br/>"
                                                        + "It is with great regret we inform you that your account has been deactivated" +
                                                        " because it is in violation of UGKPS usage policy.<br/><br/>"
                                                        + "If you need to reach out to us, please feel free to use the UGKPS website " +
                                                        "Contact Us form to write to us. <br/><br/>";
                                        subject = "Account Deactivated";
                                    }

                                    if (functions.SendEmail(users[i].EmailID, body, subject) == null)
                                    {
                                        ModelState.Clear();
                                    }
                                    else
                                    {
                                        TempData["Message"] = TempData["Message"] + users[i].FirstName + " " + users[i].LastName
                                            + " - Something went wrong!!! Error sending Email.<br/>";
                                    }
                                }
                                else
                                {
                                    TempData["Message"] = TempData["Message"] + users[i].FirstName + " " + users[i].LastName
                                        + " - Something went wrong with request!!!<br/>";
                                }
                            }
                        }

                        #region Insert FamilyMemberDetails Self
                        FamilyMember member = new FamilyMember
                        {
                            FirstName = users[i].FirstName,
                            LastName = users[i].LastName,
                            Age = users[i].Age,
                            Relation = "Self"
                        };
                        _queryResult = functions.CheckExistingAndInsertFamilyMemberDetails(member, users[i].UserID);
                        if (_queryResult > 0)
                        {
                            TempData["Message"] = TempData["Message"] + users[i].FirstName + " " + users[i].LastName
                                + " - Command successully executed for Adding user as Self family Member <br/>";
                            ModelState.Clear();

                        }
                        #endregion

                    }

                    #region InvestmentTab Access Record Insert
                    InvestmentAccess investAccessUser = new InvestmentAccess
                    {
                        UserID = users[i].UserID,
                        IsInvestTabVisible = users[i].InvestAccess.IsInvestTabVisible,
                        HasAcceptedDisclaimer = false
                    };
                    _queryResult = functions.CheckExistingAndInsertInvestmentAccessRecord(investAccessUser);
                    if (_queryResult > 0)
                    {
                        TempData["Message"] = TempData["Message"] + users[i].FirstName + " " + users[i].LastName
                            + " - Command successully executed for Adding user's Investment Tab Access record<br/>";
                        ModelState.Clear();
                    }
                    #endregion

                    #region InvestmentTab Access Record Update
                    if(UnchangedUserInvestmentAccessDetails.IsInvestTabVisible != users[i].InvestAccess.IsInvestTabVisible)
                    {
                        investAccessUser.InvestTab_User_Spec_ID = UnchangedUserInvestmentAccessDetails.InvestTab_User_Spec_ID;
                        investAccessUser.IsInvestTabVisible = users[i].InvestAccess.IsInvestTabVisible;
                        investAccessUser.HasAcceptedDisclaimer = false;
                        investAccessUser.DateAccepted = null;
                    
                        _queryResult = functions.UpdateInvestmentAccessRecord(investAccessUser);
                        if (_queryResult <= 0)
                        {
                            TempData["Message"] = TempData["Message"] + users[i].FirstName + " " + users[i].LastName
                            + " - Something went wrong!!! Error updating User Investment Tab Access<br/>";
                        }
                        else
                        {
                            TempData["Message"] = TempData["Message"] + users[i].FirstName + " " + users[i].LastName
                            + " - Command successully executed!<br/>";
                        }
                    }
                    #endregion
                }

                
            }
            return RedirectToAction("Users");
            //return View(users);
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactUs(ContactUs contactusdetails)
        {
            bool IsMember;
            if (ModelState.IsValid)
            {
                IsMember = functions.CheckUser("EmailID", contactusdetails.EmailID) != null ? true : false;
                int _queryResult = functions.InsertInquiryDetails(contactusdetails, IsMember);
                if (_queryResult != 0)
                {
                    ModelState.Clear();
                    ViewBag.Message = "Success! Thank you for writing to us. We will get back to you soon!";
                }
                else
                {
                    ViewBag.Message = "Oops! Something went wrong. Your form was not submitted!";
                }
            }
            return View();
        }

        public ActionResult Events()
        {
            EventsViewModel model = new EventsViewModel();
            model.currEvent = functions.GetAndFillCurrentEvent();
            User user = functions.GetUserDetails(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
            user.UserFamily = functions.GetUserFamilyMembers(user);
            model.Members = user.UserFamily;

            if (model.currEvent.Event_EndDate < DateTime.Now)
            {
                model.currEvent = new Event();
                model.Members = new List<FamilyMember>();
                return View(model);
            }
            if ((model.currEvent.Event_StartDate - DateTime.Now).TotalDays < 10)
            {
                ViewData["RegistrationClosed"] = "Registration Closed";
                return View(model);
            }

            

            if (Session["UserID"] != null)
            {
                model.Members = functions.GetUserEventRegistrationInfo(model.Members);
                foreach(FamilyMember member in model.Members)
                {
                    if (member.IsChecked)
                    {
                        ViewData["AlreadyRegisteredForEvent"] = "yes";
                        break;
                    }
                    int ageDiffTillNow = DateTime.Now.Year - member.CreatedDate.Year;
                    if (DateTime.Now.DayOfYear < member.CreatedDate.DayOfYear) ageDiffTillNow = ageDiffTillNow - 1;
                    member.Age += ageDiffTillNow;

                }
            }
            else
            {
                ViewData["LoginToRegister"] = "Please login to register for this event.";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Events(List<FamilyMember> members)
        {
            int _queryResult = 0;
            int addedMembers = 0;
            int changedMembers = 0;
            bool alreadyRegistered = false;
            bool firstRegister = true;

            Event CurrEvent = functions.GetAndFillCurrentEvent();

            User user = functions.GetUserDetails(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
            user.UserFamily = functions.GetUserFamilyMembers(user);

            List<FamilyMember> unchangedRegistrationInfo = functions.GetUserEventRegistrationInfo(user.UserFamily);
            foreach (FamilyMember member in unchangedRegistrationInfo)
            {
                if (member.IsChecked)
                {
                    firstRegister = false;
                    break;
                }
            }
            foreach (FamilyMember member in members)
            {
                if (member.IsChecked)
                {
                    alreadyRegistered = true;
                    if (functions.InsertEventRegistrationDetails(member, CurrEvent.EventID) > 0){
                        _queryResult++;
                        addedMembers++;
                    }
                }
                else
                {
                    if(functions.RemoveFamilyMemberFromEventRegistration(member.FamilyMemberID, CurrEvent.EventID) > 0) { 
                        _queryResult++;
                        changedMembers++;
                    }
                }
            }
            if(_queryResult > 0)
            {
                if(addedMembers > 0 && changedMembers==0)
                {
                    if (alreadyRegistered && !firstRegister)
                    {
                        ViewBag.Message = "You have successfully updated your registration for this event.";
                        ViewData["AlreadyRegisteredForEvent"] = "yes";
                    }
                    else
                    {
                        ViewBag.Message = "You have successfully registered for this event.";
                        ViewData["AlreadyRegisteredForEvent"] = "yes";
                    }
                }
                else if(addedMembers > 0 && changedMembers > 0)
                {
                    ViewBag.Message = "You have successfully updated your registration for this event.";
                    ViewData["AlreadyRegisteredForEvent"] = "yes";
                }
                else
                {
                    ViewBag.Message = "You have successfully updated your registration for this event.";
                    if (!alreadyRegistered)
                    {
                        ViewData["AlreadyRegisteredForEvent"] = "no";
                    }
                    else
                    {
                        ViewData["AlreadyRegisteredForEvent"] = "yes";
                    }
                }
            }
            //if ((_queryResult == 0) && (changedMembers == 0) && (addedMembers == 0))
            //{

            //} else if (_queryResult == changedMembers)
            //{
            //    ViewBag.Message = "You have successfully registered for this event.";
            //    ViewData["AlreadyRegisteredForEvent"] = "yes";
            //}
            EventsViewModel model = new EventsViewModel();
            model.currEvent = functions.GetAndFillCurrentEvent();
            model.Members = members;
            return View(model);
        }

        [HttpGet]
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CookiesPolicy()
        {
            return View();
        }

        public ActionResult FetchUsers(int FetchCode, int? page)
        {
            string _query = "";
            switch (FetchCode)
            {
                case 0:
                    _query = "SELECT * FROM [dbo].[Users] ORDER BY UserID DESC";
                    break;
                case 1:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsNewRequest = 'true' ORDER BY UserID DESC";
                    break;
                case 2:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsUserActivated = 'true' ORDER BY UserID DESC";
                    break;
                case 3:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsUserActivated = 'false' " +
                        "AND IsNewRequest = 'false' AND IsDeactivated = 'false' ORDER BY UserID DESC";
                    break;
                case 4:
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsDeactivated = 'true' ORDER BY UserID DESC";
                    break;
            }
            DataTable dtblUsers = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daUsers = new SqlDataAdapter(_query, _mainconnString);
                daUsers.Fill(dtblUsers);
            }

            List<User> List = new List<User>();

            for (int i = 0; i < dtblUsers.Rows.Count; i++)
            {
                User user = new User();
                for (int j = 0; j < dtblUsers.Columns.Count; j++)
                {
                    string property = dtblUsers.Columns[j].ColumnName.ToString();
                    if ((property != "PWResetCode") && (property != "ActivationCode") && (property != "DateOfBirth"))
                    {
                        user.GetType().GetProperty(property).SetValue(user, dtblUsers.Rows[i][j]);
                    }
                }
                user.UserFamily = functions.GetUserFamilyMembers(user);

                foreach (FamilyMember member in user.UserFamily)
                {
                    int ageDiffTillNow = DateTime.Now.Year - member.CreatedDate.Year;
                    if (DateTime.Now.DayOfYear < member.CreatedDate.DayOfYear) ageDiffTillNow = ageDiffTillNow - 1;
                    member.Age += ageDiffTillNow;
                }

                InvestmentAccess UserInvestmentAccessDetails = functions.GetUsersInvestmentAccessDetails(user.UserID);
                user.InvestAccess = UserInvestmentAccessDetails;
                List.Add(user);
            }

            foreach (User user in List)
            {
                int ageDiffTillNow = DateTime.Now.Year - user.CreatedDate.Year;
                if (DateTime.Now.DayOfYear < user.CreatedDate.DayOfYear) ageDiffTillNow = ageDiffTillNow - 1;
                user.Age += ageDiffTillNow;

                List<string> listOfAccountAction = new List<string> { "Approve", "Disapprove", "Deactivate" };
                if (user.IsNewRequest) listOfAccountAction.Remove("Deactivate");
                else if (user.IsUserActivated)
                {
                    listOfAccountAction.Remove("Approve");
                    listOfAccountAction.Remove("Disapprove");
                }
                else if (user.IsDeactivated)
                {
                    listOfAccountAction.Clear();
                    listOfAccountAction.Add("Activate");
                }
                else
                {
                    listOfAccountAction.Remove("Deactivate");
                    listOfAccountAction.Remove("Disapprove");
                }

                ViewData["ListOfAccountAction" + user.UserID.ToString()] = new SelectList(listOfAccountAction, "Account Action");
            }

            if (List != null)
            {
                return View("Users", List.ToPagedList(page ?? 1, 10));
                //return Json(List, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View("Users", List.ToPagedList(page ?? 1, 10));
                //return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpGet]
        public ActionResult CommitteeMembers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CommitteeMembers(string id)
        {
            return View();
        }

        [CustomAuthorize(UserRole = "Admin")]
        public ActionResult EventRegistrations(string sortBy, int? page)
        {
            //Sorting Implementation 12-05-2020
            #region sort Parameters Viewbag insert/update

            if (!string.IsNullOrEmpty(sortBy))
            {
                TempData["SortParameterName"] = sortBy;
            }
            else
            {
                TempData["SortParameterName"] = "UserID";
            }

            ViewBag.SortByParameter = sortBy;

            #endregion

            EventRegistrationsViewModel model = functions.GetEventRegistrationsDetails();
            model = functions.CalculateTotalFeesPerUser(model);
            model = functions.CheckUser_AlreadyAttendedEvent(model);


            //Sorting Implementation 12-05-2020
            #region Implement Sorting through OrderBy in List
            var sortByParameter = (string)TempData["SortParameterName"];

            if (sortByParameter.IndexOf("Desc") >= 0)
            {
                sortByParameter = sortByParameter.Substring(0, sortByParameter.IndexOf("Desc"));
            }

            var propertyInfo = typeof(User).GetProperty(sortByParameter);

            if (((string)TempData["SortParameterName"]).Contains("Desc"))
            {
                model.ListOfUsers = model.ListOfUsers.OrderByDescending(user => propertyInfo.GetValue(user, null)).ToList();
            }
            else
            {
                model.ListOfUsers = model.ListOfUsers.OrderBy(user => propertyInfo.GetValue(user, null)).ToList();
            }
            
            #endregion

            return View(model.ListOfUsers.ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        [CustomAuthorize(UserRole = "Admin")]
        public ActionResult AdminArea()
        {
            #region Get and Fill Users as dropdown
            string _query = "SELECT * FROM [dbo].[Users] ORDER BY UserID DESC";

            DataTable dtblUsers = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daUsers = new SqlDataAdapter(_query, _mainconnString);
                daUsers.Fill(dtblUsers);
            }

            List<User> List = new List<User>();

            for (int i = 0; i < dtblUsers.Rows.Count; i++)
            {
                User user = new User();
                for (int j = 0; j < dtblUsers.Columns.Count; j++)
                {
                    string property = dtblUsers.Columns[j].ColumnName.ToString();
                    if ((property != "PWResetCode") && (property != "ActivationCode") && (property != "DateOfBirth"))
                    {
                        user.GetType().GetProperty(property).SetValue(user, dtblUsers.Rows[i][j]);
                    }
                }
                List.Add(user);
            }
            var listOfEmailRecipients = new List<SelectListItem>();
            listOfEmailRecipients.Add(new SelectListItem() { Value = "All", Text = "All" });
            listOfEmailRecipients.Add(new SelectListItem() { Value = "Active Members", Text = "Active Members" });
            listOfEmailRecipients.Add(new SelectListItem() { Value = "Non-Active Members", Text = "Non-Active Members" });
            listOfEmailRecipients.Add(new SelectListItem() { Value = "", Text = "------------------------------------------------------------" });

            string userDetails = "";

            foreach (User user in List)
            {
                userDetails = user.FirstName.ToString() + " " + user.LastName.ToString() + ", " + user.Native.ToString();
                listOfEmailRecipients.Add(new SelectListItem() { Value = user.UserID.ToString(), Text = userDetails });
            }


            //List<string> listOfEmailRecipients = new List<string> { "All", "Active Members", "Non-Active Members" };
            ViewData["ListOfEmailRecipients"] = new SelectList(listOfEmailRecipients, "Value", "Text");
            #endregion

            List<Event> events = functions.GetEvents();
            var EventList = new List<SelectListItem>();

            foreach(Event evnt in events)
            {
                EventList.Add( new SelectListItem() { Value = evnt.EventID.ToString(), Text = evnt.Event_Name });
            }
            EventList.Add(new SelectListItem() { Value = "0", Text = "None" });


            ViewData["ListOfEvents"] = new SelectList(EventList, "Value", "Text");



            AdminAreaViewModel adminAreaModel = new AdminAreaViewModel();
            _query = "SELECT * FROM [dbo].[Inquiries] ORDER BY InquiryID DESC";

            DataTable dtblInquiries = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daInquiries = new SqlDataAdapter(_query, _mainconnString);
                daInquiries.Fill(dtblInquiries);
            }

            adminAreaModel.Inquiries = new List<ContactUs>();

            for (int i = 0; i < dtblInquiries.Rows.Count; i++)
            {
                ContactUs inquiry = new ContactUs();
                for (int j = 0; j < dtblInquiries.Columns.Count; j++)
                {
                    string property = dtblInquiries.Columns[j].ColumnName.ToString();
                    inquiry.GetType().GetProperty(property).SetValue(inquiry, dtblInquiries.Rows[i][j]);
                    
                }
                adminAreaModel.Inquiries.Add(inquiry);
            }

            return View(adminAreaModel);
        }

        public JsonResult SendEmailToMembers(EmailModel emailModel)
        {
            string _query = "";
            switch (emailModel.SendTo)
            {
                case "All":
                    _query = "SELECT * FROM [dbo].[Users] ORDER BY UserID DESC";
                    break;
                case "Active Members":
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsUserActivated = 'true' ORDER BY UserID DESC";
                    break;
                case "Non-Active Members":
                    _query = "SELECT * FROM [dbo].[Users] WHERE IsUserActivated = 'false' AND IsNewRequest = 'false' ORDER BY UserID DESC";
                    break;
                default:
                    _query = "SELECT * FROM [dbo].[Users] WHERE UserID=" + Convert.ToInt32(emailModel.SendTo) + " ORDER BY UserID DESC";
                    break;
            }


            DataTable dtblUsers = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daUsers = new SqlDataAdapter(_query, _mainconnString);
                daUsers.Fill(dtblUsers);
            }

            List<User> List = new List<User>();

            for (int i = 0; i < dtblUsers.Rows.Count; i++)
            {
                User user = new User();
                for (int j = 0; j < dtblUsers.Columns.Count; j++)
                {
                    string property = dtblUsers.Columns[j].ColumnName.ToString();
                    if ( (property != "PWResetCode") && (property != "ActivationCode") && (property != "DateOfBirth"))
                    {
                        user.GetType().GetProperty(property).SetValue(user, dtblUsers.Rows[i][j]);
                    }
                }
                List.Add(user);
            }

            foreach(User user in List)
            {
                var emailBody = "Dear " + user.FirstName + " " + user.LastName + ",<br/><br/><pre>"
                                                    + emailModel.Body + "</pre><br/><br/>";
                if (functions.SendEmail(user.EmailID, emailBody, emailModel.Subject) != null)
                {
                    ModelState.Clear();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            ModelState.Clear();
            ViewBag.Message = "Email Sent to Members.";
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ActivateEvent(EventSelectionModel selectedEvent)
        {
            int _queryResult = functions.ActivateEvent(Convert.ToInt32(selectedEvent.SelectedEventID));
            if(_queryResult > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MarkUserAsAttendedEvent(int UserID)
        {
            int _queryResult = functions.MarkUserAsAttendedEvent(UserID);
            if (_queryResult > 0)
            {
                return Json(new { data = UserID, success= true }, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddEvent(Event myevent)
        {
            if (ModelState.IsValid)
            {
                if (myevent.Fees_Membership == null) myevent.Fees_Membership = 0;
                int _queryResult = functions.AddEvent(myevent);
                if (_queryResult > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReplyToInquiry(ContactUs inquiry)
        {
            string body = "Dear " + inquiry.FirstName + " " + inquiry.LastName + ",<br/><br/>" + inquiry.ReplyMessage
                                                        + " <br/><br/>";
            string subject = "Reply to your Inquiry";

            int _queryResult = functions.UpdateInquiry(Convert.ToInt32(inquiry.InquiryID));

            if (functions.SendEmail(inquiry.EmailID, body, subject) == null)
            {
                ModelState.Clear();
                string value = string.Empty;
                value = JsonConvert.SerializeObject(inquiry, Formatting.Indented);
                return Json(new
                {
                    success = true,
                    InquiryID = inquiry.InquiryID
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}