using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    public AudioClip sonidoBoton;
    AudioSource audioSource;

    void Awake () {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sonidoBoton;
    }

	public void CambiarEscena (string escena) {
        audioSource.Play();
        Application.LoadLevel(escena);
	}

    public void AbrirCerrarPanel (GameObject obj) {
        audioSource.Play();
        obj.SetActive(!obj.activeSelf);
    }
}
