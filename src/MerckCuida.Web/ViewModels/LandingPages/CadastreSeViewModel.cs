using MerckCuida.Domain.ValueObjects;
using MerckCuida.Web.ValueObjects;
using System.Web;

namespace MerckCuida.Web.ViewModels.LandingPages
{
    public class CadastreSeViewModel
    {
        public string CPF { get; set; }
        public IEnumerable<UF> UFs { get; set; }
        public IEnumerable<ProdutoInterPlayers> Produtos { get; set; }

        public CadastreSeViewModel(string cpf, IEnumerable<UF> ufs, IEnumerable<ProdutoInterPlayers> produtos)
        {
            this.CPF = cpf;
            this.UFs = ufs;
            this.Produtos = produtos;
        }

        public string ProdutosAsJavascriptArray()
        {
            var nomes = string.Join(',', Produtos.Select(x => $"'{HttpUtility.HtmlEncode(x.Nome)}'"));
            return $"[{nomes}]";
        }
    }
}