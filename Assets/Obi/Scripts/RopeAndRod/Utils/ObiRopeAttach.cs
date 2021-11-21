using UnityEngine;
using System.Collections;

namespace Obi
{
    public class ObiRopeAttach : MonoBehaviour
    {
        public ObiPathSmoother generator;
        [Range(0,1)]
        public float m;
        public GameObject sa;
      
        
        public bool hazır = false;
        public void Awake()
        {

           
        }
        public void LateUpdate()
		{

            
                ObiPathFrame section = generator.GetSectionAt(m);
                transform.position = generator.transform.TransformPoint(section.position);
                //transform.rotation = generator.transform.rotation * (Quaternion.LookRotation(section.tangent * -1.3f, section.binormal));
                
           


          
            
        }
        public void Update()
        {
            if(generator == null)
            {
                generator = sa.GetComponent<ObiPathSmoother>();

            }
                

        }

    }
}