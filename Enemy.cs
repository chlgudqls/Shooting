using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public int enemyScore;
    //딱 봐도 여기에 객체넣고 hit될때 동작하게
    public Sprite[] sprites;

    //여기서 에너미 스프라이트 정보가져오고
    SpriteRenderer spriteRenderer;
    //속력을 가져와야되기때문에 가져옴
    //Rigidbody2D rigid;
    //아깐 안되던데 이거없어도되네
    //public GameObject bulletObjA;
    //public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public float curShotDelay;
    public float maxShotDelay;
    Animator anim;
    //Think함수 만드는데 갑자기 패턴인덱스 변수생성 
    //같은 패턴에 대한 인덱스변수
    //패턴 실행 횟수라고하네
    public int patternIndex;
    //패턴 반복할거라서 만든다고함 같은 패턴을 여러번 반복하기위해서 만드는변수같은데
    public int curPatternCount;
    //이건 배열로 한다고함 패턴 많이 쓸거라서 
    public int[] maxPatternCount;

    public GameObject player;
    //프리팹은 바로 가져올수없다
    //근데 위에는 게임오브젝트로 가져와놓고 왜여긴 오브젝트메니저임
    //객체의 위치가져오는거랑 ObjectManager
    //이거 왜가져온거지 
    public ObjectManager objectManager;
    //UI합쳐지는 값이 표현되게 ----- 뭔말?

    public GameManager gameManager;

    //객체가 생성될때도 실행되는건가봄 맞네 근데 생성될땐 비활성화니까 조건문에 활성화상태일때만 2초뒤에멈추게함 
    //객체가 활성화되면 실행되는 함수
    void OnEnable()
    {

        //원인 파악은 못하고 급한대로 OnEnable에서 처리했음  보스처럼 바꾸려다가
        switch(enemyName)
        {
            case "B":
                health = 40;
                Invoke("Stop", 2);
                break;
            case "L":
                health = 40;
                spriteRenderer.sprite = sprites[0];
                break;
            case "M":
                health = 10;
                spriteRenderer.sprite = sprites[0];
                break;
            case "S":
                health = 3;
                spriteRenderer.sprite = sprites[0];
                break;
        }    
    }

    void Stop()
    {
        //비활성화 상태일때는 실행하지않겠다
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        //멈춘 이후에 패턴을 돌리기 위한 함수를 추가한다고함
        Invoke("Think", 2);
    }
    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        //-------------이거 빼고 해보기 일단넣고
        //초기화 안하면 밑에서 Think계속 호출하기때문에 나중에 숫자 계속 커진다는 것 같음
        //비교 자체가 안된다고함
        curPatternCount = 0;
        //저번에 위치 잘못해서 실행조차안됨 여기에 놓는건맞았음
        patternIndex = 1;

        //여기서 패턴 구분
        switch (patternIndex)
        {

            case 0:
                //if (health <= 0) return;
                FireFoward();
                break;
            case 1:
                //if (health <= 0) return;
                FireShot();
                break;
            case 2:
                //if (health <= 0) return;
                FireArc();
                break;
            case 3:
                //if (health <= 0) return;
                FireAround();
                break;
        }
    }
    //패턴 함수
    void FireFoward()
    {
        //if (health <= 0) return;
        Debug.Log("앞으로 4발 발사.");
        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;
        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;

        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();


        //이런 류는 속도가 빠르다고함
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        curPatternCount++;

        //이 패턴을 몇번쓸건지 조건문안에 변수사용
        //배열이랑 왜 비교를 하지 
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFoward", 2);
        else
        //공격을 한 뒤에 생각을 해야된다고함 생각한다는게 다른패턴변경한다는건가
        //다 채워지면 생각을 한다고함
        Invoke("Think", 3);

    }
    void FireShot()
    {
        //if (health <= 0) return;
        Debug.Log("플레이어 방향으로 샷건.");
        //보스니까 5발
        for(int index=0; index<5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirc = player.transform.position - bullet.transform.position;
            //위치가 겹치기 때문에 랜덤백터를 생성
            //어떻게 위치를 다르게 하나 했는데 랜덤생성후에
            //y값을 음수로 두니까 총알이 좀 안퍼짐 y값의 간격이 좁아야 많이 퍼지는건지
            Vector2 ranvec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f,2f)) ;
            //더해주면 됨 그냥
            dirc += ranvec;
            rigid.AddForce(dirc.normalized * 5, ForceMode2D.Impulse);
        }
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3);
    }
    void FireArc()
    {
        //if (health <= 0) return;
        Debug.Log("부채모양으로 발사.");

        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        //이거 안하면 어떻게 되는데 뺴고 해보기-------------
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        //왜 -1이지 이것도 바꿔보면 알겠지 ------------------- 방향
        //뭔 함수인지 모르겠다 다시봐야됨 ============================
        //그니까 처음에 흩뿌림 그다음 가고 안돌아옴 원주율곱해서 약간 곡선으로 뿌리고 돌아옴 그다음은 2곱해서 원의둘레 좀더 범위가 넓어짐
        //둘레값을 많이줄수록 빠르게 파형을 그림
        //cos을 써도 시작각도만 다르고 모양자체는 같다
        //원이 y를 중심으로 x가 바뀌기 때문에 -1이였고
        //cos, sin은 시작 각도 
        Vector2 dircVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dircVec.normalized * 5, ForceMode2D.Impulse);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 3);
    }
    void FireAround()
    {
        //if (health <= 0) return;
        Debug.Log("원 형태로 전체 공격.");
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

        for (int index=0; index<roundNumA; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //생성되는 총알의 순번을 활용하여 방향 결정한다고함
            //xy값이 같으면 완전히 일직선이라고함 그래서 Sin으로함 바꿔서 보기 -------------------
            //왜 달라야되는지 바꿔도 모르겠음
            //한번실행할때마다 값이변하는 index를 이용해서 위치값을 바꾼거겠지
            Vector2 dircVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum)
                                         ,Mathf.Sin(Mathf.PI * 2 * index / roundNum ));
            rigid.AddForce(dircVec.normalized * 2, ForceMode2D.Impulse);

            //퍼지는 방향으로 총알을 회전시키려고함
            //이것도 다시 봐야겠네
            //뭐지 이게 외워야되나
            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 0.7f);
        else
            Invoke("Think", 3);
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //옆방향에서 오는 물체도 추가하기위해서 gamemanager로 로직옮김
        //rigid = GetComponent<Rigidbody2D>();
        //rigid.velocity = Vector2.down * speed;

        //보스만 anim쓰니까 조건부걸어놨음
        if(enemyName == "B")
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        //보스는 기존 함수안씀 그래서 뺌
        if (enemyName == "B")
            return;

        Fire();
        //자세히 이해가 안감 deltatime을 현재딜레이 변수에 넣고 그게 정해준 딜레이시간을 넘으면 그때 발사되는 구조인데 - 장전
        Reload();
    }
    public void OnHIt(int dmg)
    {
        //삭제되기전에 두발맞으면 아이템두개나온다고하네 이건 어떻게아는거지
        //캐릭터도 그랬음 두발맞으면 목숨두개나감
        //그러고보니 이전 슈팅게임만들때도 디버그보고알았음 맞으면 체력이 그 이하인데도 더맞고 죽음
        if (health <= 0)
            return;

        health -= dmg;
        
        if (enemyName == "B")
            //trigger는 get이 없음
            anim.SetTrigger("OnHit");
        else
        {
            spriteRenderer.sprite = sprites[1];
            //그냥 호출하면 한프레임에서 스프라이트가 작동하기때문에 바뀐줄도모름 그래서 시간간격을 준다
            Invoke("ReturnSprite", 0.1f);
        }

        if (health <= 0)
        {
            //1. 첫번째는 적이죽었을때 각적들에 부여된 스코어를 넣어주는거고 두번째는 플레이의 스코어변수에 계속해서 쌓이는 데이터를 UI에 넣어주는것임
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            //랜덤으로 아이템 드랍임 변수하나 만듬
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
            if(ran < 5)
            {
                Debug.Log("No Item");
            }
            else if(ran < 6)
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
                //Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);
            }
            else if (ran < 8)
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
                //Instantiate(itemPower, transform.position, itemPower.transform.rotation);
            }
            else if (ran < 10)
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
                //Instantiate(itemBoom, transform.position, itemBoom.transform.rotation);
            }
            //Destroy라서 에러남
;            //Destroy(gameObject);
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);
            //세가지방법 health가 0일때 함수return or 보스죽으면 CancleInvoke
            CancelInvoke();
            //이걸 사용해도 죽고나서 한번 사라질뿐 패턴은 계속 생김 그래서 cancle이랑 같이 써주면됨
            //objectManager.DeleteAllObj("Boss");


            //적이 죽는 로직인데 보스가 마지막에 나오니까 여기에 end로직 추가
            //Boss Kill
            //맨밑이 제일 나중이니까 죽었다 죽은게 B이면 이라는뜻
            if (enemyName == "B")
                gameManager.StageEnd();
        }
        
        //적이 죽으면 점수오르니까 여기에 로직추가

    }
    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //보스는 벽에 닿아도 안없어짐 재등장할때 각도달라지는거아닌가 아니겠네
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            //이게 collision이것만 가져오는게아니고 클래스정보도 가져오는건지
            Bullet bullet = collision.gameObject.GetComponent < Bullet>();
            OnHIt(bullet.dmg);

            //Destroy(collision.gameObject);
            //겨우 이거때문에 한방에 다죽고 점수안오르고 템도안떨굼 이게 뭔지 -------------알아야겠음
            //그리고 또하나 안죽음 방금 맞았는데뭔지
            collision.gameObject.SetActive(false);
        }
    }

    //플레이어 로직 그대로 가져오고 파워,겟버튼 등 필요없는거 지움
    void Fire()
    {
        //근데 왜반대로 쓰지 반대로 쓰면 if에 걸리지만 않으면 밑이 실행되는 구조인지
        
        if (curShotDelay < maxShotDelay)
            return;
        //if문으로 총알을 쏠수있는 객체를 정해둠
        //플레이어 방향으로 총알이 가게하려는데 
        //프리펩이 이미 Scene에 올라온 오브젝트에 접근 불가능해서 문제가 생긴듯
        //1.그래서 생성되고나서 접근하게 gamemanager 로직 추가
        //목표물위치에 자기위치를 뺀값이 왜 목표물 방향인지
        if(enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;

            //Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            //좌표값들이라서 1이 넘어온다고함
            //단위백터가아니라고함 vector3.up 이런것들은 단위백터임 그래서 단위화 시켜야된다고함
            Vector3 dirc = player.transform.position - bullet.transform.position;
            rigid.AddForce(dirc.normalized * 4, ForceMode2D.Impulse);
        }
        else if(enemyName == "L")
        {
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            //Instantiate(bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation);
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;

            //Instantiate(bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();

            Vector3 dircA = player.transform.position - bulletL.transform.position;
            Vector3 dircB = player.transform.position - bulletR.transform.position;
            rigidL.AddForce(dircA.normalized * 4, ForceMode2D.Impulse);
            rigidR.AddForce(dircB.normalized * 4, ForceMode2D.Impulse);

        }

        //총알을 쏜다음에 0으로 초기화시키는 의미인데 이유는 maxShotDelay가 고정값이라서
        curShotDelay = 0;
    }
    void Reload()
    {
        //이게 장전되는 시간 고정된값  그냥  실제흐르는 시간이구나
        curShotDelay += Time.deltaTime;
    }
}
