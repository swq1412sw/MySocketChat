using MySocket;
using MySocketClient.MyControl;
using System.Text.Json;
using System.Windows.Forms;

namespace MySocketClient
{
    public partial class ChatForm : Form
    {
        public int SelfColorArpg { get; set; }
        public int RefColorArpg { get; set; } 
        public User UserInfo { get; set; }
        public User UserSelf { get; set; }
        public List<MesData> Mes { get; set; }

        public SocketClient socketClient { get; set; }
        
        private delegate void MyDelegateType();
        MyDelegateType Itemchange = new(() => { });
        MyDelegateType flowLayoutPanel1Size= new(() => { });
        //private FormWindowState previousWindowState = FormWindowState.Normal;
        public ChatForm(User user,User userself, List<MesData> data, SocketClient socketClient,int SelfColorArpg,int RefColorArpg)
        {
            this.SelfColorArpg = SelfColorArpg;
            this.RefColorArpg = RefColorArpg;
            this.UserInfo = user;
            this.UserSelf = userself;
            Mes = data;
            this.socketClient = socketClient;
            InitializeComponent();
            this.Text = GetText();
            
            
            this.Load+=(o,e)=>flowLayoutPanel1.BeginInvoke(() => 
            {
                foreach (MesData item in Mes)
                {
                    AddMesMethod(item,false);
                }
                flowLayoutPanel1.AutoScrollPosition = new Point(0, flowLayoutPanel1.DisplayRectangle.Height);
            });
            button1.Click += (o, e) => { SenderMesMethod(); };
            flowLayoutPanel1.AutoScroll = true; 
            flowLayoutPanel1Size += () =>flowLayoutPanel1.Invoke(()=>
            {
                var percentage = (float)flowLayoutPanel1.VerticalScroll.Value / flowLayoutPanel1.VerticalScroll.Maximum;
                flowLayoutPanel1.AutoScroll = false;
                flowLayoutPanel1.AutoScroll = true;
                flowLayoutPanel1.VerticalScroll.Value = (int)(percentage * flowLayoutPanel1.VerticalScroll.Maximum);
                flowLayoutPanel1.PerformLayout();
            //    //previousWindowState = this.WindowState;

            });
            this.SizeChanged += (o, e) => test();
            this.SizeChanged += (o, e) => test2();
            

        }
        private void test() 
        {
            Itemchange?.Invoke();
        }

        private void test2()
        {
            splitContainer1.SplitterDistance = splitContainer1.Height - 150;
            flowLayoutPanel1Size?.Invoke();
        }

        private string GetText() 
        {
            return $"和用户 {UserInfo.UserName} 的聊天界面,  {UserInfo.UserIp ?? string.Empty}  {UserInfo.UserPort}";
        }

        public void UserInfoChange(string username) 
        {
            UserInfo.UserName= username;
            this.Text= GetText();
        }

        public void StopChat(string textconet) 
        {
            if (textconet.StartsWith("发送失败")) 
            {
                if (flowLayoutPanel1.Controls.Count > 0) 
                {
                    flowLayoutPanel1.Controls.RemoveAt(flowLayoutPanel1.Controls.Count - 1);
                }   
            }
            MessageBox.Show(textconet);
            button1.Enabled = false;
        }
        public void AddMesMethod(MesData data,bool flagAdd=true) 
        {
            int ItemColor = data.IsSelf ? SelfColorArpg : RefColorArpg;
            ChatMessageShow itemControl = new (data.IsSelf,ItemColor, data.DateTimeText, data.Mes);
            itemControl.PanleWigth = this.splitContainer1.Panel1.Width - SystemInformation.VerticalScrollBarWidth-20;
            this.Itemchange += () =>flowLayoutPanel1.Invoke(()=>
            {
                itemControl.Invoke((MethodInvoker)delegate
                {
                    itemControl.PanleWigth = this.flowLayoutPanel1.Width - SystemInformation.VerticalScrollBarWidth-20;
                });
            });
            this.flowLayoutPanel1.Controls.Add(itemControl);
            if(flagAdd) Mes.Add(data);
        }

        public void SenderMesMethod() 
        {
            if (string.IsNullOrEmpty(textBox1.Text)) return;
            if (textBox1.ReadOnly) return;
            textBox1.ReadOnly = true;
            var dataMes = new MesData(textBox1.Text, DateTime.Now.ToString("F"), true);
            var sendData = JsonSerializer.Serialize(new SocketMesData<string>(SocketMesType.NormolMes, UserSelf, UserInfo, textBox1.Text, dataMes.DateTimeText));
            socketClient.SendMes(mes: sendData,
               successAction: sh => 
               { 
                   this.Invoke(() =>
                   { 
                       textBox1.ReadOnly = false;
                       AddMesMethod(dataMes);

                   }); 
               },
               failAction: (sh, e) => { this.Invoke(() => { MessageBox.Show("服务器异常"); }); }
                );
            textBox1.Text = string.Empty;
            flowLayoutPanel1.AutoScrollPosition = new Point(0, flowLayoutPanel1.DisplayRectangle.Height);
        }
    }
}
