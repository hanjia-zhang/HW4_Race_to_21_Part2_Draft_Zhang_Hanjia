using System;
namespace RaceTo21
{
    public class Card
    {
        public string id;
        public string displayName;
        

        public Card(string shortCardName, string longCardName)
        {
            id = shortCardName;
            displayName = longCardName;
        }
    }
}
