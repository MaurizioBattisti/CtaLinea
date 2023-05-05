using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.ScheduledTasks
{
    public class SchedulerTaskLogItem
    {
        public int LogId { get; set; }
        public int TaskId { get; set; }
        public Guid Tag { get; set; }
        public string? Code { get; set; }
        public string? Message { get; set; } = null;
        public DateTime Time { get; set; }
    }
}
