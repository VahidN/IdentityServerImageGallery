namespace DNT.IDP.Controllers.UserRegistration
{
    public class RegistrationInputModel
    {
        public string ReturnUrl { get; set; }
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }

        public bool IsProvisioningFromExternal => !string.IsNullOrWhiteSpace(Provider);
    }
}