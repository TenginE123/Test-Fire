using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : Singleton<PoolManager> {

    public GameObject objectContainer;
    int objectsToSpawn      = 18;
    int containersToSpawn   = 6;
    int projectilesToSpawn  = 6;

    //obstacle object
    public GameObject objectFab;                                        //pool object prefab
    List<GameObject> objectList     = new List<GameObject>();        //list to contain prefab ref's //public allows access by other classes

    //container object
    public GameObject containerFab;
    List<GameObject> containerList  = new List<GameObject>();

    //projectile object
    public GameObject projectileFab;
    List<GameObject> projectileList = new List<GameObject>();

    // Use this for initialization
    void Start () {
		for(int i = 0; i < objectsToSpawn; i++){
            //instantiate objects
			GameObject obj = Instantiate(objectFab, Vector3.zero, Quaternion.identity) as GameObject;
			obj.transform.parent = objectContainer.transform;
			obj.SetActive(false);
			objectList.Add(obj);
        }
        for (int i = 0; i < containersToSpawn; i++)
        {
            //instantiate containers
            GameObject cont = Instantiate(containerFab, Vector3.zero, Quaternion.identity) as GameObject;
            cont.transform.parent = objectContainer.transform;
            cont.SetActive(false);
            containerList.Add(cont);
        }
        for (int i = 0; i < projectilesToSpawn; i++)
        {
            //instantiate projectiles
            GameObject proj = Instantiate(projectileFab, Vector3.zero, Quaternion.identity) as GameObject;
            proj.transform.parent = objectContainer.transform;
            proj.SetActive(false);
            projectileList.Add(proj);
        }
    }

    //Instead of constant updates
    //Create an event that triggers when the game state changes?
    void Update()
    {
        if(GameManager.Instance.Modes == GameManager.Mode.Game)
        {
            if (GameManager.Instance.GameMode != GameManager.GameState.Normal)
            {
                if (!IsInvoking("DisableObjects"))  //added invocation to allow existing objects to scroll out of site
                {
                    Invoke("DisableObjects", 2);
                }
            }
        }
        else
        {
            DisableObjects();
        }
    }

    //External access for returning an available Obstacle object
    public GameObject GetObstacle()
    {
        for (int i = 0; i < Instance.objectList.Count; i++)
        {
            if (Instance.objectList[i].activeInHierarchy == false)
            {
                Instance.objectList[i].SetActive(true);
                return Instance.objectList[i] as GameObject;
                //break; //exit the loop!!!
            }
        }

        Debug.Log("Error: PoolManager has run out of pooled OBSTACLE objects.");
        return null;
    }

    //External access for returning an available Container object
    public GameObject GetContainer()
    {
        for (int i = 0; i < Instance.containerList.Count; i++)
        {
            if (Instance.containerList[i].activeInHierarchy == false)
            {
                Instance.containerList[i].SetActive(true);
                return Instance.containerList[i] as GameObject;
                //break; //exit the loop!!!
            }
        }

        Debug.Log("Error: PoolManager has run out of pooled CONTAINER objects.");
        return null;
    }

    //External access for returning an available Container object
    public GameObject GetProjectile()
    {
        for (int i = 0; i < Instance.projectileList.Count; i++)
        {
            if (Instance.projectileList[i].activeInHierarchy == false)
            {
                Instance.projectileList[i].SetActive(true);
                return Instance.projectileList[i] as GameObject;
                //break; //exit the loop!!!
            }
        }

        Debug.Log("Error: PoolManager has run out of pooled PROJECTILE objects.");
        return null;
    }

    void DisableObjects()
    {
        foreach (GameObject obst in objectList)
        {
            obst.SetActive(false);
        }
    } 
}
