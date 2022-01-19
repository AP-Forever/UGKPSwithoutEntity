using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UGKPSwithoutEntity.Models
{
    public class ResetPassword
    {
        [Display(Name = "New Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Confirm Password and Pasword do not match.")]
        public string ConfirmPassword { get; set; }

        public string PWResetCode { get; set; }
    }
}