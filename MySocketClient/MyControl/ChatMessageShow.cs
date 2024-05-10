using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySocketClient.MyControl
{
    public partial class ChatMessageShow : UserControl
    {
        bool _isSelf;
        int _colorArg;
        public bool IsSelf { get => _isSelf; set 
            {
                _isSelf = value;
                if (value)
                {
                    panel1.Dock = DockStyle.Right;
                    label1.Dock = DockStyle.Right;
                    label2.Dock = DockStyle.Right;
                    label1.TextAlign = ContentAlignment.TopRight;
                    label2.TextAlign = ContentAlignment.TopRight;
                }
                else 
                {
                    panel1.Dock = DockStyle.Left;
                    label1.Dock = DockStyle.Left;
                    label2.Dock = DockStyle.Left;
                    label1.TextAlign = ContentAlignment.TopLeft;
                    label2.TextAlign = ContentAlignment.TopLeft;
                }
            }
        }
        public int ColorArg { get=>_colorArg; set 
            {
                _colorArg = value;
                Color item= Color.FromArgb(value);
                this.panel1.BackColor = item;
                this.splitContainer1.BackColor=item;
                this.splitContainer1.Panel1.BackColor = item;
                this.splitContainer1.Panel2.BackColor = item;
                this.label1.BackColor = item;
                this.label2.BackColor = item;
            }
        }

        public int PanleWigth { set { this.Width = value;panel1.Width = this.Width*3/5; label1.MaximumSize = new Size(this.panel1.Width - 5, 0);
                label2.MaximumSize = new Size(this.panel1.Width - 5, 0);
            } }
        public ChatMessageShow(bool isSelf, int colorArg,string text1,string text2)
        {
            InitializeComponent();
            label1.SizeChanged += (o, e) => { splitContainer1.SplitterDistance = label1.Height + 1; };
            label2.SizeChanged += (o, e) => { this.Height = label1.Height + label2.Height + 5; };
            IsSelf = isSelf;
            ColorArg = colorArg;
            label1.AutoSize = true;
            label2.AutoSize = true;
            label1.MaximumSize = new Size(splitContainer1.Panel1.Width - 5, 0);
            label2.MaximumSize = new Size(splitContainer1.Panel1.Width - 5, 0);
            label1.Text = text1;
            label2.Text = text2;
            this.Height = panel1.Height + 2;
            ColorArg = colorArg;
        }
    }
}
