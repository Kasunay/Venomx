using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;



public class GrapplingHook : MonoBehaviour
{

    public ObiSolver solver;
    public ObiCollider character;
    public float hookExtendRetractSpeed = 2;
    public Material material;
    public ObiRopeSection section;

    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    private ObiRopeCursor cursor;

    private RaycastHit hookAttachment;

    private ObiRopePrefabPlugger prefabPlugger;
    public GameObject plyr;
    public Animator _animationController = null;
   // public GameObject kanca;

  

    private bool havada;
   
   // private ObiRopeAttach ropeAttach;
  
    void Awake()
    {

        // Create both the rope and the solver:	
        rope = gameObject.AddComponent<ObiRope>();
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 5);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
       
        rope.GetComponent<MeshRenderer>().material = material;

        // Setup a blueprint for the rope:
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = 0.5f;

        // Tweak rope parameters:
        rope.maxBending = 0.02f;

        // Add a cursor to be able to change rope length:
        cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
        cursor.cursorMu = 0;
        cursor.direction = true;


        
    }


    

    private void OnDestroy()
    {
        DestroyImmediate(blueprint);
    }

    /**
	 * Raycast against the scene to see if we can attach the hook to something.
	 */
    private void LaunchHook()
    {

        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hookAttachment,15))
        {

           


            Transform objectHit = hookAttachment.transform;
        

            if(objectHit.tag == "sim")
            {
              
                StartCoroutine(AttachHook());


              

                    if (Input.mousePosition.x < Screen.width / 2)
                    {
                    
                        var currRotation = plyr.transform.rotation;

                        var targetRotation = Quaternion.Euler(new Vector3(0, -2 * 90 / Mathf.PI, 0));

                    plyr.transform.rotation = Quaternion.Lerp(currRotation, targetRotation, Time.fixedDeltaTime * 12);

                      }


                    else if (Input.mousePosition.x > Screen.width / 2)
                    {
                        var currRotation = plyr.transform.rotation;
                      
                        var targetRotation = Quaternion.Euler(new Vector3(0, 2 * 90 / Mathf.PI, 0));

                    plyr.transform.rotation = Quaternion.Lerp(currRotation, targetRotation, Time.fixedDeltaTime * 12);
                    }



              
            

            }
            if (objectHit.tag == "yukarı")
            {

                StartCoroutine(AttachHook());

                havada = true;
                _animationController.SetBool("süzül", false);

                /*      if (Input.mousePosition.x < Screen.width / 2)
                     {

                        var currRotation = plyr.transform.rotation;

                         var targetRotation = Quaternion.Euler(new Vector3(0, -2 * 90 / Mathf.PI, 0));

                         plyr.transform.rotation = Quaternion.Lerp(currRotation, targetRotation, Time.fixedDeltaTime * 12);

                     }


                     else if (Input.mousePosition.x > Screen.width / 2)
                     {
                         var currRotation = plyr.transform.rotation;

                         var targetRotation = Quaternion.Euler(new Vector3(0, 2 * 90 / Mathf.PI, 0));

                         plyr.transform.rotation = Quaternion.Lerp(currRotation, targetRotation, Time.fixedDeltaTime * 12);
                     }

         */




            }



        }

    }

    private IEnumerator AttachHook()
    {
        yield return 0;
        Vector3 localHit = rope.transform.InverseTransformPoint(hookAttachment.point);
       
        int filter = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything,0);
         
        // Procedurally generate the rope path (a simple straight line):
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(Vector3.zero, -localHit.normalized, localHit.normalized, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "Hook start");
        blueprint.path.AddControlPoint(localHit, -localHit.normalized, localHit.normalized, Vector3.up, 0.1f, 0.1f, 1, filter, Color.white, "Hook end");
        blueprint.path.FlushEvents();

       
        // Generate the particle representation of the rope (wait until it has finished):
        yield return blueprint.Generate();
        
        // Set the blueprint (this adds particles/constraints to the solver and starts simulating them).
        rope.ropeBlueprint = blueprint;
        rope.GetComponent<MeshRenderer>().enabled = true;

        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = rope.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        pinConstraints.Clear();
        var batch = new ObiPinConstraintsBatch();
        batch.AddConstraint(rope.solverIndices[0], character, transform.localPosition, Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.AddConstraint(rope.solverIndices[blueprint.activeParticleCount - 1], hookAttachment.collider.GetComponent<ObiColliderBase>(),
                                                          hookAttachment.collider.transform.InverseTransformPoint(hookAttachment.point), Quaternion.identity, 0, 0, float.PositiveInfinity);
        batch.activeConstraintCount = 2;
        pinConstraints.AddBatch(batch);

        rope.SetConstraintsDirty(Oni.ConstraintType.Pin);


    }

    private void DetachHook()
    {
        // Set the rope blueprint to null (automatically removes the previous blueprint from the solver, if any).
        rope.GetComponent<MeshRenderer>().enabled = false;
        rope.ropeBlueprint = null;
       
        var currRotation = plyr.transform.rotation;

        var targetRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        plyr.transform.rotation = Quaternion.Lerp(currRotation, targetRotation, Time.fixedDeltaTime * 45);
        havada = false;
        _animationController.SetBool("süzül", true);
    }


    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (!rope.isLoaded)
                LaunchHook();
            else
                DetachHook();
        }

        if (havada)
        {
            
        }
        else
        {
            _animationController.SetBool("havada", false);
        
        }
     

        if (rope.isLoaded)
        {
            //kanca.GetComponent<MeshRenderer>().enabled = true;
          //  kanca.transform.rotation = Quaternion.Euler(new Vector3(0,  90 , 0));
            if (rope.CalculateLength() >= 3f)
            {
                cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime * 2.3f);


            }
       

           



        }
        else
        {

           // kanca.GetComponent<MeshRenderer>().enabled = false;
        }






        
    }
}
