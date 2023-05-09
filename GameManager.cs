using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    //�Ѿ��� �÷��̾ ���������� ���� ���Ӹ޴����� �����Ѵ�
    //public GameObject[] enemyObjs;
    //Ǯ������ �ٲٷ��� ������İ� �޶� string���� �ٲ�
    //�������̾ƴϰ� ���ڿ��� �������⋚����
    public string[] enemyObjs;
    public Transform[] spawnPoints;
    public Transform playerPos;

    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;

    //�� ���� �����̴� �ָ��峪 �𸣰���
    //public float maxSpawnDelay;
    //ù��°�����̸� �ȳ־��ٰ��ϳ� ��������
    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;

    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    public int stage;
    void Awake()
    {
        spawnList = new List<Spawn>();
        //�� ��ġ�� �Ųٷθ� �������� �� ��ġ�� �߿��Ѱ� ������ �ε����� ���±��� �����غ���
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        //�ʱ�ȭ �� ���� ������ �־�� �ȴٰ���
        StageStart();
    }
    public void StageStart()
    {
        //#.Stage UI Load
        stageAnim.SetTrigger("On");
        //�������� ���ڸ� �ֱ����ؼ� ������
        stageAnim.GetComponent<Text>().text = "Stage " + stage + "\nStart";
        clearAnim.GetComponent<Text>().text = "Stage " + stage + "\nClear!";

        //#.Enemy Spawn File Read
        ReadSpawnFile();

        //#.Fade In
        fadeAnim.SetTrigger("In");
    }
    //������ü�� �־ ��������Ʈ���� �ٷΰ������±���
    //�׷��� �ؽ�Ʈ�� �ٷ� �ֳ� ���ϸ����͸� �ִ°ǰ� �ߴµ�
    //Idle�� text�� ��ġ�±������־ ������ٰ��ϴµ�
    //�ٸ� ���ϸ����ʹ� �����޴��Ű�����
    //�̰Ŷ����� ���ʹ̵� �ȳ���
    //�׷����� ��ũ��Ʈ���� �׳� ���ϸ����ͳ�
    public void StageEnd()
    {
        //#.Clear UI Load
        clearAnim.SetTrigger("On");

        //#.Fade Out
        fadeAnim.SetTrigger("Out");
        //#.Player Repos
        player.transform.position = playerPos.position;
        //End�ϰ� ���� �������� �Ѿ��. �̷������� �����ϴ±��� �ó������� �߿��ϳ�
        //#.Stage Increament
        stage++;
        //������ ���������� ������ ����ó�� GameOver�Լ� �̿��ؼ�
        if (stage > 2)
            Invoke("GameOver", 5.5f);
        else
        Invoke("StageStart", 5);
    }
    void ReadSpawnFile()
    {
        //�� ���� �����ʱ�ȭ
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //������ ���� �б�
        //�ؽ�Ʈ ���� ���� Ŭ����
        //���� �ҷ���
        //index�� ���� �̰͸��ϴ°� �ƴ϶� ���� �κа� ������ �κ��� ����ٰ��� �̷� ������ ���? ������ �������� �Ѿ���ϰڳ�
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
        //���ϳ� ���� ����
        StringReader stringReader = new StringReader(textFile.text);
        //���پ�
        //�ݺ�
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null)
                break;

            //������ ������ ����
            //����ü ����Ʈ�� ����
            Spawn spawnData = new Spawn();
            //�ؽ�Ʈ�� Ÿ�Կ� �°� ����ȯ������
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);

            spawnList.Add(spawnData);
        }
        //StringReader�� ����� ������ �۾��� ������ ����
        stringReader.Close();

        //ù��° ���� ������ ����
        //line�� ���� ������ ���� ���°������
        //����Ʈ�� ����������� �����ϴµ�
        nextSpawnDelay = spawnList[0].delay;
    }
    void Update()
    {
        //�� ������ �������ֱ�, ��������ġ, ������ ����
        curSpawnDelay += Time.deltaTime;

        //�÷��׺��� �̰� �� �ǹ��� �𸣰ڳ�
        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            //nextSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }
        Player playerLogic = player.GetComponent<Player>();
        //�÷��̽��ھ ���� �����͸� �����̿� �־��ش�. �ٵ� �� ������Ʈ������
        //���ڸ��� ��ǥ�ϱ����ؼ� 0:n0��
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }


    //������ Instantiate�� ������Ʈ Ǯ������ ��ü
    //���� �� ���������� ����ü�� Ȱ���� �������� ��ü
    void SpawnEnemy()
    {
        //����ü������ ���� �����ʿ���ٰ���
        int enemyIndex = 0;
        switch(spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        //��ġ�� ��ó�� �迭�� �����־ �������°���  ���±��� �ǿܷ� �����ϰ� ���������̿���
        //int ranEnemy = Random.Range(0, 3);
        //int ranPoint = Random.Range(0, 9);
        int enemyPoint = spawnList[spawnIndex].point;
        //���ڱ� ������ ġȯ�ؾ߰ڴٴ� ������ ���Ѱ���
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        //��ġ�� ������ �ν��Ͻ� �������� ����
        //�Ʒ����� ������ �־ ����ϱ⋚���� �������ʿ����
        enemy.transform.position = spawnPoints[enemyPoint].position;

        //���� �������̾ƴϰ� stirng ���ڿ��� ����´� 
        //Instantiate(enemyObjs[ranEnemy], 
        //                    spawnPoints[ranPoint].position, 
        //                    spawnPoints[ranPoint].rotation);

        //�� ����� �ӵ� GamaManager�� ���� 
        //��ġ�� ���س��� �ӵ�����
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        //�׳� ��ũ��Ʈ�� ���ʹ̸��������°� �ƴ϶� ������ ���ʹ̷����� �������±���
        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        //���� ���������� �𸣰ڴµ� �� Ŭ������ player���������� �� ������ �������� player������ player ������ �������
        //�� �ΰ��ΰ� enemy���� �������� �׷��� �������Ŀ� �־��ֱ� ���ؼ� enemy���� �־��
        //�� �Ѿ��� enemy�� �ֱ⶧��
        enemyLogic.player = player;
        //���� �ϴܳѱ� �������̶� �������µ�
        //-----------------���� �غ��� ��Եǳ� ������ �Ϻη� �������� �˰Ͱ��⵵
        enemyLogic.objectManager = objectManager;

        enemyLogic.gameManager = this;

        if (enemyPoint == 5 || enemyPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if(enemyPoint == 7 || enemyPoint == 8)
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else
        {
            rigid.velocity = new Vector2(0 , enemyLogic.speed * (-1));
        }
        //������ �ε��� ���� �Ѹ��� ����������
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }
        //���� ������ �ð� �����ε� �� ����ü�� �����̸� �ִ°���
        //��¥ �밡���� ������ ���� �����غ��� �����Ѱ���
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }
    public void RespawnPlayer()
    {
        
        //�ٷ� �����ϸ� �ȵȴٰ���
        Invoke("RespawnPlayerExe", 2f);
    }
    void RespawnPlayerExe()
    {
        //player��ġ�� ������ ����
        player.transform.position = Vector3.down * 4.2f;
        player.SetActive(true);

        //�÷��̾� ������ �����;ߵǳ� ����  �Ȱ������� ������ �ȶ� ������Ʈ�� �����ϱ� �������±��� �׳�
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }
    public void UpdateLifeIcon(int life)
    {
        //�� ���� ���ذ� �Ȱ��µ� �׸��� ���� �ʱ�ȭ���ϴϱ� 3���Ѳ����� ������
        //���ְ� �����ִ°� ������ �̷� ��������
        //�����ִ°� ����°Ű� 
        //�ؿ��ִ°� ��Ÿ���°�?
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0.5f);
        }
        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }
    public void UpdateBoomIcon(int boom)
    {
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0.5f);
        }
        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);
        }
    }
    //�ٵ� �� ���⼭ explosion��?
    //�� ���� �Ӹ�����
    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }
    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }    
}

