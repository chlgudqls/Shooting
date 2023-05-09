using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string type;
    Rigidbody2D rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    //아이템도 활성화 되면 이렇게 데이터 들어가게가능 
    void OnEnable()
    {
        rigid.velocity = Vector2.down * 1;
    }
}
