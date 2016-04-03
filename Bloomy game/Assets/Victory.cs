using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D coll) {
        Character _char = coll.gameObject.GetComponent<Character>();

        if(_char != null) {
            if(GameManagerScript.gameManager != null &&_char.bando==0 && _char.vivo) {
                _char.invencible = true;
                _char.vivo = false;
                _char.Moverse(0);
                GameManagerScript.gameManager.Victoria();
            }
        } 
    }
}
