namespace Schefco.TaskFlow.Application.Common.Utilities
{
    public static class PasswordGenerator
    {
        public static string GeneratePassword()
        {
            // Character arrays
            char[] upper = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
            char[] lower = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] symbols = { '!', '@', '#', '$', '%', '^', '&', '*', '?' };

            List<char> tempPassword = new();

            // Pick one from each group
            var rand = new Random();

            tempPassword.Add(upper[rand.Next(upper.Length)]);
            tempPassword.Add(lower[rand.Next(lower.Length)]);
            tempPassword.Add(digits[rand.Next(digits.Length)]);
            tempPassword.Add(symbols[rand.Next(symbols.Length)]);

            // Combine the chars
            var allChars = upper.Concat(lower).Concat(digits).Concat(symbols).ToArray();

            // Fill in remaining chars until 10 total
            while (tempPassword.Count < 10)
            {
                var randChar = allChars[rand.Next(allChars.Length)];
                tempPassword.Add(randChar);
            }

            // Shuffle temp password
            for (int i = tempPassword.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (tempPassword[i], tempPassword[j]) = (tempPassword[j], tempPassword[i]);
            }

            // Return temp password as string
            return new string(tempPassword.ToArray());
        }
    }
}
