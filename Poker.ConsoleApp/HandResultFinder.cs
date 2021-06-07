using System;
using System.Collections.Generic;
using System.Linq;
using Poker.ConsoleApp.Enums;
using Poker.ConsoleApp.Models;

namespace Poker.ConsoleApp
{
    public static class HandResultFinder
    {
        public static void CheckFlushRoyale(PlayerInfo playerInfo, List<Card> groundList)
        {
            List<Card> checkList = groundList.ToList();
            checkList.AddRange(playerInfo.PlayerHandList);
            var groupedList = checkList.GroupBy(card => card.Color);
            int groupCount = groupedList.Count();
            if (groupCount < 4)
            {
                for (int i = 0; i < groupCount; i++)
                {
                    List<Card> flushRoyaleList = groupedList.ElementAt(i).ToList().FindAll(card => card.Value >= 10 || card.Value == 1);
                    if (flushRoyaleList.Count == 5)
                    {
                        List<Card> bestFiveList = new List<Card>();
                        Card ace = flushRoyaleList.Find(card => card.Value == 1);
                        flushRoyaleList.Remove(ace);
                        bestFiveList.Add(ace);
                        bestFiveList.AddRange(flushRoyaleList.OrderByDescending(card => card.Value));
                        playerInfo.BestFiveList = bestFiveList;
                        playerInfo.HandResult = HandResult.FlushRoyale;
                    }
                }
            }
        }

        public static void CheckStraightRoyale(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                var groupedList = checkList.GroupBy(card => card.Color);
                int groupCount = groupedList.Count();
                if (groupCount < 4)
                {
                    for (int i = 0; i < groupCount; i++)
                    {
                        int firstPartValidCardCount = 0;
                        int secondPartValidCardCount = 0;
                        int thirdPartValidCardCount = 0;
                        List<Card> finalList = groupedList.ElementAt(i).OrderByDescending(card => card.Value).ToList();
                        if (finalList.Count >= 5)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                if (Math.Abs(finalList.ElementAt(j).Value - finalList.ElementAt(j + 1).Value) == 1)
                                {
                                    firstPartValidCardCount++;
                                }
                                if (finalList.Count == 6 || finalList.Count == 7)
                                {
                                    if (Math.Abs(finalList.ElementAt(j + 1).Value - finalList.ElementAt(j + 2).Value) == 1)
                                    {
                                        secondPartValidCardCount++;
                                    }
                                    if (finalList.Count == 7)
                                    {
                                        if (Math.Abs(finalList.ElementAt(j + 2).Value - finalList.ElementAt(j + 3).Value) == 1)
                                        {
                                            thirdPartValidCardCount++;
                                        }
                                    }
                                }
                            }
                            List<Card> bestFiveList = new List<Card>();
                            if (firstPartValidCardCount == 4)
                            {
                                bestFiveList = finalList.GetRange(0, 5);
                                playerInfo.BestFiveList = bestFiveList;
                                playerInfo.HandResult = HandResult.StraightRoyale;
                                break;
                            }
                            else if (secondPartValidCardCount == 4)
                            {
                                bestFiveList = finalList.GetRange(1, 5);
                                playerInfo.BestFiveList = bestFiveList;
                                playerInfo.HandResult = HandResult.StraightRoyale;
                                break;
                            }
                            else if (thirdPartValidCardCount == 4)
                            {
                                bestFiveList = finalList.GetRange(2, 5);
                                playerInfo.BestFiveList = bestFiveList;
                                playerInfo.HandResult = HandResult.StraightRoyale;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void CheckFourOfAKind(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                var groupedList = checkList.GroupBy(card => card.Value);
                int groupCount = groupedList.Count();
                if (groupCount < 5)
                {
                    for (int i = 0; i < groupCount; i++)
                    {
                        List<Card> fourOfAKindList = groupedList.ElementAt(i).ToList();
                        if (fourOfAKindList.Count == 4)
                        {
                            List<Card> bestFiveList = new List<Card>();
                            checkList.RemoveAll(card => card.Value == fourOfAKindList.First().Value);
                            Card ace = checkList.Find(card => card.Value == 1);
                            fourOfAKindList.Add(ace == null ? checkList.Find(card => card.Value == checkList.Max(maxCard => maxCard.Value)) : ace);
                            bestFiveList.AddRange(fourOfAKindList);
                            playerInfo.BestFiveList = bestFiveList;
                            playerInfo.HandResult = HandResult.FourOfAKind;
                            break;
                        }
                    }
                }
            }
        }

        public static void CheckFullHouse(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                var firstGroupedList = checkList.GroupBy(card => card.Value);
                int firstGroupCount = firstGroupedList.Count();
                if (firstGroupCount < 5)
                {
                    List<Card> bestTrioList = new List<Card>();
                    for (int i = 0; i < firstGroupCount; i++)
                    {
                        List<Card> trioList = firstGroupedList.ElementAt(i).ToList();
                        if (trioList.Count == 3)
                        {
                            if (!bestTrioList.Any() ||
                               trioList.First().Value == 1 ||
                               (bestTrioList.First().Value != 1 && trioList.First().Value > bestTrioList.First().Value))
                            {
                                bestTrioList.Clear();
                                bestTrioList = trioList;
                            }
                        }
                    }
                    if (bestTrioList.Any())
                    {
                        checkList.RemoveAll(card => card.Value == bestTrioList.First().Value);
                        var secondGroupedList = checkList.GroupBy(card => card.Value);
                        int secondGroupCount = secondGroupedList.Count();
                        if (secondGroupCount < 4)
                        {
                            List<Card> bestDuoList = new List<Card>();
                            for (int i = 0; i < secondGroupCount; i++)
                            {
                                List<Card> duoList = secondGroupedList.ElementAt(i).ToList();
                                if (duoList.Count >= 2)
                                {
                                    if (!bestDuoList.Any() ||
                                       duoList.First().Value == 1 ||
                                       (bestDuoList.First().Value != 1 && duoList.First().Value > bestDuoList.First().Value))
                                    {
                                        bestDuoList.Clear();
                                        bestDuoList = duoList;
                                    }
                                }
                            }
                            if (bestDuoList.Any())
                            {
                                List<Card> bestFiveList = new List<Card>();
                                bestFiveList.AddRange(bestTrioList.GetRange(0, 3));
                                bestFiveList.AddRange(bestDuoList.GetRange(0, 2));
                                playerInfo.BestFiveList = bestFiveList;
                                playerInfo.HandResult = HandResult.FullHouse;
                            }
                        }
                    }
                }
            }
        }

        public static void CheckFlush(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                var groupedList = checkList.GroupBy(card => card.Color);
                int groupCount = groupedList.Count();
                if (groupCount < 4)
                {
                    for (int i = 0; i < groupCount; i++)
                    {
                        List<Card> flushList = groupedList.ElementAt(i).ToList();
                        if (flushList.Count >= 5)
                        {
                            List<Card> bestFiveList = new List<Card>();
                            flushList = flushList.OrderByDescending(card => card.Value).ToList();
                            Card ace = flushList.Find(card => card.Value == 1);
                            if (ace == null)
                            {
                                bestFiveList.AddRange(flushList.GetRange(0, 5));
                            }
                            else
                            {
                                bestFiveList.Add(ace);
                                bestFiveList.AddRange(flushList.GetRange(0, 4));
                            }
                            playerInfo.BestFiveList = bestFiveList;
                            playerInfo.HandResult = HandResult.Flush;
                            break;
                        }
                    }
                }
            }
        }

        public static void CheckStraight(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                List<Card> distinctList = checkList.GroupBy(card => card.Value).Select(card => card.First()).ToList();
                List<Card> sortedList = new List<Card>();
                Card ace = distinctList.Find(card => card.Value == 1);
                if (ace == null)
                {
                    sortedList = distinctList.OrderByDescending(card => card.Value).ToList();
                }
                else
                {
                    sortedList.Add(ace);
                    sortedList.AddRange(distinctList.OrderByDescending(card => card.Value).ToList().GetRange(0, 4));
                }
                if (sortedList.Count >= 5)
                {
                    int firstPartValidCardCount = 0;
                    int secondPartValidCardCount = 0;
                    int thirdPartValidCardCount = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (Math.Abs(sortedList.ElementAt(i).Value - sortedList.ElementAt(i + 1).Value) == 1 ||
                            Math.Abs(sortedList.ElementAt(i).Value - sortedList.ElementAt(i + 1).Value) == 12)
                        {
                            firstPartValidCardCount++;
                        }
                        if (sortedList.Count == 6 || sortedList.Count == 7)
                        {
                            if (Math.Abs(sortedList.ElementAt(i + 1).Value - sortedList.ElementAt(i + 2).Value) == 1)
                            {
                                secondPartValidCardCount++;
                            }
                            if (sortedList.Count == 7)
                            {
                                if (Math.Abs(sortedList.ElementAt(i + 2).Value - sortedList.ElementAt(i + 3).Value) == 1)
                                {
                                    thirdPartValidCardCount++;
                                }
                            }
                        }
                    }
                    if (firstPartValidCardCount == 4)
                    {
                        List<Card> bestFiveList = new List<Card>();
                        bestFiveList = sortedList.GetRange(0, 5);
                        playerInfo.BestFiveList = bestFiveList;
                        playerInfo.HandResult = HandResult.Straight;
                    }
                    else if (secondPartValidCardCount == 4)
                    {
                        List<Card> bestFiveList = new List<Card>();
                        bestFiveList = sortedList.GetRange(1, 5);
                        playerInfo.BestFiveList = bestFiveList;
                        playerInfo.HandResult = HandResult.Straight;
                    }
                    else if (thirdPartValidCardCount == 4)
                    {
                        List<Card> bestFiveList = new List<Card>();
                        bestFiveList = sortedList.GetRange(2, 5);
                        playerInfo.BestFiveList = bestFiveList;
                        playerInfo.HandResult = HandResult.Straight;
                    }
                }
            }
        }

        public static void CheckThreeOfAKind(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                var groupedList = checkList.GroupBy(card => card.Value);
                int groupCount = groupedList.Count();
                if (groupCount < 6)
                {
                    for (int i = 0; i < groupCount; i++)
                    {
                        List<Card> trioList = groupedList.ElementAt(i).ToList();
                        if (trioList.Count == 3)
                        {
                            List<Card> bestFiveList = new List<Card>();
                            bestFiveList.AddRange(trioList);
                            checkList.RemoveAll(card => card.Value == trioList.First().Value);
                            checkList = checkList.OrderByDescending(card => card.Value).ToList();
                            Card ace = checkList.Find(card => card.Value == 1);
                            if (ace == null)
                            {
                                bestFiveList.AddRange(checkList.GetRange(0, 2));
                            }
                            else
                            {
                                bestFiveList.Add(ace);
                                bestFiveList.Add(checkList.First());
                            }
                            playerInfo.BestFiveList = bestFiveList;
                            playerInfo.HandResult = HandResult.ThreeOfAKind;
                            break;
                        }
                    }
                }
            }
        }

        public static void CheckTwoPairs(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                var firstGroupedList = checkList.GroupBy(card => card.Value);
                int firstGroupCount = firstGroupedList.Count();
                if (firstGroupCount < 6)
                {
                    List<Card> firstBestDuoList = new List<Card>();
                    for (int i = 0; i < firstGroupCount; i++)
                    {
                        List<Card> duoList = firstGroupedList.ElementAt(i).ToList();
                        if (duoList.Count == 2)
                        {
                            if (!firstBestDuoList.Any() ||
                                duoList.First().Value == 1 ||
                               (duoList.First().Value > firstBestDuoList.First().Value && firstBestDuoList.First().Value != 1))
                            {
                                firstBestDuoList.Clear();
                                firstBestDuoList.AddRange(duoList.GetRange(0, 2));
                            }
                        }
                    }
                    if (firstBestDuoList.Any())
                    {
                        checkList.RemoveAll(card => card.Value == firstBestDuoList.First().Value);
                        var secondGroupedList = checkList.GroupBy(card => card.Value);
                        int secondGroupCount = secondGroupedList.Count();
                        if (secondGroupCount < 5)
                        {
                            List<Card> secondBestDuoList = new List<Card>();
                            for (int i = 0; i < secondGroupCount; i++)
                            {
                                List<Card> secondDuoList = secondGroupedList.ElementAt(i).ToList();
                                if (secondDuoList.Count == 2)
                                {
                                    if (!secondBestDuoList.Any() ||
                                       secondDuoList.First().Value > secondBestDuoList.First().Value)
                                    {
                                        secondBestDuoList.Clear();
                                        secondBestDuoList.AddRange(secondDuoList.GetRange(0, 2));
                                    }
                                }
                            }
                            if (secondBestDuoList.Any())
                            {
                                List<Card> bestFiveList = new List<Card>();
                                checkList.RemoveAll(card => card.Value == secondBestDuoList.First().Value);
                                checkList = checkList.OrderByDescending(card => card.Value).ToList();
                                bestFiveList.AddRange(firstBestDuoList);
                                bestFiveList.AddRange(secondBestDuoList);
                                Card ace = checkList.Find(card => card.Value == 1);
                                if (ace == null)
                                {
                                    bestFiveList.Add(checkList.First());
                                }
                                else
                                {
                                    bestFiveList.Add(ace);
                                }
                                playerInfo.BestFiveList = bestFiveList;
                                playerInfo.HandResult = HandResult.TwoPairs;
                            }
                        }
                    }
                }
            }
        }

        public static void CheckOnePair(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                var groupedList = checkList.GroupBy(card => card.Value);
                int groupCount = groupedList.Count();
                if (groupCount < 7)
                {
                    for (int i = 0; i < groupCount; i++)
                    {
                        List<Card> duoList = groupedList.ElementAt(i).ToList();
                        if (duoList.Count == 2)
                        {
                            List<Card> bestFiveList = new List<Card>();
                            bestFiveList.AddRange(duoList);
                            checkList.RemoveAll(card => card.Value == duoList.First().Value);
                            checkList = checkList.OrderByDescending(card => card.Value).ToList();
                            Card ace = checkList.Find(card => card.Value == 1);
                            if (ace == null)
                            {
                                bestFiveList.AddRange(checkList.GetRange(0, 3));
                            }
                            else
                            {
                                bestFiveList.Add(ace);
                                bestFiveList.AddRange(checkList.GetRange(0, 2));
                            }
                            playerInfo.BestFiveList = bestFiveList;
                            playerInfo.HandResult = HandResult.OnePair;
                            break;
                        }
                    }
                }
            }
        }

        public static void CheckHighCard(PlayerInfo playerInfo, List<Card> groundList)
        {
            if (playerInfo.HandResult == HandResult.HighCard)
            {
                List<Card> bestFiveList = new List<Card>();
                List<Card> checkList = groundList.ToList();
                checkList.AddRange(playerInfo.PlayerHandList);
                checkList = checkList.OrderByDescending(card => card.Value).ToList();
                Card ace = checkList.Find(card => card.Value == 1);
                if (ace == null)
                {
                    bestFiveList.AddRange(checkList.GetRange(0, 5));
                }
                else
                {
                    bestFiveList.Add(ace);
                    bestFiveList.AddRange(checkList.GetRange(0, 4));
                }
                playerInfo.BestFiveList = bestFiveList;
                playerInfo.HandResult = HandResult.HighCard;
            }
        }
    }
}
