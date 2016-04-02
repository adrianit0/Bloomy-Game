using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum DIRECCION { Izquierda, Derecha};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour {
    
    
    public int vidaMaxima = 100, vidaActual = 100;
    public float manaMaximo = 100, manaActual = 100;
    public float regMana = 10;
    public int bando = 0;

    public float velocidad = 1;
    public float salto = 1;
    public float defensa = 0.9f;

    public bool vivo = true;

    public SpriteRenderer escudo;
    public Sprite escudoNormal;
    public Sprite escudoDañado;

    public Image barraVida;
    public Image barraMana;
    public Image barraArma;

    public Transform posArrI;
    public Transform posAbaD;

    public Transform posicionArma;
    public int armaInicial;
    GameObject balaPrefab;

    float mov = 0;
    DIRECCION dir;
    bool enSuelo = true;
    bool enSueloInternal = false;
    bool atacando = false;
    bool defendiendo = false;

    float lastTimeShot = 0;
    float timeNeededShot = 0;
    float timeAnimationShot = 0.5f;

    float lastTimeMana = 0;
    float deltaMana = 0.2f;
    LayerMask layer;

    Rigidbody2D rigid;
    BoxCollider2D coll;
    Animator anim;

    void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void Start () {
        if(deltaMana > 0 && bando == 0)
            InvokeRepeating("RegMana", 0, deltaMana);

        CambiarArma(armaInicial);

        if(GameManagerScript.gameManager != null)
            layer = GameManagerScript.gameManager.layerMaskGeneral;
    }

	void FixedUpdate () {
        if (posArrI!=null&&posAbaD!=null) {
            enSuelo = Physics2D.OverlapArea(posArrI.position, posAbaD.position, layer);
            if (enSuelo!=enSueloInternal) {
                anim.SetBool("EnSuelo", enSuelo);
                enSueloInternal = enSuelo;
            }
        }

        rigid.velocity = new Vector2 (velocidad * mov * Time.fixedDeltaTime, rigid.velocity.y);
        
        lastTimeMana += Time.fixedDeltaTime;
        lastTimeShot += Time.fixedDeltaTime;

        if (atacando && lastTimeShot > timeAnimationShot) {
            anim.SetBool("Attack", false);
            atacando = false;
        }
    }

    ///-1 izquierda
    /// 0 quieto
    /// 1 derecha
    public void Moverse (float direccion) {
        mov = Mathf.Clamp(direccion, -1, 1);

        if(mov < 0)
            dir = DIRECCION.Izquierda;
        else if(mov > 0)
            dir = DIRECCION.Derecha;

        transform.localScale = new Vector3((dir == DIRECCION.Izquierda) ? 1 : -1, transform.localScale.y, transform.localScale.z);

        anim.SetBool("Movement", mov != 0);
    }

    public void Saltar () {
        if(enSuelo)
            rigid.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
    }

    public void Defender () {

    }

    public void Disparar() {
        if (lastTimeShot>timeNeededShot) {
            GameObject _balaObj = (GameObject) Instantiate(balaPrefab, posicionArma.transform.position, Quaternion.identity);
            Weapon _balaScript = _balaObj.GetComponent<Weapon>();

            if (_balaScript.costeMana>manaActual) {
                _balaScript.DestruirBala();
                return;
            }

            lastTimeShot = 0;
            atacando = true;

            _balaScript.bando = bando;
            _balaScript.SetVelocidad(_balaScript.velocidad * ((dir == DIRECCION.Derecha) ? 1 : -1));
            _balaScript.transform.localScale = new Vector2((dir == DIRECCION.Derecha) ? -1 : 1, 1);
            _balaScript.origenBala = this.gameObject;

            timeNeededShot = _balaScript.delay;

            ConsumirMana(_balaScript.costeMana);

            anim.SetBool("Attack", true);
        }
    }

    public void InfligirDaño (int cantidad) {
        float def = (defendiendo) ? defensa : 1;
        cantidad = Mathf.RoundToInt((float)cantidad * def);

        if(cantidad < 0)
            cantidad = 0;

        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        barraVida.fillAmount = (float) vidaActual / (float) vidaMaxima;

        if(vidaActual == 0)
            Morir();
    }

    public void CurarVida (int cantidad) {
        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        barraVida.fillAmount = (float)vidaActual / (float)vidaMaxima;
    }

    void Morir() {
        vivo = false;
    }

    public void ConsumirMana (float cantidad) {
        lastTimeMana = 0;

        manaActual = Mathf.Clamp (manaActual-cantidad, 0, manaMaximo);
        if (barraMana != null) {
            barraMana.fillAmount = manaActual / manaMaximo;
        }
    }

    void RegMana () {
        if(manaActual >= manaMaximo)
            return;

        float regPower = (lastTimeMana < 1) ? 0 : (lastTimeMana > 3) ? 1 : (lastTimeMana - 1) / 2;
        if(regPower == 0)
            return;

        regPower = regMana * regPower * deltaMana;
        manaActual = Mathf.Clamp(manaActual + regPower, 0, manaMaximo);

        if(barraMana != null) {
            barraMana.fillAmount = manaActual / manaMaximo;
        }
    }

    public void CambiarArma (int nuevaArma) {
        armaInicial = nuevaArma;
        WeaponInfo _info = GameManagerScript.gameManager.listaObjetos.armas[nuevaArma];

        barraArma.sprite = _info.weaponIcon;
        balaPrefab = _info.weaponPrefab;
        
    }
}