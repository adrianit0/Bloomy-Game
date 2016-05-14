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

    public SeguirPersonaje camaraScript;
    public GameObject[] personajes;
    public GameObject[] interfazPersonaje;
    float countPersonajes;
    public GameObject itemPrefab;

    public AudioSource mainSource;

    public static GameManagerScript gameManager;

    public int[] curvaExperiencia = new int [61];

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

        for (int i = 0; i < personajes.Length; i++) {
            if (personajes[i].activeSelf) {
                AñadirPersonaje(i, true);
            }
        }
    }

    public int GetLevel (int experiencialTotal) {
        for (int i = 0; i < curvaExperiencia.Length; i++) {
            if (experiencialTotal < curvaExperiencia[i]) {
                return (i - 1);
            }
        }

        return 1;
    }

    public int GetExperienceNecessaryForNextLevel (int level) {
        if (level == 60) {
            return 0;
        }
        return curvaExperiencia [level+1];
    }

    void Update () {
        if (Input.GetKeyUp (KeyCode.Return)) {
            int cant = 0;
            for (int i = 0; i < personajes.Length; i++) {
                if(personajes[i].activeSelf) {
                    cant++;
                }
            }
            if(cant != personajes.Length) {
                AñadirPersonaje(1);
            }
        }
    }

    public void AñadirPersonaje (int personaje, bool forzarAñadir = false) {
        personaje = Mathf.Clamp(personaje, 0, 1);
        int otro = (personaje == 0) ? 1 : 0;
        if (personajes[personaje].activeSelf&&!forzarAñadir) {
            Debug.LogWarning(string.Format("El personaje {0} está en juego.", personaje));
            return;
        }

        countPersonajes++;
        camaraScript.AñadirObjetivo(personajes[personaje].transform);
        interfazPersonaje[personaje].SetActive(true);

        if (!personajes[personaje].activeSelf) {
            personajes[personaje].transform.position = new Vector3 (
                Random.Range(personajes[otro].transform.position.x-1, personajes[otro].transform.position.x+1),
                personajes[otro].transform.position.y,
                personajes[otro].transform.position.z
                );

            personajes[personaje].SetActive(true);
        }
    }

    public void FinPartida (GameObject personaje) {
        camaraScript.EliminarObjetivo(personaje.transform);
        countPersonajes--;
        if (countPersonajes<=0)
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