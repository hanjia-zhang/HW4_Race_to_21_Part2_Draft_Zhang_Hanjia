using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name { get; set; }//hz
		public List<Card> cards { get; set; }//new list for player "hands"
		public PlayerStatus status { get; private set; }//hz apply data encapsulation principles
		public int score { get; private set; }//hz apply data encapsulation principles


		public void setScore(int score)//hz set function to set score
        {
			this.score = score;
        }

		public void setStatus(PlayerStatus status)//hz set function to set player status
		{
			this.status = status;
        }


		public Player(string n)
		{
			name = n;
			cards = new List<Card>();//Generate player cards
			status = PlayerStatus.active;
		}


		/* Introduces player by name
		 * Called by CardTable object
		 */
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum);
		}
	}
}

