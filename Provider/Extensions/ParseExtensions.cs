namespace Project.Provider.Extensions
{
    public static class ParseExtensions
    {
        public static decimal? ToDecimal(this string? input)
        {

            if (decimal.TryParse(input, out decimal decimalValue))
            {

                return Math.Round(decimalValue, 2);
            }
            return null;
        }

        public static int? ToInt(this string? input)
        {
            if (int.TryParse(input, out int parsedValue))
            {
                return parsedValue;
            }
            return null;
        }

        public static Guid ToGuid(this string? input)
        {
            if (string.IsNullOrEmpty(input) || !Guid.TryParse(input, out Guid userId))
            {
                return Guid.Empty;
            }

            return userId;
        }

        public static decimal FormatDecimal(this decimal number)
        {
            string formattedNumber = number.ToString("0.00##########");
            // Remove trailing zeros
            formattedNumber = formattedNumber.TrimEnd('0');
            if (formattedNumber.LastOrDefault() == '.')
            {
                formattedNumber = formattedNumber + "00";
            }
            var result = formattedNumber.ToDecimal();
            return result ?? 0;
        }
    }
}
