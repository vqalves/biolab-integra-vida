using BiolabIntegraVida.Domain.ValueObjects;
using BiolabIntegraVida.Web.ValueObjects;
using System.Web;

namespace BiolabIntegraVida.Web.ViewModels.LandingPages
{
    public class CadastreSeViewModel
    {
        public string CPF { get; set; }
        public IEnumerable<UF> UFs { get; set; }
        public IEnumerable<ProdutoInterPlayers> Produtos { get; set; }
        public string GoogleAnalyticsId { get; set; }

        public CadastreSeViewModel(
            string cpf,
            IEnumerable<UF> ufs,
            IEnumerable<ProdutoInterPlayers> produtos,
            string googleAnalyticsId)
        {
            this.CPF = cpf;
            this.UFs = ufs;
            this.Produtos = produtos;
            this.GoogleAnalyticsId = googleAnalyticsId;
        }

        public string ProdutosAsJavascriptArray()
        {
            var nomes = string.Join(',', Produtos.Select(x => $"'{HttpUtility.HtmlEncode(x.Nome)}'"));
            return $"[{nomes}]";
        }
    }
}