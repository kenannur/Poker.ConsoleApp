using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Poker.ConsoleApp.Enums;
using Poker.ConsoleApp.Models;

namespace Poker.ConsoleApp
{
    class Program
    {
        private static readonly List<Card> deckList = new(52);
        private static readonly List<Card> groundList = new(5);
        private static readonly List<PlayerInfo> playerInfoList = new();
        private readonly Random random = new();
        private readonly Dictionary<int, int> dictionary = new()
        {
            {2, 2}, {3, 3}, {4, 4}, {5, 5}, {6, 6}, {7, 7}, {8, 8}, {9, 9}, {10, 10}, {11, 11}, {12, 12}, {13, 13}, {1, 14}, {14, 1}
        };

        static void Main(string[] args)
        {
            CreatePlayers(numberOfPlayers: 2);
            NewHand();
            CreateDeck();
        }

        private static void CreatePlayers(byte numberOfPlayers)
        {
            for (int i = 1; i <= numberOfPlayers; i++)
            {
                playerInfoList.Add(new PlayerInfo
                {
                    PlayerHandList = new List<Card>(),
                    BestFiveList = new List<Card>(),
                    PlayerNumber = i
                });
            }
        }

        private static void NewHand()
        {
            groundList.Clear();
            foreach (PlayerInfo playerInfo in playerInfoList)
            {
                playerInfo.BestFiveList.Clear();
                playerInfo.HandResult = HandResult.HighCard;
                playerInfo.PlayerHandList.Clear();
                playerInfo.IsWinner = false;
            }
        }

        private static void CreateDeck()
        {
            deckList.Clear();
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 13; j++)
                {
                    deckList.Add(new Card((Color)i, j));
                }
            }
        }

        private void Deal(DealType dealType)
        {
            switch (dealType)
            {
                case DealType.Player:
                    foreach (PlayerInfo playerInfo in playerInfoList)
                    {
                        for (int k = 1; k <= 2; k++)
                        {
                            DealOneCard(playerInfo.PlayerHandList);
                        }
                    }
                    break;
                case DealType.Flop:
                    for (int i = 1; i <= 3; i++)
                    {
                        DealOneCard(groundList);
                    }
                    break;
                case DealType.Turn:
                    DealOneCard(groundList);
                    break;
                case DealType.River:
                    DealOneCard(groundList);
                    break;
            }
        }

        private void DealOneCard(List<Card> cardOwnerList)
        {
            int randomNumber = random.Next(deckList.Count);
            Card randomCard = deckList.ElementAt(randomNumber);
            cardOwnerList.Add(randomCard);
            deckList.Remove(randomCard);
        }

        private static void FindHandResults()
        {
            foreach (PlayerInfo playerInfo in playerInfoList)
            {
                HandResultFinder.CheckFlushRoyale(playerInfo, groundList);
                HandResultFinder.CheckStraightRoyale(playerInfo, groundList);
                HandResultFinder.CheckFourOfAKind(playerInfo, groundList);
                HandResultFinder.CheckFullHouse(playerInfo, groundList);
                HandResultFinder.CheckFlush(playerInfo, groundList);
                HandResultFinder.CheckStraight(playerInfo, groundList);
                HandResultFinder.CheckThreeOfAKind(playerInfo, groundList);
                HandResultFinder.CheckTwoPairs(playerInfo, groundList);
                HandResultFinder.CheckOnePair(playerInfo, groundList);
                HandResultFinder.CheckHighCard(playerInfo, groundList);
            }
        }

        private void CheckBestHandResult()
        {
            List<PlayerInfo> sortedPlayerInfoList = playerInfoList.OrderByDescending(playerInfo => (int)playerInfo.HandResult).ToList();
            List<PlayerInfo> sameBestPlayerInfoList = sortedPlayerInfoList.FindAll(playerInfo => playerInfo.HandResult == sortedPlayerInfoList.First().HandResult);
            FindBestResult(0, sameBestPlayerInfoList);
        }

        private void FindBestResult(int cardIndex, List<PlayerInfo> bestPlayerInfoHandResults)
        {
            int maxValue = bestPlayerInfoHandResults.Max(playerInfo => dictionary[playerInfo.BestFiveList.ElementAt(cardIndex).Value]);
            List<PlayerInfo> playerInfoControlList = bestPlayerInfoHandResults.FindAll(playerInfo => playerInfo.BestFiveList.ElementAt(cardIndex).Value == dictionary[maxValue]);
            if (playerInfoControlList.Count == 1)
            {
                playerInfoControlList.First().IsWinner = true;
            }
            else
            {
                if (cardIndex == 4)
                {
                    playerInfoControlList.ForEach(playerInfo => playerInfo.IsWinner = true);
                }
                else
                {
                    cardIndex++;
                    FindBestResult(cardIndex, playerInfoControlList);
                }
            }
        }

        private static void ShowWinnerMessage()
        {
            var builder = new StringBuilder();
            List<PlayerInfo> winnerPlayerList = playerInfoList.FindAll(playerInfo => playerInfo.IsWinner == true);
            int winnerPlayerCount = winnerPlayerList.Count;
            builder.Append(winnerPlayerCount == 1 ? "Kazanan Oyuncu: " : "Kazanan Oyuncular: ");
            int number = 1;
            winnerPlayerList.ForEach(player =>
            {
                builder.Append(player.PlayerNumber);
                builder.Append(number < winnerPlayerCount ? " - " : string.Empty);
                number++;
            });
            builder.Append(Environment.NewLine);
            builder.Append(winnerPlayerList.First().HandResult.ToString());

            Console.WriteLine(builder.ToString());
        }

    }
}
