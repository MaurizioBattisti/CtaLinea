namespace CtaLineaApp.Application.Exceptions
{
    public class BadRequestException<TPayload>
        : ApplicationException
    {
        public TPayload? Payload { get; private set; }

        public BadRequestException() : base() { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
        public BadRequestException(string message, TPayload payload) : base(message)
        {
            this.Payload = payload;
        }
    }
}
