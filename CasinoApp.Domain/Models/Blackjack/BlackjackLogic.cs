using System;
using System.Collections.Generic;
using System.Linq;

namespace CasinoApp.Domain.Models.Blackjack
{

    public class Hand
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        
        public int GetScore()
        {
            int score = 0;
            int aceCount = 0;

            foreach (var card in Cards)
            {
                if (!card.IsFaceUp) continue; 

                if (card.Rank == Rank.Ace)
                    aceCount++;
                else
                    score += card.Value;
            }
            
            for (int i = 0; i < aceCount; i++)
            {
                score += 11;
            }
            
            while (score > 21 && aceCount > 0)
            {
                score -= 10; 
                aceCount--;
            }

            return score;
        }
        
        public void AddCard(Card card) => Cards.Add(card);
        public bool IsBusted => GetScore() > 21;
        public bool IsBlackjack => Cards.Count == 2 && GetScore() == 21;
    }


    public class Deck
    {
        private List<Card> _cards = new List<Card>();
        private Random _random = new Random();

        public Deck()
        {

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    _cards.Add(new Card(suit, rank));
                }
            }
            Shuffle();
        }

        public void Shuffle()
        {
            int n = _cards.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                Card value = _cards[k];
                _cards[k] = _cards[n];
                _cards[n] = value;
            }
        }

        public Card DrawCard()
        {
            if (_cards.Count == 0) throw new Exception("Balíček je prázdný!");
            Card card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }
    }
}