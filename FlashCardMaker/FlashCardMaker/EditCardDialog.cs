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
    public partial class EditCardDialog : Form {
        public EditCardDialog() {
            InitializeComponent();
        }

        public EditCardDialog(CardData card) : this() {
            this.Card = card;
        }

        public CardData Card { get; protected set; }

        private void EditCardDialog_Load(object sender, EventArgs e) {
            if (Card.Question != null) {
                // Set edit window title
                this.Text = "Edit Card";
                // Load card info into UI
                this.rtbxQuestion.Text = Card.Question;
                this.rtbxAnswer.Text = Card.Answer;
            } else {
                // Set new card window title
                this.Text = "New Card";
            }
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Card = new CardData() {
                Question = this.rtbxQuestion.Text,
                Answer = this.rtbxAnswer.Text
            };
        }
    }
}
