using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //프리팹이 디스트로이 될때 가비지메모리가 쌓인다 
    //그러면 자체적으로 가비지컬랙터 실행되지만 렉이걸린다
    //그렇기때문에 가비지컬랙터를 피하기위해서 오브젝트풀링을 쓴다
    //미리 생성해두고 활성화 비활성화하는 방식으로 메모리를 최적화한다
    public GameObject enemyLPrefab;
    public GameObject enemyMPrefab;
    public GameObject enemySPrefab;
    public GameObject enemyBPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemBoomPrefab;
    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletBossAPrefab;
    public GameObject bulletBossBPrefab;
    public GameObject bulletFollowerPrefab;
    public GameObject explosionPrefab;

    GameObject[] enemyL;
    GameObject[] enemyM;
    GameObject[] enemyS;
    GameObject[] enemyB;

    GameObject[] itemPower;
    GameObject[] itemCoin;
    GameObject[] itemBoom;

    GameObject[] bulletPlayerA;
    GameObject[] bulletPlayerB;
    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;
    GameObject[] bulletBossA;
    GameObject[] bulletBossB;
    GameObject[] bulletFollower;

    GameObject[] explosion;

    GameObject[] targetPool;

    

    void Awake()
    {
        //시작할때(첫 로딩시간) = 장면 배치 + 오브젝트 풀 생성
        enemyL = new GameObject[10];
        enemyM = new GameObject[10];
        enemyS = new GameObject[10];
        enemyB = new GameObject[1];

        itemPower = new GameObject[10];
        itemCoin = new GameObject[10];
        itemBoom = new GameObject[10];

        bulletPlayerA = new GameObject[100];
        bulletPlayerB = new GameObject[100];
        bulletEnemyA = new GameObject[100];
        bulletEnemyB = new GameObject[100];
        bulletBossA = new GameObject[50];
        bulletBossB = new GameObject[1000];
        bulletFollower = new GameObject[100];

        explosion = new GameObject[100];

        Generate();
    }
    //여긴 미리 만들어두고 false해둔거고
    void Generate()
    {
        for(int index = 0; index<enemyL.Length; index++)
        {
            //enemyL[index]
            enemyL[index] = Instantiate(enemyLPrefab);
            enemyL[index].SetActive(false);
        }
        for (int index = 0; index < enemyM.Length; index++)
        {
            enemyM[index] = Instantiate(enemyMPrefab);
            enemyM[index].SetActive(false);
        }
        for (int index = 0; index < enemyS.Length; index++)
        {
            enemyS[index] = Instantiate(enemySPrefab);
            enemyS[index].SetActive(false);
        }
        for (int index = 0; index < itemPower.Length; index++)
        {
            itemPower[index] = Instantiate(itemPowerPrefab);
            itemPower[index].SetActive(false);
        }
        for (int index = 0; index < itemCoin.Length; index++)
        {
            itemCoin[index] = Instantiate(itemCoinPrefab);
            itemCoin[index].SetActive(false);
        }
        for (int index = 0; index < itemBoom.Length; index++)
        {
            itemBoom[index] = Instantiate(itemBoomPrefab);
            itemBoom[index].SetActive(false);
        }
        for (int index = 0; index < bulletPlayerA.Length; index++)
        {
            bulletPlayerA[index] = Instantiate(bulletPlayerAPrefab);
            bulletPlayerA[index].SetActive(false);
        }
        for (int index = 0; index < bulletPlayerB.Length; index++)
        {
            bulletPlayerB[index] = Instantiate(bulletPlayerBPrefab);
            bulletPlayerB[index].SetActive(false);
        }
        for (int index = 0; index < bulletEnemyA.Length; index++)
        {
            bulletEnemyA[index] = Instantiate(bulletEnemyAPrefab);
            bulletEnemyA[index].SetActive(false);
        }
        for (int index = 0; index < bulletEnemyB.Length; index++)
        {
            bulletEnemyB[index] = Instantiate(bulletEnemyBPrefab);
            bulletEnemyB[index].SetActive(false);
        }
        for (int index = 0; index < bulletBossA.Length; index++)
        {
            bulletBossA[index] = Instantiate(bulletBossAPrefab);
            bulletBossA[index].SetActive(false);
        }
        for (int index = 0; index < bulletBossB.Length; index++)
        {
            bulletBossB[index] = Instantiate(bulletBossBPrefab);
            bulletBossB[index].SetActive(false);
        }
        for (int index = 0; index < bulletFollower.Length; index++)
        {
            bulletFollower[index] = Instantiate(bulletFollowerPrefab);
            bulletFollower[index].SetActive(false);
        }
        for (int index = 0; index < enemyB.Length; index++)
        {
            enemyB[index] = Instantiate(enemyBPrefab);
            enemyB[index].SetActive(false);
        }
        for (int index = 0; index < explosion.Length; index++)
        {
            explosion[index] = Instantiate(explosionPrefab);
            explosion[index].SetActive(false);
        }
    }

    //오브젝트풀에 접근할수있는 함수
    //여긴 생성되면 그 수량만큼만 생성해주는거고
    public GameObject MakeObj(string type)
    {
        switch(type)
        {
            case "EnemyL":
                //targetPool 전역변수한개 만들어서 가능
                //for(int index = 0; index < enemyL.Length; index++)
                //{
                //    if (!enemyL[index].activeSelf)
                //    {
                //        enemyL[index].SetActive(true);
                //        return enemyL[index];
                //    }
                //}
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
            case "BulletFollower":
                targetPool = bulletFollower;
                break;
            case "Explosion":
                targetPool = explosion;
                break;
        }
        //아 에너미는 못가져오는 이유 프리팹은 실행되기전엔 없는놈임 실행된후 false로 생성 되니까 이 함수를 실행하려면 
        //에너미가 생성된후에 가져오는게 맞네 그래서 true로 바뀔수가있음 객체도 없는데 true로 바꿀순없잖아
        //여기서 넘겨받은 문자열을
        for (int index = 0; index < targetPool.Length; index++)
        {
            if (!targetPool[index].activeSelf)
            {
                targetPool[index].SetActive(true);
                return targetPool[index];
            }
        }
        //호출했는데 없으면 null반환
        return null;
    }
    public GameObject[] GetPool(string type)
    {
        switch (type)
        {
            case "EnemyL":
                targetPool = enemyL;
                break;
            case "EnemyM":
                targetPool = enemyM;
                break;
            case "EnemyS":
                targetPool = enemyS;
                break;
            case "EnemyB":
                targetPool = enemyB;
                break;
            case "ItemPower":
                targetPool = itemPower;
                break;
            case "ItemCoin":
                targetPool = itemCoin;
                break;
            case "ItemBoom":
                targetPool = itemBoom;
                break;
            case "BulletPlayerA":
                targetPool = bulletPlayerA;
                break;
            case "BulletPlayerB":
                targetPool = bulletPlayerB;
                break;
            case "BulletEnemyA":
                targetPool = bulletEnemyA;
                break;
            case "BulletEnemyB":
                targetPool = bulletEnemyB;
                break;
            case "BulletBossA":
                targetPool = bulletBossA;
                break;
            case "BulletBossB":
                targetPool = bulletBossB;
                break;
            case "BulletFollower":
                targetPool = bulletFollower;
                break;
            case "Explosion":
                targetPool = explosion;
                break;

        }
        return targetPool; 
    }
    public void DeleteAllObj(string type)
    {
        if(type == "Boss")
        {
            for (int index = 0; index < bulletBossA.Length; index++)
                bulletBossA[index].SetActive(false);

            for (int index = 0; index < bulletBossB.Length; index++)
                bulletBossB[index].SetActive(false);
        }
    }
}
