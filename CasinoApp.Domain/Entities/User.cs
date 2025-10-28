namespace CasinoApp.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public decimal Balance { get; set; }
    }
}