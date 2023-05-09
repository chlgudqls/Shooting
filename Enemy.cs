using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public int enemyScore;
    //�� ���� ���⿡ ��ü�ְ� hit�ɶ� �����ϰ�
    public Sprite[] sprites;

    //���⼭ ���ʹ� ��������Ʈ ������������
    SpriteRenderer spriteRenderer;
    //�ӷ��� �����;ߵǱ⶧���� ������
    //Rigidbody2D rigid;
    //�Ʊ� �ȵǴ��� �̰ž���ǳ�
    //public GameObject bulletObjA;
    //public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public float curShotDelay;
    public float maxShotDelay;
    Animator anim;
    //Think�Լ� ����µ� ���ڱ� �����ε��� �������� 
    //���� ���Ͽ� ���� �ε�������
    //���� ���� Ƚ������ϳ�
    public int patternIndex;
    //���� �ݺ��ҰŶ� ����ٰ��� ���� ������ ������ �ݺ��ϱ����ؼ� ����º���������
    public int curPatternCount;
    //�̰� �迭�� �Ѵٰ��� ���� ���� ���Ŷ� 
    public int[] maxPatternCount;

    public GameObject player;
    //�������� �ٷ� �����ü�����
    //�ٵ� ������ ���ӿ�����Ʈ�� �����ͳ��� �ֿ��� ������Ʈ�޴�����
    //��ü�� ��ġ�������°Ŷ� ObjectManager
    //�̰� �ְ����°��� 
    public ObjectManager objectManager;
    //UI�������� ���� ǥ���ǰ� ----- ����?

    public GameManager gameManager;

    //��ü�� �����ɶ��� ����Ǵ°ǰ��� �³� �ٵ� �����ɶ� ��Ȱ��ȭ�ϱ� ���ǹ��� Ȱ��ȭ�����϶��� 2�ʵڿ����߰��� 
    //��ü�� Ȱ��ȭ�Ǹ� ����Ǵ� �Լ�
    void OnEnable()
    {

        //���� �ľ��� ���ϰ� ���Ѵ�� OnEnable���� ó������  ����ó�� �ٲٷ��ٰ�
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
        //��Ȱ��ȭ �����϶��� ���������ʰڴ�
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        //���� ���Ŀ� ������ ������ ���� �Լ��� �߰��Ѵٰ���
        Invoke("Think", 2);
    }
    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        //-------------�̰� ���� �غ��� �ϴְܳ�
        //�ʱ�ȭ ���ϸ� �ؿ��� Think��� ȣ���ϱ⶧���� ���߿� ���� ��� Ŀ���ٴ� �� ����
        //�� ��ü�� �ȵȴٰ���
        curPatternCount = 0;
        //������ ��ġ �߸��ؼ� ���������ȵ� ���⿡ ���°Ǹ¾���
        patternIndex = 1;

        //���⼭ ���� ����
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
    //���� �Լ�
    void FireFoward()
    {
        //if (health <= 0) return;
        Debug.Log("������ 4�� �߻�.");
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


        //�̷� ���� �ӵ��� �����ٰ���
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        curPatternCount++;

        //�� ������ ��������� ���ǹ��ȿ� �������
        //�迭�̶� �� �񱳸� ���� 
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFoward", 2);
        else
        //������ �� �ڿ� ������ �ؾߵȴٰ��� �����Ѵٴ°� �ٸ����Ϻ����Ѵٴ°ǰ�
        //�� ä������ ������ �Ѵٰ���
        Invoke("Think", 3);

    }
    void FireShot()
    {
        //if (health <= 0) return;
        Debug.Log("�÷��̾� �������� ����.");
        //�����ϱ� 5��
        for(int index=0; index<5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirc = player.transform.position - bullet.transform.position;
            //��ġ�� ��ġ�� ������ �������͸� ����
            //��� ��ġ�� �ٸ��� �ϳ� �ߴµ� ���������Ŀ�
            //y���� ������ �δϱ� �Ѿ��� �� ������ y���� ������ ���ƾ� ���� �����°���
            Vector2 ranvec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f,2f)) ;
            //�����ָ� �� �׳�
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
        Debug.Log("��ä������� �߻�.");

        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        //�̰� ���ϸ� ��� �Ǵµ� ���� �غ���-------------
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        //�� -1���� �̰͵� �ٲ㺸�� �˰��� ------------------- ����
        //�� �Լ����� �𸣰ڴ� �ٽú��ߵ� ============================
        //�״ϱ� ó���� ��Ѹ� �״��� ���� �ȵ��ƿ� ���������ؼ� �ణ ����� �Ѹ��� ���ƿ� �״����� 2���ؼ� ���ǵѷ� ���� ������ �о���
        //�ѷ����� �����ټ��� ������ ������ �׸�
        //cos�� �ᵵ ���۰����� �ٸ��� �����ü�� ����
        //���� y�� �߽����� x�� �ٲ�� ������ -1�̿���
        //cos, sin�� ���� ���� 
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
        Debug.Log("�� ���·� ��ü ����.");
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;

        for (int index=0; index<roundNumA; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //�����Ǵ� �Ѿ��� ������ Ȱ���Ͽ� ���� �����Ѵٰ���
            //xy���� ������ ������ �������̶���� �׷��� Sin������ �ٲ㼭 ���� -------------------
            //�� �޶�ߵǴ��� �ٲ㵵 �𸣰���
            //�ѹ������Ҷ����� ���̺��ϴ� index�� �̿��ؼ� ��ġ���� �ٲ۰Ű���
            Vector2 dircVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum)
                                         ,Mathf.Sin(Mathf.PI * 2 * index / roundNum ));
            rigid.AddForce(dircVec.normalized * 2, ForceMode2D.Impulse);

            //������ �������� �Ѿ��� ȸ����Ű������
            //�̰͵� �ٽ� ���߰ڳ�
            //���� �̰� �ܿ��ߵǳ�
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
        //�����⿡�� ���� ��ü�� �߰��ϱ����ؼ� gamemanager�� �����ű�
        //rigid = GetComponent<Rigidbody2D>();
        //rigid.velocity = Vector2.down * speed;

        //������ anim���ϱ� ���Ǻΰɾ����
        if(enemyName == "B")
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        //������ ���� �Լ��Ⱦ� �׷��� ��
        if (enemyName == "B")
            return;

        Fire();
        //�ڼ��� ���ذ� �Ȱ� deltatime�� ��������� ������ �ְ� �װ� ������ �����̽ð��� ������ �׶� �߻�Ǵ� �����ε� - ����
        Reload();
    }
    public void OnHIt(int dmg)
    {
        //�����Ǳ����� �ι߸����� �����۵ΰ����´ٰ��ϳ� �̰� ��Ծƴ°���
        //ĳ���͵� �׷��� �ι߸����� ����ΰ�����
        //�׷����� ���� ���ð��Ӹ��鶧�� ����׺���˾��� ������ ü���� �� �����ε��� ���°� ����
        if (health <= 0)
            return;

        health -= dmg;
        
        if (enemyName == "B")
            //trigger�� get�� ����
            anim.SetTrigger("OnHit");
        else
        {
            spriteRenderer.sprite = sprites[1];
            //�׳� ȣ���ϸ� �������ӿ��� ��������Ʈ�� �۵��ϱ⶧���� �ٲ��ٵ��� �׷��� �ð������� �ش�
            Invoke("ReturnSprite", 0.1f);
        }

        if (health <= 0)
        {
            //1. ù��°�� �����׾����� �����鿡 �ο��� ���ھ �־��ִ°Ű� �ι�°�� �÷����� ���ھ���� ����ؼ� ���̴� �����͸� UI�� �־��ִ°���
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            //�������� ������ ����� �����ϳ� ����
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
            //Destroy�� ������
;            //Destroy(gameObject);
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);
            //��������� health�� 0�϶� �Լ�return or ���������� CancleInvoke
            CancelInvoke();
            //�̰� ����ص� �װ��� �ѹ� ������� ������ ��� ���� �׷��� cancle�̶� ���� ���ָ��
            //objectManager.DeleteAllObj("Boss");


            //���� �״� �����ε� ������ �������� �����ϱ� ���⿡ end���� �߰�
            //Boss Kill
            //�ǹ��� ���� �����̴ϱ� �׾��� ������ B�̸� �̶�¶�
            if (enemyName == "B")
                gameManager.StageEnd();
        }
        
        //���� ������ ���������ϱ� ���⿡ �����߰�

    }
    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //������ ���� ��Ƶ� �Ⱦ����� ������Ҷ� �����޶����°žƴѰ� �ƴϰڳ�
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            //�̰� collision�̰͸� �������°Ծƴϰ� Ŭ���������� �������°���
            Bullet bullet = collision.gameObject.GetComponent < Bullet>();
            OnHIt(bullet.dmg);

            //Destroy(collision.gameObject);
            //�ܿ� �̰Ŷ����� �ѹ濡 ���װ� �����ȿ����� �۵��ȶ��� �̰� ���� -------------�˾ƾ߰���
            //�׸��� ���ϳ� ������ ��� �¾Ҵµ�����
            collision.gameObject.SetActive(false);
        }
    }

    //�÷��̾� ���� �״�� �������� �Ŀ�,�ٹ�ư �� �ʿ���°� ����
    void Fire()
    {
        //�ٵ� �ֹݴ�� ���� �ݴ�� ���� if�� �ɸ����� ������ ���� ����Ǵ� ��������
        
        if (curShotDelay < maxShotDelay)
            return;
        //if������ �Ѿ��� ����ִ� ��ü�� ���ص�
        //�÷��̾� �������� �Ѿ��� �����Ϸ��µ� 
        //�������� �̹� Scene�� �ö�� ������Ʈ�� ���� �Ұ����ؼ� ������ �����
        //1.�׷��� �����ǰ��� �����ϰ� gamemanager ���� �߰�
        //��ǥ����ġ�� �ڱ���ġ�� ������ �� ��ǥ�� ��������
        if(enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;

            //Instantiate(bulletObjA, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            //��ǥ�����̶� 1�� �Ѿ�´ٰ���
            //�������Ͱ��ƴ϶���� vector3.up �̷��͵��� ���������� �׷��� ����ȭ ���Ѿߵȴٰ���
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

        //�Ѿ��� ������� 0���� �ʱ�ȭ��Ű�� �ǹ��ε� ������ maxShotDelay�� �������̶�
        curShotDelay = 0;
    }
    void Reload()
    {
        //�̰� �����Ǵ� �ð� �����Ȱ�  �׳�  �����帣�� �ð��̱���
        curShotDelay += Time.deltaTime;
    }
}
