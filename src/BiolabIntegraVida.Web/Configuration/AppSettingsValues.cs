using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BiolabIntegraVida.Web.Configuration
{
    public class AppSettingsValues
    {
        public bool ApplicationConfigCompressContent { get; init; }
        public string GoogleMapsApiKey { get; init; }

        public string InterPlayersAdB2cTokenUrl { get; init; }
        public string InterPlayersAdB2cClientId { get; init; }
        public string InterPlayersAdB2cClientSecret { get; init; }
        public string InterPlayersAdB2cScope { get; init; }

        public string InterPlayersLoyaltyAdministratorId { get; init; }

        public string InterPlayersLoyaltyUrlConsultarUf { get; init; }
        public string InterPlayersLoyaltyUrlConsultarMedicamentos { get; init; }
        public string InterPlayersLoyaltyUrlConsultarEstabelecimentos { get; init; }
        public string InterPlayersLoyaltyUrlConsultarProfissional { get; init; }
        public string InterPlayersLoyaltyUrlConsultarPaciente { get; init; }
        public string InterPlayersLoyaltyUrlConsultarStatusLgpd { get; init; }
        public string InterPlayersLoyaltyUrlAceitarLgpd { get; init; }
        public string InterPlayersLoyaltyUrlCadastrarPaciente { get; init; }
        public string InterPlayersLoyaltyUrlAderirProduto { get; init; }

        public IEnumerable<InterPlayersCustomProductName> InterPlayersCustomProductNames { get; init; }

        public AppSettingsValues(IConfiguration config)
        {
            ApplicationConfigCompressContent = GetValueOrThrow<bool>(config, "ApplicationConfig:CompressContent");
            GoogleMapsApiKey = GetValueOrThrow<string>(config, "GoogleMaps:ApiKey");

            InterPlayersAdB2cTokenUrl = GetValueOrThrow<string>(config, "InterPlayers:AdB2c:TokenUrl");
            InterPlayersAdB2cScope = GetValueOrThrow<string>(config, "InterPlayers:AdB2c:Scope");
            InterPlayersAdB2cClientId = GetValueOrThrow<string>(config, "InterPlayers:AdB2c:ClientId");
            InterPlayersAdB2cClientSecret = GetValueOrThrow<string>(config, "InterPlayers:AdB2c:ClientSecret");

            InterPlayersLoyaltyAdministratorId = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:AdministratorId");

            InterPlayersLoyaltyUrlConsultarUf = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlConsultarUf");
            InterPlayersLoyaltyUrlConsultarMedicamentos = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlConsultarMedicamentos");
            InterPlayersLoyaltyUrlConsultarEstabelecimentos = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlConsultarEstabelecimentos");
            InterPlayersLoyaltyUrlConsultarProfissional = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlConsultarProfissional");
            InterPlayersLoyaltyUrlConsultarPaciente = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlConsultarPaciente");
            InterPlayersLoyaltyUrlConsultarStatusLgpd = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlConsultarStatusLgpd");
            InterPlayersLoyaltyUrlAceitarLgpd = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlAceitarLgpd");
            InterPlayersLoyaltyUrlCadastrarPaciente = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlCadastrarPaciente");
            InterPlayersLoyaltyUrlAderirProduto = GetValueOrThrow<string>(config, "InterPlayers:Loyalty:UrlAderirProduto");

            InterPlayersCustomProductNames = GetSectionOrThrow<List<InterPlayersCustomProductName>>(config, "InterPlayers:CustomProductNames");
        }

        private T GetSectionOrThrow<T>(IConfiguration config, string key)
        {

            var items = config.GetRequiredSection(key).Get<T>();

            if(items == null)
                throw new ArgumentException($"AppSettings '{key}' was not set");

            return items;
        }

        private T GetValueOrThrow<T>(IConfiguration config, string key)
        {
            var value = config.GetValue<T>(key);

            if (value == null)
                throw new ArgumentException($"AppSettings '{key}' was not set");

            return value;
        }
    }
}