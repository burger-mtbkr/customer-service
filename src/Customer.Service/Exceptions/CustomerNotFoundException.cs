namespace Customer.Service.Exceptions
{
    public class CustomerNotFoundException: Exception
    {
        public CustomerNotFoundException(string mesage) : base(mesage) { }
    }
}
