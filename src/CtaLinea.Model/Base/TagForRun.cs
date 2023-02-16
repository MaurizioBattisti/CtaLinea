namespace CtaLinea.Model.Base
{
    public class TagForRun
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public int Ordinal { get; set; } = 1000;
        public string BgColor { get; set; } = "black";
        public string Color { get; set; } = "white";
    }
}
