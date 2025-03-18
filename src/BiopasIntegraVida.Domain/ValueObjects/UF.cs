namespace BiopasIntegraVida.Domain.ValueObjects
{
    public class UF
    {
        public static readonly UF AC = new UF("AC", "Acre");
        public static readonly UF AL = new UF("AL", "Alagoas");
        public static readonly UF AP = new UF("AP", "Amapá");
        public static readonly UF AM = new UF("AM", "Amazonas");
        public static readonly UF BA = new UF("BA", "Bahia");
        public static readonly UF CE = new UF("CE", "Ceará");
        public static readonly UF DF = new UF("DF", "Distrito Federal");
        public static readonly UF ES = new UF("ES", "Espírito Santo");
        public static readonly UF GO = new UF("GO", "Goiás");
        public static readonly UF MA = new UF("MA", "Maranhão");
        public static readonly UF MT = new UF("MT", "Mato Grosso");
        public static readonly UF MS = new UF("MS", "Mato Grosso do Sul");
        public static readonly UF MG = new UF("MG", "Minas Gerais");
        public static readonly UF PA = new UF("PA", "Pará");
        public static readonly UF PB = new UF("PB", "Paraíba");
        public static readonly UF PR = new UF("PR", "Paraná");
        public static readonly UF PE = new UF("PE", "Pernambuco");
        public static readonly UF PI = new UF("PI", "Piauí");
        public static readonly UF RJ = new UF("RJ", "Rio de Janeiro");
        public static readonly UF RN = new UF("RN", "Rio Grande do Norte");
        public static readonly UF RS = new UF("RS", "Rio Grande do Sul");
        public static readonly UF RO = new UF("RO", "Rondônia");
        public static readonly UF RR = new UF("RR", "Roraima");
        public static readonly UF SC = new UF("SC", "Santa Catarina");
        public static readonly UF SP = new UF("SP", "São Paulo");
        public static readonly UF SE = new UF("SE", "Sergipe");
        public static readonly UF TO = new UF("TO", "Tocantins");

        public string Sigla { get; init; }
        public string Nome { get; init; }

        public UF(string sigla, string nome)
        {
            this.Sigla = sigla;
            this.Nome = nome;
        }

        public static IEnumerable<UF> ListAll()
        {
            yield return AC;
            yield return AL;
            yield return AP;
            yield return AM;
            yield return BA;
            yield return CE;
            yield return DF;
            yield return ES;
            yield return GO;
            yield return MA;
            yield return MT;
            yield return MS;
            yield return MG;
            yield return PA;
            yield return PB;
            yield return PR;
            yield return PE;
            yield return PI;
            yield return RJ;
            yield return RN;
            yield return RS;
            yield return RO;
            yield return RR;
            yield return SC;
            yield return SP;
            yield return SE;
            yield return TO;
        }

        public static UF? FindBySigla(string? sigla)
        {
            if (!string.IsNullOrWhiteSpace(sigla))
                foreach (var uf in ListAll())
                    if (uf.Sigla.Equals(sigla, StringComparison.OrdinalIgnoreCase))
                        return uf;

            return null;
        }
    }
}