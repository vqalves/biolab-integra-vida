namespace BiopasIntegraVida.InterPlayers.Services
{
    public interface IInterPlayersConfig
    {
        string GetAdB2cTokenUrl();
        string GetAdB2cScope();
        string GetAdB2cClientId();
        string GetAdB2cClientSecret();

        string GetLoyaltyAdministratorId();
        string GetLoyaltyUrlConsultarUf();
        string GetLoyaltyUrlConsultarMedicamentos();
        string GetLoyaltyUrlConsultarEstabelecimentos();
        string GetLoyaltyUrlConsultarProfissional();
        string GetLoyaltyUrlConsultarPaciente();
        string GetLoyaltyUrlConsultarStatusLgpd();
        string GetLoyaltyUrlAceitarLgpd();
        string GetLoyaltyUrlCadastrarPaciente();
        string GetLoyaltyUrlAderirProduto();
    }
}