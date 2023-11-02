using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    private bool isRunning = false;

    private Animator anim;

    private CharacterController controller;
    private float jumpForce = 4.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLane = 1;

    //speed modifier
    private float originalSpeed = 7.0f;
    private float speed; 
    private float speedIncreaseLastTick;
    private float speadIncreaseTime = 2.5f;
    private float speadIncreaseAmount = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        speed = originalSpeed;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isRunning)
            return;

        if (Time.time - speedIncreaseLastTick > speadIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speadIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed); 
            //change modifier text
             
        }

        if(MobileInput.Instance.SwipeLeft)
          //move left
            MoveLane(false);
        if(MobileInput.Instance.SwipeRight)
            //move right  
            MoveLane(true);  

         //Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if(desiredLane == 0)
        targetPosition += Vector3.left * LANE_DISTANCE ;
        else if (desiredLane == 2) 
         targetPosition += Vector3.right * LANE_DISTANCE;

         //Calculating move delta
         Vector3 moveVector = Vector3.zero;
         moveVector.x = (targetPosition - transform.position).normalized.x* speed;
          
         bool isGrounded = IsGrounded();
            anim.SetBool("Grounded", isGrounded);  

         //calculate Y
         if(IsGrounded()){
            verticalVelocity = -0.1f;
            if(MobileInput.Instance.SwipeUp)
            {
                //Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce; 
            }
            else if(MobileInput.Instance.SwipeDown)
            {
                //slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
         }
         else{
            verticalVelocity -= (gravity * Time.deltaTime);

            //fast falling mechanic
            if((MobileInput.Instance.SwipeDown))
            {
                verticalVelocity -= jumpForce;
            }
         }
         
         moveVector.y = verticalVelocity;
         moveVector.z = speed;

         //Move the Pengu
         controller.Move(moveVector * Time.deltaTime);

         //Rotating the Pengu to where he is going
         Vector3 dir = controller.velocity;
         dir.y = 0;
         transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
    ;}

    private void MoveLane(bool goingRight)
    {
       desiredLane +=(goingRight) ? 1:-1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
      
       /* This entire line of code can also be written as
       
        if(!goingRight)
        {
            desiredLane--;
            if (desiredLane == -1)
            desiredLane= 0;
        }
        else
        {
            desiredLane++;
            if (desiredLane == 3)
            desiredLane= 2;
        }*/
    }
     
    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x,(controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z), Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

       
        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRunning");
    }
    public void StartSliding()
    {
       anim.SetBool("Sliding", true); 
       controller.height /= 2;
       controller.center = new Vector3(controller.center.x, controller.center.y/2, controller.center.z);
    }
    public void StopSliding()
    {
        anim.SetBool("Sliding", false); 
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y*2, controller.center.z);
    }
    private void Crash()
        {
            anim.SetTrigger("Death"); 
            isRunning = false;
            GameManager.Instance.OnDeath();
        }
    
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
            break;
        }
    }
}
