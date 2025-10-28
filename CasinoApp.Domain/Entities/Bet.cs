namespace CasinoApp.Domain.Entities
{
    public class Bet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public decimal Amount { get; set; }
        public bool Won { get; set; }

        public User? User { get; set; }
        public Game? Game { get; set; }
    }
}