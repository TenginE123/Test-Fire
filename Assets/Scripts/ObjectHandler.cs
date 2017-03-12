using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandler : MonoBehaviour, IScrollable {

    //holds information about the obstacle objects that it is parent to at initialization
    //moves all child objects down
    int speed;

    private void OnEnable()
    {
        //call line drawing function here?
        //pass the obstacle type and its values
        speed = GameManager.Instance.ScrollSpeed;
        transform.position = ResetPos(transform.position); //reset the position
        EventManager.ScrollObjects += Scroll;
    }

    private void OnDisable()
    {
        EventManager.ScrollObjects -= Scroll;
        transform.position = ResetPos(transform.position); //reset the position //Possibly causing visual glitch
        //set active to false      
    }

    public void Scroll()
    {
        //disable obstacle once it reaches the bottom of the game space
        if (transform.position.y < -400)
        {
            gameObject.SetActive(false);
            //Call "IncrementScore" function
            //Call "Decrement NumObstacles" value
        }

        speed = GameManager.Instance.ScrollSpeed;               //allow for gamespeed updates at runtime
        Vector2 velocity = Vector2.down * (speed * Time.deltaTime); // * dir; removed. all objects scroll down, and only down

        transform.Translate(velocity);
    }

    Vector2 ResetPos(Vector2 pos)
    {
        pos = new Vector2(0, 400);
        return pos;
    }

    //Container "Constructor" Method.
    public void BuildContainer()
    {
        //receive an array of child objects
    }
}
