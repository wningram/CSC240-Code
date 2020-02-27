using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCardMaker {
    public partial class Form1 : Form {
        protected string deckName;
        protected CardData[] cards;
        protected int currentIndx;

        public Form1() {
            InitializeComponent();
            this.cards = null;
            this.deckName = null;
            this.currentIndx = 0;
        }

        /// <summary>
        /// Refreshes the UI with the contents of the <see cref="cards"/> array.
        /// </summary>
        public void RefreshUI() {
            if (cards != null)
                flashCard1.Data = cards[currentIndx];
            btnSaveAs.Enabled = cards != null;
            // Set Card counter value
            lblCardCounter.Text = $"{currentIndx + 1} of {cards.Length}";
        }

        /// <summary>
        /// Clears the <see cref="cards"/> array and refreshes the UI by calling<see cref="RefreshUI"/>.
        /// </summary>
        public void ResetProject() {
            currentIndx = 0;
            cards = new CardData[] {
                new CardData {
                    Question = "Enter a prompt for this card.",
                    Answer = "Enter a answer for this card."
                }
            };
            RefreshUI();
        }

        private void btnAnswer_Click(object sender, EventArgs e) {
            flashCard1.ToggleAnswer();
        }

        private void btnLoad_Click(object sender, EventArgs e) {
            try {
                if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                    CardsFileReader reader = new CardsFileReader(openFileDialog1.FileName);
                    // Read cards data from the file
                    CardsFile cFile =  reader.ReadData();
                    // Populate cards data into UI
                    this.deckName = cFile.DeckName;
                    this.cards = cFile.Cards;
                }
            } catch (ArgumentException) {
                MessageBox.Show(
                    "The file you have selected is not valid.",
                    "Invalid File",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnNext_Click(object sender, EventArgs e) {
            // Increment card index if we're not on the last card
           if (this.currentIndx < this.cards.Length - 1) this.currentIndx++;
            RefreshUI();
        }

        private void btnPrev_Click(object sender, EventArgs e) {
            // Decrement card index if we're not on the first card
            if (this.currentIndx > 0) this.currentIndx--;
            RefreshUI();
        }

        private void btnSaveAs_Click(object sender, EventArgs e) {
            CardsFile cFile = null;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                cFile = new CardsFile(saveFileDialog1.FileName);
            }

            try {
                CardsFileWriter writer = new CardsFileWriter(cFile);
                writer.WriteData();
            } catch (ArgumentException) {
                MessageBox.Show("Deck data is corrupted or invalid, could not save.", 
                    "Invalid or Corrupted Project Data", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void btnNew_Click(object sender, EventArgs e) {
            ResetProject();
        }

        private void Form1_Load(object sender, EventArgs e) {
            ResetProject();
        }

        private void btnNewCard_Click(object sender, EventArgs e) {
            EditCardDialog newCardDialog = new EditCardDialog();
            if (newCardDialog.ShowDialog() == DialogResult.OK) {
                CardData result = newCardDialog.Card;
                if (!string.IsNullOrEmpty(result.Question)) {
                    cards = cards.Append(result).ToArray();
                } else {
                    MessageBox.Show("Cannot add an empty card.", "Card is Empty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                RefreshUI();
            }
        }
    }
}
