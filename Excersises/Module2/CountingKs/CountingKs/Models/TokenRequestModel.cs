namespace CountingKs.Models
{
    public class TokenRequestModel
    {
        // ApiKey: magic key the user was granted when they registered with you as a developer
        public string ApiKey { get; set; }
        public string Signature { get; set; } // signature the developer entered
    }
}