using LinqToDB.Mapping;

namespace DMSZ_API.Data.Sales
{
    /// <summary>
    /// Продаваемый товар.
    /// </summary>
    [Table]
    public class SellingProduct
    {
        [PrimaryKey, Identity, Column]
        public Guid ID { get; set; }

        /// <summary>
        /// Цена за 1кг товара!
        /// </summary>
        [Column]
        public decimal Cost { get; set; }

        /// <summary>
        /// Вес товара в кг!
        /// </summary>
        [Column]
        public float Weight { get; set; }

        #region Проданный продукт.

        /// <summary>
        /// Id продукта.
        /// </summary>
        [Column]
        public Guid SoldItemId { get; set; }
        [Nullable]
        private Product _Product;
        /// <summary>
        /// Проданный продукт.
        /// </summary>
        [Association(Storage = nameof(_Product), ThisKey = nameof(SoldItemId), OtherKey = nameof(Data.Product.Id))]
        public Product Product
        {
            get { return this._Product; }
            set { this._Product = value; }
        }

        #endregion

        [Column]
        public Guid CartId { get; set; }
    }
}
