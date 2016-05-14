using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum DIRECCION { Izquierda, Derecha};

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Character : MonoBehaviour {

    public int ID;

    public int bando = 0;

    public int nivel;       //NIVEL MAXIMO = 60
    public int experiencia; //EXPERIENCIA
    [Range(0, 10)]
    public int ataque, fortaleza, proteccion, mana, energia, agilidad;

    float dañoBase = 1f, retrocesoBase = 1f;
    float vidaMaxima = 100, vidaActual = 100, vidaACurar = 0, regVida = 5;
    float manaMaximo = 100, manaActual = 100, regMana = 20;
    float energiaMaximo = 100, energiaActual = 100, regEnergia = 10;
    float defensa = 0.4f, costeEscudo = 20, costeGolpeEscudo = 10;
    float velocidad = 3.6f, salto = 12, velocidadAtaque = 1f, delayHechizo = 1f;

    public bool interactable = true;
    bool internalInteractable = false;
    public bool vivo = true;
    public bool invencible = false;
    public bool manaInfinito = false;

    public SpriteRenderer cuerpo;
    public SpriteRenderer cara;
    public SpriteRenderer mascara;
    public SpriteRenderer espada;
    public SpriteRenderer escudo;
    public Sprite escudoNormal;
    public Sprite escudoDañado;

    Color[] colorEquipo = new Color[3];

    public Image marcoVida;
    RectTransform marcoVidaTrans;
    public Image barraVida;

    public Transform posArrI;
    public Transform posAbaD;
    public Transform posicionArma;

    public int espadaInicial = 0, armaInicial = 0;

    GameObject balaPrefab;
    Sprite[] spriteEspada = new Sprite[4];

    float mov = 0;
    DIRECCION dir;
    [HideInInspector]
    public bool enSuelo = true;
    bool enSueloInternal = false;
    bool atacando = false;
    bool defendiendo = false;

    Vector2 retrocesoActual;
    public float decrecion = 1;
    float lerpRetroceso = 0;

    float lastPunch = 0;
    float lastTimeShot = 0, timeNeededShot = 0, timeAnimationShot = 0.5f;
    float lastJump = 0, timeJump = 0.25f;


    float fixedDelta;

    float lastTimeMana = 0;
    float deltaReg = 0.2f;
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
    CharUI interfaz;

    void Awake () {
        source = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        interfaz = GetComponent<CharUI>();

        fixedDelta = Time.fixedDeltaTime;

        if (marcoVida!=null) {
            marcoVidaTrans = marcoVida.GetComponent<RectTransform>();
        }
    }

    void Start () {
        if(deltaReg > 0 && bando == 0) {
            InvokeRepeating("RegMana", deltaReg, deltaReg);
            InvokeRepeating("RegEnergia", deltaReg, deltaReg);
            InvokeRepeating("RegVida", deltaReg, deltaReg);
        }
            

        CambiarArma(TipoArma.Espada, espadaInicial);
        CambiarArma(TipoArma.Magia, armaInicial);
        MirarHacia(-transform.localScale.x);

        ObservarNivel();

        if(GameManagerScript.gameManager != null) {
            layer = GameManagerScript.gameManager.layerMaskGeneral;
            //if(GameManagerScript.gameManager.personajes == null && bando == 0)
            //    GameManagerScript.gameManager.personajes = this.gameObject;
        }

        if (espada!= null)
            espada.sortingOrder = ID * 5;
        cuerpo.sortingOrder = ID * 5 + 1;
        cara.sortingOrder = ID * 5 + 2;
        mascara.sortingOrder = ID * 5 + 3;
        escudo.sortingOrder = ID * 5 + 4;
        colorEquipo[0] = cuerpo.color;
        colorEquipo[1] = cara.color;
        colorEquipo[2] = mascara.color;

        cuerpo.gameObject.SetActive(false);

        if (interactable) {
            internalInteractable = interactable;
            interactable = false;
        }

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

        rigid.velocity = new Vector2 (velocidad * mov, rigid.velocity.y) + retrocesoActual;

        if (retrocesoActual != Vector2.zero) {
            retrocesoActual = Vector2.Lerp(retrocesoActual, Vector2.zero, lerpRetroceso);
            lerpRetroceso += fixedDelta*decrecion;
        }

        lastPunch += fixedDelta;
        lastTimeMana += fixedDelta;
        lastTimeShot += fixedDelta;
        lastJump += fixedDelta;

        if(transform.position.y < -20 && vivo)
            Morir();
    }

    ///-1 izquierda
    /// 0 quieto
    /// 1 derecha
    public void Moverse (float direccion) {
        if(!interactable)
            return;

        mov = Mathf.Clamp(direccion, -1, 1);

        //if(defendiendo && mov != 0)
        //    Defender(false, true);

        MirarHacia(mov);
        
        anim.SetBool("Movement", mov != 0);
    }

    public void MirarHacia (float direccion) {
        if(!interactable)
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
        if(!interactable || lastJump<timeJump)
            return;

        //if(defendiendo)
        //    Defender(false, true);
        lastJump = 0;

        if(enSuelo) {
            rigid.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
            source.clip = sonidoSaltar;
            source.Play();
        }
            
    }

    public void Defender (bool activado/*, bool force = false*/) {
        defendiendo = activado;
        anim.SetBool("Protect", activado);
        escudo.sprite = escudoNormal;
        escudo.gameObject.SetActive(activado);
    }

    public void Disparar() {
        if(!interactable)
            return;


        if (lastTimeShot>timeNeededShot) {
            Weapon _balaScript = balaPrefab.GetComponent<Weapon>();

            if (_balaScript.costeMana>manaActual) {
                return;
            }

            GameObject _balaObj = (GameObject)Instantiate(balaPrefab, posicionArma.transform.position, Quaternion.identity);
             _balaScript = _balaObj.GetComponent<Weapon>();

            source.clip = sonidoDisparo;
            source.Play();

            lastTimeShot = 0;
            atacando = true;

            _balaScript.bando = bando;
            _balaScript.SetVelocidad(_balaScript.velocidad * ((dir == DIRECCION.Derecha) ? 1 : -1));
            _balaScript.transform.localScale = new Vector2((dir == DIRECCION.Derecha) ? -1 : 1, 1);
            _balaScript.origenBala = this.gameObject;
            _balaScript.daño *= dañoBase;
            _balaScript.retroceso *= retrocesoBase;

            timeNeededShot = _balaScript.delay*delayHechizo;

            ConsumirMana(_balaScript.costeMana);

            anim.SetTrigger("Magic");

            if (interfaz!=null) {
                interfaz.UsarArma(TipoArma.Magia, _balaScript.delay);
            }
        }
    }

    public void Punch () {
        if (lastPunch>(1/velocidadAtaque)) {
            lastPunch = 0;
            anim.SetTrigger("Punch");
        }
    }

    public void InfligirDaño (Weapon bala) {
        if(invencible)
            return;

        float def = 1;

        if(defendiendo && Mathf.Sign(bala.transform.localScale.x)!=Mathf.Sign(this.transform.localScale.x)) {
            def = 1 - defensa;
            anim.SetTrigger("Protecting");
            escudo.sprite = escudoDañado;
            ConsumirEnergía(costeGolpeEscudo);
        } else {
            StartCoroutine(CambiarColor());
        }

        int cantidad = Mathf.RoundToInt((float)bala.daño * def);

        if(cantidad < 0)
            cantidad = 0;

        source.clip = (defendiendo) ? sonidoCubrir : sonidoHit;
        source.Play();

        if(marcoVida != null && bando > 0 && !marcoVida.gameObject.activeSelf) {
            marcoVida.gameObject.SetActive(true);
        }

        if (bala.retroceso != 0) {
            retrocesoActual += new Vector2(-Mathf.Sign(bala.transform.localScale.x) * bala.retroceso, 0);
        }

        vidaActual = Mathf.Clamp(vidaActual - cantidad, 0, vidaMaxima);
        
        if (interfaz!=null) {
            interfaz.ModificarBarraVida(vidaMaxima, vidaActual, cantidad, vidaACurar);
        } else {
            barraVida.fillAmount = (float)vidaActual / (float)vidaMaxima;
        }
        
        GetAggro(bala.origenBala);

        if(vidaActual == 0) {
            if (bala.origenBala.GetComponent<Character>().bando==0) {
                bala.origenBala.GetComponent<Character>().ConseguirExperiencia(nivel);
            }
            Morir();
        }
            
    }

    IEnumerator CambiarColor () {
        StopCoroutine("CambiarColor");
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        cuerpo.color = Color.red;
        cara.color = Color.red;
        mascara.color = Color.red;

        float lerp = 0;
        while (lerp < 1) {
            lerp += Time.deltaTime;

            cuerpo.color = Color.Lerp (cuerpo.color, colorEquipo[0], lerp);
            cara.color = Color.Lerp(cara.color, colorEquipo[1], lerp);
            mascara.color = Color.Lerp(mascara.color, colorEquipo[2], lerp);
            yield return wait;
        }
    }

    public void CurarVida (float cantidad, bool vidaPorTiempo) {
        if (vidaPorTiempo) {
            vidaACurar += cantidad;
            if(interfaz != null) 
                interfaz.ModificarBarraVida(vidaMaxima, vidaActual, 0, vidaACurar);
        } else {
            vidaActual = Mathf.Clamp(vidaActual + cantidad, 0, vidaMaxima);
            interfaz.ModificarBarraVida(vidaMaxima, vidaActual, 0, vidaACurar);
        }
    }

    public void ConseguirExperiencia (int cantidad) {
        experiencia += cantidad;
        if (experiencia > GameManagerScript.gameManager.GetExperienceNecessaryForNextLevel(nivel)) {
            SubirNivel();
        }
        if (interfaz!=null) {
            interfaz.ModificarBarraExperiencia(GameManagerScript.gameManager.GetExperienceNecessaryForNextLevel(nivel), experiencia);
        }

        if(interfaz == null)
            return;
    }

    void SubirNivel () {
        //Incluir animacion de subida de nivel, si procede.
        ObservarNivel();
    }

    public void ObservarNivel () {
        nivel = GameManagerScript.gameManager.GetLevel(experiencia);
        if (interfaz!=null) {
            interfaz.nivel.text = "Level " + nivel.ToString();
            int dif = Mathf.Clamp (nivel - (ataque + fortaleza + proteccion + mana + energia + agilidad), 0, 60);
            interfaz.bonus.text = (dif!=0) ? "+" + dif.ToString() : "";
            interfaz.ModificarBarraExperiencia(GameManagerScript.gameManager.GetExperienceNecessaryForNextLevel(nivel), experiencia);
        }
    }

    void Morir() {
        source.clip = sonidoMorir;
        source.Play();

        Moverse(0);
        Defender(false);

        vivo = false;
        interactable = false;
        anim.SetTrigger("Morir");
        coll.enabled = true;

        if (bando == 0 && GameManagerScript.gameManager != null) {
            GameManagerScript.gameManager.FinPartida(gameObject);
        }

        if(marcoVida != null && bando > 0 && marcoVida.gameObject.activeSelf)
            marcoVida.gameObject.SetActive(false);
    }

    public void ConsumirMana (float cantidad) {
        if(manaInfinito)
            return;

        lastTimeMana = 0;
        manaActual = Mathf.Clamp (manaActual-cantidad, 0, manaMaximo);

        if(interfaz != null) {
            interfaz.ModificarBarraMana(manaMaximo, manaActual);
        }
    }

    public void ConsumirEnergía (float cantidad) {
        energiaActual = Mathf.Clamp(energiaActual - cantidad, 0, energiaMaximo);

        if(interfaz != null) {
            interfaz.ModificarBarraEnergia(energiaMaximo, energiaActual);
        }
    }

    void RegMana () {
        if(manaActual >= manaMaximo)
            return;

        float regPower = (lastTimeMana < 0.5f) ? 0 : (lastTimeMana > 1) ? 1 : (lastTimeMana - 0.5f) * 2;
        
        if(regPower == 0)
            return;

        if(mov != 0)
            regPower /= 2;

        regPower = regMana * regPower * deltaReg;
        manaActual = Mathf.Clamp(manaActual + regPower, 0, manaMaximo);

        if (interfaz!=null) {
            interfaz.ModificarBarraMana(manaMaximo, manaActual);
        }
    }

    void RegEnergia () {
        if(defendiendo) {
            ConsumirEnergía(costeEscudo*deltaReg);
            if(costeEscudo*deltaReg > energiaActual)
                Defender(false);
        } else if((energiaActual) >= energiaMaximo) {
            return;
        } else {
            energiaActual = Mathf.Clamp(energiaActual + regEnergia*deltaReg, 0, energiaMaximo);

            if(interfaz != null) {
                interfaz.ModificarBarraEnergia(energiaMaximo, energiaActual);
            }
        }
    }

    void RegVida () {
        if(vidaACurar == 0)
            return;

        float _reg = Mathf.Min(vidaACurar, regVida*deltaReg);
        vidaACurar -= _reg;
        CurarVida(_reg, false);
    }

    public void CambiarArma (TipoArma tipo, int nuevaArma) {
        Sprite _sprite = null;

        if (tipo == TipoArma.Espada) {
            espadaInicial = nuevaArma;
            SwordInfo _infoEspada = GameManagerScript.gameManager.listaObjetos.espadas[nuevaArma];
            _sprite = _infoEspada.icono;
            spriteEspada = _infoEspada.spriteArma;
            CambiarSpriteArma(3);
        } else if (tipo == TipoArma.Magia) {
            armaInicial = nuevaArma;
            WeaponInfo _info = GameManagerScript.gameManager.listaObjetos.armas[nuevaArma];
            _sprite = _info.icono;
            balaPrefab = _info.weaponPrefab;
        }

        if (interfaz!= null) {
            interfaz.CambiarIcono(tipo, _sprite);
        }
    }

    public void GetAggro (GameObject _objetivo) {
        CharIA _ia = GetComponent<CharIA>();
        if (_ia!=null) {
            _ia.GetAggro(_objetivo);
        }
    }

    public void Inicializado () {
        if(internalInteractable == true)
            interactable = true;

        anim.SetBool("Nacido", true);
    }

    public void CambiarSpriteArma (int index) {
        index = Mathf.Clamp(index, 0, spriteEspada.Length-1);
        if (espada != null)
            espada.sprite = spriteEspada[index];
    }

    void ConfigurarStats () {
        //ATAQUE
        dañoBase = 0.95f + ataque * 0.3f + nivel * 0.05f;
        retrocesoBase = 0.9f + ataque * 0.1f;

        //FORTALEZA
        float lastVida = vidaMaxima;
        vidaMaxima = 90 + fortaleza * 40 + nivel * 10;
        vidaActual = (vidaActual * vidaMaxima) / lastVida;
        regVida = 5 + 3 * fortaleza;

        //PROTECCION
        defensa = 0.4f + proteccion * 0.04f;
        costeEscudo = 20 - proteccion * 1f;
        costeGolpeEscudo = 10f - proteccion * 0.5f;

        //MANA
        manaMaximo = 95 + mana * 35 + nivel * 5;
        regMana = 20 + mana * 6;

        //ENERGIA
        energiaMaximo = 100 + energia * 20;
        regEnergia = 10 * energia * 4;

        //AGILIDAD
        velocidad = 3.5f + agilidad * 0.1f;
        velocidadAtaque = 0.9f + agilidad * 0.1f;
        delayHechizo = 1.04f - agilidad * 0.04f;
    }
}