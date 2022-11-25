using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Transform cam;


    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -9.81f;


    private float currentVelocity;
    [SerializeField] private float smoothTime = 0.5f;

    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundSensor;
    [SerializeField] private float sensorRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 playerVelocity;


    [SerializeField] private LayerMask ignoreLayer;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 10f, ignoreLayer))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green);

            Vector3 hitPosition = hit.point;

            //Estos son ejemplos de lo que se puede almacenar, no hace falta ponerlo.
            string hitName = hit.transform.name;
            string hitTag = hit.transform.tag;
            float hitDistance = hit.distance;
            Animator hitAnim = hit.transform.gameObject.GetComponent<Animator>();

            if(hitTag == "obstaculo")
            {
                Debug.Log("impacto en obstaculo");
            }

            if(hitAnim != null)
            {
                hitAnim.SetBool("Jump", true);
            }
        }

        else
        {
            Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if(Physics.Raycast(ray, out rayHit))
            {
                Debug.Log(rayHit.transform.name);

                //Esto es un ejemplo de que mas se puede poner no hace falta ponerlo.
                if(rayHit.transform.name == "Capsule")
                {
                    Debug.Log("Personaje");
                }
            }
        }

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if(movement != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref currentVelocity, smoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            controller.Move(moveDirection * speed * Time.deltaTime);
        }

        //Descomentar para el examen de CharacterController y comentarlo para el examen de Raycast(mejor borrarlo directamente en vez de comentarlo)
        //isGrounded = Physics.CheckSphere(groundSensor.position, sensorRadius, groundLayer);

        if(Physics.Raycast(groundSensor.position, Vector3.down, sensorRadius, groundLayer))
        {
            isGrounded = true;
            Debug.DrawRay(groundSensor.position, Vector3.down * sensorRadius, Color.green);
        }

        else
        {
            isGrounded = false;
            Debug.DrawRay(groundSensor.position, Vector3.down * sensorRadius, Color.red);
        }

        if(playerVelocity.y < 0 && isGrounded)
        {
            playerVelocity.y = 0;
        }

        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(groundSensor.position, Vector3.down * sensorRadius);
    }
}