using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;
    Animator anim;
    //이거 왜 지우면안되지 어차피 여기생성안하는데

    public float curShotDelay;
    public float maxShotDelay;
    public int power;
    public int maxPower;
    //붐을 키보드로 제어할건데 변수를 만듬
    public int boom;
    public int Maxboom;
    //파워가 2까지있어서 max변수생성
    public GameManager gameManager;
    public ObjectManager objectManager;
    public GameObject boomEffect;
    public int score;
    public int life;
    public bool isHit;
    public bool isBoomTime;
    public GameObject[] followers;
    
    public bool isRespawnTime;
    SpriteRenderer spriteRenderer;

    //어딘지
    public bool[] joyControl;
    //눌렀는지
    public bool isControl;
    public bool isButtonA;
    public bool isButtonB;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        //활성화 할때마다 true로 
        Unbeatable();
        //무적활성화후엔 비활성화 인보크가 매개변수를 안받아서 매개변수를 없애버림
        Invoke("Unbeatable", 3);
    }
    //전달받은 값이 active이다 true로 아니면 false로
    void Unbeatable()
    {
        //if조건에 isRespawnTime를 넘기는건 알겠는데 왜 반대로 했지
        //isRespawnTime가 false라서 true일때 실행시키려고 
        //이전에도 true일때 실행 
        //이건 실행할때 플래그가 한번바뀌어서 Invoke로 무적이 풀리게할수있어서 대입시킨후 트루 펄스
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            for (int index = 0; index < followers.Length; index++)
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            for (int index = 0; index < followers.Length; index++)
                followers[index].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    }    
    void Update()
    {
        Move();
        Fire();
        Boom();
        //자세히 이해가 안감 deltatime을 현재딜레이 변수에 넣고 그게 정해준 딜레이시간을 넘으면 그때 발사되는 구조인데 - 장전
        Reload();
    }
    public void JoyPanel(int type)
    {
        for(int index=0; index<9; index++)
        {
            //무슨뜻이지
            joyControl[index] = index == type;
        }
    } 
    public void JoyDown()
    {
        isControl = true;
    }
    public void JoyUp()
    {
        isControl = false;
    }

    void Move()
    { 
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (joyControl[0]) { h = -1; v = 1; }
        if (joyControl[1]) { h = 0; v = 1; }
        if (joyControl[2]) { h = 1; v = 1; }
        if (joyControl[3]) { h = -1; v = 0; }
        if (joyControl[4]) { h = 0; v = 0; }
        if (joyControl[5]) { h = 1; v = 0; }
        if (joyControl[6]) { h = -1; v = -1; }
        if (joyControl[7]) { h = 0; v = -1; }
        if (joyControl[8]) { h = 1; v = -1; }


        //h값이 움직이는 상태라는건데 왜 1인거지
        //if 문을 이런식으로 쓸수가있네
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1) || !isControl)
            h = 0;
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1) || !isControl)
            v = 0;
        Vector3 curPos = transform.position;
        //trasform 이동에는 무조건 time.deltatime을 사용
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        //왜 curPos 까지 만들어서 더해주는건지 빼면 안움직이긴함
        transform.position = curPos + nextPos;

        if(Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        anim.SetInteger("Input", (int)h);
    }
    //fire처럼 구현
    //갑자기 case에 있던걸 옮겨버리니까 당황스럽네 키입력이 들어가니까 옮긴거구나 먹자마자 쏘는거면 case인데
    void Boom()
    {
        //if (!Input.GetButton("Fire2"))
        //    return;
        if (!isButtonB)
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);

        Invoke("offBoomEffect", 3f);
        //태그로 장면의 모든 오브젝트를 추출
        //태그로 가져온다고 쳐도 구분은 못하겠네 그냥 태그로 가져오고 모든걸 가져옴 Enemy를 태그로가진 모든 객체들 기체 구분못함
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //인덱스에 있는 에너미들의 에너미컴포넌트가져와서 데미지 1000넘겨줌 
        //근데 그냥 targetpool 아니근데 어떻게가져오는거지 활성화체크해서 가져오는데
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int index = 0; index < enemiesL.Length; index++)
        {
            if(enemiesL[index].activeSelf)
            {
            Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
            enemyLogic.OnHIt(1000);
            }
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHIt(1000);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHIt(1000);
            }
        }
        //여기는 태그가 EnemyBullet인 모든걸 배열에 담아서 변수의 길이를 인덱스에 담아서 삭제
        //문자열 에너미 넘겨서 활성화체크후 활성화된놈들만 false로 반환
        //GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");

        for (int index = 0; index < bulletsA.Length; index++)
        {
            if(bulletsA[index].activeSelf)
            {
                //Destroy(bullets[index]);
                bulletsA[index].SetActive(false);
            }
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if(bulletsB[index].activeSelf)
            {
                //Destroy(bullets[index]);
                bulletsB[index].SetActive(false);
            }
        }
    }
    public void ButtonADown()
    {
        isButtonA = true;
    }
    public void ButtonAUp()
    {
        isButtonA = false;
    }
    public void ButtonBDown()
    {
        isButtonB = true;
    }
    void Fire()
    {
        //근데 왜반대로 쓰지 반대로 쓰면 if에 걸리지만 않으면 밑이 실행되는 구조인지
        //if (!Input.GetButton("Fire1"))
        //    return;
        if (!isButtonA)
            return;

        if (curShotDelay < maxShotDelay)
            return;

        //총알의 파워업은 여기서 변함 왜냐 여기서 생성하니까
        //여기서 넘어오는걸 채크하는게 아니고 그냥 전역변수인 power를 체크함
        //case를 5까지만들거나 default로 처리
        switch(power)
        {
            case 0:
                //문자열 비교이기때문에 문자만넘기면되는데 enemy는 배열이였기때문에 string으로 다시만든거임
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                    //Instantiate(bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
            break;
            case 1:
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                //Instantiate(bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation);
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;

                //Instantiate(bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            //case 2:
            //case 3:
            //case 4:
            //case 5:
            default:
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.3f;

                //Instantiate(bulletObjA, transform.position + Vector3.left * 0.3f, transform.rotation);
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;

                //Instantiate(bulletObjB, transform.position, transform.rotation);
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.3f;

                //Instantiate(bulletObjA, transform.position + Vector3.right * 0.3f, transform.rotation);
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        //총알을 쏜다음에 0으로 초기화시키는 의미인데 이유는 maxShotDelay가 고정값이라서
        curShotDelay = 0;
    }
    void Reload()
    {
        //이게 장전되는 시간 고정된값  그냥  실제흐르는 시간이구나
        curShotDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                //닿았을때 닿은 부위에 따라 플래그 체크가 된다
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;

            }
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            //isRespawnTime일때 안맞음 이미 isRespawnTime일때로 봐야되나
            if (isRespawnTime)
                return;

            if (isHit)
                return;

            
            isHit = true;
            //닿으면 감소하니까 맞음
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");
            //더 이상 떨어지면 게임 종료떠야지 여기서
            if (life == 0)
            {
                gameManager.GameOver();
            }
            //굳이 else는 왜하지
            else
            {
            //근데 왜 false 위에 위치 시키지
            //for문빼니까 리스폰으로 빠지는데
            gameManager.RespawnPlayer();
            }
            //이미 사라져서 실행되지않음
            //gamemanager에서 처리
            gameObject.SetActive(false);
            //조건에 해당하는 collision임 이미
            //Destroy(collision.gameObject);

            //collision.gameObject.SetActive(false);
            
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            //타입을 객체마다 지정한경우 스위치문 사용가능
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    //파워가 최대면 점수로 처리
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == Maxboom)
                        score += 500;
                    else
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    break;
            }
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
        }
    }
    void offBoomEffect()
    {
        boomEffect.SetActive(false);
        //다음에 또 폭탄을 쓰려면 false로 
        isBoomTime = false;
        //버튼으로 바꿔서 
        isButtonB = false;
    }
    void AddFollower()
    {
        if (power == 3)
            followers[0].SetActive(true);
        else if (power == 4)
            followers[1].SetActive(true);
        else if (power == 5)
            followers[2].SetActive(true);

    }
    //보더에서 벗어낫을때 false로 바꿔줘야 됨 if문을 안타게하려면
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                //닿았을때 닿은 부위에 따라 플래그 체크가 된다
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;

            }

        }
    }


}
