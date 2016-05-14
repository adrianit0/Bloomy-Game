using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MirarDesde { Derecha, Izquierda}

[ExecuteInEditMode]
public class ScreenPos : MonoBehaviour {

    public Camera camara;
    public DesirePosition[] positions;
    public LayerMask maskGUI;
    public bool forzarUpdate = false;

    float lastWidht = 0;
    
    List<BotonAndroid> listaBotonesPulsados = new List<BotonAndroid>();
    List<BotonAndroid> listaLastBotonesPulsados = new List<BotonAndroid>();

    RaycastHit2D[] hits;
    BotonAndroid _boton;

    void Start () {
        if(camara == null)
            camara = Camera.main;
    }

    void Update () {
        //Debug.Log(Input.mousePosition.x + " - " + Screen.width);
        if (Screen.width != lastWidht || forzarUpdate) {
            lastWidht = Screen.width;
            ConfigurarPantalla();
        }

#if UNITY_ANDROID
        AndroidServiceTouch();
#endif
    }

    void AndroidServiceTouch () {
        
        foreach(Touch touch in Input.touches) {
            //if(touch.phase == TouchPhase.Began) {
            Debug.Log("Boton " + touch.fingerId + " pulsado.");
            hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touch.position), Vector3.zero, 0, maskGUI);
            for (int i = 0; i < hits.Length; i++) {
                if(hits[i]) {
                    _boton = hits[i].transform.gameObject.GetComponent<BotonAndroid>();

                    if (!listaBotonesPulsados.Contains(_boton))
                        listaBotonesPulsados.Add(_boton);
                }
            }
            //}
        }

        if(listaBotonesPulsados.Count > 0 || listaLastBotonesPulsados.Count > 0)
            ControlarBotones();
    }

    void ControlarBotones () {
        for (int i = 0; i < listaBotonesPulsados.Count; i++) {
            if (listaLastBotonesPulsados.Contains(listaBotonesPulsados[i])) {
                listaLastBotonesPulsados.Remove(listaBotonesPulsados[i]);
                i--;
            }
        }

        for (int i = 0; i < listaLastBotonesPulsados.Count; i++) {
            listaLastBotonesPulsados[i].ControlarBoton(false);
        }

        for(int i = 0; i < listaBotonesPulsados.Count; i++) {
            if(!listaLastBotonesPulsados.Contains(listaBotonesPulsados[i])) {
                listaBotonesPulsados[i].ControlarBoton(true);
            }
        }

        listaLastBotonesPulsados = listaBotonesPulsados;
        listaBotonesPulsados = new List<BotonAndroid>();
    }

    public void ConfigurarPantalla () {
        for (int i = 0; i < positions.Length; i++) {
            Vector3 newPos = camara.ScreenToWorldPoint(new Vector3(
                (positions[i].lookAt == MirarDesde.Derecha) ? Screen.width - positions[i].desireWidth : positions[i].desireWidth, 
                0, 0));
            newPos = new Vector3(newPos.x - camara.transform.localPosition.x, 0, 0);
            positions[i].objeto.transform.localPosition = new Vector3(
                newPos.x,
                positions[i].objeto.transform.localPosition.y,
                positions[i].objeto.transform.localPosition.z
                );
            positions[i].xPos = positions[i].objeto.transform.localPosition.x;
        }
    }
}

[System.Serializable]
public class DesirePosition {
    public GameObject objeto;
    public MirarDesde lookAt = MirarDesde.Derecha;
    public float desireWidth = 0;
    public float xPos = 0;
}

