using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Helpers
{
    public class CheckResultItem
    {
        public string Category { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public object? Id { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder(512);
            if (string.IsNullOrEmpty(this.Category) == false)
            {
                sb.Append(" Cat: " + this.Category);
            }
            if (string.IsNullOrEmpty(this.Title) == false)
            {
                sb.Append(" " + this.Title);
            }
            if (string.IsNullOrEmpty(this.Description) == false)
            {
                sb.Append(" " + this.Description);
            }

            return sb.ToString();
        }
    }
}
