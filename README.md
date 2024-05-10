# MySocketChat
基于winform写的一个超简单的socket在线聊天的客户端，服务端是控制台项目，目前不支持群聊，不支持消息保存本地，只能进行简单的文本发送和接收

## MyChatServer

控制台写的服务器

运行目录下会有socketConfig.ini配置文件

```
{"Type":0,"Port":5536,"Num":20}
```

Type表示类型，0代表监听本地127.0.0.1，1表示监听局域网IP，2表示监听任何IP

Port表示监听端口，设置在1024到

Num表示监听的数量最大值

如果没有第一次会自动生成，默认生成是127.0.0.1 端口5534，监听数量最大20

## MySocketClient

winform写的客户端

运行目录下会有ClientConfig.ini配置文件

```
{"Ip":"127.0.0.1","Port":5536,"Type":0,"UserName":"\u54C8\u54C8\u54C8","SendColorName":-16744448,"RecColorName":-8388353}
```

Type表示类型，0代表连接到本地127.0.0.1，1表示监听局域网IP，未找到局域网使用配置的IP进行连接，2表示连接到配置IP

port表示连接绑定的端口，UserName表示用户名，存在着转码显示问题。SendColorName表示发送消息的颜色的ARPG，RecColorName表示接受消息显示的颜色的ARPG



当配置文件读取异常或是读取的数据异常，将会重写默认的数据到文件内容。
