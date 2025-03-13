using MerckCuida.InterPlayers.ValueObjects;
using MerckCuida.Web.Configuration;

namespace MerckCuida.Web.ValueObjects
{
    public class ProdutoInterPlayers
    {
        public string Nome { get; init; }
        public InterPlayersListProductResult.Product ProdutoOriginal { get; init; }

        public ProdutoInterPlayers(
            string nome, 
            InterPlayersListProductResult.Product produtoOriginal)
        {
            Nome = nome;
            ProdutoOriginal = produtoOriginal;
        }

        public static IEnumerable<ProdutoInterPlayers> ListarProdutos(
            IEnumerable<InterPlayersCustomProductName> produtosCustomizados,
            IEnumerable<InterPlayersListProductResult.Product> produtosInterPlayers)
        {
            foreach(var item in produtosInterPlayers)
            {
                var nomeCustomizado = produtosCustomizados.FirstOrDefault(x => x.ean == item.ean);
                var nome = nomeCustomizado?.name ?? item.productName ?? string.Empty;

                yield return new ProdutoInterPlayers
                (
                    nome: nome,
                    produtoOriginal: item
                );
            }
        }
    }
}