namespace MerckCuida.Domain.ValueObjects
{
    public class CPF
    {
        public string SemMascara { get; init; }
        public string ComMascara { get; init; }

        private CPF(string semMascara) 
        {
            this.SemMascara = semMascara;

            this.ComMascara = string.Format
            (
                "{0}{1}{2}.{3}{4}{5}.{6}{7}{8}-{9}{10}",
                semMascara[0], 
                semMascara[1], 
                semMascara[2],
                semMascara[3],
                semMascara[4],
                semMascara[5],
                semMascara[6],
                semMascara[7],
                semMascara[8],
                semMascara[9], 
                semMascara[10]
            );
        }

        public static CPF Parse(string cpf)
        {
            if (!IsValid(cpf))
                throw new ArgumentException($"Invalid CPF {cpf}");

            var apenasNumeros = new string(cpf.Where(x => Char.IsNumber(x)).ToArray());
            return new CPF(apenasNumeros);
        }

        public static bool IsValid(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            var invalidCPFs = new string[] {
                "00000000000",
                "11111111111",
                "22222222222",
                "33333333333",
                "44444444444",
                "55555555555",
                "66666666666",
                "77777777777",
                "88888888888",
                "99999999999"
            };

            if (invalidCPFs.Contains(cpf))
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}