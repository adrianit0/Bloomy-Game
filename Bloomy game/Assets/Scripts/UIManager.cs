using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    
	public void CambiarEscena (string escena) {
        Application.LoadLevel(escena);
	}
}
