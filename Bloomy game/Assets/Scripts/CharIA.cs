using UnityEngine;
using System.Collections;

public enum ESTADOENEMIGO { Pacifico, Agresivo};

public class CharIA : MonoBehaviour {

    public bool interactable = true;

    public float vision = 15;           //Distancia con la que buscará al personaje
    public float posicionMaxima = 8;    //Distancia entre el objetivo y este personaje
    public float distanciaReposo = 50;  //Distancia con la que quedará en reposo si está muy lejos del personaje.

    public bool quieto = false;
    public float xCentro;
    public float radioMov;
    public float[] recorrido;
    int recorridoActual;
    float diffError = 0.5f;

    GameObject personajePrincipal;
    ESTADOENEMIGO estado = ESTADOENEMIGO.Pacifico;

    public GameObject objetivo;

    bool activado = true;
    Character _char;
    LayerMask layervision;

    public float controlTime = 0.25f; //Contra menor sea el valor, mas cosas hará por segundo 0.25f son 4 acciones por segundos
    public float tiempoAgresivo = 6;  //Si pasa este tiempo sin ver al personaje este volverá a ser neutral
    float ultimaVezVisto = 0;
    float lastPosition = 0;
    
    Vector3 differenceVision = new Vector3(-0.35f, 0.35f);

    void Awake () {
        _char = GetComponent<Character>();
    }

    void Start () {
        GetCharacter();
        InvokeRepeating("SystemControl", 1, 1);
        InvokeRepeating("ControlPersonaje", controlTime, controlTime);
        
        if (recorrido.Length == 0) {
            quieto = true;
            xCentro = transform.position.x;
            radioMov = 1;
        } else if (recorrido.Length == 1) {
            quieto = true;
            xCentro = recorrido[0];
        } else if(quieto) {
            xCentro = transform.position.x;
            radioMov = 1;
        }
    }

    void GetCharacter () {
        if(GameManagerScript.gameManager != null && GameManagerScript.gameManager.personajes != null) {
            personajePrincipal = GameManagerScript.gameManager.personajes[0];
            layervision = GameManagerScript.gameManager.layerDetectionEnemigo;
        }
    }

    void SystemControl () {
        if (personajePrincipal == null) {
            GetCharacter();
            return;
        }
        if(Vector2.Distance(personajePrincipal.transform.position, this.transform.position) > distanciaReposo || !_char.vivo) {
            activado = false;
            _char.Moverse(0);
        } else {
            activado = true;
        }
            
    }

    void ControlPersonaje () {
        if(!activado)
            return;

        RaycastHit2D hit = new RaycastHit2D();
        float[] yPos = new float[3] { 0, 5, -5 };
        for (int i = 0; i < yPos.Length; i++) {
            hit = Physics2D.Raycast(transform.position+differenceVision, new Vector3(Mathf.Sign(transform.localScale.x) * vision * -1, yPos[i], 0), vision, layervision);
            if(hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Personaje")) {
                GetAggro(hit.collider.gameObject);
                break;
            }
                
        }

        if (estado == ESTADOENEMIGO.Pacifico) {
            if(transform.position.x < (xCentro - radioMov) || transform.position.x > (xCentro + radioMov)) {
                MoverseHacia(new Vector3 (xCentro, 0, 0), 0, false);
            } else if(quieto) {
                _char.Moverse(0);
            } else if (transform.position.x < (recorrido[recorridoActual] - diffError) || transform.position.x > (recorrido[recorridoActual] + diffError)) {
                MoverseHacia(new Vector3(recorrido[recorridoActual], 0, 0), 0, false);
            } else {
                recorridoActual++;
                if(recorridoActual >= recorrido.Length)
                    recorridoActual = 0;
                _char.Moverse(0);
            }

        } else if (estado == ESTADOENEMIGO.Agresivo) {
            if (Mathf.Round(transform.position.y)==Mathf.Round(objetivo.transform.position.y)) {
                _char.MirarHacia(Mathf.Sign(objetivo.transform.position.x- transform.position.x));
                _char.Disparar();
            }

            MoverseHacia(objetivo.transform.position, posicionMaxima, true);

            if(ultimaVezVisto > tiempoAgresivo) {
                estado = ESTADOENEMIGO.Pacifico;
                _char.Moverse(0);
                objetivo = null;
            }
            ultimaVezVisto += controlTime;
        }
    }

    void MoverseHacia (Vector3 destino, float posDes, bool aggro) {
        float sign = Mathf.Sign(transform.position.x - destino.x);
        float center = destino.x + (posDes * sign);

        if(Mathf.Round(center) == Mathf.Round(transform.position.x) && _char.enSuelo) {
            _char.Moverse(Mathf.Sign(destino.x - transform.position.x));
            if(aggro)
                _char.Disparar();
            _char.Saltar();
        } else if(!_char.enSuelo) {
            //No hace nada
        } else {
            _char.Moverse(Mathf.Sign(center - transform.position.x));
        }


        if(lastPosition == transform.position.x) {
            _char.Saltar();
        }
        lastPosition = transform.position.x;
    }

    public void GetAggro (GameObject _objetivo) {
        objetivo = _objetivo;
        estado = ESTADOENEMIGO.Agresivo;
        ultimaVezVisto = 0;
    }

    /*void Update () {
        if(!activado)
            return;
        Debug.DrawRay(transform.position + differenceVision, new Vector3(Mathf.Sign(transform.localScale.x) * vision * -1, 0, 0), Color.red);
        Debug.DrawRay(transform.position + differenceVision, new Vector3(Mathf.Sign(transform.localScale.x) * vision * -1, 5, 0), Color.red);
        Debug.DrawRay(transform.position + differenceVision, new Vector3(Mathf.Sign(transform.localScale.x) * vision * -1, -5, 0), Color.red);
    }*/
}
