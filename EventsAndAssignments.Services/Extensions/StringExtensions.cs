using System.Globalization;

namespace EventsAndAssignments.Services.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeWords(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string[] words = input.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = textInfo.ToTitleCase(words[i]);
                }
            }

            return string.Join(" ", words);
        }

        public static bool HasValue(this string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}