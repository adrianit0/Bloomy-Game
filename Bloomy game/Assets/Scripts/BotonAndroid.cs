using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BotonAndroid : MonoBehaviour {

    public Color colorNormal;
    public Color colorPulsado;
    
    public UnityEvent onEntry;
    public UnityEvent onExit;

    bool pulsado = false;
    bool lastPulsado = false;

    SpriteRenderer render;

    void Awake () {
        render = GetComponent<SpriteRenderer>();
    }

    public void ControlarBoton (bool state) {
        pulsado = state;

        if (pulsado != lastPulsado) {
            lastPulsado = pulsado;

            if(pulsado)
                PulsarDown();
            else
                PulsarUp();
        }
    }

    void PulsarDown () {
        render.color = colorPulsado;
        onEntry.Invoke();
    }

    void PulsarUp () {
        render.color = colorNormal;
        onExit.Invoke();
    }
}
