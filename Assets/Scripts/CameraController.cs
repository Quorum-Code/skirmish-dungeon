using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera mainCam;
    GameObject[] pawnObjs = null;

    [SerializeField] GameObject cameraHost;
    [SerializeField] float cameraRotSpeed = 5f;
    int rotInput = 0;

    [SerializeField] float cameraMoveSpeed = 1f;
    Vector3 moveInput = Vector3.zero;

    IEnumerator smoothRotCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraHost == null) 
        {
            Debug.LogError("No camera host");
            Destroy(this);
        }

        mainCam = Camera.main;
        pawnObjs = GameObject.FindGameObjectsWithTag("Pawn");
    }

    // Update is called once per frame
    void Update()
    {
        animatePawns();

        if (Input.GetKeyUp(KeyCode.E))
            rotInput = 1;
        else if (Input.GetKeyUp(KeyCode.Q))
            rotInput = -1;

        moveInput.x = Input.GetAxis("Vertical");
        moveInput.y = Input.GetAxis("Horizontal");

        Vector3 moveDir = (mainCam.transform.forward * moveInput.y + mainCam.transform.right * moveInput.x);
        cameraHost.transform.Translate(moveDir * cameraMoveSpeed * Time.deltaTime);

        checkRot();
    }

    private void checkRot() 
    {
        if (rotInput != 0 && smoothRotCoroutine == null) 
        {
            smoothRotCoroutine = animateRotation(rotInput);
            StartCoroutine(smoothRotCoroutine);

            rotInput = 0;
        }
    }

    private void animatePawns() 
    {
        foreach (GameObject pawn in pawnObjs) 
        {
            pawn.transform.rotation = mainCam.transform.rotation;
        }
    }

    IEnumerator animateRotation(int direction) 
    {
        Quaternion old = cameraHost.transform.rotation;
        Quaternion target = Quaternion.Euler(0, old.eulerAngles.y + 90 * direction, 0);

        float delta = 0f;
        float timeMod = 2f;
        while (delta < 1f)
        {
            delta += Time.deltaTime * timeMod;

            // Lerp rotation
            cameraHost.transform.rotation = Quaternion.Slerp(old, target, delta);
            if (delta > .9f)
                timeMod = 1f;

            yield return null;
        }
        cameraHost.transform.rotation = target;

        smoothRotCoroutine = null;
    }
}
