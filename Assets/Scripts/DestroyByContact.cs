using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

    public GameObject explosion;
    public GameObject playerExplosion;


    
    public int scoreValue;//分值
    private GameController gameController; //gameController脚本类

    // Use this for initialization
    void Start () {
        //获取GameController的实例
        //首先找到GameController的GameObject，然后在通过GetComponent方法来获取GameController脚本的实例
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
            Debug.Log("Cannot find 'GameController' script");
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        //初始时障碍物会与盒子boundary发生碰撞触发此函数，要规避此种触发
        if (other.tag == "Boundary")
        {
            return;
        }
        //生成一个爆炸效果
        Instantiate(explosion, transform.position, transform.rotation);

        //飞船碰到障碍物时爆炸效果
        if(other.tag == "Player")
        {
            Instantiate(playerExplosion, other.transform.position, other.transform.rotation);

            //飞船爆炸时调用gameover方法
            gameController.GameOver();  
        }

        gameController.AddScore(scoreValue);
        //Debug.Log(other.name);
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
