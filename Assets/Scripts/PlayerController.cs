using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {

    private float speed;

    public float tilt;

    public Boundary boundary;

    private float nextFire;  //记录下一发子弹的时间
    public float fireRate;    //控制子弹发射的间隔时间
    public GameObject shot;    //保存我们发射的子弹Prefab
    public Transform shotSqawn;   //保存子弹的发射点
    public socket skt;

    // Use this for initialization
    void Start () {
        //csvController加载csv文件，单例模式，这个类只有一个对象，这个对象只能加载一个csv文件
        //csvController.GetInstance().loadFile(Application.dataPath + "/Res", "csvTest.csv");
        //根据索引读取csvController中的list（csv文件的内容）数据
        //speed = csvController.GetInstance().getInt(1, 2);
        //Debug.Log("Player speed is " + csvController.GetInstance().getInt(1,2));
        
        
        //从java后端获取玩家参数
        //speed,fireRate
        skt = socket.GetInstance();
        //返回数据 应为  speed-fireRate
        String msg = skt.SendMessage("req002");
        Debug.Log("request msg:" + "req002");
        Debug.Log("response msg:" + msg);
        if (msg!=null && msg.Split(skt.getSp2()).Length>1)
        {
            speed = float.Parse(msg.Split(skt.getSp2())[0]);
            fireRate = float.Parse(msg.Split(skt.getSp2())[1]);
            Debug.Log("speed:" + speed + "   firerete:" + fireRate);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        //按下发射键（ctrl或鼠标左键）并且到达下一发子弹发射时间
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            //Instantiate方法生成一发子弹，他有3个参数，一个是发射的对象(复制出一个新的)，一个是发射对象的position，一个是发射对象的rotation
            Instantiate(shot, shotSqawn.position, shotSqawn.rotation);

            //当子弹发射时开启音效
            GetComponent<AudioSource>().Play();
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical  = Input.GetAxis("Vertical");

        //moveHorizontal和moveVertical记录玩家输入的方向数据
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        //刚体根据矢量方向movement移动，位移为speed
        GetComponent<Rigidbody>().velocity = movement * speed;

        //移动时倾斜，以Z为中心轴旋转
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * - tilt);

        //首先序列化Boundary类,保存飞船在X和Z轴上的范围
        //Mathf.Clamp将飞船的x和z值锁定在屏幕范围内
        GetComponent<Rigidbody>().position = new Vector3(
            Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
            );

        //Debug.Log(boundary.xMin + "::" + boundary.xMax);
    }


}
