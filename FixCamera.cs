using UnityEngine;

public class FixCamera : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Camera targetCamera;
    public Color color;

    public float maxDistance=2;
    public float minDistance=1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        meshRenderer= GetComponent<MeshRenderer>();
        targetCamera = Camera.main;
        color = meshRenderer.material.color; 
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, targetCamera.transform.position);
           
        meshRenderer.material.color = new Color (color.r,color.g,color.b, Mathf.InverseLerp(minDistance,maxDistance,distance));
    }

  
    
}
