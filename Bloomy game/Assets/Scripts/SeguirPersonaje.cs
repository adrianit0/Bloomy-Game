using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeguirPersonaje : MonoBehaviour {

    List<Transform> personajes = new List<Transform>();

    public float separacion = 3f; //Espacio de separación entre el personaje y el centro de la cámara

    public float velocidad = 15;
    public float constanteMultiplicativa = 0.05f;

    public bool fijarLadoY = true; //Fija el lado Y de la cámara, descarcar si quieres liberarlo
    
    void Update() {
        float sumaMedia = 0;
        for(int i = 0; i < personajes.Count; i++) {
            sumaMedia += personajes[i].position.x;
        }
        sumaMedia = sumaMedia / personajes.Count;
        AcercarCamara(new Vector3 (sumaMedia, transform.position.y, transform.position.z), velocidad, constanteMultiplicativa);
    }

    void AcercarCamara(Vector3 posicionPersonaje, float velocidad = 3, float incrementoVelocidad = 0.01f) {
        float _distancia = Vector3.Distance(this.transform.position, posicionPersonaje);
        if(Mathf.Abs(_distancia) >= 0.25f) {
            Vector3 _pos = transform.position;
            float _step = velocidad * (1 + (_distancia * incrementoVelocidad)) * Time.deltaTime;
            Vector3 _persPosition = new Vector3(posicionPersonaje.x + separacion, posicionPersonaje.y, posicionPersonaje.z);
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
    public void CambiarObjetivo(GameObject nuevoObjetivo, int index) {
        personajes[index] = nuevoObjetivo.transform;
    }

    public void AñadirObjetivo (Transform nuevoObjetivo) {
        personajes.Add(nuevoObjetivo);
    }

    public void EliminarObjetivo (Transform personaje) {
        if (!personajes.Contains(personaje)) {
            Debug.Log("El personaje no está aquí");
        }
        personajes.Remove(personaje);
    }
}
