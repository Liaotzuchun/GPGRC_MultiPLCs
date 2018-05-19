using System.Globalization;
using System.Windows.Controls;

namespace GPGO_MultiPLCs.Views.Helpers
{
    public class ValueIsDouble : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;

            return double.TryParse(str, out _) ? ValidationResult.ValidResult : new ValidationResult(false, "無法轉換為double");
        }
    }

    public class ValueIsNotNull : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;

            return !string.IsNullOrEmpty(str) ? ValidationResult.ValidResult : new ValidationResult(false, "值不可為空值");
        }
    }
}