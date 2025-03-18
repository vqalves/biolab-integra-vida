namespace BiopasIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersListProfessionalResult
    {
        public List<Professional>? professional { get; set; }
        public int? totalItemCount { get; set; }
        public int? page { get; set; }
        public int? pageCount { get; set; }
        public int? pageCurrent { get; set; }

        public class Professional
        {
            public string? professionalType { get; set; }
            public string? professionalId { get; set; }
            public string? professionalState { get; set; }
            public string? professionalName { get; set; }
        }
    }
}