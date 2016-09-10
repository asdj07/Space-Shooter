using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public GameObject hazard;   //设置生成的障碍对象
    public Vector3 spawnValue;  //设置障碍生成的位置
    public int hazardCount;     //每次生成的障碍的数量
    public float spawnWait;     //每个障碍生成的间隔时间
    public float startWait;     //游戏开始生成障碍的准备时间
    public float waveWait;      //设置每一波的间隔时间

    public Text scoreText;
    public Text restartText;
    public Text gameOverText;
    private bool gameOver;
    private bool restart;
    private int score;

    IEnumerator SpawnWaves()
    {
        //让代码等待一定时间，这是个协同程序，可以让游戏不暂停的同时，让代码暂停
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                //随机生成障碍位置
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValue.x, spawnValue.x), spawnValue.y, spawnValue.z);
                //Rotation属性如何确定，Rotation属性是一个Quaternion类型的变量，
                //所以我们定义了一个spawnRotation的变量，用来记录障碍的Rotation属性，在这里我们不需要让障碍一开始带着任何的旋转
                //所以我们直接赋值Quaternion.identity
                Quaternion spawnRotation = Quaternion.identity;

                Instantiate(hazard, spawnPosition, spawnRotation);
                
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

    void UpdateScore()
    {
        scoreText.text = "Score :" + score; 
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
        UpdateScore();
        //因为SpawnWaves中有协同程序，StartCoroutine为启动一个协同程序
        StartCoroutine(SpawnWaves());
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
}
