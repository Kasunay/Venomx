using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PController : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    
    [SerializeField] private float rotationSpeed = 5;
    
    [SerializeField] private Vector2 movementLimit = Vector2.one;

    [SerializeField] private GameObject model = null;
    
    public Animator _animationController = null;
    public Animator _ınsananimationController = null;
    private MeshRenderer _renderer = null;

    private Player _player = null;

    private bool _canMove = true;

    private Vector2 _direction = Vector2.zero;

    private Rigidbody rb;
   
    public PlayerState currentState { get; private set; } = PlayerState.OnStandRun;
  
    private void Start()
    {
        _player = GetComponent<Player>();
        
        
        
       
        rb = GetComponent<Rigidbody>();
        _renderer = GetComponentInChildren<MeshRenderer>();

       // StartCoroutine(DetermineAnimationStateViaBrokenParts());
    }

    private void FixedUpdate()
    {
        if (CanMove())
        {
            // Handle Movement
            var direction = new Vector3(0, 0, speed);

            transform.position += direction * Time.fixedDeltaTime;
            

        }
     

        ValidateLocation();

      

    }
    public void Update()
    {
        float DisstanceToTheGround = GetComponent<Collider>().bounds.extents.y;


        bool IsGrounded = Physics.Raycast(transform.position, Vector3.down, DisstanceToTheGround + 3f);
        

        if (IsGrounded)
        {
            _animationController.SetBool("havada", true);
            _ınsananimationController.SetBool("havada", true);
            
        }
        else
        {

            _animationController.SetBool("süzül", false);
            _animationController.SetBool("havada", false);


            _ınsananimationController.SetBool("süzül", false);
            _ınsananimationController.SetBool("havada", false);
            //yerde
        }
    }



    private void UpdateState(PlayerState newState)
    {
        currentState = newState;
    }
    
    public void StopMovement()
    {
       _canMove = true;
    }

    private bool CanMove()
    {
        return _canMove;
    }
    
    private void OnDragged(Vector2 direction)
    {
        _direction = direction;
    }

    private void OnReleased()
    {
        _direction = Vector2.zero;
    }

    private void OnPressed()
    {
        // TODO
    }

    private void ValidateLocation()
    {
       var currentLocation = transform.position;

        if (currentLocation.x >= movementLimit.y)
        {
            currentLocation.x = movementLimit.y;
            
            _direction = Vector2.zero;
        }
            
        else if (currentLocation.x <= movementLimit.x)
        {
            currentLocation.x = movementLimit.x;
            
            _direction = Vector2.zero;
        }

        transform.position = currentLocation;
      
    }

    private void OnTriggerEnter(Collider other)
    {
      /*  if (other.CompareTag("AFinish"))
        {
            _animationController.DisableAnimator();

            model.transform.DOScale(Vector3.one * -.25F, 1F).OnComplete(StartFinishAnimation);

            model.transform.DOMoveY(1.5F, 1);

            DOTween.To(() => _renderer.sharedMaterial.GetFloat("_Smooth"),
                x => _renderer.sharedMaterial.SetFloat("_Smooth", x), 9, 2);

            GameManager.Instance.RestartGame(15);
        }
        
        else if (other.CompareTag("ASpeed"))
        {
            _animationController.IncreaseRunAnimationRate(2);

            DOTween.To(() => speed, x => speed = x, speed * 2, 4).SetEase(Ease.InCirc);
        }*/
    }

    private void StartFinishAnimation()
    {
        foreach (var piece in _player.BodyParts)
        {
            StartCoroutine(Animate(piece.transform));
        }
    }
    
    private IEnumerator Animate(Transform piece)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0, .75F));

            var randomLocation = 
                new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 1.15F;

            piece.DOLocalMove(randomLocation, .75F);
        
            yield return new WaitForSeconds(.75F);
        }
    }

   
 


}
