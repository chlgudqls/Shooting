using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public bool isRotate;
    void Update()
    {
        //�̰� �ۼ��ϰ� ��ü���� üũ����
        //�ֳ� üũ�� �Ѿ˸� �ڱ⽺���� ȸ���ϰ� �Ϸ��� 
        if(isRotate)
        {
            transform.Rotate(Vector3.forward * 10);
        }
    }
    //istrigger �Ѽ� ������ �浹���� 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
            //Destroy(gameObject);
            gameObject.SetActive(false);
    }
}
