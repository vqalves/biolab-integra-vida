namespace MerckCuida.InterPlayers.ValueObjects
{
    public class InterPlayersListProductResult
    {
        public IEnumerable<Product>? product { get; set; }
        public int? totalItemCount { get; set; }
        public int? page { get; set; }
        public int? pageCount { get; set; }
        public int? pageCurrent { get; set; }


        public class Product
        {
            public string? ean { get; set; }
            public string? productName { get; set; }
            public string? programId { get; set; }
            public string? programType { get; set; }
            public ProductBrand? brand { get; set; }
        }

        public class ProductBrand
        {
            public string? brandId { get; set; }
            public string? brandName { get; set; }
        }
    }
}