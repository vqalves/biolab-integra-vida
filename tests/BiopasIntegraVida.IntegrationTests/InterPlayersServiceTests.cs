using BiopasIntegraVida.Domain.ValueObjects;
using BiopasIntegraVida.IntegrationTests.Mocks;
using BiopasIntegraVida.InterPlayers.Services;
using BiopasIntegraVida.InterPlayers.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;

namespace BiopasIntegraVida.IntegrationTests;

public class InterPlayersServiceTests
{
    private readonly InterPlayersService InterPlayersService;

    public InterPlayersServiceTests()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory
            .Setup(p => p.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient());

        var interPlayersConfig = new InterPlayersConfigMock();

        this.InterPlayersService = new InterPlayersService
        (
            mockFactory.Object,
            interPlayersConfig,
            loggerFactory.CreateLogger<InterPlayersService>()
        );
    }

    [Fact]
    public async Task GerarToken()
    {
        var token = await InterPlayersService.GerarTokenJwtAsync();
        Assert.NotNull(token?.access_token);
    }

    [Fact]
    public async Task ConsultarMedicamentos()
    {
        var medicamentos = await InterPlayersService.ConsultarMedicamentosWithoutCacheAsync();
        Assert.True(medicamentos.Success);
    }

    [Fact]
    public async Task ConsultarUfs()
    {
        var ufs = await InterPlayersService.ConsultarUfsWithoutCacheAsync();
        Assert.True(ufs.Success);
    }

    [Fact]
    public async Task ConsultarEstabelecimentos()
    {
        var cep = CEP.Parse("02514010");
        var estabelecimentos = await InterPlayersService.ConsultarEstabelecimentosAsync(cep);
        Assert.True(estabelecimentos.Success);
    }

    [Fact]
    public async Task ConsultarProfissionais()
    {
        var crm = CRM.Parse("85652");
        var profissional = await InterPlayersService.ConsultarProfissionaisAsync(crm, UF.SP);
        Assert.True(profissional.Success);
    }

    [Fact]
    public async Task ConsultarStatusConsumer()
    {
        var cpf = CPF.Parse("539.477.410-22");
        var paciente = await InterPlayersService.ConsultarStatusConsumerAsync(cpf);
        Assert.True(paciente.Success);
    }
    
    [Fact]
    public async Task ConsultarStatusLgpd()
    {
        var cpf = CPF.Parse("539.477.410-22");
        var paciente = await InterPlayersService.ConsultarStatusConsumerAsync(cpf);
        var consumerId = paciente.Data!.GetConsumerId();

        var status = await InterPlayersService.ConsultarStatusLgpdAsync(consumerId!);
        Assert.True(status.Success || InterPlayersGetLgpdStatusResult.IsTermoNaoEncontrado(status.ErrorData));
    }

    [Fact]
    public async Task AceitarStatusLgpd()
    {
        var cpf = CPF.Parse("539.477.410-22");
        var paciente = await InterPlayersService.ConsultarStatusConsumerAsync(cpf);
        var consumerId = paciente.Data!.GetConsumerId()!;

        var status = await InterPlayersService.AceitarStatusLgpdAsync(
            consumerId: consumerId,
            ddd: TelefoneDDD.Parse("11"),
            telefone: Telefone.Parse("912341234"),
            optInCelular: true,
            optInEmail: true
        );

        Assert.True(status.Success);
    }

    [Fact]
    public async Task AderirProduto()
    {
        var dataNascimento = new DateTime(1980, 1, 1);
        var uf = UF.SP;
        var cpf = CPF.Parse("539.477.410-22");
        var crm = CRM.Parse("85652");

        var paciente = await InterPlayersService.ConsultarStatusConsumerAsync(cpf);
        var consumerId = paciente.Data!.GetConsumerId()!;

        var profissionais = await InterPlayersService.ConsultarProfissionaisAsync(crm, uf);
        var profissional = profissionais.Data!.professional!.First();

        var medicamentos = await InterPlayersService.ConsultarMedicamentosWithoutCacheAsync();
        var medicamento = medicamentos.Data!.product!.First();

        var status = await InterPlayersService.AderirProdutoAsync
        (
            consumerId: consumerId!,
            dataNascimento: dataNascimento,
            produto: medicamento,
            profissional: profissional
        );

        Assert.True(status.Success);
    }
    

    [Fact]
    public async Task CadastrarPaciente()
    {
        var nome = "John Doe";
        var nascimento = new DateTime(1980, 1, 1);
        var genero = InterPlayersPersonGenre.INDEFINIDO;
        var cpf = CPF.Parse("539.477.410-22");
        var mail = Email.Parse("test@teste.com");
        var ddd = TelefoneDDD.Parse("11");
        var telefone = Telefone.Parse("99123-1234");
        var uf = UF.SP;

        var crm = CRM.Parse("85652");
        var profissionais = await InterPlayersService.ConsultarProfissionaisAsync(crm, uf);
        var profissional = profissionais.Data!.professional!.First();

        var medicamentos = await InterPlayersService.ConsultarMedicamentosWithoutCacheAsync();
        var medicamento = medicamentos.Data!.product!.First();

        var statusPaciente = await InterPlayersService.ConsultarStatusConsumerAsync(cpf);
        var consumerId = statusPaciente.Data!.GetConsumerId()!;

        var statusLgpd = await InterPlayersService.ConsultarStatusLgpdAsync(consumerId!);
        if(!statusLgpd.Success || InterPlayersGetLgpdStatusResult.IsTermoNaoEncontrado(statusLgpd.ErrorData))
        {
            var cadastroLgpd = await InterPlayersService.AceitarStatusLgpdAsync(
                consumerId: consumerId,
                ddd: TelefoneDDD.Parse("11"),
                telefone: Telefone.Parse("912341234"),
                optInCelular: true,
                optInEmail: true
            );
        }

        var status = await InterPlayersService.CadastrarPacienteAsync
        (
            nome: nome,
            genero: genero,
            dataNascimento: nascimento,
            cpf: cpf,
            email: mail,
            ddd: ddd,
            telefone: telefone,
            uf: uf,
            product: medicamento,
            professional: profissional,
            optInCelular: true,
            optInEmail: true
        );

        Assert.True(status.Success || InterPlayersCadastrarPacienteResult.JaPossuiCadastro(status.ErrorData));
    }
}
