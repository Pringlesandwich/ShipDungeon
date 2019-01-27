using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageHandler : MonoBehaviour {


    public float health;

    private float launchMulitplyer = 1;

    private bool dead = false;

    public Image healthBar;
    public Image oxygenBar;
    public float oxygen;
    private float deltaOxygen;
    public float oxygenDeplition;

    private Vector3 fill = new Vector3(0, 0, 0);

    private void Start()
    {
        deltaOxygen = oxygen;
    }

    private void Update()
    {
        deltaOxygen -= Time.deltaTime * oxygenDeplition;
        //oxygen
        try
        {
            oxygenBar.fillAmount = deltaOxygen / oxygen;
        }
        catch { }
        if (deltaOxygen < 0.0f)
        {
            if (GetComponent<PlayerController>())
            {
                var player = GetComponent<PlayerController>();
                player.Kill();
            }
        }
    }

    public void Damage(float amount, Vector3? hitLocation)
    {
        if (!dead)
        {
            health -= amount;
            //if (health > 0)
            //    Debug.Log("Health Remaining: " + health);
            try
            {
                var controller = GetComponent<EnemyController>();
                controller.isAgro = true;
                controller.Hit();
            }
            catch { }

            //health bar
            try
            {
                if (health > 0)
                {
                    healthBar.fillAmount = (health / 100.0f);
                }
                else
                {
                    healthBar.fillAmount = 0;
                }
            }
            catch { }

            if (health <= 0)
            {
                //Destroy(this.gameObject);
                Debug.Log(this.name + " Is Dead");
                launchMulitplyer = 5;
                dead = true;
                try
                {
                    var controller = GetComponent<PlayerController>();
                    controller.Kill();
                }
                catch { }
                try
                {
                    var controller = GetComponent<EnemyController>();
                    controller.Kill();
                }
                catch { }
            }
        }

        //prototype - just for fun
        try
        {
            var rb = GetComponent<Rigidbody>();
            if (dead)
            {
                rb.AddForce(Vector3.up * 200.0f); // juggle mechanic
            }
            Vector3 newHitLocation = hitLocation ?? this.transform.position;
            Vector3 direction = this.transform.position - newHitLocation;
            try
            {
                //only launch enemies on hit
                if (GetComponent<EnemyController>())
                {
                    rb.AddForce(direction * (100.0f * launchMulitplyer));
                }
            }
            catch { }
                       
        }
        catch
        {
            //no rb
        }

    }




}
