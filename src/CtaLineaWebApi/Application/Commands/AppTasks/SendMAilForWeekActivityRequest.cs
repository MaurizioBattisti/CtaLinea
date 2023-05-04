using System;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class SendMAilForWeekActivityRequest
        : BaseTaskRequest
    {
        public string? ForseDestination { get; set; }
        public Guid? AssociateId { get; set; }
        public DateTime? ReferenceDate { get; set; }

    }
}
