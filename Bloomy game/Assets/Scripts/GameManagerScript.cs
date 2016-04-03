using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameManagerScript : MonoBehaviour {

    public ListaItem listaObjetos;

    public GameObject canvasGameOver;
    public RectTransform panelGameOver;

    public LayerMask layerMaskGeneral;
    public LayerMask layerDetectionEnemigo;

    public GameObject personajePrincipal;
    public GameObject itemPrefab;

    public AudioSource mainSource;

    public static GameManagerScript gameManager;

    AudioSource source;

    void Awake () {
        if (gameManager == null) {
            gameManager = this;
            //DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }

        source = GetComponent<AudioSource>();
    }

    void Start () {
        canvasGameOver.SetActive(false);
    }

    public void FinPartida () {
        StartCoroutine(GameOver());
        
    }

    public void ReproducirSonido (AudioClip sonido) {
        source.clip = sonido;
        source.Play();
    }

    IEnumerator GameOver () {
        yield return new WaitForSeconds (1);
        mainSource.Stop();
        panelGameOver.localScale = Vector3.zero;
        canvasGameOver.SetActive(true);

        float value = 0;
        float tam = 0;

        while(value < 1) {
            value += Time.deltaTime;
            tam = Mathf.Lerp(0, 1, value);
            panelGameOver.localScale = new Vector3(tam, tam, tam);
            yield return null;
        }
    }
}