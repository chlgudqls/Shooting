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
    //�̰� �� �����ȵ��� ������ ����������ϴµ�

    public float curShotDelay;
    public float maxShotDelay;
    public int power;
    public int maxPower;
    //���� Ű����� �����Ұǵ� ������ ����
    public int boom;
    public int Maxboom;
    //�Ŀ��� 2�����־ max��������
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

    //�����
    public bool[] joyControl;
    //��������
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
        //Ȱ��ȭ �Ҷ����� true�� 
        Unbeatable();
        //����Ȱ��ȭ�Ŀ� ��Ȱ��ȭ �κ�ũ�� �Ű������� �ȹ޾Ƽ� �Ű������� ���ֹ���
        Invoke("Unbeatable", 3);
    }
    //���޹��� ���� active�̴� true�� �ƴϸ� false��
    void Unbeatable()
    {
        //if���ǿ� isRespawnTime�� �ѱ�°� �˰ڴµ� �� �ݴ�� ����
        //isRespawnTime�� false�� true�϶� �����Ű���� 
        //�������� true�϶� ���� 
        //�̰� �����Ҷ� �÷��װ� �ѹ��ٲ� Invoke�� ������ Ǯ�����Ҽ��־ ���Խ�Ų�� Ʈ�� �޽�
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
        //�ڼ��� ���ذ� �Ȱ� deltatime�� ��������� ������ �ְ� �װ� ������ �����̽ð��� ������ �׶� �߻�Ǵ� �����ε� - ����
        Reload();
    }
    public void JoyPanel(int type)
    {
        for(int index=0; index<9; index++)
        {
            //����������
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


        //h���� �����̴� ���¶�°ǵ� �� 1�ΰ���
        //if ���� �̷������� �������ֳ�
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1) || !isControl)
            h = 0;
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1) || !isControl)
            v = 0;
        Vector3 curPos = transform.position;
        //trasform �̵����� ������ time.deltatime�� ���
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        //�� curPos ���� ���� �����ִ°��� ���� �ȿ����̱���
        transform.position = curPos + nextPos;

        if(Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        anim.SetInteger("Input", (int)h);
    }
    //fireó�� ����
    //���ڱ� case�� �ִ��� �Űܹ����ϱ� ��Ȳ������ Ű�Է��� ���ϱ� �ű�ű��� ���ڸ��� ��°Ÿ� case�ε�
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
        //�±׷� ����� ��� ������Ʈ�� ����
        //�±׷� �����´ٰ� �ĵ� ������ ���ϰڳ� �׳� �±׷� �������� ���� ������ Enemy�� �±׷ΰ��� ��� ��ü�� ��ü ���и���
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //�ε����� �ִ� ���ʹ̵��� ���ʹ�������Ʈ�����ͼ� ������ 1000�Ѱ��� 
        //�ٵ� �׳� targetpool �ƴϱٵ� ��԰������°��� Ȱ��ȭüũ�ؼ� �������µ�
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
        //����� �±װ� EnemyBullet�� ���� �迭�� ��Ƽ� ������ ���̸� �ε����� ��Ƽ� ����
        //���ڿ� ���ʹ� �Ѱܼ� Ȱ��ȭüũ�� Ȱ��ȭ�ȳ�鸸 false�� ��ȯ
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
        //�ٵ� �ֹݴ�� ���� �ݴ�� ���� if�� �ɸ����� ������ ���� ����Ǵ� ��������
        //if (!Input.GetButton("Fire1"))
        //    return;
        if (!isButtonA)
            return;

        if (curShotDelay < maxShotDelay)
            return;

        //�Ѿ��� �Ŀ����� ���⼭ ���� �ֳ� ���⼭ �����ϴϱ�
        //���⼭ �Ѿ���°� äũ�ϴ°� �ƴϰ� �׳� ���������� power�� üũ��
        //case�� 5��������ų� default�� ó��
        switch(power)
        {
            case 0:
                //���ڿ� ���̱⶧���� ���ڸ��ѱ��Ǵµ� enemy�� �迭�̿��⶧���� string���� �ٽø������
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

        //�Ѿ��� ������� 0���� �ʱ�ȭ��Ű�� �ǹ��ε� ������ maxShotDelay�� �������̶�
        curShotDelay = 0;
    }
    void Reload()
    {
        //�̰� �����Ǵ� �ð� �����Ȱ�  �׳�  �����帣�� �ð��̱���
        curShotDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                //������� ���� ������ ���� �÷��� üũ�� �ȴ�
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
            //isRespawnTime�϶� �ȸ��� �̹� isRespawnTime�϶��� ���ߵǳ�
            if (isRespawnTime)
                return;

            if (isHit)
                return;

            
            isHit = true;
            //������ �����ϴϱ� ����
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");
            //�� �̻� �������� ���� ���ᶰ���� ���⼭
            if (life == 0)
            {
                gameManager.GameOver();
            }
            //���� else�� ������
            else
            {
            //�ٵ� �� false ���� ��ġ ��Ű��
            //for�����ϱ� ���������� �����µ�
            gameManager.RespawnPlayer();
            }
            //�̹� ������� �����������
            //gamemanager���� ó��
            gameObject.SetActive(false);
            //���ǿ� �ش��ϴ� collision�� �̹�
            //Destroy(collision.gameObject);

            //collision.gameObject.SetActive(false);
            
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            //Ÿ���� ��ü���� �����Ѱ�� ����ġ�� ��밡��
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    //�Ŀ��� �ִ�� ������ ó��
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
        //������ �� ��ź�� ������ false�� 
        isBoomTime = false;
        //��ư���� �ٲ㼭 
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
    //�������� ������� false�� �ٲ���� �� if���� ��Ÿ���Ϸ���
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                //������� ���� ������ ���� �÷��� üũ�� �ȴ�
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
