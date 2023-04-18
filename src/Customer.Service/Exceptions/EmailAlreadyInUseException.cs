namespace Customer.Service.Exceptions
{
    public class EmailAlreadyInUseException: Exception
    {
        public EmailAlreadyInUseException(string mesage) : base(mesage) { }
    }
}
