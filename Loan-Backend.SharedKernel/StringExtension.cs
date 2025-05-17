
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;

namespace Loan_Backend.SharedKernel
{

    public static class StringExtensions
    {
        public static string ToJson(this object value)
        {
            if (value == null) return "{}";

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }

        public static string ToProperCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            input = input.ToLower(); // Make everything lowercase first

            StringBuilder result = new StringBuilder(input.Length);
            bool newWord = true;

            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c))
                {
                    newWord = true;
                    result.Append(c);
                }
                else if (newWord)
                {
                    result.Append(char.ToUpper(c));
                    newWord = false;
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static string ToTrimmedAndLowerCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return string.Empty; }
            input = input.Trim().ToLower();

            return input;
        }

        public static string FormatDateWithOrdinal(this DateTime date)
        {
            int day = date.Day;
            string suffix = GetDaySuffix(day);
            return $"{day}{suffix}-{date:MMM-yyyy}";
        }

        private static string GetDaySuffix(int day)
        {
            if (day >= 11 && day <= 13) return "th";

            return (day % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };
        }

    }

}
