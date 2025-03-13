namespace BiolabIntegraVida.Domain.ValueObjects
{
    public class CRM
    {
        public string Valor { get; init; }

        public CRM(string crm)
        {
            Valor = crm;
        }

        public static CRM Parse(string crm)
        {
            crm = new string(crm.Where(x => Char.IsNumber(x)).ToArray());

            if (!IsValid(crm))
                throw new ArgumentException($"CRM '{crm}' is invalid");

            return new CRM(crm);
        }

        public static bool IsValid(string? crm)
        {
            if (string.IsNullOrWhiteSpace(crm))
                return false;

            var numbers = crm.Where(x => Char.IsNumber(x)).ToArray();
            return numbers.Length == 5 || numbers.Length == 6;
        }
    }
}