namespace Customer.Service.Models
{
    public record UserModel: BaseModel
    {
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Password { get; set; }
        public string Salt { get; init; }

        public UserResponse Response => new()
        {
            Id = Id,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            CreatedDate = CreatedDate
        };
    }

    public record UserResponse: BaseModel
    {
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
