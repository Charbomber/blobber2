using UnityEngine;

public class Billboard : MonoBehaviour {
    Camera mainCamera;

    // COPIED FROM: https://gamedevbeginner.com/billboards-in-unity-and-how-to-make-your-own/
    // No plagarism here im just lazy

    void Start() {
        mainCamera = Camera.main;
    }

    void LateUpdate() {
        transform.rotation = mainCamera.transform.rotation;
    }
}
