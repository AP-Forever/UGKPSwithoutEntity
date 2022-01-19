using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UGKPSwithoutEntity.Models
{
    public class UserProfile
    {
        public User User { get; set; }

        public FamilyMember FamilyMember { get; set; }

    }
}