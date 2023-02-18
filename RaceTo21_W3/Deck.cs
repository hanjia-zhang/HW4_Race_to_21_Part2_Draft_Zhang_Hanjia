using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        List<Card> cards = new List<Card>();
        Dictionary<string, string> image = new Dictionary<string, string>();
        public Deck()
        {
            Console.WriteLine("*********** Building deck...");
            List <string> suits = new List<string>{ "Spades", "Hearts", "Clubs", "Diamonds" };//HZ put them into list

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string cardLongName;

                    switch (cardVal)
                    {
                        case 1:
                            cardName = "A";
                            cardLongName = "Ace";
                            break;
                        case 11:
                            cardName = "J";
                            cardLongName = "Jack";
                            break;
                        case 12:
                            cardName = "Q";
                            cardLongName = "Queen";
                            break;
                        case 13:
                            cardName = "K";
                            cardLongName = "King";
                            break;
                        default:
                            cardName = cardVal.ToString();
                            cardLongName = cardName;
                            break;              
                    }
                    Card card = new Card(cardName + cardSuit.First<char>(), cardLongName + " of " + cardSuit);
                    cards.Add(card);
                    string path = @"card_assets/card_" + cardSuit + "_" + cardName + ".png";//hz test ouput image   
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), path);//hz test ouput image


                    image.Add(card.id, filePath);
                }
            }
        }

        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i=0; i<cards.Count; i++)
            {
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;

                //cards[i] = cards[swapindex]; // makes a duplicate
                //cards[swapindex] = cards[i]; //doesn't change anything
            }
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */

        public void ShowAllCards()
        {
            for (int i=0; i<cards.Count; i++)
            {
                Console.Write(i+": "+cards[i].displayName); // a list property can look like an Array!
                if (i < cards.Count -1)
                {
                    Console.Write(" ");
                } else
                {
                    Console.WriteLine("");
                }
            }
        }

        public Card DealTopCard()
        {
            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            // Console.WriteLine("I'm giving you " + card);

          

            return card;
        }

        
    }
}

