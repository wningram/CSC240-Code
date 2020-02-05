using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashCardMaker {

    public struct CardData {
        public string Question;
        public string Answer;
    }

    public partial class FlashCard : UserControl {
        private CardData _data;
        public FlashCard() {
            InitializeComponent();
        }

        public FlashCard(CardData data) : this() {
            Data = data;
        }

        /// <summary>
        /// The Question/Answer values that should be used to propulate this control in the form of a <see cref="CardData"/> value.
        /// </summary>
        public CardData Data { 
            get { return this._data; }
            set { this._data = value; RefreshUI(); }
        }

        /// <summary>
        /// If the answer label is visible, changes its visibility and vice versa.
        /// </summary>
        public void ToggleAnswer() {
            // Toggle visibility of answer label
            lblAnswer.Visible = !lblAnswer.Visible;
        }

        public void RefreshUI() {
            // Populate UI with Card Data
            lblQuestion.Text = Data.Question;
            lblAnswer.Text = Data.Answer;
            lblAnswer.Visible = false;
        }

        private void FlashCard_Load(object sender, EventArgs e) {
            RefreshUI();
        }
    }
}
