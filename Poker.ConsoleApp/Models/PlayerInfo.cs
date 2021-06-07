using System.Collections.Generic;
using Poker.ConsoleApp.Enums;

namespace Poker.ConsoleApp.Models
{
    public class PlayerInfo
    {
        public List<Card> PlayerHandList;
        public int PlayerNumber;
        public HandResult HandResult;
        public List<Card> BestFiveList;
        public bool IsWinner;
    }
}
