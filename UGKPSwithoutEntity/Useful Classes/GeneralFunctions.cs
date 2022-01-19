using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;

namespace UGKPSwithoutEntity.Models
{
    public class GeneralFunctions
    {
        readonly string _mainconnString = ConfigurationManager.ConnectionStrings["ugkpsConnectionString"].ConnectionString; //Connection string from web.config

        [NonAction]
        public string CheckUser(string field, string value)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                //string selectquery = "Select * from [dbo].[Users] where " + field + " = @value";
                using (SqlCommand sqlcmd = new SqlCommand("sp_CheckUser", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@column", field);
                    sqlcmd.Parameters.AddWithValue("@value", value);
                    sqlCon.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader["UserID"].ToString();
                        }
                    }
                    return null;
                }
            }
        }

        [NonAction]
        public string SendEmail(string UserEmail, string body, string subject)
        {
            using (MailMessage mail = new MailMessage())
            {
                string password = "Kadvapatidar2020!";
                mail.From = new MailAddress("ugkps.toronto@gmail.com");
                mail.To.Add(UserEmail);
                mail.Subject = subject;
                mail.Body = body + "Regards,<br/>" + 
                    "UGKPS Toronto<br/>" + 
                    "<a href='http://www.ugkps.com' target='_blank'>www.ugkps.com</a>";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("ugkps.toronto@gmail.com", password);
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        return ex.Message.ToString();
                    }
                }
            }
            return null;
        }

        [NonAction]
        public int InsertInquiryDetails(ContactUs contactusdetails, bool IsMember)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                using (SqlCommand sqlcmd = new SqlCommand("sp_InsertInquiryDetails", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@IsMember", IsMember);
                    sqlcmd.Parameters.AddWithValue("@FirstName", contactusdetails.FirstName);
                    sqlcmd.Parameters.AddWithValue("@LastName", contactusdetails.LastName);
                    sqlcmd.Parameters.AddWithValue("@ContactNumber", contactusdetails.ContactNumber);
                    sqlcmd.Parameters.AddWithValue("@EmailID", contactusdetails.EmailID);
                    sqlcmd.Parameters.AddWithValue("@Message", contactusdetails.Message);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public int InsertEventRegistrationDetails(FamilyMember member, int EventID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                using (SqlCommand sqlcmd = new SqlCommand("sp_InsertEventRegistrationDetails", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["UserID"]));
                    sqlcmd.Parameters.AddWithValue("@EventID", EventID);
                    sqlcmd.Parameters.AddWithValue("@FamilyMemberID", member.FamilyMemberID);
                    sqlcmd.Parameters.AddWithValue("@Age", member.Age);
                    sqlcmd.Parameters.AddWithValue("@DateRegistered", DateTime.Now);

                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
                
            }
        }

        [NonAction]
        public Event GetAndFillCurrentEvent()
        {
            Event currEvent = new Event();
            DataTable dtblEvent = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daEvent = new SqlDataAdapter("SELECT * FROM [dbo].[Events] Where IsActive = 'true'", _mainconnString);
                daEvent.Fill(dtblEvent);
            }
            for (int i = 0; i < dtblEvent.Rows.Count; i++)
            {

                for (int j = 0; j < dtblEvent.Columns.Count; j++)
                {
                    string property = dtblEvent.Columns[j].ColumnName.ToString();
                    if (property != "IsActive" && property != "IsCanceled")
                    {
                        currEvent.GetType().GetProperty(property).SetValue(currEvent, dtblEvent.Rows[i][j]);
                    }
                }
            }
            return currEvent;
        }



        [NonAction]
        public List<Event> GetEvents()
        {            
            DataTable dtblEvent = new DataTable();
            List<Event> ListOfEvents = new List<Event>();

            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daEvent = new SqlDataAdapter("SELECT * FROM [dbo].[Events]", _mainconnString);
                daEvent.Fill(dtblEvent);
            }
            for (int i = 0; i < dtblEvent.Rows.Count; i++)
            {
                Event currEvent = new Event();
                for (int j = 0; j < dtblEvent.Columns.Count; j++)
                {
                    string property = dtblEvent.Columns[j].ColumnName.ToString();
                    if (property != "IsActive" && property != "IsCanceled")
                    {
                        currEvent.GetType().GetProperty(property).SetValue(currEvent, dtblEvent.Rows[i][j]);
                    }
                }
                ListOfEvents.Add(currEvent);
            }

            return ListOfEvents;
        }



        [NonAction]
        public List<FamilyMember> GetUserEventRegistrationInfo(List<FamilyMember> familyMembers)
        {
            foreach (FamilyMember member in familyMembers)
            {
                using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                {
                    //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                    //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                    using (SqlCommand sqlcmd = new SqlCommand("sp_GetAttendingUserFamilyMember", sqlCon))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@FamilymemberID", member.FamilyMemberID);
                        sqlCon.Open();
                        SqlDataReader reader = sqlcmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            member.IsChecked = true;
                        }
                        else
                        {
                            member.IsChecked = false;
                        }
                        
                    }
                }
            }
            return familyMembers;
        }


        [NonAction]
        public int CheckExistingAndInsertFamilyMemberDetails(FamilyMember familyMember, int UserID)
        {
            bool IsFamilyMemberExists = false;
            
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                using (SqlCommand sqlcmd = new SqlCommand("sp_CheckFamilyMemberExists", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", UserID);
                    sqlcmd.Parameters.AddWithValue("@FirstName", familyMember.FirstName);
                    sqlcmd.Parameters.AddWithValue("@LastName", familyMember.LastName);
                    sqlcmd.Parameters.AddWithValue("@Age", familyMember.Age);
                    sqlcmd.Parameters.AddWithValue("@Relation", familyMember.Relation);
                    sqlCon.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();
                    if (reader.HasRows) IsFamilyMemberExists = true;
                }
            }
            if (!IsFamilyMemberExists)
            {
                using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                {
                    //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                    //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                    using (SqlCommand sqlcmd = new SqlCommand("sp_InsertFamilyMemberDetails", sqlCon))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@UserID", UserID);
                        sqlcmd.Parameters.AddWithValue("@FirstName", familyMember.FirstName);
                        sqlcmd.Parameters.AddWithValue("@LastName", familyMember.LastName);
                        sqlcmd.Parameters.AddWithValue("@Age", familyMember.Age);
                        sqlcmd.Parameters.AddWithValue("@Relation", familyMember.Relation);
                        sqlCon.Open();
                        return sqlcmd.ExecuteNonQuery();
                    }
                }
            }
            return 0;
            
        }

        [NonAction]
        public List<FamilyMember> GetUserFamilyMembers(User user)
        {
            
            user.UserFamily = new List<FamilyMember>();
            DataTable dtblUserFamilyMembers = new DataTable();
            if (HttpContext.Current.Session["UserID"] != null)
            {
                using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                {
                    using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
                    {
                        sqlcon.Open();
                        SqlDataAdapter daUserFamilyMember = new SqlDataAdapter("SELECT * FROM [dbo].[FamilyMembers] where (UserId = " + user.UserID + ")", _mainconnString);
                        daUserFamilyMember.Fill(dtblUserFamilyMembers);
                    }
                    for (int i = 0; i < dtblUserFamilyMembers.Rows.Count; i++)
                    {
                        FamilyMember familyMember = new FamilyMember();
                        for (int j = 0; j < dtblUserFamilyMembers.Columns.Count; j++)
                        {
                            string property = dtblUserFamilyMembers.Columns[j].ColumnName.ToString();
                            if (property != "UserID" && (property != "DateOfBirth"))
                            {
                                familyMember.GetType().GetProperty(property).SetValue(familyMember, dtblUserFamilyMembers.Rows[i][j]);
                            }

                        }
                        user.UserFamily.Add(familyMember);
                    }
                }
            }
            return user.UserFamily;
        }

        [NonAction]
        public User GetUserDetails(int UserID)
        {
            DataTable dtblUsers = new DataTable();
            User user = new User();
            if(UserID != 0)
            {
                using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
                {
                    sqlcon.Open();
                    SqlDataAdapter daUsers = new SqlDataAdapter("SELECT * FROM [dbo].[Users] where (UserId = " + UserID + ")", _mainconnString);
                    daUsers.Fill(dtblUsers);
                }

                for (int i = 0; i < dtblUsers.Rows.Count; i++)
                {

                    for (int j = 0; j < dtblUsers.Columns.Count; j++)
                    {
                        string property = dtblUsers.Columns[j].ColumnName.ToString();

                        if ((property != "PWResetCode") && (property != "ActivationCode") && (property != "DateOfBirth"))
                        {
                            user.GetType().GetProperty(property).SetValue(user, dtblUsers.Rows[i][j]);
                        }
                    }
                }
            }
            
            return user;
        }

        //[NonAction]
        //public User UpdateAndGetUserDetailsFromActivationCode(string ActivationCode)
        //{
        //    DataTable dtblUsers = new DataTable();
        //    User user = new User();
        //    int _queryResult = 0;

        //    if (_queryResult > 0)
        //    {
        //        using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
        //        {
        //            sqlcon.Open();
        //            SqlDataAdapter daUsers = new SqlDataAdapter("SELECT * FROM [dbo].[Users] where (ActivationCode = " + ActivationCode + ")", _mainconnString);
        //            daUsers.Fill(dtblUsers);
        //        }

        //        for (int i = 0; i < dtblUsers.Rows.Count; i++)
        //        {

        //            for (int j = 0; j < dtblUsers.Columns.Count; j++)
        //            {
        //                string property = dtblUsers.Columns[j].ColumnName.ToString();

        //                if ((property != "PWResetCode") && (property != "ActivationCode") && (property != "DateOfBirth"))
        //                {
        //                    user.GetType().GetProperty(property).SetValue(user, dtblUsers.Rows[i][j]);
        //                }
        //            }
        //        }
        //    }

        //    return user;
        //}

        [NonAction]
        public int UpdateFamilyMemberDetails(FamilyMember familyMember)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                using (SqlCommand sqlcmd = new SqlCommand("sp_UpdateFamilyMemberDetails", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@FamilyMemberID", familyMember.FamilyMemberID);
                    sqlcmd.Parameters.AddWithValue("@FirstName", familyMember.FirstName);
                    sqlcmd.Parameters.AddWithValue("@LastName", familyMember.LastName);
                    sqlcmd.Parameters.AddWithValue("@Relation", familyMember.Relation);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public FamilyMember GetFamilyMember(int id)
        {
            DataTable dtblFamilyMember = new DataTable();
            FamilyMember  member = new FamilyMember();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daUsers = new SqlDataAdapter("SELECT * FROM [dbo].[FamilyMembers] where (FamilyMemberID = " + id + ")", _mainconnString);
                daUsers.Fill(dtblFamilyMember);
            }

            for (int i = 0; i < dtblFamilyMember.Rows.Count; i++)
            {

                for (int j = 0; j < dtblFamilyMember.Columns.Count; j++)
                {
                    string property = dtblFamilyMember.Columns[j].ColumnName.ToString();

                    if (property != "UserID" && (property != "DateOfBirth")) 
                    {
                        member.GetType().GetProperty(property).SetValue(member, dtblFamilyMember.Rows[i][j]);
                    }
                }
            }

            return member;
        }

        [NonAction]
        public int RemoveFamilyMember(int FamilyMemberID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {

                using (SqlCommand sqlcmd = new SqlCommand("sp_DeleteFamilyMemberRecord", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@FamilyMemberID", FamilyMemberID);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public int RegisterUser(Registration UserRegisterInfo, string Country, string ActivationCode)
        {
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {

                using (SqlCommand sqlcmd = new SqlCommand("sp_RegisterUser", sqlcon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@FirstName", UserRegisterInfo.FirstName);
                    sqlcmd.Parameters.AddWithValue("@LastName", UserRegisterInfo.LastName);
                    sqlcmd.Parameters.AddWithValue("@EmailID", UserRegisterInfo.EmailID);
                    sqlcmd.Parameters.AddWithValue("@Password", UserRegisterInfo.Password);
                    sqlcmd.Parameters.AddWithValue("@Age", UserRegisterInfo.Age);
                    sqlcmd.Parameters.AddWithValue("@Address", UserRegisterInfo.Address);
                    sqlcmd.Parameters.AddWithValue("@City", UserRegisterInfo.City);
                    sqlcmd.Parameters.AddWithValue("@State", UserRegisterInfo.State);
                    sqlcmd.Parameters.AddWithValue("@Country", Country);
                    sqlcmd.Parameters.AddWithValue("@ZipCode", UserRegisterInfo.ZipCode);
                    sqlcmd.Parameters.AddWithValue("@PhoneNumber", UserRegisterInfo.PhoneNumber);
                    sqlcmd.Parameters.AddWithValue("@Native", UserRegisterInfo.Native);
                    sqlcmd.Parameters.AddWithValue("@IsEmailVerified", false);
                    sqlcmd.Parameters.AddWithValue("@ActivationCode", ActivationCode);
                    sqlcmd.Parameters.AddWithValue("@IsNewRequest", true);

                    sqlcon.Open();
                    try
                    {
                        return sqlcmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }

                }
            }
        }

        [NonAction]
        public int RemoveFamilyMemberFromEventRegistration(int FamilyMemberID, int EventID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {

                using (SqlCommand sqlcmd = new SqlCommand("sp_RemoveFamilyMemberFromEventRegistration", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@FamilyMemberID", FamilyMemberID);
                    sqlcmd.Parameters.AddWithValue("@EventID", EventID);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public int ActivateEvent(int EventID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("sp_ActivateEvent", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@EventID", EventID);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public EventRegistrationsViewModel GetEventRegistrationsDetails()
        {
            DataTable dtblEventRegistrationsDetails = new DataTable();
            EventRegistrationsViewModel evntRegViewModel = new EventRegistrationsViewModel();
            evntRegViewModel.currEvent = new Event();
            evntRegViewModel.ListOfUsers = new List<WebsiteUser>();

            bool IsFirstRecordForThisUser = true;
            bool IsEventDetailsFilled = false;
            int currUserIndex = 0;

            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daEventRegistrations = new SqlDataAdapter("sp_GetEventRegistrationsDetails", _mainconnString);
                daEventRegistrations.SelectCommand.CommandType = CommandType.StoredProcedure;
                daEventRegistrations.Fill(dtblEventRegistrationsDetails);
            }

            for(int i = 0; i < dtblEventRegistrationsDetails.Rows.Count; i++)
            {
                if(i > 0 && (dtblEventRegistrationsDetails.Rows[i]["UserID"].ToString() != dtblEventRegistrationsDetails.Rows[i - 1]["UserID"].ToString()))
                {
                    IsFirstRecordForThisUser = true;
                    currUserIndex++;
                }
                if (!IsEventDetailsFilled)
                {
                    evntRegViewModel.currEvent.EventID = Convert.ToInt32(dtblEventRegistrationsDetails.Rows[i]["EventID"]);
                    evntRegViewModel.currEvent.Event_Name = dtblEventRegistrationsDetails.Rows[i]["Event_Name"].ToString();
                    evntRegViewModel.currEvent.Price_Adult = Convert.ToInt32(dtblEventRegistrationsDetails.Rows[i]["Price_Adult"]);
                    evntRegViewModel.currEvent.Price_Child = Convert.ToInt32(dtblEventRegistrationsDetails.Rows[i]["Price_Child"]);
                    evntRegViewModel.currEvent.Fees_Membership = Convert.ToInt32(dtblEventRegistrationsDetails.Rows[i]["Fees_Membership"]);
                    IsEventDetailsFilled = true;
                }
                if (IsFirstRecordForThisUser)
                {
                    WebsiteUser user = new WebsiteUser();
                    user.UserID = Convert.ToInt32(dtblEventRegistrationsDetails.Rows[i]["UserID"]);
                    user.FirstName = dtblEventRegistrationsDetails.Rows[i]["Mem_FirstName"].ToString();
                    user.LastName = dtblEventRegistrationsDetails.Rows[i]["Mem_LastName"].ToString();
                    user.EmailID = dtblEventRegistrationsDetails.Rows[i]["EmailID"].ToString();
                    user.Address = dtblEventRegistrationsDetails.Rows[i]["Address"].ToString();
                    user.City = dtblEventRegistrationsDetails.Rows[i]["City"].ToString();
                    user.State = dtblEventRegistrationsDetails.Rows[i]["State"].ToString();
                    user.Country = dtblEventRegistrationsDetails.Rows[i]["Country"].ToString();
                    user.ZipCode = dtblEventRegistrationsDetails.Rows[i]["ZipCode"].ToString();
                    user.PhoneNumber = dtblEventRegistrationsDetails.Rows[i]["PhoneNumber"].ToString();
                    user.Native = dtblEventRegistrationsDetails.Rows[i]["Native"].ToString();
                    evntRegViewModel.ListOfUsers.Add(user);
                    evntRegViewModel.ListOfUsers[currUserIndex].FamilyMembers = new List<UserFamilyMember>();
                    IsFirstRecordForThisUser = false;
                }
                UserFamilyMember userFamilyMember = new UserFamilyMember();
                userFamilyMember.FamilyMemberID = Convert.ToInt32(dtblEventRegistrationsDetails.Rows[i]["FamilyMemberID"]);
                userFamilyMember.FirstName = dtblEventRegistrationsDetails.Rows[i]["FirstName"].ToString();
                userFamilyMember.LastName = dtblEventRegistrationsDetails.Rows[i]["LastName"].ToString();
                userFamilyMember.Age = (int) dtblEventRegistrationsDetails.Rows[i]["Age"];
                userFamilyMember.Relation = dtblEventRegistrationsDetails.Rows[i]["Relation"].ToString();
                userFamilyMember.DateRegistered = (DateTime) dtblEventRegistrationsDetails.Rows[i]["DateRegistered"];
                evntRegViewModel.ListOfUsers[currUserIndex].FamilyMembers.Add(userFamilyMember);
            }

            return evntRegViewModel;
        }

        [NonAction]
        public EventRegistrationsViewModel CalculateTotalFeesPerUser(EventRegistrationsViewModel model)
        {
            foreach (WebsiteUser user in model.ListOfUsers)
            {
                user.NoOfAdults = 0;
                user.NoOfKids = 0;
                user.NoOfChildWithTicket = 0;
                user.TotalTicketAmount = 0;
                user.TotalDue = 0;

                foreach (UserFamilyMember familyMember in user.FamilyMembers)
                {
                    if (familyMember.Age <= 5)
                    {
                        user.NoOfKids++;
                    }
                    else if (familyMember.Age <= 10)
                    {
                        user.NoOfKids++;
                        user.NoOfChildWithTicket++;
                    }
                    else
                    {
                        user.NoOfAdults++;
                    }
                }
                user.TotalTicketAmount = (user.NoOfAdults * model.currEvent.Price_Adult) + (user.NoOfChildWithTicket * model.currEvent.Price_Child);
                user.TotalDue = user.TotalTicketAmount + model.currEvent.Fees_Membership;
            }
            return model;
        }

        [NonAction]
        public int MarkUserAsAttendedEvent(int UserID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("sp_MarkUserAsAttendedEvent", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", UserID);
                    sqlcmd.Parameters.AddWithValue("@DateAttended", DateTime.Now);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public EventRegistrationsViewModel CheckUser_AlreadyAttendedEvent(EventRegistrationsViewModel model)
        {
            foreach(WebsiteUser user in model.ListOfUsers)
            {            
                using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                {
                    using (SqlCommand sqlcmd = new SqlCommand("sp_HasUserAlreadyAttendedEvent", sqlCon))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@UserID", user.UserID);
                        sqlcmd.Parameters.AddWithValue("@EventID", model.currEvent.EventID);
                        sqlCon.Open();
                        SqlDataReader reader = sqlcmd.ExecuteReader();
                        if (reader.HasRows) user.HasAlreadyAttended = true;
                    }
                }
            }
            return model;
        }

        [NonAction]
        public int InsertStock(Stock stockModel, User user)
        {
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("sp_InsertStock", sqlcon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", user.UserID);
                    sqlcmd.Parameters.AddWithValue("@UserName", user.FirstName + " " + user.LastName);
                    sqlcmd.Parameters.AddWithValue("@Ticker", stockModel.Ticker);
                    sqlcmd.Parameters.AddWithValue("@Exchange", stockModel.Exchange);
                    sqlcmd.Parameters.AddWithValue("@CompanyName", stockModel.CompanyName);
                    sqlcmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                    sqlcmd.Parameters.AddWithValue("@Price", stockModel.Price);
                    sqlcmd.Parameters.AddWithValue("@Price_High", stockModel.Price_High);
                    sqlcmd.Parameters.AddWithValue("@Price_Low", stockModel.Price_Low);
                    sqlcmd.Parameters.AddWithValue("@Day_Price_High", stockModel.Day_Price_High);
                    sqlcmd.Parameters.AddWithValue("@Day_Price_Low", stockModel.Day_Price_Low);
                    sqlcmd.Parameters.AddWithValue("@P_E_Ratio", stockModel.P_E_Ratio);
                    sqlcmd.Parameters.AddWithValue("@EPS", stockModel.EPS);
                    sqlcmd.Parameters.AddWithValue("@DividendPerShare", stockModel.DividendPerShare);
                    sqlcmd.Parameters.AddWithValue("@DividendPercent", stockModel.DividendPercent);
                    sqlcmd.Parameters.AddWithValue("@DividendYieldPercent", stockModel.DividendYieldPercent);
                    sqlcmd.Parameters.AddWithValue("@Comment", stockModel.Comment);
                    sqlcmd.Parameters.AddWithValue("@Sector", stockModel.Sector);

                    sqlcon.Open();
                    try
                    {
                        return sqlcmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }

                }
            }
        }

        [NonAction]
        public Stock GetStockDetails(int id)
        {
            DataTable dtblStock = new DataTable();
            Stock stock = new Stock();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daStock = new SqlDataAdapter("SELECT * FROM [dbo].[Stocks_Investment] where (SI_ID = " + id + ")", _mainconnString);
                daStock.Fill(dtblStock);
            }

            for (int i = 0; i < dtblStock.Rows.Count; i++)
            {

                for (int j = 0; j < dtblStock.Columns.Count; j++)
                {
                    string property = dtblStock.Columns[j].ColumnName.ToString();
                    stock.GetType().GetProperty(property).SetValue(stock, dtblStock.Rows[i][j]);
                    
                }
            }

            return stock;
        }

        [NonAction]
        public Stock GetStockDetailsByTicker(string ticker, string exchange)
        {
            DataTable dtblStock = new DataTable();
            Stock stock = new Stock();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daStock = new SqlDataAdapter("SELECT * FROM [dbo].[Stocks_Investment] where (Ticker = '"  + ticker + "') AND (Exchange = '" + exchange + "')", _mainconnString);
                daStock.Fill(dtblStock);
            }

            for (int i = 0; i < dtblStock.Rows.Count; i++)
            {

                for (int j = 0; j < dtblStock.Columns.Count; j++)
                {
                    string property = dtblStock.Columns[j].ColumnName.ToString();
                    stock.GetType().GetProperty(property).SetValue(stock, dtblStock.Rows[i][j]);

                }
            }

            return stock;
        }

        [NonAction]
        public int UpdateStockDetails(Stock stock)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                using (SqlCommand sqlcmd = new SqlCommand("sp_UpdateStockDetails", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@SI_ID", stock.SI_ID);
                    sqlcmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                    sqlcmd.Parameters.AddWithValue("@Price", stock.Price);
                    sqlcmd.Parameters.AddWithValue("@Price_High", stock.Price_High);
                    sqlcmd.Parameters.AddWithValue("@Price_Low", stock.Price_Low);
                    sqlcmd.Parameters.AddWithValue("@Day_Price_High", stock.Day_Price_High);
                    sqlcmd.Parameters.AddWithValue("@Day_Price_Low", stock.Day_Price_Low);
                    sqlcmd.Parameters.AddWithValue("@P_E_Ratio", stock.P_E_Ratio);
                    sqlcmd.Parameters.AddWithValue("@EPS", stock.EPS);
                    sqlcmd.Parameters.AddWithValue("@DividendPerShare", stock.DividendPerShare);
                    sqlcmd.Parameters.AddWithValue("@DividendPercent", stock.DividendPercent);
                    sqlcmd.Parameters.AddWithValue("@DividendYieldPercent", stock.DividendYieldPercent);
                    sqlcmd.Parameters.AddWithValue("@Comment", stock.Comment ?? "");
                    sqlcmd.Parameters.AddWithValue("@Sector", stock.Sector ?? "");
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public int RemoveStock(int StockID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {

                using (SqlCommand sqlcmd = new SqlCommand("sp_DeleteStockRecord", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@SI_ID", StockID);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public int AddEvent(Event myevent)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {

                using (SqlCommand sqlcmd = new SqlCommand("sp_AddEvent", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@Event_Name", myevent.Event_Name);
                    sqlcmd.Parameters.AddWithValue("@Event_Location", myevent.Event_Location);
                    sqlcmd.Parameters.AddWithValue("@Event_Description", myevent.Event_Description);
                    sqlcmd.Parameters.AddWithValue("@Event_StartDate", myevent.Event_StartDate);
                    sqlcmd.Parameters.AddWithValue("@Event_EndDate", myevent.Event_EndDate);
                    sqlcmd.Parameters.AddWithValue("@Date_Created", DateTime.Now);
                    sqlcmd.Parameters.AddWithValue("@Date_Updated", DateTime.Now);
                    sqlcmd.Parameters.AddWithValue("@Price_Adult", myevent.Price_Adult);
                    sqlcmd.Parameters.AddWithValue("@Price_Child", myevent.Price_Child);
                    sqlcmd.Parameters.AddWithValue("@Fees_Membership", myevent.Fees_Membership);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public int CheckExistingAndInsertInvestmentAccessRecord(InvestmentAccess userInvestAccess)
        {
            bool IsUserInvestRecordExist = false;

            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("sp_CheckUserInvestAccessRecord", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", userInvestAccess.UserID);
                    sqlCon.Open();
                    SqlDataReader reader = sqlcmd.ExecuteReader();
                    if (reader.HasRows) IsUserInvestRecordExist = true;
                }
            }
            if (!IsUserInvestRecordExist)
            {
                using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
                {
                    using (SqlCommand sqlcmd = new SqlCommand("sp_InsertUserInvestAccessRecord", sqlCon))
                    {
                        sqlcmd.CommandType = CommandType.StoredProcedure;
                        sqlcmd.Parameters.AddWithValue("@UserID", userInvestAccess.UserID);
                        sqlcmd.Parameters.AddWithValue("@IsInvestTabVisible", userInvestAccess.IsInvestTabVisible);
                        sqlcmd.Parameters.AddWithValue("@HasAcceptedDisclaimer", userInvestAccess.HasAcceptedDisclaimer);
                        sqlCon.Open();
                        return sqlcmd.ExecuteNonQuery();
                    }
                }
            }
            return 0;

        }

        [NonAction]
        public InvestmentAccess GetUsersInvestmentAccessDetails(int UserID)
        {
            DataTable dtblUsersInvestAccess = new DataTable();
            InvestmentAccess userInvestAccess = new InvestmentAccess();
            if (UserID != 0)
            {
                using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
                {
                    sqlcon.Open();
                    SqlDataAdapter daUsersInvestAccess = new SqlDataAdapter("SELECT * FROM [dbo].[InvestTab_User_Specification] where (UserId = " + UserID + ")", _mainconnString);
                    daUsersInvestAccess.Fill(dtblUsersInvestAccess);
                }

                for (int i = 0; i < dtblUsersInvestAccess.Rows.Count; i++)
                {

                    for (int j = 0; j < dtblUsersInvestAccess.Columns.Count; j++)
                    {
                        string property = dtblUsersInvestAccess.Columns[j].ColumnName.ToString();
                        if (property == "DateAccepted")
                        {

                            if(userInvestAccess.HasAcceptedDisclaimer == true) userInvestAccess.GetType().GetProperty(property).SetValue(userInvestAccess, dtblUsersInvestAccess.Rows[i][j]);
                        }
                        else
                        {
                            userInvestAccess.GetType().GetProperty(property).SetValue(userInvestAccess, dtblUsersInvestAccess.Rows[i][j]);
                        }
                        
                    }
                }
            }

            return userInvestAccess;
        }

        [NonAction]
        public int UpdateInvestmentAccessRecord(InvestmentAccess userInvestAccess)
        {
            
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                //string insertquery =    "Insert Into [dbo].[Inquiries] (IsMember, FirstName, LastName, ContactNumber, EmailID, Message) "+
                //                        "VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)";
                using (SqlCommand sqlcmd = new SqlCommand("sp_UpdateUserInvestmentAccessRecord", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@InvestTab_User_Spec_ID", userInvestAccess.InvestTab_User_Spec_ID);
                    sqlcmd.Parameters.AddWithValue("@UserID", userInvestAccess.UserID);
                    sqlcmd.Parameters.AddWithValue("@IsInvestTabVisible", userInvestAccess.IsInvestTabVisible);
                    sqlcmd.Parameters.AddWithValue("@HasAcceptedDisclaimer", userInvestAccess.HasAcceptedDisclaimer);
                    if(userInvestAccess.DateAccepted == null)
                    {
                        sqlcmd.Parameters.AddWithValue("@DateAccepted", "");
                    }
                    else
                    {
                        sqlcmd.Parameters.AddWithValue("@DateAccepted", userInvestAccess.DateAccepted);
                    }
                    
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }


        [NonAction]
        public int UpdateInquiry(int inquiryID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {
                
                using (SqlCommand sqlcmd = new SqlCommand("sp_UpdateInquiryRecord", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@InquiryID", inquiryID);

                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public int InsertTransaction(Transaction transModel)
        {
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                using (SqlCommand sqlcmd = new SqlCommand("sp_InsertStockTransaction", sqlcon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@UserID", Convert.ToInt32(HttpContext.Current.Session["UserID"]));
                    sqlcmd.Parameters.AddWithValue("@Ticker", transModel.Ticker);
                    sqlcmd.Parameters.AddWithValue("@Exchange", transModel.Exchange);
                    sqlcmd.Parameters.AddWithValue("@CompanyName", transModel.CompanyName);
                    sqlcmd.Parameters.AddWithValue("@Price", transModel.Price);
                    sqlcmd.Parameters.AddWithValue("@Quantity", transModel.Quantity);

                    sqlcon.Open();
                    try
                    {
                        return sqlcmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }

                }
            }
        }

        [NonAction]
        public int RemoveStockTransactionRecord(int Stock_Transaction_ID)
        {
            using (SqlConnection sqlCon = new SqlConnection(_mainconnString))
            {

                using (SqlCommand sqlcmd = new SqlCommand("sp_DeleteStockTransactionRecord", sqlCon))
                {
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.AddWithValue("@Stock_Transaction_ID", Stock_Transaction_ID);
                    sqlCon.Open();
                    return sqlcmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public Transaction GetStockTransactionByID(int id)
        {
            DataTable dtblStockTransaction = new DataTable();
            Transaction trans = new Transaction();
            using (SqlConnection sqlcon = new SqlConnection(_mainconnString))
            {
                sqlcon.Open();
                SqlDataAdapter daTransaction = new SqlDataAdapter("SELECT * FROM [dbo].[Stocks_Transactions] where (Stock_Transaction_ID = " + id + ")", _mainconnString);
                daTransaction.Fill(dtblStockTransaction);
            }

            for (int i = 0; i < dtblStockTransaction.Rows.Count; i++)
            {

                for (int j = 0; j < dtblStockTransaction.Columns.Count; j++)
                {
                    string property = dtblStockTransaction.Columns[j].ColumnName.ToString();
                    trans.GetType().GetProperty(property).SetValue(trans, dtblStockTransaction.Rows[i][j]);
                }
            }

            return trans;
        }

    } 
}