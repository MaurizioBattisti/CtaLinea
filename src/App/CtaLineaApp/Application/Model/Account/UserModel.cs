namespace CtaLineaApp.Application.Model.Account
{
    public class UserModel
    {
        public string Description { get; set; } = String.Empty;
        public string Username { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public IEnumerable<string> Roles { get; set; } = new string[] { };
        public Guid? AssociateId { get; set; }
        public bool MustChangePAssword { get; set; } = false;

        public string Token { get; set; } = String.Empty;
        public DateTime Expiration { get; set; }
    }
}
