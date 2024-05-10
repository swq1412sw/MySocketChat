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
    public partial class SettingForm : Form
    {
        public ClientConfig UpdateClientConfig { get; set; }
        public ClientConfig OldClientConfig { get; set; }
        public SettingForm(ClientConfig c)
        {
            OldClientConfig = c;
            UpdateClientConfig = new(c.Ip,c.Port,c.Type,c.UserName,c.SendColorName,c.RecColorName);
            InitializeComponent();
            this.Load += (o, e) =>
            {
                label7.BeginInvoke(() => { label7.Hide(); });
                label8.BeginInvoke(() => { label8.Hide(); });
            };
            
            List<ShowInfo> types = new () { new("本地", 0), new("局域网", 1), new("固定IP", 2) };
            comboBox1.DataSource = types;
            comboBox1.DisplayMember = "Title";
            comboBox1.ValueMember="Type";
            comboBox1.SelectedValue = 0;
            textBox2.ReadOnly = true;
            comboBox1.SelectedValueChanged += (o, e) =>textBox2.ReadOnly = ((comboBox1.SelectedValue as int?) ?? -1) != 0;
            comboBox1.DataBindings.Add("SelectedValue", UpdateClientConfig, "Type", true, DataSourceUpdateMode.OnPropertyChanged);
            textBox1.DataBindings.Add("Text", UpdateClientConfig, "UserName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBox2.DataBindings.Add("Text", UpdateClientConfig, "Ip", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDown1.DataBindings.Add("Value", UpdateClientConfig, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
            label7.TextChanged += (o, e) => { if(int.TryParse(label7.Text, out int item))  button1.BackColor = Color.FromArgb(item); };
            label8.TextChanged += (o, e) => { if (int.TryParse(label8.Text, out int item))  button2.BackColor = Color.FromArgb(item); };
            label7.DataBindings.Add("Text", UpdateClientConfig, "SendColorName", true, DataSourceUpdateMode.OnPropertyChanged);
            label8.DataBindings.Add("Text", UpdateClientConfig, "RecColorName", true, DataSourceUpdateMode.OnPropertyChanged);
            button1.Click += new EventHandler(SelectColor_Click);
            button2.Click += new EventHandler(SelectColor_Click);
            button3.Click += (o, e)=>SaveSetting();
            button4.Click += (o, e) => CancleSelect();
        }

        private void SelectColor_Click(object? sender, EventArgs e) 
        {
            var item = sender as Button;
            if (item is not null) 
            {
                ColorDialog colorSelectItem = new ColorDialog();
                colorSelectItem.Color = Color.FromArgb(item.TabIndex);
                if (colorSelectItem.ShowDialog() == DialogResult.OK) 
                {

                    item.Text = colorSelectItem.Color.Name;
                    if (item.Name.EndsWith("1"))
                    {
                        label7.Text = colorSelectItem.Color.ToArgb().ToString();
                    }
                    else 
                    {
                        label8.Text = colorSelectItem.Color.ToArgb().ToString();
                    }
                }
            }
        }

        private void SaveSetting() 
        {
            if (!UpdateClientConfig.CheckPortAndIp()) 
            {
                MessageBox.Show("端口和IP不合理");
                return;
            }
            if (UpdateClientConfig.UserName.Length < 2 || UpdateClientConfig.UserName.Length > 6) 
            {
                MessageBox.Show("名字长度在2到6");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancleSelect() 
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }

    internal class ShowInfo 
    {
        public string Title { get; set; }
        public int Type { get; set; }

        public ShowInfo(string title, int type)
        {
            Title = title;
            Type = type;
        }
    }
}
