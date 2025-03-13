namespace MerckCuida.Domain.ValueObjects
{
    public class CEP
    {
        public string SemMascara { get; init; }
        public string ComMascara { get; init; }

        public CEP(string semMascara)
        {
            SemMascara = semMascara;

            ComMascara = string.Format
            (
                "{0}{1}{2}{3}{4}-{5}{6}{7}",
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
        
        public static CEP Parse(string cep)
        {
            if (!IsValid(cep))
                throw new ArgumentException(cep);

            cep = new string(cep.Where(x => Char.IsNumber(x)).ToArray());
            return new CEP(cep);
        }

        public static bool IsValid(string? cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return false;

            cep = new string(cep.Where(x => Char.IsNumber(x)).ToArray());
            return cep.Length == 8;
        }
    }
}
