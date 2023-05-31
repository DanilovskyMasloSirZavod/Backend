using LinqToDB.Mapping;

namespace DMSZ_API.Data
{
    /// <summary>
    /// Таблица названий занимаемых позиций.
    /// </summary>
    [Table]
    public class Position
    {
        /// <summary>
        /// Id занимаемой позиции.
        /// </summary>
        [PrimaryKey, Identity]
        public Int16 Id { get; set; }

        /// <summary>
        /// Название занимаемой позиции.
        /// </summary>
        [Column, Nullable]
        public string PositionName { get; set; }
    }

    /// <summary>
    /// Таблица места работы.
    /// </summary>
    [Table]
    public class Place
    {
        /// <summary>
        /// Id места работы.
        /// </summary>
        [PrimaryKey, Identity]
        public Int32 Id { get; set; }

        /// <summary>
        /// Место работы.
        /// </summary>
        [Column, Nullable]
        public string WorkPlace { get; set; }
    }

    /// <summary>
    /// Таблица описания работы.
    /// </summary>
    [Table]
    public class Exact
    {
        /// <summary>
        /// Id описания работы.
        /// </summary>
        [PrimaryKey, Identity]
        public Int32 Id { get; set; }

        /// <summary>
        /// Описание работы или расширенное представление.
        /// </summary>
        [Column, Nullable]
        public string Clarification { get; set; }
    }

    /// <summary>
    /// Таблица информации должности.
    /// </summary>
    [Table]
    public class Job
    {
        /// <summary>
        /// Id работы.
        /// </summary>
        [PrimaryKey, Identity]
        public Guid Id { get; set; }

        #region Занимаемая позиция.

        /// <summary>
        /// Id занимаемой позиции.
        /// </summary>
        [Column]
        public Int16 PositionId { get; set; }
        [Nullable]
        private Position _Position;
        /// <summary>
        /// Занимаемая позиция.
        /// </summary>
        [Association(Storage = nameof(_Position), ThisKey = nameof(PositionId), OtherKey = nameof(Data.Position.Id))]
        public Position Position
        {
            get { return this._Position; }
            set { this._Position = value; }
        }

        #endregion

        #region Место работы.

        /// <summary>
        /// Id места работы.
        /// </summary>
        [Column]
        public Int32 PlaceId { get; set; }
        [Nullable]
        private Place _Place;
        /// <summary>
        /// Место работы.
        /// </summary>
        [Association(Storage = nameof(_Place), ThisKey = nameof(PlaceId), OtherKey = nameof(Data.Place.Id))]
        public Place Place
        {
            get { return this._Place; }
            set { this._Place = value; }
        }

        #endregion

        #region Описание работы.

        /// <summary>
        /// Id описания работы.
        /// </summary>
        [Column]
        public Int32 ExactId { get; set; }
        [Nullable]
        private Exact _Exact;
        /// <summary>
        /// Описание работы или расширенное представление.
        /// </summary>
        [Association(Storage = nameof(_Exact), ThisKey = nameof(ExactId), OtherKey = nameof(Data.Exact.Id))]
        public Exact Exact
        {
            get { return this._Exact; }
            set { this._Exact = value; }
        }

        #endregion

    }
}
