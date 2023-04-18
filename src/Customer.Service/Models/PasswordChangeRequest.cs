namespace Customer.Service.Models
{
    public record PasswordChangeRequest
    {
        public string OldPassword { get; init; }
        public string NewPassword { get; init; }
    }
}
