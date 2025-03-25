using System.Globalization;

namespace BiopasIntegraVida.Domain.ValueObjects
{
    public class DataNascimento
    {
        public DateTime DateTime { get; init; }

        public DataNascimento(DateTime dateTime)
        {
            this.DateTime = dateTime;
        }

        public static bool TryParse(string? value, out DateTime parsedDate)
        {
            parsedDate = DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            return DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }

        public static DateTime Parse(string? value)
        {
            if (TryParse(value, out DateTime parsedDate))
                return parsedDate;

            throw new FormatException($"'{value}' has an invalid DateTime format");
        }
    }
}