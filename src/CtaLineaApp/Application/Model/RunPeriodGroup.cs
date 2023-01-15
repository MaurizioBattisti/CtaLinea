using CtaLinea.Model.Runs;
using System.Text;

namespace CtaLineaApp.Application.Model
{
    public class RunPeriodGroup
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}

        public IList<RunPeriod> SubPeriods { get; set; } = new List<RunPeriod>();

        public override string ToString()
        {
            var sb = new StringBuilder(256);
            sb.Append("Periodo:");
            if (this.StartDate != null)
            {
                sb.AppendFormat(" {0:dd/MM/yyyy}", this.StartDate);
            }
            else
            {
                sb.Append(" Inizio");
            }

            sb.Append(" - ");

            if (this.EndDate != null)
            {
                sb.AppendFormat("{0:dd/MM/yyyy}", this.EndDate);
            }
            else
            {
                sb.Append("Fine");
            }

            return sb.ToString();
        }
    }
}
