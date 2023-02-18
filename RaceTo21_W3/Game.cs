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
        private bool cheating = true; // lets you cheat for testing purposes if true


        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            //deck.ShowAllCards();
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
                        player.setStatus(PlayerStatus.win);//hz call the function from the Player class
                    }
                    else 
                    {
                        int answer = 0;

                        bool checkinput = true;//hz Check the input valid or not
                        while(checkinput)//hz Using while loop to check user input, if input is not num keep asking until get valid input
                        {
                            Console.WriteLine("How many cards you want to draw 0 - 3, " + player.name);//hz Ask user how many cards that they want
                            
                            try//hz Using "try" to check if the input can be transfer to number or not
                            {
                                
                                answer = Convert.ToInt32(Console.ReadLine());//hz Read the input

                                if(answer >=0 && answer <4)//hz if input is in the range, take the input
                                {
                                    checkinput = false;
                                }
                                else
                                {
                                    Console.WriteLine("Out of range!"); //hz if input is out of range, keep asking
                                }
                            }
                            catch (Exception ex)//hz using catch to detect the error when user get invalid input and remind user get correct input
                            {
                                Console.WriteLine(player.name + " please input number");
                            }
                        };
                        


                            
                        if (answer != 0)
                        {
                        
                            for (int i = 0; i < answer; i++)//hz Add cards to player by using for loop
                            {
                                Card card = deck.DealTopCard();
                                player.cards.Add(card);
                            
                            }

                           player.setScore(ScoreHand(player));//hz call the function from the Player class

                            if (player.score > 21)
                            {
                                player.setStatus(PlayerStatus.bust);//hz
                                numberOfPlayers--;// hz decrease the total number of current players
                            }
                            else if (player.score == 21)
                            {
                                player.setStatus(PlayerStatus.win);//hz call the function from the Player class
                            }
                        }
                    
                        else
                        {
                            Console.WriteLine(player.name + " Stay.");//Show the player stay

                            player.setStatus(PlayerStatus.stay); //hz call the function from the Player class                           
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

                    players = remainCheck(players);//HZ check the number of remain player
                    currentPlayer = 0; //HZ reset the currentPlayer, keep the current players inside of the number of players range
                    numberOfPlayers = players.Count;// HZ set number of players equal current player list

                    for (int i = 0; i < players.Count - 1; i++)//HZ random the rest of players' slots
                    {
                        Player tmp = players[i];
                        int swapindex = rng.Next(players.Count);
                        players[i] = players[swapindex];
                        players[swapindex] = tmp;  
                    }

                    

                    if(players.Count > 0)//HZ if number of players greater than 0, start new round
                    {
                        deck = new Deck();//HZ 
                        deck.Shuffle();//HZ
                        nextTask = Task.PlayerTurn;//HZ
                    }   
                    else
                    {
                        nextTask = Task.GameOver;//HZ
                        Console.Write("Press <Enter> to exit... ");
                        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
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

        private List<Player> remainCheck(List<Player> players)//HZ Using for loop to ask each single player wants to play one more round or not
        {

            List<Player> newPlayers = new List<Player>();
            for(int i = 0; i < players.Count; i++ )
            {
                Console.WriteLine("Do " + players[i].name +" want to play again Y/N");
                string response = Console.ReadLine().ToUpper().Trim();
                if(response == "Y")
                {
                    players[i].setStatus(PlayerStatus.active);//hz call the function from the Player class
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
