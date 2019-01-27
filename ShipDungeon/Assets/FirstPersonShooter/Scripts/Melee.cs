using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour {

    public BoxCollider hitBox;
    private MeleeHitBox meleeBox;

    public float damage;

    public float cooldown;

    public float timeTillHit;

    private bool canAttack = true;

    private void Start()
    {
        try
        {
            meleeBox = hitBox.GetComponent<MeleeHitBox>();
        }
        catch {}
    }

    public void attack()
    {
        if (canAttack)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        canAttack = false;

        yield return new WaitForSeconds(timeTillHit);

        //hitBox.enabled = true;
        ////get all coliding with hit box
        ////hitBox.GetComponent<BoxCollider>().bounds.

        ////send message to damagecolliders


        //hitBox.enabled = false;

        List<GameObject> hitTargets = meleeBox.GetTargets();

        Debug.Log("hitTargets : " + hitTargets.Count);

        DamageHandler DH;

        foreach (var i in hitTargets)
        { 
            DH = i.GetComponent<DamageHandler>();
            DH.Damage(damage, this.transform.position);
        }

        yield return new WaitForSeconds(cooldown - timeTillHit);

        canAttack = true;

    }

}
