namespace Customer.Service.Exceptions
{
    public class InvalidPasswordException: Exception
    {
        public InvalidPasswordException(string mesage) : base(mesage) { }
    }
}