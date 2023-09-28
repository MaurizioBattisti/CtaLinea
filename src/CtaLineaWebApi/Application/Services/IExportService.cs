using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Services
{
    public interface IExportService
    {
        Task ExportToExcelASync<TEntity>(
            string filename, 
            string sheetTitle, 
            IEnumerable<TEntity> items) 
            where TEntity : class;
        Task ExportToExcelASync<TEntity>(
            string filename,
            IEnumerable<Tuple<string, IEnumerable<TEntity>>> multiSheet
            )
            where TEntity : class;
    }
}