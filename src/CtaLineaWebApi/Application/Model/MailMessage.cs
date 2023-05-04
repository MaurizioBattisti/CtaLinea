using MimeKit;
using System.Collections.Generic;
using System.Linq;

namespace CtaLineaWebApi.Application.Model
{
    public class MailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public MailMessage(
            IEnumerable<string> to, 
            string subject, 
            string content)
        {
            To = new List<MailboxAddress>();
            foreach (var SingleTo in to)
            {
                To.Add(
                    MailboxAddress.Parse (SingleTo)
                    );
            }
            Subject = subject;
            Content = content;
        }
    }
}
