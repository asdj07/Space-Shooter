using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class GameController : MonoBehaviour {
    public GameObject hazard;   //设置生成的障碍对象
    public Vector3 spawnValue;  //设置障碍生成的位置
    public int hazardCount;     //每次生成的障碍的数量
    public float spawnWait;     //每个障碍生成的间隔时间
    public float startWait;     //游戏开始生成障碍的准备时间
    public float waveWait;      //设置每一波的间隔时间

    public GameObject hazard_enemy;   //设置生成的障碍对象
    public Vector3 spawnValue_enemy;  //设置障碍生成的位置
    public int hazardCount_enemy;     //每次生成的障碍的数量
    public float spawnWait_enemy;     //每个障碍生成的间隔时间
    public float startWait_enemy;     //游戏开始生成障碍的准备时间
    public float waveWait_enemy;      //设置每一波的间隔时间

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    public Text msgText;
    private bool gameOver;
    private bool restart;
    private int score;


    public socket skt;

    IEnumerator SpawnWaves()
    {
        //让代码等待一定时间，这是个协同程序，可以让游戏不暂停的同时，让代码暂停
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                //随机生成障碍位置
                Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-spawnValue.x, spawnValue.x), spawnValue.y, spawnValue.z);
                //Rotation属性如何确定，Rotation属性是一个Quaternion类型的变量，
                //所以我们定义了一个spawnRotation的变量，用来记录障碍的Rotation属性，在这里我们不需要让障碍一开始带着任何的旋转
                //所以我们直接赋值Quaternion.identity
                Quaternion spawnRotation = Quaternion.identity;

                Instantiate(hazard, spawnPosition, spawnRotation);



                //随机生成障碍位置
                spawnPosition = new Vector3(UnityEngine.Random.Range(-spawnValue.x, spawnValue.x), spawnValue.y, spawnValue.z);
                //Rotation属性如何确定，Rotation属性是一个Quaternion类型的变量，
                //所以我们定义了一个spawnRotation的变量，用来记录障碍的Rotation属性，在这里我们不需要让障碍一开始带着任何的旋转
                //所以我们直接赋值Quaternion.identity
                spawnRotation = Quaternion.identity;

                Instantiate(hazard_enemy, spawnPosition, spawnRotation);



                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            if(gameOver == true)
            {
                restartText.text = "Press R for Restart";
                restart = true;
                break;
            }
        }
        
    }

    IEnumerator SpawnEnemy()
    {
        //让代码等待一定时间，这是个协同程序，可以让游戏不暂停的同时，让代码暂停
        yield return new WaitForSeconds(startWait_enemy);
        while (true)
        {
            for (int i = 0; i < hazardCount_enemy; i++)
            {
                //随机生成障碍位置
                Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(-spawnValue.x, spawnValue.x), spawnValue.y, spawnValue.z);
                //Rotation属性如何确定，Rotation属性是一个Quaternion类型的变量，
                //所以我们定义了一个spawnRotation的变量，用来记录障碍的Rotation属性，在这里我们不需要让障碍一开始带着任何的旋转
                //所以我们直接赋值Quaternion.identity
                Quaternion spawnRotation = Quaternion.identity;

                Instantiate(hazard_enemy, spawnPosition, spawnRotation);

                yield return new WaitForSeconds(spawnWait_enemy);
            }
            yield return new WaitForSeconds(waveWait_enemy);

            if (gameOver == true)
            {
                restartText.text = "Press R for Restart";
                restart = true;
                break;
            }
        }

    }


    void UpdateScore()
    {
        scoreText.text = "Score :" + score;

        //请求参数   class:param-param-...
        String strReq = "req001" + skt.getSp1() + score;
        Debug.Log("request msg:" + strReq);
        String msg = skt.SendMessage(strReq);
        Debug.Log("response:" + msg);
        msgText.text =  msg;
}

    public void  AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    public void GameOver ()
    {
        gameOverText.text = "Game Over !";
        gameOver = true;

    }


	// Use this for initialization
	void Start () {
        gameOver = false;
        restart = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;

        skt = socket.GetInstance();

        UpdateScore();
        //因为SpawnWaves中有协同程序，StartCoroutine为启动一个协同程序
        StartCoroutine(SpawnWaves());
        //StartCoroutine(SpawnEnemy());

        


    }


    void test()
    {
        for(int i = 0; i < 100; i++)
        {
            Debug.Log(i);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            { 
                //重新启动
                Application.LoadLevel(Application.loadedLevel);
            }
        }
	}




    void Socket_Server()
    {
        int port = 8080;
        string host = "127.0.0.1";

        //创建终结点
        IPAddress ip = IPAddress.Parse(host);
        IPEndPoint ipe = new IPEndPoint(ip, port);

        //创建Socket并开始监听

        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建一个Socket对象，如果用UDP协议，则要用SocketTyype.Dgram类型的套接字
        s.Bind(ipe);    //绑定EndPoint对象(2000端口和ip地址)
        s.Listen(0);    //开始监听

        Debug.Log("等待客户端连接.....");
        //Console.WriteLine("等待客户端连接");

        //接受到Client连接，为此连接建立新的Socket，并接受消息
        Socket temp = s.Accept();   //为新建立的连接创建新的Socket
        Debug.Log("建立连接");
        //Console.WriteLine("建立连接");
        string recvStr = "";
        byte[] recvBytes = new byte[1024];
        int bytes;
        bytes = temp.Receive(recvBytes, recvBytes.Length, 0); //从客户端接受消息
        recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);

        //给Client端返回信息
        Debug.Log("server get message: "+ recvStr);
        //Console.WriteLine("server get message:{0}", recvStr);    //把客户端传来的信息显示出来
        string sendStr = "ok!Client send message successful!";
        byte[] bs = Encoding.ASCII.GetBytes(sendStr);
        temp.Send(bs, bs.Length, 0);  //返回信息给客户端
        temp.Close();
        s.Close();

        //Console.ReadLine();
    }



    public static void ServerMethod()
    {
        IPAddress serverIP = IPAddress.Parse("127.0.0.1");//以本机作测试
        IPEndPoint serverFullAddr = new IPEndPoint(serverIP, 1234);//取端口号8888//完整终端地址

        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(serverFullAddr);
        while (true)
        {
            server.Listen(10);//监听是否有连接传入，指定挂起的连接队列的最大值为10
                              //执行accept()，当挂起队列为空时将阻塞本线程，同时由于上一语句，定时器将停止，直//至有连接传入
            Socket acceptSock = server.Accept();
            byte[] byteArray = new byte[100];
            acceptSock.Receive(byteArray);//接收数据
                                          //将字节数组转成字符串
            string strRec = System.Text.Encoding.Default.GetString(byteArray);
            Debug.Log(strRec);
            if(strRec == "1")
            {
                //重新启动
                Application.LoadLevel(Application.loadedLevel);
            }
            string strSend = "Your message has been received successfully.";
            byte[] bytes = System.Text.Encoding.Default.GetBytes(strSend);
            acceptSock.Send(bytes);
            acceptSock.Close();
        }

    }



}
