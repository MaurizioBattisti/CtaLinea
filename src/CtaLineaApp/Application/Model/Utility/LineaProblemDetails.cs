using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CtaLineaApp.Application.Model.Utility
{
    public class LineaProblemDetails
        : ProblemDetails
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "errors")]
        public IDictionary<string , string []>? Errors { get; set; }
    }
}
