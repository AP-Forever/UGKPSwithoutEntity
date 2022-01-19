using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UGKPSwithoutEntity.Attributes
{
    public sealed class ValidateBirthDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime _userDOB = Convert.ToDateTime(value);
                if (_userDOB >= DateTime.Now)
                {
                    return new ValidationResult("Birth Date cannot be greater than current date.");
                }else if((DateTime.Now -_userDOB).TotalDays / 365.2425 < 16)
                {
                    return new ValidationResult("You must be 16 years old to register.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public sealed class ValidateBirthDateOnlyDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                DateTime _userDOB = Convert.ToDateTime(value);
                if (_userDOB >= DateTime.Now)
                {
                    return new ValidationResult("Birth Date cannot be greater than current date.");
                }
            }
            return ValidationResult.Success;
        }
    }
}