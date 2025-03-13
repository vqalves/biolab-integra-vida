namespace BiolabIntegraVida.Infrastructure
{
    public sealed class StringGenerator
    {
        private StringGenerator() { }
        public static string GenerateRandom(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
                stringChars[i] = chars[Random.Shared.Next(chars.Length)];

            return new string(stringChars);
        }
    }
}
