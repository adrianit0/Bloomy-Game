using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum DIRECCION { Izquierda, Derecha};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour {

    public int ID;

    public int vidaMaxima = 100, vidaActual = 100;
    public float manaMaximo = 100, manaActual = 100;
    public float regMana = 10;
    public int bando = 0;

    public float velocidad = 1;
    public float salto = 1;
    public float defensa = 0.9f;

    public bool vivo = true;
    public bool manaInfinito = false;

    public SpriteRenderer cuerpo;
    public SpriteRenderer cara;
    public SpriteRenderer mascara;
    public SpriteRenderer escudo;
    public Sprite escudoNormal;
    public Sprite escudoDañado;

    public Image marcoVida;
    RectTransform marcoVidaTrans;
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
    [HideInInspector]
    public bool enSuelo = true;
    bool enSueloInternal = false;
    bool atacando = false;
    bool defendiendo = false;

    float lastTimeShot = 0;
    float timeNeededShot = 0;
    float timeAnimationShot = 0.5f;

    float lastTimeMana = 0;
    float deltaMana = 0.2f;
    LayerMask layer;

    public AudioClip sonidoSaltar;
    public AudioClip sonidoDisparo;
    public AudioClip sonidoHit;
    public AudioClip sonidoCubrir;
    public AudioClip sonidoMorir;
    AudioSource source;

    Rigidbody2D rigid;
    BoxCollider2D coll;
    Animator anim;

    void Awake () {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        if (marcoVida!=null) {
            marcoVidaTrans = marcoVida.GetComponent<RectTransform>();
        }
    }

    void Start () {
        if(deltaMana > 0 && bando == 0)
            InvokeRepeating("RegMana", 0, deltaMana);

        CambiarArma(armaInicial);
        MirarHacia(-transform.localScale.x);

        if(GameManagerScript.gameManager != null) {
            layer = GameManagerScript.gameManager.layerMaskGeneral;
            if(GameManagerScript.gameManager.personajePrincipal == null && bando == 0)
                GameManagerScript.gameManager.personajePrincipal = this.gameObject;
        }

        cuerpo.sortingOrder = ID * 5;
        cara.sortingOrder = ID * 5 + 1;
        mascara.sortingOrder = ID * 5 + 2;
        escudo.sortingOrder = ID * 5 + 3;


        if(marcoVida != null && bando > 0)
            marcoVida.gameObject.SetActive(false);
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

        if(transform.position.y < -20 && vivo)
            Morir();
    }

    ///-1 izquierda
    /// 0 quieto
    /// 1 derecha
    public void Moverse (float direccion) {
        if(!vivo)
            return;

        mov = Mathf.Clamp(direccion, -1, 1);

        if(defendiendo && mov != 0)
            Defender(false, true);

        MirarHacia(mov);
        
        anim.SetBool("Movement", mov != 0);
    }

    public void MirarHacia (float direccion) {
        if(!vivo)
            return;

        if(direccion < 0)
            dir = DIRECCION.Izquierda;
        else if(direccion > 0)
            dir = DIRECCION.Derecha;

        transform.localScale = new Vector3((dir == DIRECCION.Izquierda) ? 1 : -1, transform.localScale.y, transform.localScale.z);

        if(marcoVida != null && bando > 0 && transform.localScale.x != marcoVidaTrans.localScale.x)
            marcoVidaTrans.localScale = new Vector3(transform.localScale.x, marcoVidaTrans.localScale.y, marcoVidaTrans.localScale.z);
    }

    public void Saltar () {
        if(!vivo)
            return;

        if(defendiendo)
            Defender(false, true);

        if(enSuelo) {
            rigid.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
            source.clip = sonidoSaltar;
            source.Play();
        }
            
    }

    public void Defender (bool activado, bool force = false) {
        if ((enSuelo && mov==0)||force) {
            defendiendo = activado;
            anim.SetBool("Protect", activado);
            escudo.sprite = escudoNormal;
            escudo.gameObject.SetActive(activado);
        }
    }

    public void Disparar() {
        if(!vivo)
            return;

        if(defendiendo)
            Defender(false, true);

        if (lastTimeShot>timeNeededShot) {
            source.clip = sonidoDisparo;
            source.Play();

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

    public void InfligirDaño (Weapon bala) {
        float def = 1;
        

        if(defendiendo && Mathf.Sign(bala.transform.localScale.x)!=Mathf.Sign(this.transform.localScale.x)) {
            def = 1 - defensa;
            anim.SetTrigger("Protecting");
            escudo.sprite = escudoDañado;
            ConsumirMana(10);
        }

        int cantidad = Mathf.RoundToInt((float)bala.daño * def);

        if(cantidad < 0)
            cantidad = 0;

        source.clip = (defendiendo) ? sonidoCubrir : sonidoHit;
        source.Play();

        if(marcoVida != null && bando > 0 && !marcoVida.gameObject.activeSelf) {
            marcoVida.gameObject.SetActive(true);
        }

        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        barraVida.fillAmount = (float) vidaActual / (float) vidaMaxima;

        GetAggro(bala.origenBala);

        if(vidaActual == 0)
            Morir();
    }

    public void CurarVida (int cantidad) {
        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        barraVida.fillAmount = (float)vidaActual / (float)vidaMaxima;
    }

    void Morir() {
        source.clip = sonidoMorir;
        source.Play();

        Moverse(0);
        Defender(false, true);

        vivo = false;
        anim.SetBool("Dying", true);
        coll.enabled = true;

        if (bando == 0 && GameManagerScript.gameManager != null) {
            GameManagerScript.gameManager.FinPartida();
        }

        if(marcoVida != null && bando > 0 && marcoVida.gameObject.activeSelf)
            marcoVida.gameObject.SetActive(false);
    }

    public void ConsumirMana (float cantidad) {
        if(manaInfinito)
            return;
        lastTimeMana = 0;

        manaActual = Mathf.Clamp (manaActual-cantidad, 0, manaMaximo);
        if (barraMana != null) {
            barraMana.fillAmount = manaActual / manaMaximo;
        }
    }

    void RegMana () {
        if(manaActual >= manaMaximo || defendiendo)
            return;

        float regPower = (lastTimeMana < 1) ? 0 : (lastTimeMana > 3) ? 1 : (lastTimeMana - 1) / 2;
        
        if(regPower == 0)
            return;

        if(mov != 0)
            regPower /= 2;

        regPower = regMana * regPower * deltaMana;
        manaActual = Mathf.Clamp(manaActual + regPower, 0, manaMaximo);

        if(barraMana != null) {
            barraMana.fillAmount = manaActual / manaMaximo;
        }
    }

    public void CambiarArma (int nuevaArma) {
        armaInicial = nuevaArma;
        WeaponInfo _info = GameManagerScript.gameManager.listaObjetos.armas[nuevaArma];

        if (barraArma != null)
            barraArma.sprite = _info.weaponIcon;
        balaPrefab = _info.weaponPrefab;
    }

    public void GetAggro (GameObject _objetivo) {
        CharIA _ia = GetComponent<CharIA>();
        if (_ia!=null) {
            _ia.GetAggro(_objetivo);
        }
    }
}