using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour {

    public int jugador = 1;
    public bool interactable = true;

    Character personaje;
    float lastMovimiento;

    void Awake () {
        personaje = GetComponent<Character>();
    }

    void Update() {
        if(!personaje.vivo)
            return;

        if (jugador == 1) {
            Player1();
        }else if (jugador == 2) {
            Player2();
        }
    }

    void Player1() {
        float movimiento = Input.GetAxis("Horizontal");

        if(Input.GetButtonDown("Jump")) {
            personaje.Saltar();
        }

        if(Input.GetButtonDown("Fire")) {
            personaje.Disparar();
        }

        /*if(Input.GetButtonDown("Punch")) {
            personaje.Punch();//
        }*/

        if(Input.GetButtonDown("Protect")) {
            personaje.Defender(true);
        } else if(Input.GetButtonUp("Protect")) {
            personaje.Defender(false);
        }

        if(movimiento != lastMovimiento) {
            personaje.Moverse(movimiento);
            lastMovimiento = movimiento;
        }
    }

    void Player2() {
        float movimiento = Input.GetAxis("HorizontalP2");

        if(Input.GetButtonDown("JumpP2")) {
            personaje.Saltar();
        }

        if(Input.GetButtonDown("FireP2")) {
            personaje.Disparar();
        }

        if(Input.GetButtonDown("ProtectP2")) {
            personaje.Defender(true);
        } else if(Input.GetButtonUp("ProtectP2")) {
            personaje.Defender(false);
        }

        if(movimiento != lastMovimiento) {
            personaje.Moverse(movimiento);
            lastMovimiento = movimiento;
        }
    }
}