namespace BiolabIntegraVida.Domain.ValueObjects
{
    public class Email
    {
        public string Address { get; init; }

        private Email(string address)
        {
            this.Address = address;
        }

        public static Email Parse(string address)
        {
            address = address.Trim().ToLower();

            if (!IsValid(address))
                throw new ArgumentException($"Mail address '{address}' is invalid");

            return new Email(address);
        }

        public static bool IsValid(string email)
        {
            var trimmedEmail = email.Trim();

            if (string.IsNullOrWhiteSpace(trimmedEmail))
                return false;

            if (trimmedEmail.EndsWith("."))
                return false; // suggested by @TK-421;

            if (!trimmedEmail.Contains("."))
                return false;

            if (trimmedEmail.Count(x => '@'.Equals(x)) != 1)
                return false;

            var domain = email.Split('@')[1];
            var domainParts = domain.Split('.').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if(domainParts.Length < 2)
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

    }
}