using MerckCuida.InterPlayers.Services;

namespace MerckCuida.Web.Configuration
{
    public class InterPlayersConfig : IInterPlayersConfig
    {        
        // Properties
        public string AdB2cClientId { get; init; }
        public string AdB2cClientSecret { get; init; }
        public string AdB2cTokenUrl { get; init; }
        public string AdB2cScope { get; init; }

        public string LoyaltyAdministratorId { get; init; }
        public string LoyaltyUrlConsultarUf { get; init; }
        public string LoyaltyUrlConsultarMedicamentos { get; init; }
        public string LoyaltyUrlConsultarEstabelecimentos { get; init; }
        public string LoyaltyUrlConsultarProfissional { get; init; }
        public string LoyaltyUrlConsultarPaciente { get; init; }
        public string LoyaltyUrlConsultarStatusLgpd { get; init; }
        public string LoyaltyUrlAceitarLgpd { get; init; }
        public string LoyaltyUrlCadastrarPaciente { get; init; }
        public string LoyaltyUrlAderirProduto { get; init; }

        // Interface
        public string GetAdB2cClientId() => AdB2cClientId;
        public string GetAdB2cClientSecret() => AdB2cClientSecret;
        public string GetAdB2cTokenUrl() => AdB2cTokenUrl;
        public string GetAdB2cScope() => AdB2cScope;

        public string GetLoyaltyAdministratorId() => LoyaltyAdministratorId;
        public string GetLoyaltyUrlConsultarUf() => LoyaltyUrlConsultarUf;
        public string GetLoyaltyUrlConsultarMedicamentos() => LoyaltyUrlConsultarMedicamentos;
        public string GetLoyaltyUrlConsultarEstabelecimentos() => LoyaltyUrlConsultarEstabelecimentos;
        public string GetLoyaltyUrlConsultarProfissional() => LoyaltyUrlConsultarProfissional;
        public string GetLoyaltyUrlConsultarPaciente() => LoyaltyUrlConsultarPaciente;
        public string GetLoyaltyUrlConsultarStatusLgpd() => LoyaltyUrlConsultarStatusLgpd;
        public string GetLoyaltyUrlAceitarLgpd() => LoyaltyUrlAceitarLgpd;
        public string GetLoyaltyUrlCadastrarPaciente() => LoyaltyUrlCadastrarPaciente;
        public string GetLoyaltyUrlAderirProduto() => LoyaltyUrlAderirProduto;

        public InterPlayersConfig(AppSettingsValues values)
        {
            AdB2cTokenUrl = values.InterPlayersAdB2cTokenUrl;
            AdB2cClientId = values.InterPlayersAdB2cClientId;
            AdB2cClientSecret = values.InterPlayersAdB2cClientSecret;
            AdB2cScope = values.InterPlayersAdB2cScope;

            LoyaltyAdministratorId = values.InterPlayersLoyaltyAdministratorId;

            LoyaltyUrlConsultarUf = values.InterPlayersLoyaltyUrlConsultarUf;
            LoyaltyUrlConsultarMedicamentos = values.InterPlayersLoyaltyUrlConsultarMedicamentos;
            LoyaltyUrlConsultarEstabelecimentos = values.InterPlayersLoyaltyUrlConsultarEstabelecimentos;
            LoyaltyUrlConsultarProfissional = values.InterPlayersLoyaltyUrlConsultarProfissional;
            LoyaltyUrlConsultarPaciente = values.InterPlayersLoyaltyUrlConsultarPaciente;
            LoyaltyUrlConsultarStatusLgpd = values.InterPlayersLoyaltyUrlConsultarStatusLgpd;
            LoyaltyUrlAceitarLgpd = values.InterPlayersLoyaltyUrlAceitarLgpd;
            LoyaltyUrlCadastrarPaciente = values.InterPlayersLoyaltyUrlCadastrarPaciente;
            LoyaltyUrlAderirProduto = values.InterPlayersLoyaltyUrlAderirProduto;
        }
    }
}