using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UGKPSwithoutEntity.Attributes
{
    public class GreaterThan : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public GreaterThan(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessage;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
            {
                throw new ArgumentException("Property with this name not found");
            }
            else
            {
                if (property.PropertyType.Name.Contains("Date"))
                {
                    var currentValue = Convert.ToDateTime(value);
                    var comparisonValue = Convert.ToDateTime(property.GetValue(validationContext.ObjectInstance));
                    if (currentValue < comparisonValue)
                        return new ValidationResult(ErrorMessage);
                }
                else if (property.PropertyType.Name.Contains("int") || property.PropertyType.Name.Contains("double")
                    || property.PropertyType.Name.Contains("flaot"))
                {
                    var currentValue = Convert.ToDouble(value);
                    var comparisonValue = Convert.ToDouble(property.GetValue(validationContext.ObjectInstance));
                    if (currentValue < comparisonValue)
                        return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}