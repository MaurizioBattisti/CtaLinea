using System.Collections.Generic;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface ITtServiceRepository
    {
        Task<bool> CanImportAsync(
            string importDescr);
        Task<ImportContext> StartImportAsync(
            string importDescr, 
            string user);

        Task ImportCopleteAsync(
            ImportContext context);
        Task ImportAbortedAsync(
            ImportContext context);
        Task ImportFailedAsync(
            ImportContext context);

        Task ReportProgressAsync(
            ImportContext context);

        Task ImportServiceASync(
            ImportContext context, TtService service, IEnumerable<TtServiceNode> nodes);
        Task FinalizeImportServiceASync(
           ImportContext context);
    }
}