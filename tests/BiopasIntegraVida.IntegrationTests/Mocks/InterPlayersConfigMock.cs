using BiopasIntegraVida.InterPlayers.Services;

namespace BiopasIntegraVida.IntegrationTests.Mocks
{
    public class InterPlayersConfigMock : IInterPlayersConfig
    {
        // ADB2C
        public string GetAdB2cClientId() => "1af339e8-289a-45d4-88c7-baaa1c9a85c2";
        public string GetAdB2cClientSecret() => "say8Q~FclwAAhdHtzbu8NMKItfymPhb06j_zrcMk";
        public string GetAdB2cScope() => "https://interplayersdevb2c.onmicrosoft.com/services/loyalty-api/.default";
        public string GetAdB2cTokenUrl() => "https://interplayersdevb2c.b2clogin.com/interplayersdevb2c.onmicrosoft.com/B2C_1_Loyalty/oauth2/v2.0/token";

        // Loyalty
        public string GetLoyaltyAdministratorId() => "021";
        public string GetLoyaltyUrlAceitarLgpd() => "https://idp-api-gtw.azure-api.net/external-loyalty-registration-pre/v1/Registrations/administrators/{administratorId}/consumers/{userId}/accept-term";
        public string GetLoyaltyUrlAderirProduto() => "https://idp-api-gtw.azure-api.net/external-loyalty-adhesion-pre/v1/Adhesion/administrators/{administratorId}/consumers/{userId}/products";
        public string GetLoyaltyUrlCadastrarPaciente() => "https://idp-api-gtw.azure-api.net/external-loyalty-registration-pre/v1/Registrations/administrators/{administratorId}/consumers";
        public string GetLoyaltyUrlConsultarEstabelecimentos() => "https://idp-api-gtw.azure-api.net/external-loyalty-store-pre/v2/Establishment/administrators/{administratorId}/types/MH/establishments?zip={zip}";
        public string GetLoyaltyUrlConsultarMedicamentos() => "https://idp-api-gtw.azure-api.net/external-loyalty-products-pre/v1/Product/administrators/{administratorId}/products";
        public string GetLoyaltyUrlConsultarPaciente() => "https://idp-api-gtw.azure-api.net/external-loyalty-registration-pre/v1/registrations/administrators/{administratorId}/consumers/status?personNumber={cpf}";
        public string GetLoyaltyUrlConsultarProfissional() => "https://idp-api-gtw.azure-api.net/external-loyalty-professionals-pre/v1/Professional/administrators/{administratorId}/professionals/{crm}/{uf}";
        public string GetLoyaltyUrlConsultarStatusLgpd() => "https://idp-api-gtw.azure-api.net/external-loyalty-registration-pre/v2/Registrations/administrators/{administratorId}/consumers/{userId}/accept-term";
        public string GetLoyaltyUrlConsultarUf() => "https://idp-api-gtw.azure-api.net/external-loyalty-logistic-pre/v1/addressData/administrators/{administratorId}/ufs";
    }
}
