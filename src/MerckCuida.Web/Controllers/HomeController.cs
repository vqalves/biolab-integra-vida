using MerckCuida.Domain.ValueObjects;
using MerckCuida.Infrastructure;
using MerckCuida.Infrastructure.Interfaces;
using MerckCuida.InterPlayers.Services;
using MerckCuida.InterPlayers.ValueObjects;
using MerckCuida.Web.Configuration;
using MerckCuida.Web.UseCases.CadastreSeDados;
using MerckCuida.Web.ValueObjects;
using MerckCuida.Web.ViewModels.LandingPages;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MerckCuida.Web.Controllers;

public class HomeController : BaseController
{
    private static readonly CultureInfo EN_US = CultureInfo.GetCultureInfo("en-US");

    private readonly ILogger<HomeController> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public HomeController(
        ILogger<HomeController> logger, 
        IDateTimeProvider dateTimeProvider
    )
    {
        _logger = logger;
        this._dateTimeProvider = dateTimeProvider;
    }

    [HttpGet("")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(
        [FromServices] AppSettingsValues values
    )
    {
        var vm = new IndexViewModel
        (
            googleMapsApiKey: values.GoogleMapsApiKey
        );

        return View("Views/LandingPages/index.cshtml", vm);
    }

    [HttpPost("/index/cadastre-se-validation")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index_CadastreseValidationAsync(
        [FromServices] InterPlayersService interPlayersService)
    {
        var result = new FormResult();

        var fieldCpf = result.WithField("cpf");
        var valueCpf = Request.Form[fieldCpf.Key].ToString();

        if(string.IsNullOrWhiteSpace(valueCpf))
            fieldCpf.Add("Preencha o CPF");
        else if (!CPF.IsValid(valueCpf))
            fieldCpf.Add("CPF inválido");
        else
        {
            var cpf = CPF.Parse(valueCpf);
            var statusPaciente = await interPlayersService.ConsultarStatusConsumerAsync(cpf);

            if(statusPaciente.Success && !statusPaciente.Data!.IsNaoExistente())
                fieldCpf.Add("CPF já faz parte do programa");
        }

        return Json(result);
    }

    [HttpPost("/index/locais-por-cep")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index_LocaisPorCep(
        [FromServices]InterPlayersService interPlayersService
    )
    {
        string valueCep = Request.Form["cep"].ToString();
        var cep = CEP.Parse(valueCep);

        var result = await interPlayersService.ConsultarEstabelecimentosAsync(cep);

        var lojas = result.Data!.establishments!.Select(x => new
        {
            name = x.GetFormattedName(),
            latitude = Convert.ToDecimal(x.latitude, EN_US),
            longitude = Convert.ToDecimal(x.longitude, EN_US),
            zipCode = x.address?.zip,
            address = x.address?.FormatFullAddress(),
            phone = x.contact?.FormatFullPhone()
        }).ToList();

        return Json(lojas);
    }

    [HttpGet("cadastre-se")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> CadastreSe(
        [FromServices] AppSettingsValues appSettingsValues,
        [FromServices] InterPlayersService interPlayersService
    )
    {
        var cpf = "000.000.000-00";
        return await CadastreSe(appSettingsValues, interPlayersService, cpf);
    }

    [HttpPost("cadastre-se")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> CadastreSe(
        [FromServices] AppSettingsValues appSettingsValues,
        [FromServices] InterPlayersService interPlayersService,
        [FromForm] string? cpf
    )
    {
        var ufs = UF.ListAll();
        var produtosInterPlayers = await interPlayersService.ConsultarMedicamentosAsync();

        if (!produtosInterPlayers.Success)
            return CustomErrorPage();

        var produtos = ProdutoInterPlayers.ListarProdutos
        (
            appSettingsValues.InterPlayersCustomProductNames,
            produtosInterPlayers.Data!.product!
        )
        .ToList();

        var vm = new CadastreSeViewModel
        (
            ufs: ufs,
            cpf: cpf ?? "000.000.000-00",
            produtos: produtos
        );

        return View("Views/LandingPages/cadastre-se.cshtml", vm);
    }

    public class CadastreSePasswordValidationResult
    {
        public bool charCountIsValid { get; set; }
        public bool specialCharIsValid { get; set; }
        public bool numberIsValid { get; set; }
        public bool matchIsValid { get; set; }


        public bool allValid { get { return charCountIsValid && specialCharIsValid && numberIsValid && matchIsValid; } }
    }

    private CadastreSePasswordValidationResult CadastreSe_PasswordValidation_Generate()
    {
        var result = new CadastreSePasswordValidationResult();

        // Carregar formulário
        var valuePassword1 = Request.Form["password-1"].ToString();
        var valuePassword2 = Request.Form["password-2"].ToString();

        // Validation
        result.charCountIsValid = !string.IsNullOrWhiteSpace(valuePassword1) && valuePassword1.Length >= 8;
        result.specialCharIsValid = "!@#$%&*-+()|/".Any(x => valuePassword1.Contains(x));
        result.numberIsValid = valuePassword1.Any(x => Char.IsNumber(x));
        result.matchIsValid = !string.IsNullOrWhiteSpace(valuePassword1) && valuePassword1 == valuePassword2;

        return result;
    }

    [HttpPost("/cadastre-se/password-validation")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult CadastreSe_PasswordValidation()
    {
        var result = CadastreSe_PasswordValidation_Generate();
        return Json(result);
    }

    private async Task<FormResult> CadastreSe_Validation_GenerateAsync(
        AppSettingsValues appSettingsValues,
        InterPlayersService interPlayersService
    )
    {
        var result = new FormResult();

        // Carregar formulário
        var fieldCpf = result.WithField(CadastreSeDadosCampos.cpf);
        string valueCpf = Request.Form[fieldCpf.Key].ToString();

        var fieldMedicamento = result.WithField(CadastreSeDadosCampos.medicamento);
        string valueMedicamento = Request.Form[fieldMedicamento.Key].ToString();

        /*
        var fieldTipoCadastro = result.WithField(CadastreSeDadosCampos.tipoCadastro);
        string valueTipoCadastro = Request.Form[fieldTipoCadastro.Key].ToString();
        */

        var fieldNome = result.WithField(CadastreSeDadosCampos.nome);
        string valueNome = Request.Form[fieldNome.Key].ToString();

        var fieldNascimento = result.WithField(CadastreSeDadosCampos.nascimento);
        string valueNascimento = Request.Form[fieldNascimento.Key].ToString();

        var fieldEmail = result.WithField(CadastreSeDadosCampos.email);
        string valueEmail = Request.Form[fieldEmail.Key].ToString();

        var fieldDdd = result.WithField(CadastreSeDadosCampos.ddd);
        string valueDdd = Request.Form[fieldDdd.Key].ToString();

        var fieldTelefone = result.WithField(CadastreSeDadosCampos.telefone);
        string valueTelefone = Request.Form[fieldTelefone.Key].ToString();

        var fieldCrm = result.WithField(CadastreSeDadosCampos.crm);
        string valueCrm = Request.Form[fieldCrm.Key].ToString();

        var fieldUf = result.WithField(CadastreSeDadosCampos.uf);
        string valueUf = Request.Form[fieldUf.Key].ToString();
        var ufBySigla = UF.FindBySigla(valueUf);

        var fieldComunicacaoCelular = result.WithField(CadastreSeDadosCampos.comunicacaoCelular);
        string valueComunicacaoCelular = Request.Form[fieldComunicacaoCelular.Key].ToString();

        var fieldComunicacaoEmail = result.WithField(CadastreSeDadosCampos.comunicacaoEmail);
        string valueComunicacaoEmail = Request.Form[fieldComunicacaoEmail.Key].ToString();

        var fieldHoneypot = result.WithField(CadastreSeDadosCampos.nomeMae);
        string valueHoneypot = Request.Form[fieldHoneypot.Key].ToString();

        var produtosInterPlayers = await interPlayersService.ConsultarMedicamentosAsync();
        var produtos = ProdutoInterPlayers.ListarProdutos
        (
            appSettingsValues.InterPlayersCustomProductNames,
            produtosInterPlayers.Data!.product!
        )
        .ToList();

        // Validation
        if (string.IsNullOrWhiteSpace(valueCpf))
            fieldCpf.Add("Preencha o CPF");
        else if (!CPF.IsValid(valueCpf))
            fieldCpf.Add("CPF inválido");
        else
        {
            var cpf = CPF.Parse(valueCpf);
            var statusPaciente = await interPlayersService.ConsultarStatusConsumerAsync(cpf);

            if (statusPaciente.Success && !statusPaciente.Data!.IsNaoExistente())
                fieldCpf.Add("CPF já faz parte do programa");
        }

        if (string.IsNullOrWhiteSpace(valueMedicamento))
            fieldMedicamento.Add("Preencha o medicamento");
        else if (!produtos.Any(x => x.ProdutoOriginal.ean == valueMedicamento))
            fieldMedicamento.Add("Selecione o medicamento da lista");

        /*
        if (string.IsNullOrWhiteSpace(valueTipoCadastro))
            fieldTipoCadastro.Add("Selecione o tipo de cadastro");
        */

        if (string.IsNullOrWhiteSpace(valueNome))
            fieldNome.Add("Preencha seu nome");
        else if(valueNome.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length < 2)
            fieldNome.Add("Preencha o nome completo");

        var isNascimentoValid = DateTime.TryParseExact(valueNascimento, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate);
        if (string.IsNullOrWhiteSpace(valueNascimento))
        {
            fieldNascimento.Add("Preencha a data de nascimento");
        }
        else if (!isNascimentoValid || parsedDate > DateTime.Now.Date || parsedDate < new DateTime(1900, 1, 1))
        {
            fieldNascimento.Add("Data inválida");
        }
        else if(AgeCalculator.Calculate(parsedDate, _dateTimeProvider) < 18)
        {
            fieldNascimento.Add("Apenas maiores de 18 anos");
        }

        if (string.IsNullOrWhiteSpace(valueEmail))
        {
            fieldEmail.Add("Preencha seu e-mail");
        }
        else if (!Email.IsValid(valueEmail))
        {
            fieldEmail.Add("E-mail inválido");
        }

        if (string.IsNullOrWhiteSpace(valueDdd))
        {
            fieldDdd.Add("Obrigatório");
        }
        else if (!TelefoneDDD.IsValid(valueDdd))
        {
            fieldDdd.Add("Inválido");
        }

        if (string.IsNullOrWhiteSpace(valueTelefone))
        {
            fieldTelefone.Add("Preencha o telefone");
        }
        else if (!Telefone.IsValid(valueTelefone))
        {
            fieldTelefone.Add("Telefone inválido");
        }

        if (string.IsNullOrWhiteSpace(valueCrm))
        {
            fieldCrm.Add("Preencha o CRM");
        }
        else if (!CRM.IsValid(valueCrm))
        {
            fieldCrm.Add("CRM inválido");
        }

        if (!fieldCrm.Any() && ufBySigla != null)
        {
            var crm = CRM.Parse(valueCrm);
            var medicos = await interPlayersService.ConsultarProfissionaisAsync(crm, ufBySigla);
            var medico = medicos.Data?.professional?.FirstOrDefault();

            if (medico == null)
                fieldCrm.Add("CRM não identificado");
        }

        if (string.IsNullOrWhiteSpace(valueUf))
        {
            fieldUf.Add("Selecione a UF");
        }
        else if (ufBySigla == null)
        {
            fieldUf.Add("UF inválida");
        }

        if (!string.IsNullOrWhiteSpace(valueHoneypot))
        {
            fieldMedicamento.Add("Dado inválido");
        }

        return result;
    }

    [HttpPost("/cadastre-se/validation")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> CadastreSe_Validation(
        [FromServices]InterPlayersService interPlayersService,
        [FromServices]AppSettingsValues appSettingsValues)
    {
        var result = await CadastreSe_Validation_GenerateAsync(
            appSettingsValues, 
            interPlayersService);

        return Json(result);
    }

    [HttpPost("/cadastre-se/submit")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> CadastreSe_Submit(
        [FromServices] InterPlayersService interPlayersService,
        [FromServices] AppSettingsValues appSettingsValues)
    {
        // var passwordValidation = CadastreSe_PasswordValidation_Generate();
        var formValidation = await CadastreSe_Validation_GenerateAsync(
            appSettingsValues, 
            interPlayersService);

        if(/* !passwordValidation.allValid || */formValidation.HasMessage)
        {
            return BadRequest(new
            {
                formValidation = formValidation,
                // passwordValidation = passwordValidation
            });
        }

        var nome = Request.Form[CadastreSeDadosCampos.nome]!;
        var cpf = CPF.Parse(Request.Form[CadastreSeDadosCampos.cpf]!);
        var email = Email.Parse(Request.Form[CadastreSeDadosCampos.email]!);
        var ddd = TelefoneDDD.Parse(Request.Form[CadastreSeDadosCampos.ddd]!);
        var telefone = Telefone.Parse(Request.Form[CadastreSeDadosCampos.telefone]!);
        var uf = UF.FindBySigla(Request.Form[CadastreSeDadosCampos.uf])!;
        var dataNascimento = Convert.ToDateTime(Request.Form[CadastreSeDadosCampos.nascimento]);
        var genero = InterPlayersPersonGenre.INDEFINIDO;

        var medicamentosIP = await interPlayersService.ConsultarMedicamentosAsync();
        var medicamentos = ProdutoInterPlayers.ListarProdutos(
            appSettingsValues.InterPlayersCustomProductNames,
            medicamentosIP.Data!.product!);

        var medicamento = medicamentos.First(x => x.ProdutoOriginal.ean == Request.Form[CadastreSeDadosCampos.medicamento]);

        var crm = CRM.Parse(Request.Form[CadastreSeDadosCampos.crm]!);
        var profissionais = await interPlayersService.ConsultarProfissionaisAsync(crm, uf);
        var profissional = profissionais.Data!.professional!.Single();

        var optInCelular = Request.Form[CadastreSeDadosCampos.comunicacaoCelular] == "on";
        var optInEmail = Request.Form[CadastreSeDadosCampos.comunicacaoEmail] == "on";

        var statusPaciente = await interPlayersService.ConsultarStatusConsumerAsync(cpf);
        var consumerId = statusPaciente.Data!.GetConsumerId();

        var statusLgpd = await interPlayersService.ConsultarStatusLgpdAsync(consumerId!);
        if (!statusLgpd.Success || InterPlayersGetLgpdStatusResult.IsTermoNaoEncontrado(statusLgpd.ErrorData))
        {
            var lgpdResult = await interPlayersService.AceitarStatusLgpdAsync(
                consumerId: consumerId!,
                ddd: TelefoneDDD.Parse("11"),
                telefone: Telefone.Parse("912341234"),
                optInCelular: true,
                optInEmail: true
            );
        }

        if (statusPaciente.Data.IsNaoExistente())
        {
            var result = await interPlayersService.CadastrarPacienteAsync
            (
                nome: nome!,
                dataNascimento: dataNascimento,
                cpf: cpf,
                email: email,
                ddd: ddd,
                telefone: telefone,
                uf: uf,
                genero: genero,
                product: medicamento.ProdutoOriginal,
                professional: profissional,
                optInCelular: optInCelular,
                optInEmail: optInEmail
            );

            if (result.Success || InterPlayersCadastrarPacienteResult.JaPossuiCadastro(result.ErrorData))
                return Ok();
            else
                return BadRequest(result.RawResult);
        }
        else
        {
            var result = await interPlayersService.AderirProdutoAsync
            (
                consumerId: consumerId!,
                dataNascimento: dataNascimento,
                produto: medicamento.ProdutoOriginal,
                profissional: profissional
            );

            if (result.Success)
                return Ok();
            else
                return BadRequest(result.RawResult);
        }
    }

    [HttpGet("confirmacao-envio")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult ConfirmacaoEnvio()
    {
        return View("Views/LandingPages/confirmacao-envio.cshtml");
    }

    [HttpGet("senha")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Senha()
    {
        return View("Views/LandingPages/senha.cshtml");
    }
}