using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public bool isRotate;
    void Update()
    {
        //이거 작성하고 객체에서 체크했음
        //왜냐 체크된 총알만 자기스스로 회전하게 하려고 
        if(isRotate)
        {
            transform.Rotate(Vector3.forward * 10);
        }
    }
    //istrigger 켜서 적끼리 충돌방지 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
            //Destroy(gameObject);
            gameObject.SetActive(false);
    }
}
