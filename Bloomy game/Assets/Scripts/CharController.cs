using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour {

    Character personaje;
    float lastMovimiento;

    void Awake () {
        personaje = GetComponent<Character>();
    }

    void Update() {
        if(!personaje.vivo)
            return;

        float movimiento = Input.GetAxis("Horizontal");

        if(Input.GetButtonDown("Jump")) {
            personaje.Saltar();
        }

        if(Input.GetButtonDown("Fire")) {
            personaje.Disparar();
        }

        if(Input.GetButtonDown("Protect")) {
            personaje.Defender();
        }

        if (movimiento != lastMovimiento) {
            personaje.Moverse(movimiento);
            lastMovimiento = movimiento;
        }
    }
}