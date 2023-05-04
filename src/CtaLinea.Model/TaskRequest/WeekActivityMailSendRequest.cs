using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.TaskRequest
{
    public class WeekActivityMailSendRequest
    {
        public string? ForseDestination { get; set; }
        public Guid? AssociateId { get; set; }
        public DateTime? ReferenceDate { get; set; }
    }
}
