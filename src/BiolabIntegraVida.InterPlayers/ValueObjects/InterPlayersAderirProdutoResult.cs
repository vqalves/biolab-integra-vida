namespace BiolabIntegraVida.InterPlayers.ValueObjects
{
    public class InterPlayersAderirProdutoResult
    {
        public List<Product>? product { get; set; }
        public object? dependent { get; set; }

        public class Brand
        {
            public string? id { get; set; }
        }

        public class Campaign
        {
            public string? campaignCode { get; set; }
            public string? nextPurchaseDiscount { get; set; }
            public string? nextPurchaseConditions { get; set; }
            public string? currentLimitBalance { get; set; }
            public string? totalLimitBalance { get; set; }
            public string? dateNextBalanceRelease { get; set; }
            public string? nextReleaseBalance { get; set; }
            public string? lastPurchaseDate { get; set; }
            public string? nextPurchaseDate { get; set; }
            public string? campaignEndDate { get; set; }
            public string? mainProduct { get; set; }
            public string? biggerDiscountByQuantity { get; set; }
        }

        public class Product
        {
            public string? ean { get; set; }
            public string? programId { get; set; }
            public string? programType { get; set; }
            public string? adhesionSource { get; set; }
            public string? adhesionDate { get; set; }
            public string? productName { get; set; }
            public Brand? brand { get; set; }
            public Professional? professional { get; set; }
            public Campaign? campaign { get; set; }
        }

        public class Professional
        {
            public string? professionalType { get; set; }
            public string? professionalId { get; set; }
            public string? professionalState { get; set; }
            public string? professionalName { get; set; }
        }
    }
}
