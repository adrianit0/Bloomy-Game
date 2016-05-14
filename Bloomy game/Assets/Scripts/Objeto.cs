﻿using UnityEngine;
using System.Collections;

public class Objeto : MonoBehaviour {

    public int objetoID = -1; //Si es inferior a -1 no será ninguno
    public int vidaRecuperada = 0;

    public AudioClip clip;

    SpriteRenderer _render;


    void Awake () {
        _render = GetComponent<SpriteRenderer>();
    }

    void Start () {
        if (GameManagerScript.gameManager != null && _render != null && objetoID > 0) {
            _render.sprite = GameManagerScript.gameManager.listaObjetos.armas[objetoID].icono;
        }
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1, ForceMode2D.Impulse);
    }

    public void CambiarArma (int id, int vidaCurada) {
        objetoID = id;
        vidaRecuperada = vidaCurada;
        if(GameManagerScript.gameManager != null && _render != null && objetoID > 0) {
            _render.sprite = GameManagerScript.gameManager.listaObjetos.armas[objetoID].icono;
        }
    }

    void Destruir () {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        Character _char = coll.gameObject.GetComponent<Character>();

        if(_char != null) {
            if(_char.bando == 0) {
                _char.CurarVida(vidaRecuperada, true);
                if(objetoID > 0) {
                    _char.CambiarArma(TipoArma.Magia, objetoID);
                }

                if (GameManagerScript.gameManager!=null) {
                    GameManagerScript.gameManager.ReproducirSonido(clip);
                }
                Destruir();
            }
        }

    }
}
