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
    public partial class InputDialog : Form {
        private string _title, _prompt;

        public InputDialog(string title="Input Data", string prompt="Please input data.") {
            InitializeComponent();
            this._title = title;
            this._prompt = prompt;
        }

        public string ResultValue { get; protected set; }

        private void btnOK_Click(object sender, EventArgs e) {
            ResultValue = this.tbxInput.Text;
        }
    }
}
