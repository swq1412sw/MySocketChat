using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MySocketClient
{
    public static class Program
    {
        public static string GetFilePath() 
        {
            string currentDirectory = Environment.CurrentDirectory;
            return  Path.Combine(currentDirectory, "ClientConfig.ini");
        }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string filepath = GetFilePath();
            ClientConfig clientConfig;
            try
            {
                if (File.Exists(filepath))
                {
                    var configData = JsonSerializer.Deserialize<ClientConfig>(File.ReadAllText(filepath));
                    bool flag = false;
                    if (configData is not null)
                    {
                        if (!configData.CheckPortAndIp())
                        {
                            flag = true;
                            configData.Ip = "127.0.0.1";
                            configData.Port = 5534;
                        }
                        if (configData.UserName.Length < 2 || configData.UserName.Length > 6)
                        {
                            flag = true;
                            configData.UserName = "默认名称";
                        }
                        if (flag) File.WriteAllText(filepath, JsonSerializer.Serialize(configData));
                        clientConfig = configData;
                    }
                    else
                    {
                        clientConfig = new("127.0.0.1", 5534, 0, "默认名称", Color.Green.ToArgb(), Color.Blue.ToArgb());
                        File.WriteAllText(filepath, JsonSerializer.Serialize(clientConfig));
                    }
                }
                else
                {
                    clientConfig = new("127.0.0.1", 5534, 0, "默认名称", Color.Green.ToArgb(), Color.Blue.ToArgb());
                    File.WriteAllText(filepath, JsonSerializer.Serialize(clientConfig));
                }
            }
            catch 
            {
                clientConfig = new("127.0.0.1", 5534, 0, "默认名称", Color.Green.ToArgb(), Color.Blue.ToArgb());
                try { File.WriteAllText(filepath, JsonSerializer.Serialize(clientConfig)); } catch { }
                
            }
            
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm(clientConfig));
        }
    }

    public class ClientConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string _ip;
        private int _port;
        private int _type;
        private string _userName;
        private int _sendColorName;
        private int _recColorName;



        public string Ip { get=>_ip; set 
            {
                if (value != _ip) 
                {
                    _ip = value;
                    NotifyPropertyChanged();
                }
            } }
        public int Port { get=>_port; set 
            {
                if (value != _port) 
                {
                    _port = value;
                    NotifyPropertyChanged();
                }
            } }
        public int Type { get=>_type; set 
            {
                if (value != _type)
                {
                    _type = value;
                    NotifyPropertyChanged();
                }
            } }

        public string UserName { get => _userName; set 
            {
                if (value != _userName)
                {
                    _userName = value;
                    NotifyPropertyChanged();
                }
            } }
        [TypeConverter(typeof(StringToIntTypeConverter))]
        public int SendColorName { get=>_sendColorName; set 
            {
                if (value != _sendColorName)
                {
                    _sendColorName = value;
                    NotifyPropertyChanged();
                }
            } }
        [TypeConverter(typeof(StringToIntTypeConverter))]
        public int RecColorName { get=>_recColorName; set 
            {
                if (value != _recColorName)
                {
                    _recColorName = value;
                    NotifyPropertyChanged();
                }
            } }

        public ClientConfig(string Ip, int Port, int Type, string UserName, int SendColorName, int RecColorName)
        {
            _ip = Ip;
            _userName = UserName;
            this.Ip = Ip;
            this.Port = Port;
            this.Type = Type;
            this.UserName = UserName;
            this.SendColorName = SendColorName;
            this.RecColorName = RecColorName;
        }

        public bool CheckIp() 
        {
            return IPAddress.TryParse(Ip, out _);
        }

        public bool CheckPortAndIp() 
        {
            return CheckIp() && (Port > 1023 && Port <= 49151);
        }

        public bool EqualesIpAndPort(ClientConfig c) 
        {
            if (!c.CheckPortAndIp()) return false;
            if (c.Port != Port) return false;
            if (IPAddress.TryParse(Ip, out IPAddress? item)) 
            {
                return item.ToString() == IPAddress.Parse(c.Ip).ToString();
            }
            return false;
        }

        public override string ToString()
        {
            return $"名字是：{UserName}，IP是：{Ip}，端口是：{Port}，类型是{Type}，发送方颜色是{Color.FromArgb(SendColorName).Name}，接收方颜色是{Color.FromArgb(RecColorName).Name}，是否通过检查{CheckPortAndIp()}";
        }

        public bool IsChanged(ClientConfig c) 
        {
            return this.Port == c.Port && this.Ip == c.Ip && this.SendColorName == c.SendColorName && this.RecColorName == c.RecColorName && this.Type == c.Type && this.UserName==c.UserName;
        }
    }

    public class StringToIntTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            return destinationType == typeof(int) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue && int.TryParse(stringValue, out int intValue))
            {
                return intValue;
            }
            return base.ConvertFrom(context, culture, value)??0;
        }

        public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is int intValue)
            {
                return intValue.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType) ?? string.Empty;
        }
    }
}