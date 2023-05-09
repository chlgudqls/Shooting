using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    //총알은 플레이어가 생성하지만 적은 게임메니저가 생성한다
    //public GameObject[] enemyObjs;
    //풀링으로 바꾸려니 기존방식과 달라서 string으로 바꿈
    //프리팹이아니고 문자열로 가져오기떄문에
    public string[] enemyObjs;
    public Transform[] spawnPoints;
    public Transform playerPos;

    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;

    //적 생성 딜레이는 왜만드나 모르겠음
    //public float maxSpawnDelay;
    //첫번째딜레이를 안넣었다고하네 뭔말이지
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
        //이 위치가 거꾸로면 오류난다 이 위치가 중요한가 여기의 인덱스를 쓰는구나 생각해보니
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        //초기화 다 끝난 다음에 넣어야 된다고함
        StageStart();
    }
    public void StageStart()
    {
        //#.Stage UI Load
        stageAnim.SetTrigger("On");
        //스테이지 숫자를 넣기위해서 가져옴
        stageAnim.GetComponent<Text>().text = "Stage " + stage + "\nStart";
        clearAnim.GetComponent<Text>().text = "Stage " + stage + "\nClear!";

        //#.Enemy Spawn File Read
        ReadSpawnFile();

        //#.Fade In
        fadeAnim.SetTrigger("In");
    }
    //직접객체를 넣어서 겟컴포넌트없이 바로가져오는구나
    //그런데 텍스트를 바로 넣네 에니메이터를 넣는건가 했는데
    //Idle과 text가 겹치는구간이있어서 사라졌다고하는데
    //다른 에니메이터는 멀쩡햇던거같은데
    //이거때문에 에너미도 안나옴
    //그러고보니 스크립트없고 그냥 에니메이터네
    public void StageEnd()
    {
        //#.Clear UI Load
        clearAnim.SetTrigger("On");

        //#.Fade Out
        fadeAnim.SetTrigger("Out");
        //#.Player Repos
        player.transform.position = playerPos.position;
        //End하고 다음 스테이지 넘어간다. 이런식으로 생각하는구나 시나리오가 중요하네
        //#.Stage Increament
        stage++;
        //구현한 스테이지가 없으면 예외처리 GameOver함수 이용해서
        if (stage > 2)
            Invoke("GameOver", 5.5f);
        else
        Invoke("StageStart", 5);
    }
    void ReadSpawnFile()
    {
        //적 출현 변수초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //리스폰 파일 읽기
        //텍스트 파일 에셋 클래스
        //파일 불러옴
        //index로 수정 이것만하는게 아니라 시작 부분과 끝나는 부분을 만든다고함 이런 생각은 어떻게? 끝나야 다음으로 넘어가긴하겠네
        TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset;
        //파일내 문자 읽음
        StringReader stringReader = new StringReader(textFile.text);
        //한줄씩
        //반복
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

            if (line == null)
                break;

            //리스폰 데이터 생성
            //구조체 리스트에 저장
            Spawn spawnData = new Spawn();
            //텍스트의 타입에 맞게 형변환시켜줌
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);

            spawnList.Add(spawnData);
        }
        //StringReader로 열어둔 파일은 작업이 끝난후 닫음
        stringReader.Close();

        //첫번째 스폰 딜레이 적용
        //line은 따로 구분을 못함 몇번째줄인지
        //리스트는 몇번쨰줄인지 구분하는데
        nextSpawnDelay = spawnList[0].delay;
    }
    void Update()
    {
        //다 랜덤임 적생성주기, 적생성위치, 적생성 종류
        curSpawnDelay += Time.deltaTime;

        //플레그변수 이거 뭔 의민지 모르겠네
        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            //nextSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }
        Player playerLogic = player.GetComponent<Player>();
        //플레이스코어에 담은 데이터를 유아이에 넣어준다. 근데 왜 업데이트문인지
        //세자리씩 쉼표하기위해서 0:n0씀
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }


    //기존의 Instantiate를 오브젝트 풀링으로 교체
    //기존 적 생성로직을 구조체를 활용한 로직으로 교체
    void SpawnEnemy()
    {
        //구조체로직은 이제 랜덤필요없다고함
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
        //위치도 적처럼 배열로 값을넣어서 꺼내쓰는건지  쓰는구나 의외로 간단하게 생각할일이였음
        //int ranEnemy = Random.Range(0, 3);
        //int ranPoint = Random.Range(0, 9);
        int enemyPoint = spawnList[spawnIndex].point;
        //갑자기 변수로 치환해야겠다는 생각을 왜한건지
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        //위치와 각도는 인스턴스 변수에서 적용
        //아래쪽은 변수에 넣어서 사용하기떄문에 수정할필요없음
        enemy.transform.position = spawnPoints[enemyPoint].position;

        //이제 프리팹이아니고 stirng 문자열로 끌고온다 
        //Instantiate(enemyObjs[ranEnemy], 
        //                    spawnPoints[ranPoint].position, 
        //                    spawnPoints[ranPoint].rotation);

        //적 비행기 속도 GamaManager가 관리 
        //위치를 정해놨고 속도정함
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        //그냥 스크립트의 에너미를가져오는게 아니라 생성된 에너미로직을 가져오는구나
        Enemy enemyLogic = enemy.GetComponent<Enemy>();

        //무슨 이유인지는 모르겠는데 두 클래스에 player변수생성후 막 생성된 프리팹의 player변수에 player 변수를 집어넣음
        //왜 두개인가 enemy에서 직접못함 그래서 생성직후에 넣어주기 위해서 enemy에도 넣어둠
        //적 총알은 enemy에 있기때문
        enemyLogic.player = player;
        //몰라 일단넘김 프리팹이라 못가져온데
        //-----------------뺴고 해보기 어떻게되나 오류를 일부러 만들어봐야 알것같기도
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
        //리스폰 인덱스 증가 한마리 생성헀으니
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }
        //다음 리스폰 시간 갱신인데 왜 구조체의 딜레이를 넣는건지
        //진짜 대가리가 굳었나 조금 생각해보니 간단한거임
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }
    public void RespawnPlayer()
    {
        
        //바로 실행하면 안된다고함
        Invoke("RespawnPlayerExe", 2f);
    }
    void RespawnPlayerExe()
    {
        //player위치를 가지고 있음
        player.transform.position = Vector3.down * 4.2f;
        player.SetActive(true);

        //플레이어 로직을 가져와야되네 왜지  안가져오면 변수도 안뜸 컴포넌트가 없으니까 가져오는구만 그냥
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }
    public void UpdateLifeIcon(int life)
    {
        //이 로직 이해가 안가는데 그리고 생명값 초기화안하니까 3개한꺼번에 없어짐
        //없애고 남아있는거 나오고 이런 구조인지
        //위에있는게 지우는거고 
        //밑에있는게 나타내는거?
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
    //근데 왜 여기서 explosion을?
    //이 로직 머리아픔
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

