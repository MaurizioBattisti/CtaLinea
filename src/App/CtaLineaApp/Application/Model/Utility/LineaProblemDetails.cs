using System.Text.Json;
using System.Text.Json.Serialization;

namespace CtaLineaApp.Application.Model.Utility
{
    public class LineaProblemDetails
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }

        public string? Detail { get; set; }

        public string? Instance { get; set; }
        public string? AdditionalProp1 { get; set; }
        public string? AdditionalProp2 { get; set; }
        public string? AdditionalProp3 { get; set; }

        [JsonPropertyName("errors")]
        public IDictionary<string , string []>? Errors { get; set; }
    }
}
