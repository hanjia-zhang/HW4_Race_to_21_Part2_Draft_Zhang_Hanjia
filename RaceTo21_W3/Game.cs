using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true

        int numberOfStay = 0;//hz the total number of players that not going to draw cards

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Task.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
            }
            else if (nextTask == Task.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    if (numberOfPlayers == 1)
                    {
                        player.status = PlayerStatus.win;
                    }
                    else if (cardTable.OfferACard(player))
                    {
                        Console.WriteLine("How many cards you want to draw 1,2,3");//hz Ask user how many cards that they want
                        int answer = Convert.ToInt32( Console.ReadLine());//hz Read the input

                        for (int i = 0; i < answer; i++)//hz Add cards to player by using for loop
                        {
                            Card card = deck.DealTopCard();
                            player.cards.Add(card);
                            
                        }

                       player.score = ScoreHand(player);

                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                            numberOfPlayers--;// hz decrease the total number of current players
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;

                        numberOfStay++; //HZ the total number of players who is stay increas 

                        if(numberOfStay == numberOfPlayers)//HZ if total number of stay players equal total number of players, then change the playerStatus to bust
                        {
                            for(int i =0; i < players.Count; i++)
                            {
                                players[i].status = PlayerStatus.bust;
                            }
                        }
                    }
                }
                
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)
            {
                if (!CheckActivePlayers())
                {
                    Player winner = DoFinalScoring();
                    cardTable.AnnounceWinner(winner);

                    for(int i=0; i < players.Count; i++) //hz set the winner to the last slot in next round
                    {
                        if(players[i] == winner)
                        {
                            Player tmp = players[players.Count - 1];
                            players[players.Count - 1] = players[i];
                            players[i] = tmp;
                        }
                    }
                    Random rng = new Random();//HZ declear random

                    players = remainCheck(players);//HZ 

                    for (int i = 0; i < players.Count - 1; i++)//HZ random the rest of players' slots
                    {
                        Player tmp = players[i];
                        int swapindex = rng.Next(players.Count);
                        players[i] = players[swapindex];
                        players[swapindex] = tmp;  
                    }

                    

                    if(players.Count > 0)//HZ
                    {
                        deck = new Deck();//HZ
                        deck.Shuffle();//HZ
                        nextTask = Task.PlayerTurn;//HZ
                    }
                    else
                    {
                        nextTask = Task.GameOver;//HZ
                    }
                    
                }
                else
                {
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Task.PlayerTurn;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        private List<Player> remainCheck(List<Player> players)//HZ
        {

            List<Player> newPlayers = new List<Player>();
            for(int i = 0; i < players.Count; i++ )
            {
                Console.WriteLine("Do " + players[i].name +" want to play again Y/N");
                string response = Console.ReadLine().ToUpper().Trim();
                if(response == "Y")
                {
                    players[i].status = PlayerStatus.active;
                    players[i].cards = new List<Card>();
                    
                    newPlayers.Add(players[i]);
                }
            }

            

            return newPlayers;
        }

        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.cards)
                {
                    string cardname = card.id;
                    string faceValue = cardname.Remove(cardname.Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        public bool CheckActivePlayers()
        {
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.active)
                {
                    return true; // at least one player is still going!
                }
            }
            return false; // everyone has stayed or busted, or someone won!
        }

        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // everyone must have busted because nobody won!
        }
    }
}
