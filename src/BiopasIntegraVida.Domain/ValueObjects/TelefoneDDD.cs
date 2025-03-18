using System.Drawing;

namespace BiopasIntegraVida.Domain.ValueObjects
{
    public class TelefoneDDD
    {
        public static readonly int[] Values = new[]
        {
            11, // São Paulo – SP
            12, // São José dos Campos – SP
            13, // Santos – SP
            14, // Bauru – SP
            15, // Sorocaba – SP
            16, // Ribeirão Preto – SP
            17, // São José do Rio Preto – SP
            18, // Presidente Prudente – SP
            19, // Campinas – SP
            21, // Rio de Janeiro – RJ
            22, // Campos dos Goytacazes – RJ
            24, // Volta Redonda – RJ
            27, // Vitória / Vila Velha – ES
            28, // Cachoeiro de Itapemirim – ES
            31, // Belo Horizonte – MG
            32, // Juiz de Fora – MG
            33, // Governador Valadares – MG
            34, // Uberlândia – MG
            35, // Poços de Caldas – MG
            37, // Divinópolis – MG
            38, // Montes Claros – MG
            41, // Curitiba – PR
            42, // Ponta Grossa – PR
            43, // Londrina – PR
            44, // Maringá – PR
            45, // Foz do Iguaçú – PR
            46, // Pato Branco / Francisco Beltrão – PR
            47, // Joinville – SC
            48, // Florianópolis – SC
            49, // Chapecó – SC
            51, // Porto Alegre – RS
            53, // Pelotas – RS
            54, // Caxias do Sul – RS
            55, // Santa Maria – RS
            61, // Brasília – DF
            62, // Goiânia – GO
            63, // Palmas – TO
            64, // Rio Verde – GO
            65, // Cuiabá – MT
            66, // Rondonópolis – MT
            67, // Campo Grande – MS
            68, // Rio Branco – AC
            69, // Porto Velho – RO
            71, // Salvador – BA
            73, // Ilhéus – BA
            74, // Juazeiro – BA
            75, // Feira de Santana – BA
            77, // Barreiras – BA
            79, // Aracaju – SE
            81, // Recife – PE
            82, // Maceió – AL
            83, // João Pessoa – PB
            84, // Natal – RN
            85, // Fortaleza – CE
            86, // Teresina – PI
            87, // Petrolina – PE
            88, // Juazeiro do Norte – CE
            89, // Picos – PI
            91, // Belém – PA
            92, // Manaus – AM
            93, // Santarém – PA
            94, // Marabá – PA
            95, // Boa Vista – RR
            96, // Macapá – AP
            97, // Coari – AM
            98, // São Luís – MA
            99, // Imperatriz – MA
        };

        public string Valor { get; init; }

        private TelefoneDDD(string valor)
        {
            this.Valor = valor;
        }

        public static TelefoneDDD Parse(string ddd)
        {
            ddd = new string(ddd.Where(x => Char.IsNumber(x)).ToArray());

            if (!IsValid(ddd))
                throw new ArgumentException($"DDD '{ddd}' invalid");

            return new TelefoneDDD(ddd);
        }

        public static bool IsValid(string? ddd)
        {
            if (string.IsNullOrWhiteSpace(ddd))
                return false;

            if (!int.TryParse(ddd, out var value))
                return false;

            return Values.Contains(value);
        }
    }
}