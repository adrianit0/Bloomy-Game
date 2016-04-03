using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GameManagerScript : MonoBehaviour {

    public ListaItem listaObjetos;

    public GameObject canvasGameOver;
    public RectTransform panelGameOver;
    public RectTransform panelVictoria;

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
        StartCoroutine(MostrarPanel(canvasGameOver, panelGameOver));
    }

    public void Victoria () {
        StartCoroutine(MostrarPanel(canvasGameOver, panelVictoria));
    }

    public void ReproducirSonido (AudioClip sonido) {
        source.clip = sonido;
        source.Play();
    }

    IEnumerator MostrarPanel (GameObject _rectCanvas, RectTransform _rectPanel) {
        yield return new WaitForSeconds (1);
        mainSource.Stop();
        _rectPanel.localScale = Vector3.zero;
        _rectPanel.gameObject.SetActive(true);
        _rectCanvas.SetActive(true);

        float value = 0;
        float tam = 0;

        while(value < 1) {
            value += Time.deltaTime;
            tam = Mathf.Lerp(0, 1, value);
            _rectPanel.localScale = new Vector3(tam, tam, tam);
            yield return null;
        }
    }
}