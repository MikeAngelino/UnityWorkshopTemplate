// Floater v0.0.2
// by Donovan Keith
//
// [MIT License](https://opensource.org/licenses/MIT)
 
using UnityEngine;
using System.Collections;
 
// Makes objects float up & down while gently spinning.
public class floatScript : MonoBehaviour {
    // User Inputs
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    public SimpleCharacterControl player;
 
    // Position Storage Variables
    Vector3 m_posOffset = new Vector3 ();
    Vector3 m_tempPos = new Vector3 ();
 
    // Use this for initialization
    void Start () {
        // Store the starting position & rotation of the object
        m_posOffset = transform.position;
        
    }
     
    // Update is called once per frame
    void Update () {
        // Spin object around Y-Axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 
        // Float up/down with a Sin()
        m_tempPos = m_posOffset;
        m_tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
 
        transform.position = m_tempPos;
        
    }
}
