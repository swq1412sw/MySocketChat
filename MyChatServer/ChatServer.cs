using MySocket;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Collections.Concurrent;

namespace MyChatServer
{
    internal class ChatServer
    {
        static JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        static ConcurrentDictionary<IPEndPoint,User> users = new();
        static void Main(string[] args)
        {
            // 获取当前工作目录的完全限定路径
            string currentDirectory = Environment.CurrentDirectory;
            var filePath = Path.Combine(currentDirectory, "socketConfig.ini");
            IPEndPoint iPEndPointConfig;
            int listenNum = 20;
            if (File.Exists(filePath))
            {
                string dataConfig = File.ReadAllText(filePath);
                try
                {
                    var configData = JsonSerializer.Deserialize<ConfigSocket>(dataConfig);
                    if (configData.Port < 1024 || configData.Port > 49151)
                    {
                        throw new Exception("端口号要在1024到49151之间");
                    }
                    if (configData.Num < 0 || configData.Num > 50) throw new Exception("配置文件的监听数量不正确");
                    listenNum = configData.Num;
                    switch (configData.Type)
                    {
                        case 0:
                            iPEndPointConfig = new(IPAddress.Loopback, configData.Port);
                            break;
                        case 1:
                            {
                                string hostName = Dns.GetHostName();
                                // 获取主机的IP地址列表
                                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                                // 从IP地址列表中筛选出IPv4地址
                                IPAddress? localIP = ipEntry.AddressList.FirstOrDefault(ip => { if (ip.AddressFamily == AddressFamily.InterNetwork) { Console.WriteLine($"Local IPv4 Address:{ip.ToString()}"); return true; } return false; });
                                if (localIP is null)
                                {
                                    Console.WriteLine("未找到局域网，本地接收");
                                    iPEndPointConfig = new(IPAddress.Loopback, configData.Port);
                                }
                                else
                                {
                                    Console.WriteLine("Local IPv4 Address: " + localIP.ToString());
                                    iPEndPointConfig = new(localIP, configData.Port);
                                }
                            }
                            break;
                        case 2:
                            iPEndPointConfig = new(IPAddress.Any, configData.Port);
                            break;
                        default:
                            iPEndPointConfig = new(IPAddress.Loopback, configData.Port);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    iPEndPointConfig = new(IPAddress.Loopback, 5534);
                    File.WriteAllText(filePath, JsonSerializer.Serialize(new ConfigSocket(0,5534,20)));
                }
            }
            else 
            {
                Console.WriteLine("配置文件不存在");
                iPEndPointConfig = new(IPAddress.Loopback, 5534);;
                File.WriteAllText(filePath, JsonSerializer.Serialize(new ConfigSocket(0, 5534, 20)));
            }

            var server = new SocketServer(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), iPEndPointConfig, listenNum);
            server.StartAcceptMes(
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
                                var senderDataAfter = new SocketMesData<string>(SocketMesType.ActionMes, mesData.UserSenderData, new(), string.Empty);
                                var senderString = JsonSerializer.Serialize(senderDataAfter);
                                bool updateflag = false;
                                if (!users.TryAdd((IPEndPoint)sh?.s?.RemoteEndPoint, mesData.UserSenderData))
                                {
                                    updateflag = true;
                                    users.TryUpdate((IPEndPoint)sh.s.RemoteEndPoint, mesData.UserSenderData, users[(IPEndPoint)sh.s.RemoteEndPoint]);
                                }
                                foreach (var item in server.dic.Keys.Where(k => !k.Equals((IPEndPoint)sh.s.RemoteEndPoint)))
                                {
                                    server.SendMes(senderString, item);
                                    if (!updateflag)
                                    {
                                        server.SendMes(JsonSerializer.Serialize(new SocketMesData<string>(SocketMesType.ActionMes, users[item], new(), string.Empty)), (IPEndPoint)sh.s.RemoteEndPoint);
                                    }


                                }
                            }
                        }
                        else if (mesData.MesType == SocketMesType.RecevMes) { }
                        else if (mesData.MesType == SocketMesType.NormolMes)
                        {
                            if (mesData.UserSenderData is not null && mesData.UserRecverData is not null)
                            {
                                var item = server.dic.Keys.Where(k => k.Address.ToString().Equals(mesData.UserRecverData.UserIp) && k.Port == mesData.UserRecverData.UserPort).FirstOrDefault();
                                if (item is not null)
                                {
                                    server.SendMes(s, item);
                                }
                                else
                                {
                                    server.SendMes(JsonSerializer.Serialize(new SocketMesData<string>(SocketMesType.RecevMes, mesData.UserRecverData, mesData.UserSenderData, "发送失败，对方已不在线")), (IPEndPoint)sh.s.RemoteEndPoint);
                                }

                            }
                        }
                        else { }
                    }
                },
                actionClose: sh =>
                {
                    var senderDataAfter = new SocketMesData<string>(SocketMesType.RecevMes, users[(IPEndPoint)sh.s.RemoteEndPoint], null, "对方已不在线");
                    var senderString = JsonSerializer.Serialize(senderDataAfter);
                    foreach (var item in server.dic.Keys.Where(k => !k.Equals((IPEndPoint)sh.s.RemoteEndPoint)))
                    {
                        server.SendMes(senderString, item);
                    }
                    server.dic.TryRemove((IPEndPoint)sh.s.RemoteEndPoint, out _);
                    users.TryRemove((IPEndPoint)sh.s.RemoteEndPoint, out _);
                    try
                    {
                        sh?.s?.Shutdown(SocketShutdown.Both);
                        sh?.s.Close();
                    }
                    catch
                    {
                    }
                },
                diyAction: false
                );
            Console.WriteLine($"当前的连接是{iPEndPointConfig.Address},端口是{iPEndPointConfig.Port},监听数量是{listenNum}");
            while (server.State)
            {
                Thread.Sleep(3000);
            }
            server.CloseServer(() =>
            {
                List<IPEndPoint> ips = new();
                foreach (var item in server.dic)
                {
                    ips.Add(item.Key);
                }
                var senderDataAfter = new SocketMesData<string>(SocketMesType.CloseMes, null, null, string.Empty);
                var senderString = JsonSerializer.Serialize(senderDataAfter);
                foreach (var item in ips)
                {
                    if (server.dic.TryRemove(item, out SocketHelpModel? model))
                    {
                        try
                        {
                            model?.SendMes(senderString);
                            model?.s?.Shutdown(SocketShutdown.Both);
                            model?.s?.Close(); model?.cts?.Cancel();
                            model.cts = null;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                }
                server.CloseServer();
            });
        }
    }
    internal class ConfigSocket 
    {
        public int Type { get; set; }
        public int Port { get; set; }
        public int Num { get; set; }

        public ConfigSocket(int Type, int Port, int Num) 
        {
            this.Type = Type;
            this.Port = Port;
            this.Num = Num;
        }
    } 
}
