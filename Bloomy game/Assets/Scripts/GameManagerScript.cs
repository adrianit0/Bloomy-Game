using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {

    public ListaItem listaObjetos;
    public LayerMask layerMaskGeneral;

    public GameObject itemPrefab;

    public static GameManagerScript gameManager;

    void Awake () {
        if (gameManager == null) {
            gameManager = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }

    public void FinPartida () {

    }
}
