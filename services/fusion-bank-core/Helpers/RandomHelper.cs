using System.Text;

namespace fusion.bank.core.Helpers
{
    public static class RandomHelper
    {
        public static string GetRandomLetters(string text, int n)
        {
            if(string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = TextHelper.CleanText(text.ToUpper().Trim().Replace(" ", ""));

            var random = new Random();   
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                int index = random.Next(0, text.Length);
                sb.Append(text[index]);
            }

            return sb.ToString();           
        }

        public static string GenerateRandomNumbers(int count)
        {
            var random = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                sb.Append(random.Next(0, 10));
            }

            return sb.ToString();
        }
    }
}
