namespace OrdersHandler.Models
{
    public class UserModel
    {
        public int Id { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + ", " + LastName;
            }
        }
    }
}
