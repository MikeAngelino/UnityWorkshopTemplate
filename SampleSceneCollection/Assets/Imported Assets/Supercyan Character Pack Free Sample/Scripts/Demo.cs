using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class Demo : MonoBehaviour {

    private readonly string[] m_animations = { "Pickup","Wave" };
    private Animator[] m_animators;
    [FormerlySerializedAs("m_cameraLogic")] [SerializeField] private CameraLogic cameraLogic;

    private void Start()
    {
        m_animators = FindObjectsOfType<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            cameraLogic.PreviousTarget();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            cameraLogic.NextTarget();
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width));

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Previous character (Q)"))
        {
            cameraLogic.PreviousTarget();
        }

        if (GUILayout.Button("Next character (E)"))
        {
            cameraLogic.NextTarget();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(16);

        for(int i = 0; i < m_animations.Length; i++)
        {
            if(i == 0) { GUILayout.BeginHorizontal(); }

            if(GUILayout.Button(m_animations[i]))
            {
                for(int j = 0; j < m_animators.Length; j++)
                {
                    m_animators[j].SetTrigger(m_animations[i]);
                }
            }

            if(i == m_animations.Length - 1) { GUILayout.EndHorizontal(); }
            else if (i == (m_animations.Length / 2)) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
        }

        GUILayout.Space(16);

        Color oldColor = GUI.color;
        GUI.color = Color.black;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("WASD or arrows: Move");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Left Shift: Walk");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Space: Jump");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUI.color = oldColor;

        GUILayout.EndVertical();
    }
}
