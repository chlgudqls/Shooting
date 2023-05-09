using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    //충돌이벤트가없어서 삭제로직
    void OnEnable()
    {
        Invoke("Disable",2f);
    }
    void Disable()
    {
        gameObject.SetActive(false);
    }
    //이거 호출은 어떤식으로 하나?
    public void StartExplosion(string target)
    {
        anim.SetTrigger("OnExplosion");

        switch(target)
        {
            case "S":
                transform.localScale = Vector3.one * 0.7f;
                break;
            case "M":
            case "P":
                transform.localScale = Vector3.one * 1f;
                break;
            case "L":
                transform.localScale = Vector3.one * 2f;
                break;
            case "B":
                transform.localScale = Vector3.one * 3f;
                break;
        }
    }
}
