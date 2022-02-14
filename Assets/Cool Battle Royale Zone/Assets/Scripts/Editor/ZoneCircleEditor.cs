using UnityEngine;
using UnityEditor;

namespace CoolBattleRoyaleZone
{
    /// <summary>
    /// Editor class which controls visualizing of zone circle
    /// </summary>

    [CustomEditor ( typeof ( ZoneCircle ) )]
    [CanEditMultipleObjects]
    public class ZoneCircleEditor : Editor
    {

        // Method for draw handles on scene in editor
        private void OnSceneGUI ( )
        {
            // Drawing the circle //

            var circle = ( ZoneCircle ) target;
            // Draw the wire disc with position and radius of circle
            Handles.color = Color.white;
            Handles.DrawWireDisc ( circle.transform.position , new Vector3 ( 0 , 1 , 0 ) , circle.Radius );
            // Draw the solid disc with position and radius of circle
            Handles.color = new Color ( 1 , 0 , 0 , 0.1f );
            Handles.DrawSolidDisc ( circle.transform.position , new Vector3 ( 0 , 1 , 0 ) , circle.Radius );
            Handles.color = new Color ( 1 , 0 , 0 , 0.1f );
            // Draw the cylinder handle cap with position and radius of circle
            Handles.CylinderHandleCap ( 0 , circle.transform.position + Vector3.up * circle.Radius ,
                                        circle.transform.rotation * Quaternion.LookRotation ( Vector3.up ) ,
                                        circle.Radius             * 2 , EventType.Repaint );


            // Drawing free move handle 
            Handles.color = Color.white;
            var center     = circle.transform.position;
            var capPos     = center + Vector3.right * circle.Radius;
            var customSize = HandleUtility.GetHandleSize ( circle.transform.position ) * 0.1f;
            var customSnap = Vector3.one                                               * 0.5f;


            EditorGUI.BeginChangeCheck ( ); // begin check of changes of handle
            var handlePos = Handles.FreeMoveHandle ( capPos , Quaternion.identity , customSize , customSnap ,
                                                     Handles.DotHandleCap );
            if ( EditorGUI.EndChangeCheck ( ) ) // if something changed
            {
                Undo.RecordObject ( circle , "Changed Radius" ); // record this changes
                // And add this delta of changed value to radius of circle
                var offset = ( handlePos.x - capPos.x ) / 2f;
                circle.Radius += offset;
            }
        }
    }
}
