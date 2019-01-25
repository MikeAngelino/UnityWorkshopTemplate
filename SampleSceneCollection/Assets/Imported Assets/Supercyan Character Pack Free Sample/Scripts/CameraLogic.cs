using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class CameraLogic : MonoBehaviour {

    private Transform m_currentTarget;
    private float m_distance = 2f;
    private float m_height = 1;
    private float m_lookAtAroundAngle = 180;

    [FormerlySerializedAs("m_targets")] [SerializeField] private List<Transform> targets;
    private int m_currentIndex;

	private void Start () {
        if(targets.Count > 0)
        {
            m_currentIndex = 0;
            m_currentTarget = targets[m_currentIndex];
        }
	}

    private void SwitchTarget(int step)
    {
        if(targets.Count == 0) { return; }
        m_currentIndex+=step;
        if (m_currentIndex > targets.Count-1) { m_currentIndex = 0; }
        if (m_currentIndex < 0) { m_currentIndex = targets.Count - 1; }
        m_currentTarget = targets[m_currentIndex];
    }

    public void NextTarget() { SwitchTarget(1); }
    public void PreviousTarget() { SwitchTarget(-1); }

    private void Update () {
        if (targets.Count == 0) { return; }
    }

    private void LateUpdate()
    {
        if(m_currentTarget == null) { return; }

        float targetHeight = m_currentTarget.position.y + m_height;
        float currentRotationAngle = m_lookAtAroundAngle;

        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        Vector3 position = m_currentTarget.position;
        position -= currentRotation * Vector3.forward * m_distance;
        position.y = targetHeight;

        transform.position = position;
        transform.LookAt(m_currentTarget.position + new Vector3(0, m_height, 0));
    }
}
