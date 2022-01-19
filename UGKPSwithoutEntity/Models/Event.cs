using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UGKPSwithoutEntity.Attributes;
using System.Web;

namespace UGKPSwithoutEntity.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Display(Name = "Event Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Event Name is required.")]
        public string Event_Name { get; set; }

        [Display(Name = "Event Location")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Event Location is required.")]
        public string Event_Location { get; set; }


        [Display(Name = "Event Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Event Description is required.")]
        public string Event_Description { get; set; }

        [Display(Name = "Event (Date & Time) From: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [LessThan("Event_EndDate", ErrorMessage = "Event Start Date&Time cannot be greater than End Date&Time.")]
        public DateTime Event_StartDate { get; set; }

        [Display(Name = "Event (Date & Time) To: ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [GreaterThan("Event_StartDate", ErrorMessage = "Event (Date&Time) To cannot be lower than Start Date&Time.")]
        public DateTime Event_EndDate { get; set; }

        public DateTime Date_Created { get; set; }

        public DateTime Date_Updated { get; set; }

        [Display(Name = "Ticket Price for Adult")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ticket Price for Adult is required.")]
        public int Price_Adult { get; set; }

        [Display(Name = "Ticket Price for Child")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ticket Price for Child is required.")]
        public int Price_Child { get; set; }

        public int Fees_Membership { get; set; }
    }

    public class EventRegistration
    {
        [Display(Name = "Adults")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field cannot be empty")]
        public int NoOfAdults { get; set; }

        [Display(Name = "Children")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field cannot be empty")]
        public int NoOfChildren { get; set; }

    }

    public class EventsViewModel
    {
        public Event currEvent { get; set; }
        public List<FamilyMember> Members{ get; set; }
    }
}