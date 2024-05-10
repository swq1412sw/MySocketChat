using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySocketClient
{
    public partial class AskSaveForm : Form
    {
        public AskSaveForm()
        {
            InitializeComponent();
            this.button1.Click += (o, e) => { Save_click(); };
            this.button2.Click += (o, e) => { Cancle_click(); };
        }

        private void Save_click() 
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancle_click()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
