namespace BiolabIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersJwtToken
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public int? not_before { get; set; }
        public int? expires_in { get; set; }
        public int? expires_on { get; set; }
        public string? resource { get; set; }
    }
}