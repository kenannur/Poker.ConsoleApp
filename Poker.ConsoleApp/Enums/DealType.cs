namespace Poker.ConsoleApp.Enums
{
    public enum DealType
    {
        /// <summary>
        /// Oyunculara 2'şer Kağıt Dağıt
        /// </summary>
        Player = 0,

        /// <summary>
        /// Yere ilk 3 Kağıdı Dağıt
        /// </summary>
        Flop = 1,

        /// <summary>
        /// Yere 4. Kağıdı Dağıt
        /// </summary>
        Turn = 2,

        /// <summary>
        /// Yere Son Kağıdı Dağıt
        /// </summary>
        River = 3,
    }
}
