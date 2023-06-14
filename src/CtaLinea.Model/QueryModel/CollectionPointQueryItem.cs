using CtaLinea.Model.Attributes;
using System.Reflection.PortableExecutable;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("cp")]
    public class CollectionPointQueryItem
    {
        [ColumnDescription(Header = "Cod. Nodo")]
        [SqlField("CollectionPointId", SortPosition = 0, FullText = true)]
        public string Id { get; set; } = string.Empty;

        [ColumnDescription(Header = "Descrizione")]
        [SqlField(FullText = true)]
        public string? Description { get; set; }
        [ColumnDescription(Header = "Indirizzo")]
        [SqlField(FullText = true)]
        public string? Address { get; set; }
        [ColumnDescription(Header = "Comune")]
        [SqlField(FullText = true)]
        public string? City { get; set; }
        [ColumnDescription(Header = "C.A.P.")]
        [SqlField(FullText = true)]
        public string? ZipCode { get; set; }

        [ColumnDescription(Header = "Latitudine")]
        public decimal? Latitude { get; set; }
        [ColumnDescription(Header = "Longitudine")]
        public decimal? Longitude { get; set; }

        [ColumnDescription(Header = "Tuoi punto")]
        [SqlField(FullText = true)]
        public string? CollectionPointType { get; set; }

        [ColumnDescription(Header = "nr nodi")]
        public int? NodeCount { get; set; }
    }
}
