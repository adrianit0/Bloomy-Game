using UnityEngine;
using System.Collections;

public class Caja : MonoBehaviour {

    public ChanceInfo[] info;

    public void Destruir () {
        Destroy(this.gameObject);
    }

    public void RomperCaja () {
        if (info.Length==0) {
            Debug.Log("Esta caja está vacia");
            Destruir();
            return;
        }
        float sumaTotal = 0;
        float[] probReal = new float[info.Length];
        float value = Random.value;

        for (int i = 0; i < info.Length; i++) {
            sumaTotal += info[i].prob;
        }

        for(int i = 0; i < info.Length; i++) {
            int o = (i == 0) ? 0 : i-1;
            probReal[i] = probReal[o] + info[i].prob/sumaTotal;
        }

        for (int i = 0; i < info.Length; i++) {
            if (value < probReal[i]) {
                ObtenerItem(info[i].objeto);
                break;
            }
        }

        Destruir();
    }

    void ObtenerItem (int itemObtenido) {
        if(GameManagerScript.gameManager == null)
            return;

        GameObject _obj = (GameObject)Instantiate(GameManagerScript.gameManager.itemPrefab, this.transform.position, Quaternion.identity);
        Objeto _item = _obj.GetComponent<Objeto>();
        _item.CambiarArma(itemObtenido, 0);
    }


}

[System.Serializable]
public class ChanceInfo {
    public int objeto;
    public float prob;
}