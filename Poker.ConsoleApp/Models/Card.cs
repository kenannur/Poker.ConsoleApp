using Poker.ConsoleApp.Enums;

namespace Poker.ConsoleApp.Models
{
    public class Card
    {
        public Color Color;
        public int Value;

        public Card(Color color, int value)
        {
            Color = color;
            Value = value;
        }
    }
}
