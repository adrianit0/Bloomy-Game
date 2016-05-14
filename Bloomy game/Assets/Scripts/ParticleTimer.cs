using UnityEngine;
using System.Collections;

public class ParticleTimer : MonoBehaviour {

    ParticleSystem particle;
    float tiempoPrudencial = 5;

	void Start () {
        particle = GetComponent<ParticleSystem>();
        Destroy(this.gameObject, (particle != null) ? particle.duration : tiempoPrudencial);
	}
}
