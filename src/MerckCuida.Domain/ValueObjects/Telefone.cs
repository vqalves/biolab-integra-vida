namespace MerckCuida.Domain.ValueObjects
{
    public class Telefone
    {
        public string SemMascara { get; init; }
        public string ComMascara { get; init; }

        private Telefone(string semMascara)
        {
            this.SemMascara = semMascara;

            if(semMascara.Length == 8)
            {
                ComMascara = string.Format
                (
                    "{0}{1}{2}{3}-{4}{5}{6}{7}",
                    semMascara[0],
                    semMascara[1],
                    semMascara[2],
                    semMascara[3],
                    semMascara[4],
                    semMascara[5],
                    semMascara[6],
                    semMascara[7]
                );
            }
            else
            {
                ComMascara = string.Format
                (
                    "{0}{1}{2}{3}{4}-{5}{6}{7}{8}",
                    semMascara[0],
                    semMascara[1],
                    semMascara[2],
                    semMascara[3],
                    semMascara[4],
                    semMascara[5],
                    semMascara[6],
                    semMascara[7],
                    semMascara[8]
                );
            }
        }

        public bool IsCelular() => SemMascara.StartsWith('9') && SemMascara.Length == 9;

        public static Telefone Parse(string value)
        {
            var numbersOnly = new string(value.Where(x => Char.IsNumber(x)).ToArray());

            if (!IsValid(numbersOnly))
                throw new ArgumentException($"Phone '{value}' is invalid");

            return new Telefone(numbersOnly);
        }

        public static bool IsValid(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            var numbers = new string(phoneNumber.Where(x => Char.IsNumber(x)).ToArray());

            if (numbers.Length == 8)
                return true;

            if (numbers.Length == 9 && numbers[0] == '9')
                return true;

            return false;
        }
    }
}