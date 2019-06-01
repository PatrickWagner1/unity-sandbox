using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript1 : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    private void mouseEvent () {
        if (Input.GetMouseButtonDown (0)) {
            Debug.Log ("Linke Maustaste gedrückt!");
            Vector3 currentPos = Input.mousePosition;
            double currentXPos = Input.mousePosition.x;
            double currentYPos = Input.mousePosition.y;
            double currentZPos = Input.mousePosition.z;
            // can be - and + depending on movement up or down
            double movement = Input.GetAxis ("Mouse ScrollWheel");
            if (movement != 0) {

                // check if current pos - movement <= 0
                if (true) {
                    // water

                }

            }
        }
    }
}