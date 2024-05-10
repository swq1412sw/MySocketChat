using MySocket;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Collections.Concurrent;

namespace MySocketClient
{
    public partial class MainForm : Form
    {
        SocketClient client;
        static List<ChatForm> forms = new();
        static ConcurrentDictionary<(string IP,int Port), List<MesData>> chats = new();
        User userSelf;
        ClientConfig clientConfig;
        bool Ischanged = false;
        public MainForm(ClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
            userSelf = new User();
            userSelf.UserName = clientConfig.UserName;
            InitializeComponent();
            label1.Text = string.Empty;
            label2.Text = string.Empty;
            label3.Text = string.Empty;
            listView1.OwnerDraw = true;
            listView1.DrawItem += new DrawListViewItemEventHandler((sender, e) =>
            {
                string text;
                if (e.Item.SubItems.Count == 3)
                {
                    text = e.Item.Text + "\r\n" + $"{e.Item.SubItems[1].Text}  {e.Item.SubItems[2].Text}";
                }
                else 
                {
                    text = e.Item.Text;
                }
                // 获取当前项的文本
                
                TextFormatFlags flags = TextFormatFlags.Left;
                // 计算文本的大小
                Size textSize = TextRenderer.MeasureText(e.Graphics, text, listView1.Font, new Size(listView1.Width, 0), flags);

                Rectangle textBackgroundBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, textSize.Height); // 绘制文本

                using (var brush = new SolidBrush(e.Item.BackColor))
                {
                    e.Graphics.FillRectangle(brush, textBackgroundBounds);
                }
                TextRenderer.DrawText(e.Graphics, text, listView1.Font, textBackgroundBounds, SystemColors.WindowText, flags);
            });
            client = new SocketClient(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), new IPEndPoint(IPAddress.Parse("198.18.0.1"), 5534));
            StartConnect();
            button1.Click += (o, e) => { ChangeNanme(); };
            listView1.DoubleClick += (o, e) => { listView1_DoubleClick(); };
            this.FormClosing += (o, e) => { BeforSocketFormClose(); };
            this.FormClosing += (o, e) =>
            {
                BeforSave();
            };
            button1.Text = "设置";
            this.Load += (o, e) => listView1.BeginInvoke(() => listView1.TileSize = new Size(this.listView1.Width - 1, listView1.TileSize.Height));
            this.Resize += (o, e) =>
            {
                listView1.BeginInvoke((MethodInvoker)delegate
                {
                    listView1.TileSize = new Size(this.listView1.Width - SystemInformation.VerticalScrollBarWidth - 4, listView1.TileSize.Height);
                });
            };    
        }
        private void BeforSocketFormClose() 
        {
            if (client.State) { try { client.CloseConnect(JsonSerializer.Serialize(new SocketMesData<string>(SocketMesType.CloseMes, userSelf, new(), string.Empty))); } catch (Exception ee) { Console.WriteLine(ee.Message); } }
        }

        private void BeforSave() 
        {
            if (this.Ischanged)
            {
                if (new AskSaveForm().ShowDialog() == DialogResult.OK)
                {
                    try { File.WriteAllText(Program.GetFilePath(), JsonSerializer.Serialize(clientConfig)); } catch { }
                }
            }
        }

        private void StartConnect() 
        {
            IPEndPoint iPEndPointConfig;
            switch (clientConfig.Type)
            {
                case 0:
                    iPEndPointConfig = new(IPAddress.Loopback, clientConfig.Port);
                    break;
                case 1:
                    {
                        string hostName = Dns.GetHostName();
                        IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                        IPAddress? localIP = ipEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                        if (localIP is null)
                        {
                            Console.WriteLine("为找到局域网，本地接收");
                            iPEndPointConfig = new(IPAddress.Parse(clientConfig.Ip), clientConfig.Port);
                        }
                        else
                        {
                            Console.WriteLine("Local IPv4 Address: " + localIP.ToString());
                            iPEndPointConfig = new(localIP, clientConfig.Port);
                        }
                    }
                    break;
                case 2:
                    iPEndPointConfig = new(IPAddress.Parse(clientConfig.Ip), clientConfig.Port);
                    break;
                default:
                    iPEndPointConfig = new(IPAddress.Loopback, clientConfig.Port);
                    break;
            }
            client = new SocketClient(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), iPEndPointConfig);
            bool connectFlag = true;
            try
            {
                client.Connect(
                    actionRec: (s, sh) =>
                    {
                        var mesData = JsonSerializer.Deserialize<SocketMesData<string>>(s);
                        if (mesData is not null)
                        {
                            if (mesData.MesType == SocketMesType.CloseMes)
                            {
                                throw new Exception("断开链接");
                            }
                            else if (mesData.MesType == SocketMesType.ActionMes)

                            {
                                if (mesData.UserSenderData is not null)
                                {
                                    var u = mesData.UserSenderData;
                                    var selectItem = GetFillter(u.UserIp, u.UserPort);
                                    if (selectItem is not null)
                                    {
                                        selectItem.Text = u.UserName;
                                        var itemForm = forms.Where(f => (f.UserInfo?.UserIp?.Equals(u.UserIp) ?? false) && f.UserInfo.UserPort == u.UserPort).FirstOrDefault();
                                        itemForm?.Invoke(() => itemForm.UserInfoChange(u.UserName));
                                    }
                                    else
                                    {
                                        if (u.UserIp is not null)
                                        {
                                            listView1.BeginInvoke(() => listView1.Items.Add(new ListViewItem(new string[] { u.UserName, u.UserIp, u.UserPort.ToString() })));
                                            this.Invoke(() => chats.TryAdd((u.UserIp, u.UserPort), new()));
                                        }
                                    }
                                }
                            }
                            else if (mesData.MesType == SocketMesType.RecevMes)
                            {
                                if (mesData.UserSenderData is not null)
                                {
                                    var u = mesData.UserSenderData;
                                    var removeItem = GetFillter(u.UserIp, u.UserPort);
                                    if (removeItem is not null)
                                    {
                                        listView1.BeginInvoke(() => listView1.Items.Remove(removeItem));
                                    }
                                    var itemForm = forms.Where(f => (f.UserInfo?.UserIp?.Equals(u.UserIp) ?? false) && f.UserInfo.UserPort == u.UserPort).FirstOrDefault();
                                    itemForm?.Invoke(() => itemForm.StopChat(mesData.Data ?? string.Empty));
                                    if (u.UserIp is not null)
                                    {
                                        chats.TryRemove((u.UserIp, u.UserPort), out _);
                                    }
                                }
                            }
                            else if (mesData.MesType == SocketMesType.NormolMes)
                            {
                                if (mesData.UserSenderData is not null && mesData.UserRecverData is not null)
                                {
                                    var u = mesData.UserSenderData;
                                    var itemForm = forms.Where(f => (f.UserInfo?.UserIp?.Equals(u.UserIp) ?? false) && f.UserInfo.UserPort == u.UserPort).FirstOrDefault();
                                    if (itemForm is not null)
                                    {
                                        itemForm.Invoke(() => itemForm.AddMesMethod(new MesData(mesData.Data ?? string.Empty, mesData.DateTimeText ?? string.Empty)));
                                    }
                                    else
                                    {
                                        if (u.UserIp is not null)
                                        {
                                            if (chats.TryGetValue((u.UserIp, u.UserPort), out List<MesData>? itemChats))
                                            {
                                                if (itemChats is not null)
                                                {
                                                    itemChats.Add(new MesData(mesData.Data ?? string.Empty, mesData?.DateTimeText ?? string.Empty));
                                                    var changeItem = GetFillter(u.UserIp, u.UserPort);
                                                    if (changeItem is not null)
                                                    {
                                                        listView1.Invoke(() => changeItem.BackColor = Color.Red);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else { }
                        }
                    },
                    actionClose: sh =>
                    {
                        client.CloseConnect(JsonSerializer.Serialize(new SocketMesData<string>(SocketMesType.CloseMes, userSelf, new(), string.Empty)));
                        ColorChange(Color.Red);
                        MessageBox.Show("服务器或本地连接异常");
                    }
                                );
                userSelf.UserName = clientConfig.UserName;
                userSelf.UserIp = (client.socketHelpModel?.s?.LocalEndPoint as IPEndPoint)?.Address.ToString();
                userSelf.UserPort = (client.socketHelpModel?.s?.LocalEndPoint as IPEndPoint)?.Port ?? 0;
                label1.Text = userSelf.UserName;
                label2.Text = userSelf.UserIp;
                label3.Text = userSelf.UserPort.ToString();
                client.SendMes(JsonSerializer.Serialize(new SocketMesData<string>(SocketMesType.ActionMes, userSelf, new(), string.Empty)));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                connectFlag = false;
            }
            if (!connectFlag)
            {
                ColorChange(Color.Red);
            }
            else 
            {
                ColorChange(Color.Tan);
            }
        }

        private void ColorChange(Color c) 
        {
            this.splitContainer1.Panel1.BackColor = c;
            this.label1.BackColor = c;
            this.label2.BackColor = c;
            this.label3.BackColor = c;
        }

        private void listView1_DoubleClick()
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListViewItem item = listView1.SelectedItems[0];
                if (item.SubItems.Count == 3) 
                {
                    string ip = item.SubItems[1].Text;
                    
                   if(int.TryParse(item.SubItems[2].Text, out int port_))
                    {
                        var selectform = forms.Where(f => ip == f.UserInfo.UserIp && port_ == f.UserInfo.UserPort).FirstOrDefault();
                        if (selectform is not null) 
                        {
                            selectform.Focus();
                            return;
                        }
                        ChatForm chatForm;
                        if (chats.TryGetValue((ip, port_), out List<MesData>? chatdata))
                        {
                            chatForm = new ChatForm(new User { UserName = item.Text, UserIp = ip, UserPort = port_ }, userSelf, chatdata,client,clientConfig.RecColorName,clientConfig.SendColorName);
                        }
                        else 
                        {
                            chatForm = new ChatForm(new User { UserName = item.Text, UserIp = ip, UserPort = port_ }, userSelf, new(),client, clientConfig.RecColorName, clientConfig.SendColorName);
                        }
                        chatForm.FormClosed += (o, e) => { forms.Remove(chatForm); };
                        forms.Add(chatForm);
                        if (Color.Red.Equals(item.BackColor)) 
                        {
                            item.BackColor = Color.White;
                        }
                        chatForm.Show();
                    }
                }
            }
        }

        private void ChangeNanme() 
        {
            button1.Enabled = false;
            SettingForm itemForm= new SettingForm(clientConfig);
            if (itemForm.ShowDialog() == DialogResult.OK) 
            {
                bool startConnect = clientConfig.Port != itemForm.UpdateClientConfig.Port ;
                this.Ischanged = clientConfig.IsChanged(itemForm.UpdateClientConfig)? Ischanged: true;
                string hostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                IPAddress? localIP = ipEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                if (clientConfig.Type == itemForm.UpdateClientConfig.Type)
                {
                    if (clientConfig.Type != 0)
                    {
                        if (!clientConfig.EqualesIpAndPort(itemForm.UpdateClientConfig))
                        {
                            if (clientConfig.Type == 1)
                            {
                                startConnect = localIP is null ? false : startConnect;
                            }
                            else { startConnect = false; }
                        }
                    }
                }
                else 
                {
                    startConnect = (localIP is null && (IPAddress.Parse(itemForm.UpdateClientConfig.Ip).Equals(IPAddress.Loopback))) ? startConnect : true;
                }
                bool nameIsChanged = clientConfig.UserName == itemForm.UpdateClientConfig.UserName;
                clientConfig = itemForm.UpdateClientConfig;
                if (startConnect)
                {
                    this.BeforSocketFormClose();
                    ColorChange(Color.Red);
                    Form[] items = forms.ToArray();
                    foreach (Form item in items)
                    {
                        item.Close();
                    }
                    chats = new();
                    listView1.Items.Clear();
                    StartConnect();
                }
                else 
                {
                    if (!nameIsChanged) 
                    {
                        label1.Text = clientConfig.UserName;
                        userSelf.UserName = clientConfig.UserName;
                        client.SendMes(JsonSerializer.Serialize(new SocketMesData<string>(SocketMesType.ActionMes, userSelf, new(), string.Empty)));
                    }
                } 
            }
            button1.Enabled = true; 
        }

        private ListViewItem? GetFillter(string? ip,int port) 
        {
            return listView1.Items.Cast<ListViewItem>().ToList().Where(item =>
            {
                if (item.SubItems.Count == 3) 
                {
                    return ip == item.SubItems[1].Text && port.ToString() == item.SubItems[2].Text;
                }
                return false;
                
            }).FirstOrDefault();
        }
    }

    public class MesData 
    {
        public string Mes { get; set; }
        public bool IsSelf { get; set; }

        public string DateTimeText { get; set; }
        public MesData(string text , string date,bool isself = false) 
        {
            Mes=text;
            IsSelf = isself;
            DateTimeText=date;
        }
    }
}
