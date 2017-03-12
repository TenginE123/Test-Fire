using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour, IExplodable {

    public GameObject shield;
    public GameObject explodeFab;
    public GameObject body;

    int dir = 1;

    public LineRenderer flame;

	// Use this for initialization
	void Awake () {
        body = transform.FindChild("Body").GetComponent<GameObject>();
        flame = transform.FindChild("Flame").GetComponent<LineRenderer>();
        shield = transform.FindChild("Shield Container").GetComponent<GameObject>();
    }

    private void OnEnable()
    {
        //body = GameObject.Find("Body");
        Transform[] gos = GetComponentsInChildren<Transform>(true);

        foreach (Transform go in gos)
        {
            if(go.gameObject.activeInHierarchy == false)
            {
                go.gameObject.SetActive(true);
            }

            switch (go.name)
            {
                case "Body":
                    body = go.gameObject;
                    break;
                case "Flame":
                    flame = go.GetComponent<LineRenderer>();
                    break;
                case "Shield Container":
                    shield = go.gameObject;
                    break;
                default:
                    Debug.Log("Error: MissileScript - Unknown Child Object");
                    break;
            }
        }
        Restart();
        if (shield.activeInHierarchy == true)       //reset shield
        {
            shield.SetActive(false);
        }
        
        EventManager.ChangeDirection += ChangeDir;
        EventManager.Restart += Restart;
    }

    private void OnDisable()
    {
        if (shield.activeInHierarchy == true)       //reset shield
        {
            shield.SetActive(false);
        }
        transform.position = new Vector2(0, -185);  //reset position
        EventManager.ChangeDirection -= ChangeDir;  //remove from changeDir event
        EventManager.Restart -= Restart;
    }
    // Update is called once per frame
    void Update () {
        int speed = GameManager.Instance.ScrollSpeed;

        if (GameManager.Instance.Modes == GameManager.Mode.Game)
        {
            flame.SetPosition(2, new Vector2(0, -(Random.Range(27, 33))));

            if (GameManager.Instance.GameMode == GameManager.GameState.Start)
            {
                //Restart();
            }
            else if (GameManager.Instance.GameMode == GameManager.GameState.Normal)
            {
                if (shield.activeInHierarchy == true)
                {
                    shield.SetActive(false);
                }
                //move missile up or down depending on gameemode
                if(transform.position.y > -185)
                {
                    Vector2 velocity = Vector2.down * (0.5f * speed * Time.deltaTime);
                    transform.Translate(velocity);
                }
            }
            else if (GameManager.Instance.GameMode == GameManager.GameState.Defense)
            {
                if (shield.activeInHierarchy == false)
                {
                    shield.SetActive(true);
                }
                //shield.transform.Rotate(Vector3.forward * (dir * ((speed * 0.5f) * Time.deltaTime)));    //Spin if true
                shield.transform.Rotate(Vector3.forward * (dir * ((speed) * Time.deltaTime)));    //Spin if true
                //move missile up or down depending on gameemode
                if (transform.position.y < 185)
                {
                    Vector2 velocity = Vector2.up * (0.5f * speed * Time.deltaTime);
                    transform.Translate(velocity);
                }
            }
        }
	}

    void ChangeDir() //removed "ref int dir" to allow for subscription to change direction event
    {
        dir = -dir; //reverse movement direction
        //can be added to a "ChangeDirection" event to be called whenever the player taps the screen
    }

    

    public void Explode()
    {
        Debug.Log("Player Exploded!");
        AudioManager.Instance.StopMain();
        AudioManager.Instance.PlayExplode();                                //Play Explode Sound
        Instantiate(explodeFab, transform.position, Quaternion.identity);   //Instantiate Explode Particles
        if (shield.activeInHierarchy == true)       //reset shield
        {
            shield.SetActive(false);
        }

        if (flame.gameObject.activeInHierarchy == true)
        {
            flame.gameObject.SetActive(false);
        }

        //if (GetComponentsInChildren<GameObject>().activeInHierarchy == true)
        if (body.activeInHierarchy == true)
        {
            body.SetActive(false);            
            //GetComponentInChildren<GameObject>().SetActive(false);
        }
        //GameManager.Instance.PauseGame();
        //GameManager.Instance.RestartGame();
        //GameManager.Instance.SetGameMode("Finish");
        GameManager.Instance.SetGameMode("GameOver");
        UIManager.Instance.Hide("GameUI");
        Invoke("CallUIGameOver", 2);
    }


    void Restart()
    {
        if (shield.activeInHierarchy == true)       //reset shield
        {
            shield.SetActive(false);
        }

        if (body.activeInHierarchy == false)
        {
            body.SetActive(true);
        }

        if (flame.gameObject.activeInHierarchy == false)
        {
            flame.gameObject.SetActive(true);
        }
        //if (gameObject.activeInHierarchy == false)
        //{
        //    gameObject.SetActive(true);
        //}
        transform.position = new Vector2(0, -185);  //reset position
    }

    void SetSheldActive()
    {

    }

    void CallUIGameOver()
    {
        UIManager.Instance.GameOver();
    }
}
