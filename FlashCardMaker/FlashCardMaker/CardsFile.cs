using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCardMaker {
    class CardsFile {

        public CardsFile(string fileLoc) {
            FileLocation = fileLoc;
        }

        public CardsFile() : this("myDeck.xml") { }

        public string DeckName { get; set; }
        public CardData[] Cards { get; set; }
        public string FileLocation { get; protected set; }
    }
}
