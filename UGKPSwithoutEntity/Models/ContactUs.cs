using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UGKPSwithoutEntity.Models
{
    public class ContactUs
    {
        [Key]
        public int InquiryID { get; set;}

        public bool IsMember { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Display(Name = "Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address is required.")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "Contact Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:###-###-####}")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
        public string ContactNumber { get; set; }

        [Display(Name = "Message")]
        [Required(AllowEmptyStrings = false, ErrorMessage ="Message is required.")]
        public string Message { get; set; }

        public bool DidReply { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime ModifiedDate { get; set; }


        public string ReplyMessage { get; set; }
        
    }
}