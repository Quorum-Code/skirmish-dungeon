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
    }

    private void LateUpdate()
    {
        CameraMove();
        CameraRotate();
    }

    private void CameraMove() 
    {
        moveInput.x = Input.GetAxis("Vertical");
        moveInput.y = Input.GetAxis("Horizontal");

        Vector3 moveDir = (cameraHost.transform.forward * moveInput.x + cameraHost.transform.right * moveInput.y);

        if (moveDir.magnitude > 1f)
        {
            moveDir = moveDir.normalized;
        }

        // moveDir = cameraHost.transform.TransformDirection(moveDir); 
        cameraHost.transform.Translate(moveDir * cameraMoveSpeed * Time.deltaTime, Space.World);
    }

    private void CameraRotate() 
    {
        if (Input.GetKeyUp(KeyCode.E))
            rotInput = 1;
        else if (Input.GetKeyUp(KeyCode.Q))
            rotInput = -1;

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

    private IEnumerator animateRotation(int direction) 
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

    private void CheckStaticSpriteGroups() 
    {
        // foreach spritegroup
    }

    class StaticSpriteGroup 
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        private float yRot = 0f;

        public StaticSpriteGroup(List<SpriteRenderer> _sprites, float _yRot) 
        {
            this.sprites.AddRange(_sprites);
            this.yRot = _yRot;
        }

        public void AddSprite(SpriteRenderer sprite) 
        {
            sprites.Add(sprite);
        }

        private void flipSprites() 
        {
            foreach (SpriteRenderer sprite in sprites) 
            {
                sprite.flipX = !sprite.flipX;
                sprite.gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
            }
        }
    }
}
