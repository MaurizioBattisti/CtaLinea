namespace CtaLineaWebApi.Auth.Model
{
    public class ChangePasswordResult
    {
        public ChangePasswordResult (
            bool changed,
            string? message = null)
        {
            this.Changed = changed;
            if (changed == true) message = null;
            this.Message = message;
        }

        public bool Changed { get; private set; }
        public string? Message { get; private set; }
    }
}
