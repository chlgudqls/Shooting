using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float curShotDelay;
    public float maxShotDelay;

    public ObjectManager objectManager;

    //이런 변수를 어떻게 생각하지 엄두가 안나는데
    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    //queue나옴
    public Queue<Vector3> parentPos;

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }
    void Update()
    {
        Watch();
        Follow();
        Fire();
        //자세히 이해가 안감 deltatime을 현재딜레이 변수에 넣고 그게 정해준 딜레이시간을 넘으면 그때 발사되는 구조인데 - 장전
        Reload();
    }
    void Watch()
    {
        //FIFO == QUEUE
        //Input Pos
        //멈춰있어도 집어넣는다고하는데 집어넣는시간이있는건가
        //그게 프레임단위고 아 업데이트문에서 실행해서 그런거고 그래서 딜레이에 50넣으면 50프레임뒤에 따라옴
        //if로 멈춰있을때 enqueue안되게
        //멈춰있다는걸 비교하는데 contains로 했네 위치를넘겨서 그값이없으면 값이 더해짐
        if (!parentPos.Contains(parent.position))
        parentPos.Enqueue(parent.position);

        //Output Pos
        //큐에 일정갯수 채워지면 그 떄부터반환 딜레이가 생김
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
    }
    void Follow()
    {
        transform.position = followPos;
    }
    void Fire()
    {
        //근데 왜반대로 쓰지 반대로 쓰면 if에 걸리지만 않으면 밑이 실행되는 구조인지
        if (!Input.GetButton("Fire1"))
            return;
        if (curShotDelay < maxShotDelay)
            return;

        //총알의 파워업은 여기서 변함 왜냐 여기서 생성하니까
     
                //문자열 비교이기때문에 문자만넘기면되는데 enemy는 배열이였기때문에 string으로 다시만든거임
                GameObject bullet = objectManager.MakeObj("BulletFollower");
                bullet.transform.position = transform.position;

                //Instantiate(bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);


              

        //총알을 쏜다음에 0으로 초기화시키는 의미인데 이유는 maxShotDelay가 고정값이라서
        curShotDelay = 0;
    }
    void Reload()
    {
        //이게 장전되는 시간 고정된값  그냥  실제흐르는 시간이구나
        curShotDelay += Time.deltaTime;
    }
}
