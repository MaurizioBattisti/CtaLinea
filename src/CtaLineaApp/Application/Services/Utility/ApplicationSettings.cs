using System.Globalization;

namespace CtaLineaApp.Application.Services.Utility
{
    public class ApplicationSettings 
        : IApplicationSettings
    {
        public ApplicationSettings()
        {
            CurrentCulture = new CultureInfo("int-IT");
        }
        public CultureInfo CurrentCulture { get; private set; }
    }
}
