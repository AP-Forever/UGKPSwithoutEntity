using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UGKPSwithoutEntity.Models
{
    public class InvestmentAccess
    {
        [Key]
        public int InvestTab_User_Spec_ID { get; set; }

        public int UserID { get; set; }
        public bool IsInvestTabVisible { get; set; }
        public bool HasAcceptedDisclaimer { get; set; }
        public DateTime? DateAccepted { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime ModifiedDate { get; set; }
    }
}