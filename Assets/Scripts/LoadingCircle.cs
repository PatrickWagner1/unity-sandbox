using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircle : MonoBehaviour
{
    /// The rect component object
    private RectTransform rectComponent;

    /// The rotate speed of the loading circle
    private float rotateSpeed = 200f;

    // Set the rect component object.
    void Start()
    {
        this.rectComponent = GetComponent<RectTransform>();
    }

    // Roateds the loading circle on update
    public void Update()
    {
        this.rectComponent.Rotate(0f, 0f, this.rotateSpeed * Time.deltaTime);
    }
}
