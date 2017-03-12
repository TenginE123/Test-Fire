using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IScrollable, IExplodable {

    public GameObject explodeFab;
    public GameObject target;

    LineRenderer flame;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        target = GameObject.Find("Missile");
        transform.position = new Vector2((int)Random.Range(-400, 400), -600);
        flame = transform.FindChild("Flame").GetComponent<LineRenderer>();
        EventManager.ScrollObjects += Scroll;
    }

    private void OnDisable()
    {
        EventManager.ScrollObjects -= Scroll;
    }

    public void Scroll()
    {
        if(GameManager.Instance.GameMode != GameManager.GameState.Defense)
        {
            gameObject.SetActive(false);
        }
        flame.SetPosition(2, new Vector2(0, (int)-(UnityEngine.Random.Range(6, 12))));

        int speed = GameManager.Instance.ScrollSpeed;               //allow for gamespeed updates at runtime

        var dir = target.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dir.normalized);

        Vector2 velocity = Vector2.up * (speed * Time.deltaTime);
        transform.Translate(velocity);
    }

    //private void CallReset()
    public void Explode()
    {
        AudioManager.Instance.PlayExplode();                                //Play Explode Sound
        Instantiate(explodeFab, transform.position, Quaternion.identity);   //Instantiate Explode Particles
        //Debug.Log("Reset Called");
        //transform.position = new Vector2((int)Random.Range(-400, 400), -600);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collision Check");
        if (collision.tag == "Shield" || collision.tag == "Player")
        {
            //CallReset();
            Explode();
            //Increment Score
        }

        if(collision.tag == "Player")
        {
            target.GetComponent<Missile>().Explode(); //Call player explode
        }
    }
}
