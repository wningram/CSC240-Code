using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace FlashCardMaker {
    class CardsFileReader {
        private string _fileLoc;

        public CardsFileReader(string fileLoc) {
            this._fileLoc = fileLoc;
            if (!isFileValid(this._fileLoc))
                throw new ArgumentException("The supplied file argument does not exist.");
        }

        public CardsFileReader() : this("myDeck.xml") { }

        /// <summary>
        /// Determines whether a file path leads to an existing file.
        /// </summary>
        /// <param name="fileLoc">The path of the file to verify.</param>
        /// <returns>True if the file is valid, otherwise False.</returns>
        private bool isFileValid(string fileLoc) {
            return File.Exists(fileLoc);
        }

        /// <summary>
        /// Reads card data from the XML formated file specified by the file location supplied to this class's constructor.
        /// </summary>
        /// <returns>A <see cref="CardsFile"/> object populated with the data from the file.</returns>
        public CardsFile ReadData() {
            CardsFile cFile = new CardsFile(this._fileLoc);
            List<CardData> cards = new List<CardData>();
            XmlDocument doc = new XmlDocument();

            doc.Load(this._fileLoc);
            // Get Deck Name and populate in cFile variable
            cFile.DeckName = doc.SelectSingleNode("/Cards").Attributes["DeckName"].Value;
            // Read each Card in the xml file and store in cards variable
            foreach (XmlNode node in doc.SelectNodes("/Cards/Card")) {
                CardData card = new CardData();
                card.Question = node.SelectSingleNode("Question").InnerText;
                card.Answer = node.SelectSingleNode("Answer").InnerText;
                cards.Add(card);
            }
            // Populate cards data in cFile variable
            cFile.Cards = cards.ToArray();

            return cFile;
        }
    }
}
