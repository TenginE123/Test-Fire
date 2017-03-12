using UnityEngine;

public class ParticleBehaviour : MonoBehaviour {

	void Update () {
        //ParticleSystem parts = GetComponent<ParticleSystem>();
        if (!IsInvoking("Kill"))
        {
            Invoke("Kill", 1);
        }
    }

    void Kill()
    {
        Destroy(this.gameObject);
    }
}
