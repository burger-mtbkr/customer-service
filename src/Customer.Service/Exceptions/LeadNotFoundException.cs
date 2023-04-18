namespace Customer.Service.Exceptions
{
    public class LeadNotFoundException: Exception
    {
        public LeadNotFoundException(string mesage) : base(mesage) { }
    }
}
