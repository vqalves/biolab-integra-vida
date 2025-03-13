namespace BiolabIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersGetLgpdStatusResult
    {
        public List<Lgpd>? lgpd { get; set; }
        public Anonymization? anonymization { get; set; }

        public class Lgpd
        {
            public string? ean { get; set; }
            public string? programId { get; set; }
            public string? dateAccepted { get; set; }
            public string? termCode { get; set; }
            public string? evidence { get; set; }
            public string? source { get; set; }
        }

        public class Anonymization
        {
            public string? dateRequest { get; set; }
            public string? dateAnonimization { get; set; }
            public string? Source { get; set; }
        }

        public static bool IsTermoNaoEncontrado(InterPlayersError? error) => error?.Deserialized?.data?.error == "LR06";
    }
}