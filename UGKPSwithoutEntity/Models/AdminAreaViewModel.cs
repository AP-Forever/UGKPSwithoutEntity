using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UGKPSwithoutEntity.Models
{
    public class AdminAreaViewModel
    {
        public List<ContactUs> Inquiries { get; set; }
    }

    public class EmailModel
    {
        [Display(Name = "Send To")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Recipients are required.")]
        public string SendTo { get; set; }

        [Display(Name = "Email Subject")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Subject is required.")]
        public string Subject { get; set; }

        [Display(Name = "Email Body")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Message is required.")]
        public string Body { get; set; }
    }

    public class EventSelectionModel
    {
        [Display(Name = "Select Event")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select an event.")]
        public string SelectedEventID { get; set; }
    }
}