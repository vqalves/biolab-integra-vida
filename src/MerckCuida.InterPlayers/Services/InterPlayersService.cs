using MerckCuida.Domain.ValueObjects;
using MerckCuida.Infrastructure;
using MerckCuida.InterPlayers.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MerckCuida.InterPlayers.Services
{
    public class InterPlayersService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IInterPlayersConfig _interPlayersConfig;
        private readonly ILogger<InterPlayersService> _logger;

        private readonly AsyncMemoryCache<InterPlayersServiceResult<IEnumerable<InterPlayersUf>>> UfsCache;
        private readonly AsyncMemoryCache<InterPlayersServiceResult<InterPlayersListProductResult>> MedicamentosCache;
        private readonly AsyncMemoryCache<InterPlayersJwtToken> TokenCache;

        public InterPlayersService(
            IHttpClientFactory httpClientFactory,
            IInterPlayersConfig interPlayersConfig,
            ILogger<InterPlayersService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _interPlayersConfig = interPlayersConfig;
            _logger = logger;

            UfsCache = new(dataGenerator: ConsultarUfsWithoutCacheAsync, expirationTimeGenerator: (d) => TimeSpan.FromMinutes(30));

            MedicamentosCache = new(dataGenerator: ConsultarMedicamentosWithoutCacheAsync, expirationTimeGenerator: (d) =>
            {
                if (d.Success)
                    return TimeSpan.FromHours(1);

                return TimeSpan.FromSeconds(0);
            });

            TokenCache = new(dataGenerator: GerarTokenJwtWithoutCacheAsync, expirationTimeGenerator: (d) =>
            {
                if (d?.expires_in == null)
                    return TimeSpan.FromSeconds(0);

                if (d.expires_in < 30)
                    return TimeSpan.FromSeconds(0);

                return TimeSpan.FromSeconds(d.expires_in.Value);
            });
        }

        public async Task<InterPlayersJwtToken> GerarTokenJwtAsync() => await TokenCache.GetDataAsync();
        public async Task<InterPlayersJwtToken> GerarTokenJwtWithoutCacheAsync()
        {
            var url = _interPlayersConfig.GetAdB2cTokenUrl();
            var scope = _interPlayersConfig.GetAdB2cScope();
            var clientId = _interPlayersConfig.GetAdB2cClientId();
            var clientSecret = _interPlayersConfig.GetAdB2cClientSecret();

            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "scope", scope }
            };

            request.Content = new FormUrlEncodedContent(requestBody);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonSerializer.Deserialize<InterPlayersJwtToken>(content)!;

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(GerarTokenJwtWithoutCacheAsync), st.ElapsedMilliseconds);
            return tokenResponse;
        }

        public async Task<InterPlayersServiceResult<IEnumerable<InterPlayersUf>>> ConsultarUfsAsync() => await UfsCache.GetDataAsync();
        public async Task<InterPlayersServiceResult<IEnumerable<InterPlayersUf>>> ConsultarUfsWithoutCacheAsync()
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlConsultarUf();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);
            var result = await HandleContentDefaultAsync<IEnumerable<InterPlayersUf>>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(ConsultarUfsWithoutCacheAsync), st.ElapsedMilliseconds);
            return result;
        }

        // Documento: /home/vinicius/Downloads/2024-12-11 DLOA/DLOA/2024-11 Landing page Merck/PDFs/09. Manual Técnico - ListProducts - v4.0.pdf
        public async Task<InterPlayersServiceResult<InterPlayersListProductResult>> ConsultarMedicamentosAsync() => await MedicamentosCache.GetDataAsync();
        public async Task<InterPlayersServiceResult<InterPlayersListProductResult>> ConsultarMedicamentosWithoutCacheAsync()
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlConsultarMedicamentos();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);
            var data = await HandleContentDefaultAsync<InterPlayersListProductResult>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(ConsultarMedicamentosWithoutCacheAsync), st.ElapsedMilliseconds);

            return data;

        }

        public async Task<InterPlayersServiceResult<InterPlayersListEstablishmentResult>> ConsultarEstabelecimentosAsync(CEP zipCode)
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlConsultarEstabelecimentos();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);
            apiUrl = apiUrl.Replace("{zip}", zipCode.SemMascara);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);

            var data = await HandleContentDefaultAsync<InterPlayersListEstablishmentResult>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(ConsultarEstabelecimentosAsync), st.ElapsedMilliseconds);

            return data;
        }

        public async Task<InterPlayersServiceResult<InterPlayersListProfessionalResult>> ConsultarProfissionaisAsync(CRM crm, UF uf)
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlConsultarProfissional();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);
            apiUrl = apiUrl.Replace("{crm}", crm.Valor);
            apiUrl = apiUrl.Replace("{uf}", uf.Sigla);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);

            var data = await HandleContentDefaultAsync<InterPlayersListProfessionalResult>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(ConsultarProfissionaisAsync), st.ElapsedMilliseconds);

            return data;
        }

        public async Task<InterPlayersServiceResult<InterPlayersGetLgpdStatusResult>> ConsultarStatusLgpdAsync(InterPlayersConsumerId consumerId)
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();
            var userId = consumerId.Value;

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlConsultarStatusLgpd();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);
            apiUrl = apiUrl.Replace("{userId}", userId);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);

            var data = await HandleContentDefaultAsync<InterPlayersGetLgpdStatusResult>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(ConsultarStatusLgpdAsync), st.ElapsedMilliseconds);

            return data;
        }

        public async Task<InterPlayersServiceResult<object?>> AceitarStatusLgpdAsync(
            InterPlayersConsumerId consumerId,
            TelefoneDDD ddd,
            Telefone telefone,
            bool optInCelular,
            bool optInEmail
        )
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();
            var userId = consumerId.Value;

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlAceitarLgpd();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);
            apiUrl = apiUrl.Replace("{userId}", userId);

            // Root
            var payloadData = new Dictionary<string, object>();
            if (telefone.IsCelular())
            {
                payloadData["dddCellPhone"] = ddd.Valor;
                payloadData["cellPhone"] = telefone.SemMascara;
            }
            else
            {
                payloadData["dddPhone"] = ddd.Valor;
                payloadData["phone"] = telefone.SemMascara;
            }

            payloadData["termCode"] = "01";
            payloadData["evidenceType"] = "SITE";
            payloadData["evidence"] = StringGenerator.GenerateRandom(11);
            payloadData["IP"] = "N/A";

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

            // Send request
            var jsonContent = JsonSerializer.Serialize(payloadData);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);

            var data = await HandleContentDefaultAsync<object?>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(AceitarStatusLgpdAsync), st.ElapsedMilliseconds);

            return data;
        }

        public async Task<InterPlayersServiceResult<InterPlayersStatusConsumerResult>> ConsultarStatusConsumerAsync(CPF cpf)
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlConsultarPaciente();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);
            apiUrl = apiUrl.Replace("{cpf}", cpf.SemMascara);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);

            var data = await HandleContentDefaultAsync<InterPlayersStatusConsumerResult>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(ConsultarStatusConsumerAsync), st.ElapsedMilliseconds);

            return data;
        }

        public async Task<InterPlayersServiceResult<InterPlayersAderirProdutoResult>> AderirProdutoAsync(
            InterPlayersConsumerId consumerId,
            DateTime dataNascimento,
            InterPlayersListProductResult.Product produto,
            InterPlayersListProfessionalResult.Professional profissional
        )
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlAderirProduto();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);
            apiUrl = apiUrl.Replace("{userId}", consumerId.Value);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            // Root
            var payloadData = new Dictionary<string, object>();
            payloadData["birthDate"] = dataNascimento.ToString("yyyy-MM-dd");

            // Product
            var productData = new Dictionary<string, object>();
            payloadData["product"] = productData;

            productData["ean"] = produto.ean!;
            productData["adhesionSource"] = "WEB";

            // Product professional
            var productProfessionalData = new Dictionary<string, object>();
            productData["professional"] = productProfessionalData;

            productProfessionalData["professionalType"] = profissional.professionalType!;
            productProfessionalData["professionalId"] = profissional.professionalId!;
            productProfessionalData["professionalState"] = profissional.professionalState!;
            productProfessionalData["professionalName"] = profissional.professionalName!;

            var jsonContent = JsonSerializer.Serialize(payloadData);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);

            var data = await HandleContentDefaultAsync<InterPlayersAderirProdutoResult>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(AderirProdutoAsync), st.ElapsedMilliseconds);

            return data;
        }



        public async Task<InterPlayersServiceResult<InterPlayersCadastrarPacienteResult>> CadastrarPacienteAsync(
            string nome,
            DateTime dataNascimento,
            CPF cpf,
            Email email,
            TelefoneDDD ddd,
            Telefone telefone,
            UF uf,
            InterPlayersPersonGenre genero,
            InterPlayersListProductResult.Product product,
            InterPlayersListProfessionalResult.Professional professional,
            bool optInCelular,
            bool optInEmail
        )
        {
            var administratorId = _interPlayersConfig.GetLoyaltyAdministratorId();

            var apiUrl = _interPlayersConfig.GetLoyaltyUrlCadastrarPaciente();
            apiUrl = apiUrl.Replace("{administratorId}", administratorId);

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

            // Root
            var payloadData = new Dictionary<string, object>();
            payloadData["name"] = nome;
            payloadData["genre"] = genero.Value;
            payloadData["birthDate"] = dataNascimento.ToString("yyyy-MM-dd");
            payloadData["personNumber"] = cpf.SemMascara;

            // Address
            var addressData = new Dictionary<string, object>();
            payloadData["address"] = addressData;

            addressData["state"] = uf.Sigla;

            // Contact
            var contactData = new Dictionary<string, object>();
            payloadData["contact"] = contactData;

            contactData["email"] = email.Address;
            if (telefone.IsCelular())
            {
                contactData["dddCellPhone"] = ddd.Valor;
                contactData["cellPhone"] = telefone.SemMascara;
            }
            else
            {
                contactData["dddPhone"] = ddd.Valor;
                contactData["phone"] = telefone.SemMascara;
            }

            // Product
            var productData = new Dictionary<string, object>();
            payloadData["product"] = productData;

            productData["ean"] = product.ean!;
            productData["campaignId"] = "";
            productData["adhesionSource"] = "WEB";

            // Product professional
            var productProfessionalData = new Dictionary<string, object>();
            productData["professional"] = productProfessionalData;

            productProfessionalData["professionalType"] = professional.professionalType!;
            productProfessionalData["professionalId"] = professional.professionalId!;
            productProfessionalData["professionalState"] = professional.professionalState!;
            productProfessionalData["professionalName"] = professional.professionalName!;

            // Optin
            var optInData = new Dictionary<string, object>();
            payloadData["optIn"] = optInData;

            optInData["informativeMaterial"] = "N";
            optInData["mail"] = "N";
            optInData["email"] = optInEmail ? "S" : "N";
            optInData["phone"] = optInCelular ? "S" : "N";
            optInData["sms"] = optInCelular ? "S" : "N";
            optInData["push"] = optInCelular ? "S" : "N";
            optInData["whatsApp"] = optInCelular ? "S" : "N";

            // Send request
            var jsonContent = JsonSerializer.Serialize(payloadData);

            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var jwtToken = await GerarTokenJwtAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.access_token);

            var st = Stopwatch.StartNew();

            var response = await client.SendAsync(request);

            var data = await HandleContentDefaultAsync<InterPlayersCadastrarPacienteResult>(response);

            _logger.LogInformation("{methodName} executed in {duration}ms", nameof(CadastrarPacienteAsync), st.ElapsedMilliseconds);

            return data;
        }


        private async Task<InterPlayersServiceResult<T>> HandleContentDefaultAsync<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return InterPlayersServiceResult<T>.CreateSuccess(default, null);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);

                var raw = await reader.ReadToEndAsync();
                var error = InterPlayersError.Deserialize(raw);

                var result = InterPlayersServiceResult<T>.CreateError(error!, raw);

                var ex = new Exception("Integration error with InterPlayers");
                _logger.LogError(ex, "Response error: {response}", result.ErrorData?.Raw);

                return result;
            }
            else if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream);

                var raw = await reader.ReadToEndAsync();
                var data = JsonSerializer.Deserialize<T>(raw);

                return InterPlayersServiceResult<T>.CreateSuccess(data!, raw);
            }
            else
            {
                var responseText = await response.Content.ReadAsStringAsync();

                throw new Exception($"Unhandled StatusCode {response.StatusCode}. Response body: {responseText}");
            }
        }
    }
}