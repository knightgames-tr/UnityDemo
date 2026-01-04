using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawer : MonoBehaviour
{
    
    public enum GizmoShape{
        Sphere,
        Arrow
    }

    [Header ("Gizmo Settings")]
    public GizmoShape _gizmoShape = GizmoShape.Sphere;
    public Color _gizmosColor = Color.magenta;
    public float _gizmosSize = 0.15f;
    void OnDrawGizmos()
    {
        Gizmos.color = _gizmosColor;

        if(_gizmoShape == GizmoShape.Sphere){
            Gizmos.DrawSphere(transform.position, _gizmosSize);
        }else if(_gizmoShape == GizmoShape.Arrow){    
            Gizmos.DrawLine(transform.position,transform.position+_gizmosSize*transform.forward);
        }
    }
}
