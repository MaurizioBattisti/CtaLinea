using System;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class ExportRunsForPatActivityRequest
        : BaseTaskRequest
    {
		public string FileName { get; set; }
		public int? ContractId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
