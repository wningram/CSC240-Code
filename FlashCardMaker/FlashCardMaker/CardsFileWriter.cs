using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace FlashCardMaker {
    class CardsFileWriter {
        private CardsFile _cFile;

        public CardsFileWriter( CardsFile cFile) {
            if (!isCardFileValid(cFile))
                throw new ArgumentException("CardsFile object is invalid.");
            else
                this._cFile = cFile;
        }

        /// <summary>
        /// Determines if a specified <see cref="CardsFile"/> is valid.
        /// </summary>
        /// <param name="cFile">The object to check.</param>
        /// <returns>True if the CardsFile object can be commited to a file, otherwise False.</returns>
        private bool isCardFileValid(CardsFile cFile) {
            bool isValid = true;
            // Check that CardsFile isn't null
            if (cFile == null)
                return false;
            // Check that FileLocation isn't null
            if (cFile.FileLocation == null || cFile.FileLocation.Length < 1)
                isValid = false;
            // Check that CardsFile has project name
            if (cFile.DeckName == null || cFile.DeckName.Length < 3)
                isValid = false;

            return isValid;
        }

        /// <summary>
        /// Commits the <see cref="CardsFile"/> object data that this object was instantiated with to a file in XML format.
        /// </summary>
        public void WriteData() {
            using (XmlWriter writer = XmlWriter.Create(this._cFile.FileLocation)) {
                writer.WriteStartElement("Cards");
                // Write deck name
                writer.WriteAttributeString("DeckName", this._cFile.DeckName);
                 // Write cards to file
                 foreach (CardData card in this._cFile.Cards) {
                    writer.WriteStartElement("Card");

                    writer.WriteStartElement("Question");
                    writer.WriteValue(card.Question);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Answer");
                    writer.WriteValue(card.Answer);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
