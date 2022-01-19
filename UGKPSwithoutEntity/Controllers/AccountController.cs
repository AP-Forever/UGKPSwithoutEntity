using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UGKPSwithoutEntity.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using UGKPSwithoutEntity.Useful_Classes;
using UGKPSwithoutEntity.Attributes;

namespace UGKPSwithoutEntity.Controllers
{
    public class AccountController : Controller
    {
        GeneralFunctions functions = new GeneralFunctions();
        string _mainconnString = ConfigurationManager.ConnectionStrings["ugkpsConnectionString"].ConnectionString; //Connection string from web.config

        // GET: Account/Registration
        public ActionResult Registration()
        {
            //List<SelectListItem> countries = new List<SelectListItem>();
            //countries.Add(new SelectListItem { Text = "Canada", Value = "0" });
            //countries.Add(new SelectListItem { Text = "USA", Value = "1" });
            //ViewData["Countries"] = countries;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(Registration UserRegisterInfo, string Country)
        {
            if (ModelState.IsValid)
            {
                if (functions.CheckUser("EmailID", UserRegisterInfo.EmailID) == null)
                {
                    UserRegisterInfo.Password = EncryptPW.Hash(UserRegisterInfo.Password);
                    UserRegisterInfo.ConfirmPassword = EncryptPW.Hash(UserRegisterInfo.ConfirmPassword);
                    string activationCode = Guid.NewGuid().ToString();
                    int _queryResult = functions.RegisterUser(UserRegisterInfo, Country, activationCode);
                    if (_queryResult > 0)
                    {
                        string body = "Dear " + UserRegisterInfo.FirstName + " " + UserRegisterInfo.LastName + ",<br/><br/>"
                                        + "Thank you for becoming part of Uttar Gujarat Kadva Patidar Samaaj in North America.<br/><br/>"
                                        + "Your account has been successfully created. <br/><br/>"
                                        + "Please click on 'Verify Email' link to verify your account"
                                        + " <br/><br/><a href='" + Url.Action("VerifyAccount", "Account", new { ActivationCode = activationCode }, "http") + "'>Verify Email</a>"
                                        //+ "We will be reviewing your details in timely manner. Once reviewed, you will receive an email from us with the status of your account.<br/><br/>"
                                        + "<br/><br/>";
                        string subject = "Welcome to UGKPS";
                        if (functions.SendEmail(UserRegisterInfo.EmailID, body, subject) == null)
                        {
                            ModelState.Clear();
                            ViewBag.Message = "Registration request successfully submitted. "
                                            + UserRegisterInfo.FirstName + " " + UserRegisterInfo.LastName
                                            + ", you are now just 1 step away from becoming part of UGKPS.";
                        }
                        else
                        {
                            ViewBag.Message = "Something went wrong!!! Error sending Email.";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Something went wrong!!! Error registering User.";
                    }
                }
                            
                else
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(UserRegisterInfo);
                }
            }
            return View();
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult UserProfile()
        {
            User user = functions.GetUserDetails(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
            //List<string> listOfRelations = new List<string>{ "Father", "Mother", "Wife", "Husband", "Son", "Daughter", "Neice", "Nephew", "Son-In-Law", "Daughter-In-Law", "Grandson", "Granddaughter", "Uncle", "Aunt" };
            //ViewBag.ListOfRelations = new SelectList(listOfRelations, "Relation");
            return View(user);
        }

        [HttpPost]
        public ActionResult UserProfile(User user)
        {
            //List<string> listOfRelations = new List<string> { "Father", "Mother", "Wife", "Husband", "Son", "Daughter", "Neice", "Nephew", "Son-In-Law", "Daughter-In-Law", "Grandson", "Granddaughter", "Uncle", "Aunt" };
            //ViewBag.ListOfRelations = new SelectList(listOfRelations, "Relation");
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
                {

                    using (SqlCommand sqlcmd = new SqlCommand("sp_UpdateUserProfile", sqlcon))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@UserID", user.UserID);
                        sqlcmd.Parameters.AddWithValue("@Address", user.Address);
                        sqlcmd.Parameters.AddWithValue("@City", user.City);
                        sqlcmd.Parameters.AddWithValue("@State", user.State);
                        sqlcmd.Parameters.AddWithValue("@Country", user.Country);
                        sqlcmd.Parameters.AddWithValue("@ZipCode", user.ZipCode);
                        sqlcmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                        sqlcmd.Parameters.AddWithValue("@Native", user.Native);
                        sqlcon.Open();
                        try
                        {
                            int _queryResult = sqlcmd.ExecuteNonQuery();
                            if (_queryResult > 0)
                            {
                                ModelState.Clear();
                                return View(user);
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }

                    }
                }
            }
            
            return View(user);
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult FamilyMembers()
        {
            User user = functions.GetUserDetails(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
            List<string> listOfRelations = new List<string> { "Father", "Mother", "Wife", "Husband", "Son", "Daughter", "Neice", "Nephew", "Son-In-Law", "Daughter-In-Law", "Grandson", "Granddaughter", "Uncle", "Aunt" };
            ViewBag.ListOfRelations = new SelectList(listOfRelations, "Relation");
            return View(user);
        }

        //GET - Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //POST - Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login UserLoginInfo)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                {
                    string selectquery = "Select * from [dbo].[Users] where (EmailID = @EmailID)";
                    using (SqlCommand sqlcmd = new SqlCommand(selectquery, sqlCon))
                    {
                        sqlcmd.Parameters.AddWithValue("@EmailID", UserLoginInfo.EmailID);
                        sqlCon.Open();
                        SqlDataReader reader = sqlcmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if ((bool)reader["IsEmailVerified"])
                                {
                                    if (string.Compare(EncryptPW.Hash(UserLoginInfo.Password), reader["Password"].ToString()) == 0)
                                    {

                                        if ((bool)reader["IsUserActivated"])
                                        {
                                            Session["UserID"] = reader["UserID"];
                                            Session["FirstName"] = reader["FirstName"].ToString();
                                            Session["LastName"] = reader["LastName"].ToString();
                                            Session["IsAdmin"] = (bool?)reader["IsAdmin"];
                                            return RedirectToAction("Index", "Home");
                                        }
                                        else
                                        {
                                            ViewBag.Message = "You account is not yet active. Please check back later.";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.Message = "The email address or password you entered is incorrect.";
                                    }
                                }                              
                                else
                                {
                                    ViewBag.Message = "Account not verified. Please check your email for the link to verify your account.";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Email Address Not Found.";
                        }
                    }
                }
            }
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult VerifyAccount(string ActivationCode)
        {
            ViewBag.Status = false;
            string UserID = functions.CheckUser("ActivationCode", ActivationCode);
            int _queryResult = 0;

            if (UserID != null)
            {
                ViewBag.Status = true;
                ActivationCode = Guid.NewGuid().ToString();
                using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                {
                    using (SqlCommand sqlcmd = new SqlCommand("sp_ActivateUserAccount", sqlCon))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@ActivationCode", ActivationCode);
                        sqlcmd.Parameters.AddWithValue("@UserID", UserID);
                        sqlCon.Open();
                        _queryResult = sqlcmd.ExecuteNonQuery();
                    }
                }

                if(_queryResult > 0) ViewBag.Message = "Your account has been verified successfully. Please check your email for further update of your account.";

                User user = functions.GetUserDetails(Convert.ToInt32(UserID));
                string body = "Dear " + user.FirstName + " " + user.LastName + ",<br/><br/>"
                                        + "Your account has been successfully verified. <br/><br/>"
                                        + "We will be reviewing your details in timely manner. Once reviewed, you will receive an email from us with the status of your account.<br/><br/>";
                string subject = "Account Verified";
                if (functions.SendEmail(user.EmailID, body, subject) != null)
                {
                    ViewBag.Message = "Something went wrong!!! Error sending Email.";
                }
            }
            else
            {
                ViewBag.Message = "Invalid Request";
            }
            return View();
        }

        //Forgot password
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            if (EmailID != "")
            {
                if (functions.CheckUser("EmailID", EmailID) != null)
                {
                    string subject = "Password Reset";
                    string pwresetcode = Guid.NewGuid().ToString();
                    string pwresetlink = "<a href='" + Url.Action("ResetPassword", "Account", new { pwresetid = pwresetcode }, "http") + "'>Here</a>";
                    string body = "Greetings, <br/><br/>"
                                    + "In order to reset your password for you UGKPS account, please click " + pwresetlink + ".<br/><br/>"; 
                    functions.SendEmail(EmailID, body, subject);
                    using (SqlConnection conn = new SqlConnection(_mainconnString))
                    {
                        //string updatequery = "UPDATE [dbo].[Users] SET PWResetCode = @pwresetcode where EmailID = @EmailID";
                        using (SqlCommand sqlcmd = new SqlCommand("sp_InsertPWResetCode", conn))
                        {
                            sqlcmd.CommandType = CommandType.StoredProcedure;
                            sqlcmd.Parameters.AddWithValue("@pwresetcode", pwresetcode);
                            sqlcmd.Parameters.AddWithValue("@EmailID", EmailID);
                            conn.Open();
                            sqlcmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Email ID not found!";
                    return View();
                }
                TempData["alertMsg"] = "Please check you email for instructions to Reset Password";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Message = "Email ID is required!";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ResetPassword(string pwresetid)
        {
            if (functions.CheckUser("PWResetCode", pwresetid) != null)
            {
                ResetPassword model = new ResetPassword();
                model.PWResetCode = pwresetid;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword model)
        {
            if (ModelState.IsValid)
            {
                using(SqlConnection conn = new SqlConnection(_mainconnString))
                {
                    //string updatequery = "UPDATE [dbo].[Users] SET Password = @password, PWResetCode = @RandomCode where PWResetCode = @pwresetcode";
                    using(SqlCommand sqlcmd = new SqlCommand("sp_UpdatePassword", conn))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@password", EncryptPW.Hash(model.NewPassword));
                        sqlcmd.Parameters.AddWithValue("@pwresetcode", model.PWResetCode);
                        sqlcmd.Parameters.AddWithValue("@RandomCode", Guid.NewGuid().ToString());
                        conn.Open();
                        int _queryresult = sqlcmd.ExecuteNonQuery();
                        if (_queryresult != 0)
                        {
                            ModelState.Clear();
                            ViewBag.Message = "Password Reset successful. Please login using new credentials.";
                        }
                    }
                }
            }
            return View(model);
        }

        [CustomAuthorize]
        public ActionResult SaveFamilyMemberRecordInDB(FamilyMember familyMember)
        {
            List<string> listOfRelations = new List<string> { "Father", "Mother", "Wife", "Husband", "Son", "Daughter", "Neice", "Nephew", "Son-In-Law", "Daughter-In-Law", "Grandson", "Granddaughter", "Uncle", "Aunt" };
            ViewBag.ListOfRelations = new SelectList(listOfRelations, "Relation");
            if (ModelState.IsValid)
            {
                if(familyMember.FamilyMemberID > 0)
                {
                    int _queryResult = functions.UpdateFamilyMemberDetails(familyMember);
                    if(_queryResult > 0)
                    {
                        ModelState.Clear();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                else{
                    int _queryResult = functions.CheckExistingAndInsertFamilyMemberDetails(familyMember, Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
                    if (_queryResult > 0)
                    {
                        ModelState.Clear();
                        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ModelState.AddModelError("MemberExists", "Member already exists!");
                    }
                }
                
            }
            return PartialView("_AddEditFamilyMembers",familyMember);

        }

        public JsonResult GetFamilyMemberList()
        {
            User user = functions.GetUserDetails(Convert.ToInt32(System.Web.HttpContext.Current.Session["UserID"]));
            List<FamilyMember> FamilyMemberList = functions.GetUserFamilyMembers(user);
            return Json(FamilyMemberList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFamilyMemberByID(int FamilyMemberID)
        {
            FamilyMember member = functions.GetFamilyMember(FamilyMemberID);
            string value = string.Empty;
            value = JsonConvert.SerializeObject(member,Formatting.Indented);
            return Json(value, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFamilyMemberRecord(int FamilyMemberID)
        {
            int _queryResult = functions.RemoveFamilyMember(FamilyMemberID);
            if(_queryResult > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

    }
}
