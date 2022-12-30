using CtaLinea.Model.Runs;
using System;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface IRunRepository
    {
        Task<RunItem> GetOneRunItemAsync(Guid runId);

        Task DeleteRunAsync(
            Guid runId);
        Task SaveRuAsync(
            RunItem runItem);
    }
}