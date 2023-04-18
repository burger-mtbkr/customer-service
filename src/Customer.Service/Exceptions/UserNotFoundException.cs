namespace Customer.Service.Exceptions
{
    public class UserNotFoundException: Exception
    {
        public UserNotFoundException(string mesage) : base(mesage) { }
    }
}
