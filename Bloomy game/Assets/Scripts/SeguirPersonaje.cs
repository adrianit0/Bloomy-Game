using UnityEngine;
using System.Collections;

public class SeguirPersonaje : MonoBehaviour {

    public Transform personaje;
    public float separacion = 3f; //Espacio de separación entre el personaje y el centro de la cámara

    public float velocidad = 15;
    public float constanteMultiplicativa = 0.05f;

    public bool fijarLadoY = true; //Fija el lado Y de la cámara, descarcar si quieres liberarlo
    

    void Update() {
        AcercarCamara(personaje.transform, velocidad, constanteMultiplicativa);
    }

    void AcercarCamara(Transform posicionPersonaje, float velocidad = 3, float incrementoVelocidad = 0.01f) {
        float _distancia = Vector3.Distance(this.transform.position, posicionPersonaje.position);
        if(Mathf.Abs(_distancia) >= 0.25f) {
            Vector3 _pos = transform.position;
            float _step = velocidad * (1 + (_distancia * incrementoVelocidad)) * Time.deltaTime;
            Vector3 _persPosition = new Vector3(posicionPersonaje.position.x + separacion, posicionPersonaje.position.y, posicionPersonaje.position.z);
            transform.position = Vector3.MoveTowards(transform.position, _persPosition, _step);
            if(fijarLadoY)
                transform.position = new Vector3(transform.position.x, _pos.y, _pos.z);
            else
                transform.position = new Vector3(transform.position.x, transform.position.y, _pos.z);
        }
    }

    //Cambia la sepacion en el eje X entre el centro de la cámara y el objetivo
    public void CambiarSeparacion(float nuevaSeparacion) {
        separacion = nuevaSeparacion;
    }

    //Cambia el objetivo a otro personaje o cosa.
    public void CambiarObjetivo(GameObject nuevoObjetivo) {
        personaje = nuevoObjetivo.transform;
    }
}
