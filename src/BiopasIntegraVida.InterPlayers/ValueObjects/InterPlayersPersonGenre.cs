namespace BiopasIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersPersonGenre
    {
        public static readonly InterPlayersPersonGenre MASCULINO = new InterPlayersPersonGenre("M", "Masculino");
        public static readonly InterPlayersPersonGenre FEMININO = new InterPlayersPersonGenre("F", "Feminino");
        public static readonly InterPlayersPersonGenre INDEFINIDO = new InterPlayersPersonGenre("I", "Indefinido");

        public string Value { get; init; }
        public string Description { get; init; }

        public InterPlayersPersonGenre(string value, string description)
        {
            Value = value;
            Description = description;
        }

        public static IEnumerable<InterPlayersPersonGenre> ListAll()
        {
            yield return MASCULINO;
            yield return FEMININO;
            yield return INDEFINIDO;
        }
    }
}
