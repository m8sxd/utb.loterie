using CasinoApp.Application.Interfaces;
using CasinoApp.Domain.Models.Blackjack;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.Json;

namespace CasinoApp.Application.Services
{
    public class TexasResult
    {
        public List<Card> PlayerHole { get; set; } = new();
        public List<Card> DealerHole { get; set; } = new();
        public List<Card> Community { get; set; } = new();
        public string Stage { get; set; }
        public string Message { get; set; }

        public string HandStrength { get; set; }

        public decimal Pot { get; set; }
        public bool IsWin { get; set; }
        public bool GameOver { get; set; }
        public decimal NewBalance { get; set; }
    }

    public class TexasHoldemService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly Random _random = new Random();

        public TexasHoldemService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<TexasResult> StartGameAsync(int userId, decimal stake, Microsoft.AspNetCore.Http.ISession session)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet == null || wallet.Balance < stake) throw new Exception("Nedostatek peněz.");

            wallet.Balance -= stake;
            await _walletRepository.UpdateAsync(wallet);

            var deck = CreateDeck();
            Shuffle(deck);

            var playerHole = deck.Take(2).ToList(); deck.RemoveRange(0, 2);
            var dealerHole = deck.Take(2).ToList(); deck.RemoveRange(0, 2);
            var community = deck.Take(5).ToList();

            var state = new GameState
            {
                Deck = deck,
                PlayerHole = playerHole,
                DealerHole = dealerHole,
                CommunityAll = community,
                CommunityVisible = new List<Card>(), 
                Stage = "PreFlop",
                Pot = stake,
                Stake = stake
            };

            SaveState(session, state);

            string strength = GetHandDescription(state.PlayerHole, state.CommunityVisible);

            return MapToResult(state, wallet.Balance, "Hra začala. Check nebo Bet?", strength);
        }

        public async Task<TexasResult> NextStepAsync(int userId, string action, Microsoft.AspNetCore.Http.ISession session)
        {
            var state = LoadState(session);
            if (state == null) throw new Exception("Hra vypršela.");

            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (action == "Bet")
            {
                if (wallet.Balance < state.Stake) throw new Exception("Na sázku nemáš dost peněz.");
                wallet.Balance -= state.Stake;
                state.Pot += state.Stake * 2;
                await _walletRepository.UpdateAsync(wallet);
            }
            if (state.Stage == "PreFlop")
            {
                state.Stage = "Flop";
                state.CommunityVisible.AddRange(state.CommunityAll.Take(3));
            }
            else if (state.Stage == "Flop")
            {
                state.Stage = "Turn";
                state.CommunityVisible.Add(state.CommunityAll[3]);
            }
            else if (state.Stage == "Turn")
            {
                state.Stage = "River";
                state.CommunityVisible.Add(state.CommunityAll[4]);
            }
            else if (state.Stage == "River")
            {
                state.Stage = "Showdown";

                int pScore = EvaluateStrength(state.PlayerHole, state.CommunityAll);
                int dScore = EvaluateStrength(state.DealerHole, state.CommunityAll);

                string pDesc = GetHandDescription(state.PlayerHole, state.CommunityAll);
                string dDesc = GetHandDescription(state.DealerHole, state.CommunityAll);

                string msg = "";
                decimal win = 0;

                if (pScore > dScore)
                {
                    win = state.Pot;
                    msg = $"VÝHRA! Máš {pDesc}, dealer má jen {dDesc}.";
                }
                else if (pScore < dScore)
                {
                    msg = $"PROHRA. Dealer má {dDesc}, ty máš jen {pDesc}.";
                }
                else
                {
                    win = state.Pot / 2;
                    msg = $"REMÍZA ({pDesc}). Vklad se vrací.";
                }

                if (win > 0)
                {
                    wallet.Balance += win;
                    await _walletRepository.UpdateAsync(wallet);
                }

                session.Remove("Texas_State");

                return new TexasResult
                {
                    PlayerHole = state.PlayerHole,
                    DealerHole = state.DealerHole,
                    Community = state.CommunityVisible,
                    Stage = "Showdown",
                    Message = msg,
                    HandStrength = pDesc, 
                    Pot = state.Pot,
                    IsWin = win > 0,
                    GameOver = true,
                    NewBalance = wallet.Balance
                };
            }

            SaveState(session, state);

            string currentStrength = GetHandDescription(state.PlayerHole, state.CommunityVisible);

            return MapToResult(state, wallet.Balance, $"{state.Stage}: Check nebo Bet?", currentStrength);
        }

        public void Fold(Microsoft.AspNetCore.Http.ISession session) => session.Remove("Texas_State");


        private class GameState
        {
            public List<Card> Deck { get; set; } = new();
            public List<Card> PlayerHole { get; set; } = new();
            public List<Card> DealerHole { get; set; } = new();
            public List<Card> CommunityAll { get; set; } = new();
            public List<Card> CommunityVisible { get; set; } = new();
            public string Stage { get; set; }
            public decimal Pot { get; set; }
            public decimal Stake { get; set; }
        }

        private void SaveState(Microsoft.AspNetCore.Http.ISession s, GameState state)
            => s.SetString("Texas_State", JsonSerializer.Serialize(state));

        private GameState LoadState(Microsoft.AspNetCore.Http.ISession s)
        {
            var json = s.GetString("Texas_State");
            return json == null ? null : JsonSerializer.Deserialize<GameState>(json);
        }

        private TexasResult MapToResult(GameState s, decimal bal, string msg, string strength)
        {
            return new TexasResult
            {
                PlayerHole = s.PlayerHole,
                // Skryjeme dealera: IsFaceUp = false
                DealerHole = new List<Card> {
                    new Card(Suit.Clubs, Rank.Two) { IsFaceUp = false },
                    new Card(Suit.Clubs, Rank.Two) { IsFaceUp = false }
                },
                Community = s.CommunityVisible,
                Stage = s.Stage,
                Message = msg,
                HandStrength = strength,
                Pot = s.Pot,
                GameOver = false,
                NewBalance = bal
            };
        }

        private List<Card> CreateDeck()
        {
            var deck = new List<Card>();
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
                foreach (Rank r in Enum.GetValues(typeof(Rank)))
                    deck.Add(new Card(s, r));
            return deck;
        }

        private void Shuffle(List<Card> deck)
        {
            int n = deck.Count;
            while (n > 1) { n--; int k = _random.Next(n + 1); (deck[k], deck[n]) = (deck[n], deck[k]); }
        }

        private string GetHandDescription(List<Card> hole, List<Card> community)
        {
            int score = EvaluateStrength(hole, community);

            if (score >= 800) return "Four of a Kind (Čtveřice)";
            if (score >= 700) return "Full House";
            if (score >= 600) return "Flush (Barva)";
            if (score >= 500) return "Straight (Postupka)";
            if (score >= 400) return "Three of a Kind (Trojice)";
            if (score >= 300) return "Two Pair (Dva páry)";
            if (score >= 200) return "Pair (Pár)";
            return "High Card (Vysoká karta)";
        }

        private int EvaluateStrength(List<Card> hole, List<Card> community)
        {
            var all = new List<Card>(hole);
            all.AddRange(community);
            if (!all.Any()) return 0;

            var groups = all.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key).ToList();
            var suits = all.GroupBy(c => c.Suit).OrderByDescending(g => g.Count()).FirstOrDefault();

            if (suits != null && suits.Count() >= 5) return 600 + (int)suits.OrderByDescending(c => c.Rank).First().Rank;
            if (groups[0].Count() == 4) return 800 + (int)groups[0].Key;
            if (groups[0].Count() == 3 && groups.Count > 1 && groups[1].Count() >= 2) return 700 + (int)groups[0].Key;

            var ranks = all.Select(c => (int)c.Rank).Distinct().OrderBy(r => r).ToList();
            for (int i = 0; i <= ranks.Count - 5; i++)
            {
                if (ranks[i + 4] - ranks[i] == 4) return 500 + ranks[i + 4];
            }

            if (groups[0].Count() == 3) return 400 + (int)groups[0].Key;
            if (groups[0].Count() == 2 && groups.Count > 1 && groups[1].Count() == 2) return 300 + (int)groups[0].Key;
            if (groups[0].Count() == 2) return 200 + (int)groups[0].Key;

            return 100 + (int)all.OrderByDescending(c => c.Rank).First().Rank;
        }
    }
}