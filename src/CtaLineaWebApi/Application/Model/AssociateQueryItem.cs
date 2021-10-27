using System;

namespace CtaLineaWebApi.Application.Model
{
    public class AssociateQueryItem
    {
        public Guid Id { get; set; }
        public  string AssociateDescription { get; set; }
        public int Category { get; set; }
    }

    public class Associate_2
    {
        public Guid Id { get; set; }
        public string AssociateDescription { get; set; }
        public int Category { get; set; }

        public string CategoryDescription { get; set; }
    }
}
