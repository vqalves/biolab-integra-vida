namespace BiolabIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersStatusConsumerResult
    {
        public Identifier? identifier { get; set; }
        public string? name { get; set; }
        public string? birthDate { get; set; }
        public string? status { get; set; }
        public IEnumerable<Product>? product { get; set; }
        public IEnumerable<Dependent>? dependent { get; set; }

        public class Identifier
        {
            public string? consumerInternalId { get; set; }

            public InterPlayersConsumerId? GetConsumerId()
            {
                if (string.IsNullOrWhiteSpace(consumerInternalId))
                    return null;

                return new InterPlayersConsumerId(consumerInternalId);
            }
        }

        public class Product
        {
            public string? programId { get; set; }
        }

        public class Dependent
        {
            public string? personaId { get; set; }
            public string? birthDate { get; set; }
            public string? age { get; set; }
            public string? name { get; set; }
            public string? personaName { get; set; }
            public IEnumerable<Product>? product { get; set; }
        }

        public InterPlayersConsumerId? GetConsumerId() => identifier?.GetConsumerId();

        public bool IsNaoExistente() => status == "NE";
    }
}