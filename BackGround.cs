using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float speed;
    //��ũ�Ѹ� ����� ����ϱ����ؼ� ��������
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;
    //viewHeight ���ٲ㺸��
    float viewHeight;

    void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }
    void Update()
    {
        //�� �ϰ� �Լ��� �и�
        Move();
        Scrolling();
        
    }
    void Move()
    {
        Vector3 curPos = transform.position;
        //������ ���� ��ġ���� ������ġ���ؾ� �̵��ϴ°ž˱�� �ٵ� ��ŸŸ���� �ְ��� �� ����̵��ؾߵǴϱ� ��ŸŸ���� �ð���
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }
    void Scrolling()
    {
        if (sprites[endIndex].position.y < viewHeight * (-1))
        {
            //���������� �ڽİ�ü�� �θ�������� ��ġ��Ŵ
            //�۷ι����� ���� y���� �ٸ����� 
            //�׷� ī�޶� size�� �޶����� 
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            //transform�����ǳ� ���� ���������ε�
            sprites[endIndex].localPosition = backSpritePos + Vector3.up * viewHeight;

            int startSaveIndex = startIndex;
            startIndex = endIndex;

            //�̰� ���İ� �迭�����͵� �� �¾ƶ������� �Ǵ°���  
            endIndex = startSaveIndex - 1 == -1 ? sprites.Length - 1 : startSaveIndex - 1;
        }
    }
} 
