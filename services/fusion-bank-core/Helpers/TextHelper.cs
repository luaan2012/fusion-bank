using System.Globalization;
using System.Text;

namespace fusion.bank.core.Helpers
{
    public static class TextHelper
    {
        public static string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark && IsAllowedCharacter(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private static bool IsAllowedCharacter(char c)
        {
            return char.IsLetterOrDigit(c) || char.IsWhiteSpace(c);
        }
    }
}
