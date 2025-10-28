using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DiplomZabpo43.classes
{
    public class EmptyTextValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string text = value as string;
            if (string.IsNullOrWhiteSpace(text))
            {
                return new ValidationResult(false, "Найдите вещество или название");
            }
            return ValidationResult.ValidResult;
        }
    }
}