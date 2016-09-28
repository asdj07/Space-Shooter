using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
public class socket {

    private const char sp1 = ':';    //分隔符
    private const char sp2 = '-';
    private const String strEnd = "\n";   //结束符

    private Socket clientSocket;              //Socket客户端对象  
    private IPAddress ipAddress;              //服务器IP地址
    private IPEndPoint ipEndpoint;            //服务器端口

    private String host = "127.0.0.1";
    private int port = 8090;

    private static socket instance;

   

    public static socket GetInstance()
    {
        if (instance == null)
        {
            instance = new socket();
        }
        return instance;
    }

    //单例的构造函数     
    private socket()
    {
        //创建Socket对象， 这里我的连接类型是TCP     
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //服务器IP地址     
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        //服务器端口     
        IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, port);
        //这是一个异步的建立连接，当连接建立成功时调用connectCallback方法     
        IAsyncResult result = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(connectCallback), clientSocket);
        //这里做一个超时的监测，当连接超过5秒还没成功表示超时     
        bool success = result.AsyncWaitHandle.WaitOne(50000, true);
        if (!success)
        {
            //超时     
            Closed();
            Debug.Log("connect Time Out");
        }
        else
        {
            //worldpackage = new List<JFPackage.WorldPackage>();     
            //Thread thread = new Thread(new ThreadStart(ReceiveSorket));     
            //thread.IsBackground = true;     
            //thread.Start();     
        }
    }

    private void connectCallback(IAsyncResult asyncConnect)
    {
        Debug.Log("Connect to Server Success");
    }



    //向服务端发送一条字符串     
    //一般不会发送字符串 应该是发送数据包     
    public String SendMessage(string str)
    {
        byte[] msg = Encoding.UTF8.GetBytes(str + "\n");

        if (!clientSocket.Connected)
        {
            clientSocket.Close();
            return null;
        }
        try
        {
            //int i = clientSocket.Send(msg);     
            IAsyncResult asyncSend = clientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);
            bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);

            if (!success)
            {
                clientSocket.Close();
                Debug.Log("Failed to SendMessage server.");
                return null;
            }
            else
            {
                //接受数据保存至bytes当中     
                byte[] bytes = new byte[4096];
                //Receive方法中会一直等待服务端回发消息     
                //如果没有回发会一直在这里等着。     
                int i = clientSocket.Receive(bytes);
                string s = System.Text.Encoding.Default.GetString(bytes);
                
                return s;

            }
        }
        catch
        {
            Debug.Log("send message error");
            return null;
        }
    }

    private void sendCallback(IAsyncResult asyncSend)
    {

    }


    //关闭Socket     
    public void Closed()
    {

        if (clientSocket != null && clientSocket.Connected)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
        clientSocket = null;
    }

    public char getSp1()
    {
        return sp1;
    }

    public char getSp2()
    {
        return sp2;
    }

}





