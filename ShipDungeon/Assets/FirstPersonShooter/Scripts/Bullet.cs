using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {


    public float damage;


    //on collision
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.GetComponent<DamageHandler>())
{
            DamageHandler dh = col.gameObject.GetComponent<DamageHandler>();
            dh.Damage(damage, this.transform.position);
            Destroy(this.gameObject);
        }
        else if(col.gameObject.GetComponentInParent<DamageHandler>())
        {
            DamageHandler dh = col.gameObject.GetComponentInParent<DamageHandler>();
            dh.Damage(damage, this.transform.position);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

}
