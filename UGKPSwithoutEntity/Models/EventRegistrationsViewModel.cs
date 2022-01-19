using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UGKPSwithoutEntity.Models;

namespace UGKPSwithoutEntity.Models
{
    public class EventRegistrationsViewModel
    {
        public List<WebsiteUser> ListOfUsers { get; set; }
        public Event currEvent { get; set; }

    }

    public class UserFamilyMember : FamilyMember
    {
        public DateTime DateRegistered { get; set; }
    }

    public class WebsiteUser : Registration
    {
        public List<UserFamilyMember> FamilyMembers { get; set; }

        public int NoOfAdults { get; set; }
        public int NoOfKids { get; set; }
        public int NoOfChildWithTicket { get; set; }
        public int TotalTicketAmount { get; set; }
        public int TotalDue { get; set; }
        public bool HasAlreadyAttended { get; set; }

    }

    
}