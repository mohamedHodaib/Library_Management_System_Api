using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Validations
{
    public class ListElementsRangeAttribute : ValidationAttribute
    {
        private readonly int _min;
        private readonly int _max;

        public ListElementsRangeAttribute(int min, int max)
        {
            if (min >= max)
                throw new ArgumentOutOfRangeException(nameof(min),"The minimum value must be less than maximum value.");
            
            _min = min;
            _max = max;
        }



        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            //skip validation if the property that has attribute not a list
            if(value is not IEnumerable list)
            {
                return ValidationResult.Success;
            }

            foreach(var item in list)
            {
                if (item is not int intValue || intValue < _min || intValue > _max)
                    return new ValidationResult($"Each number on the list much be between {_min} and {_max} inclusive.");
            }

            return ValidationResult.Success;
        }
    }
}
