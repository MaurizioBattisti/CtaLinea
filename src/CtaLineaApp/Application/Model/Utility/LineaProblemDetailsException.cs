namespace CtaLineaApp.Application.Model.Utility
{
    public class LineaProblemDetailsException
        : ApplicationException
    {
        public LineaProblemDetails? Details { get; private set; }

        public LineaProblemDetailsException () : base () { }
        public LineaProblemDetailsException (string message) : base (message) { }  
        public LineaProblemDetailsException (string message, Exception innerException) : base (message, innerException) { }
        public LineaProblemDetailsException(string message, LineaProblemDetails details) : base(message) 
        {
            this.Details = details;
        }
    }
}
