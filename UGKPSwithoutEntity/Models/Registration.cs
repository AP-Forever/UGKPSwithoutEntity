using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UGKPSwithoutEntity.Attributes;

namespace UGKPSwithoutEntity.Models
{
    public class Registration
    {
        [Key]
        public int UserID { get; set; }
        
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

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm Password and Pasword do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Age")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Age is required.")]
        [Range(18, 100, ErrorMessage = "You must be at least 18 years old to register.")]
        public int Age { get; set; }

        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Display(Name = "City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Display(Name = "State")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Display(Name = "Country")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Contry is required.")]
        public string Country { get; set; }

        [Display(Name = "Zip Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Zip Code is required.")]
        public string ZipCode { get; set; }

        [Display(Name = "Phone Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number is required.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:###-###-####}")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Native")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Native is required.")]
        public string Native { get; set; }

        public bool IsUserActivated { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsNewRequest { get; set; }
        public bool IsDeactivated { get; set; }
        public bool IsAdmin { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:DD-MM-YYYY}")]
        public DateTime ModifiedDate { get; set; }
    }

    public enum Country
    {
            Canada,
            USA
    }

    public class User : Registration
    {
        public InvestmentAccess InvestAccess { get; set; } 
        
        public List<FamilyMember> UserFamily { get; set; }

        //public FamilyMember FamilyMember { get; set; }

        public string AccountAction { get; set; }
    }

    public class ListOfUsers
    {
        public List<User> UsersList {get; set;}
    }

    public class FamilyMember
    {
        [Key]
        public int FamilyMemberID { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        
        [Display(Name = "Age")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Age is required.")]
        [Range(0, 100, ErrorMessage = "Age must be greater than 0.")]
        public int Age { get; set; }

        [Display(Name = "Relation")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Relation is required.")]
        public string Relation { get; set; }

        public bool IsChecked { get; set; }

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