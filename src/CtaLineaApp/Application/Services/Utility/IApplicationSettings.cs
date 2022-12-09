using System.Globalization;

namespace CtaLineaApp.Application.Services.Utility
{
    public interface IApplicationSettings
    {
        CultureInfo CurrentCulture { get; }
    }
}