using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TipoArma { Espada, Magia};

public class CharUI : MonoBehaviour {

    public Text nombre, nivel, bonus;
    [Space(10)]
    public Image salud, cura, daño, escudo;  //BARRA VIDA
    [Space(10)]
    public Image mana, energia, experiencia; //OTRAS BARRAS
    [Space(10)]
    public Image cabeza, cara, mascara, circuloDaño, sombraMuerte;      //SPRITE CUERPO
    [Space(10)]
    public Image iconoEspada, enfriamientoEspada, sinEnergia;
    [Space(10)]
    public Image iconoMagia, enfriamientoMagia, sinMana;

    float dañoInterno;
    
    void Start () {
        circuloDaño.color = new Color(circuloDaño.color.r, circuloDaño.color.g, circuloDaño.color.b, 0);
        circuloDaño.gameObject.SetActive(true);
    }

    public void ModificarBarraVida (float vidaMaxima, float vidaActual, float damage = 0, float healMax = 0) {
        dañoInterno += damage;
        
        salud.fillAmount = vidaActual / vidaMaxima;
        daño.fillAmount = (vidaActual + dañoInterno) / vidaMaxima;
        cura.fillAmount = (vidaActual + healMax) / vidaMaxima;

        if (damage>0) {
            float diferencia =  daño.fillAmount - salud.fillAmount;
            StartCoroutine(BarraDaño(1, diferencia));
            
            StartCoroutine(MostrarCirculo(1, 2));

            if(vidaActual <= 0)
                sombraMuerte.gameObject.SetActive(true);
        }
    }

    public void ModificarBarraMana (float manaMaximo, float manaActual) {
        mana.fillAmount = manaActual / manaMaximo;
    }

    public void ModificarBarraEnergia (float energiaMaxima, float energiaActual) {
        energia.fillAmount = energiaActual / energiaMaxima;
    }

    public void ModificarBarraExperiencia (float experienciaMaximo, float experienciaActual) {
        experiencia.fillAmount = experienciaActual / experienciaMaximo;
    }

    public void SeleccionarSprite (Color _cabeza, Sprite _cara, Sprite _mascara) {
        if(cabeza == null || cara == null || mascara == null)
            return;
        cabeza.color = _cabeza;
        cara.sprite = _cara;
        mascara.sprite = _mascara;
    }

    public void CambiarIcono (TipoArma tipo, Sprite nuevoIcono) {
        if (tipo ==  TipoArma.Espada) {
            iconoEspada.sprite = nuevoIcono;
        }else if (tipo == TipoArma.Magia) {
            iconoMagia.sprite = nuevoIcono;
        }
    }

    public void UsarArma(TipoArma tipo, float enfriamiento) {
        if(enfriamiento <= 0)
            return;

        StopCoroutine("Enfriamiento");
        StartCoroutine (Enfriamiento(tipo, enfriamiento));
    }
    

    public void HabilitarArma (TipoArma tipo, bool habilitar) {
        if(tipo == TipoArma.Espada) {
            enfriamientoEspada.gameObject.SetActive(habilitar);
        } else if(tipo == TipoArma.Magia) {
            enfriamientoMagia.gameObject.SetActive(habilitar);
        }
    }

    IEnumerator Enfriamiento (TipoArma tipo, float enfriamiento) {
        yield return null;
        Image destino = (tipo == TipoArma.Espada) ? enfriamientoEspada : enfriamientoMagia;
        destino.fillAmount = 1;
        float lerp = 0;
        float tempo = Time.deltaTime/enfriamiento;

        while(lerp < 1) {
            lerp += tempo;
            destino.fillAmount = Mathf.Lerp(1, 0, lerp);
            yield return null;
        }
    }

    IEnumerator MostrarCirculo (float delay, float tiempo) {
        Color _color = circuloDaño.color;
        circuloDaño.color = new Color(_color.r, _color.g, _color.b, 1);
        yield return new WaitForSeconds(delay);
        float lerp = 0;
        float tempo = Time.deltaTime / tiempo;
        while(lerp < 1) {
            lerp += tempo;
            circuloDaño.color = Color.Lerp(new Color(_color.r, _color.g, _color.b, 1), new Color(_color.r, _color.g, _color.b, 0), lerp);
            yield return null;
        }
    }

    IEnumerator BarraDaño(float tiempo, float diferencia) {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        daño.fillAmount = salud.fillAmount + diferencia;
        float lerp = 0;
        float tempo = Time.deltaTime / tiempo;
        while (lerp < 1) {
            lerp += tempo;
            dañoInterno = Mathf.Lerp(dañoInterno, 0, lerp);
            diferencia = Mathf.Lerp(diferencia, 0, lerp);
            daño.fillAmount = salud.fillAmount + diferencia;
            yield return wait;
        }
    }
}
