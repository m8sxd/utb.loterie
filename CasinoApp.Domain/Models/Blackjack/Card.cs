using System;

namespace CasinoApp.Domain.Models.Blackjack
{
    public class Card
    {
        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public bool IsFaceUp { get; set; } = true; 

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        
        public int Value
        {
            get
            {
                if (Rank >= Rank.Ten && Rank <= Rank.King) return 10; 
                if (Rank == Rank.Ace) return 11; 
                return (int)Rank; 
            }
        }

        public override string ToString()
        {
            if (!IsFaceUp) return "🂠"; 

            string suitSymbol = Suit switch
            {
                Suit.Hearts => "♥️",
                Suit.Diamonds => "♦️",
                Suit.Clubs => "♣️",
                Suit.Spades => "♠️",
                _ => ""
            };

            string rankSymbol = Rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                _ => ((int)Rank).ToString()
            };

            return $"{suitSymbol} {rankSymbol}";
        }
        

        public string ColorCssClass => (Suit == Suit.Hearts || Suit == Suit.Diamonds) ? "text-danger" : "text-dark";
    }
}