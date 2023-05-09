using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    //스크롤링 기법을 사용하기위해서 변수선언
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;
    //viewHeight 값바꿔보기
    float viewHeight;

    void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }
    void Update()
    {
        //다 하고 함수로 분리
        Move();
        Scrolling();
        
    }
    void Move()
    {
        Vector3 curPos = transform.position;
        //이전에 현재 위치에서 다음위치더해야 이동하는거알긴암 근데 델타타임은 왜곱함 아 계속이동해야되니까 델타타임은 시계임
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }
    void Scrolling()
    {
        if (sprites[endIndex].position.y < viewHeight * (-1))
        {
            //로컬인이유 자식객체라서 부모기준으로 위치시킴
            //글로벌에서 보면 y값이 다른이유 
            //그럼 카메라 size가 달라지면 
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            //transform빼도되나 빼도 정상적으로됨
            sprites[endIndex].localPosition = backSpritePos + Vector3.up * viewHeight;

            int startSaveIndex = startIndex;
            startIndex = endIndex;

            //이건 계산식과 배열순서와도 잘 맞아떨어져서 되는거임  
            endIndex = startSaveIndex - 1 == -1 ? sprites.Length - 1 : startSaveIndex - 1;
        }
    }
} 
