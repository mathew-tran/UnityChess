using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is just so when user clicks off board, it removes the plates // no other functionality yet.
public class Board : MonoBehaviour
{

    private void OnMouseUp() {
        Chessman.DestroyMovePlates();
        GameObject.FindGameObjectWithTag("SelectedPlate").transform.position = new Vector3(-300, -300, 0);
    }
}
