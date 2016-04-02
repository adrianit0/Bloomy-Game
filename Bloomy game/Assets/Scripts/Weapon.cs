using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public int daño = 10;
    public float costeMana = 10f;
    public float delay = 0.5f;
    public int bando = 0;

    public float velocidad = 1;
    
    public float tiempoMaximo = 3f;
    float tiempoActual = 0f;

    public Sprite[] sprites = new Sprite[2];
    public int framePerSecond = 5;
    int actualFrame = 0;

    public GameObject origenBala;

    SpriteRenderer _render;

    Rigidbody2D rigid;

    void Awake () {
        _render = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        if(framePerSecond > 0 && sprites.Length > 0)
            InvokeRepeating("SpriteChange", 0, 1/(float)framePerSecond);
    }

    void Update () {
        tiempoActual += Time.deltaTime;

        if (tiempoActual > tiempoMaximo) {
            DestruirBala();
        }
    }

    public void SetVelocidad (float velocidad) {
        rigid.velocity = new Vector2(velocidad, rigid.velocity.y);
    }

    void SpriteChange () {
        actualFrame++;
        if(actualFrame >= sprites.Length)
            actualFrame = 0;

        _render.sprite = sprites[actualFrame];
    }

    public void DestruirBala () {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D (Collider2D coll) {
        Character _char = coll.gameObject.GetComponent<Character>();
        Caja _box = coll.gameObject.GetComponent<Caja>();

        if (_char != null) {
            if (bando != _char.bando && _char.vivo) {
                _char.InfligirDaño(daño);
                DestruirBala();
            }
        } else if (_box != null) {
            _box.RomperCaja();
            DestruirBala();
        }
    }
}
