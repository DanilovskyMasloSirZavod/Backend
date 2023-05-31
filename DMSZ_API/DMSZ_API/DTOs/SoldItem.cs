using DMSZ_API.Data;
using DMSZ_API.Data.Sales;
using System.Diagnostics.CodeAnalysis;

namespace DMSZ_API.DTOs
{
    /// <summary>
    /// Проданный товар.
    /// </summary>
    public class SoldItem
    {
        public SoldItem(SellingProduct soldProduct)
        {
            Product = soldProduct.Product;
            Price = soldProduct.Cost;
            Weight = soldProduct.Weight;
        }

        public Product Product { get; set; }
        public decimal Price { get; set; }

        public float Weight { get; set; }
    }

    public class SoldItemComparer : IEqualityComparer<SoldItem>
    {
        public bool Equals(SoldItem? x, SoldItem? y)
        {
            return x.Product.Id.Equals(y.Product.Id);
        }

        public int GetHashCode([DisallowNull] SoldItem obj)
        {
            return obj.Product.Id.GetHashCode();
        }
    }
}
