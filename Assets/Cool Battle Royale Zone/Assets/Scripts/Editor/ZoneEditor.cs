using UnityEngine;
using UnityEditor;


namespace CoolBattleRoyaleZone
{
    /// <summary>
    /// Editor class which controls visualizing of zone script and his handles on scene in editor
    /// </summary>

    [CustomEditor ( typeof ( Zone ) )]
    [CanEditMultipleObjects]
    public class ZoneEditor : Editor
    {
        private SerializedProperty _propertyLastCircle;
        private SerializedProperty _propertyFirstCircle;
        private SerializedProperty _propertyZoneCirclesSizeCurve;

        private SerializedProperty _propertyEvents;

        private SerializedProperty _propertyZoneCircles;

        // Method for getting all properties in script
        private void OnEnable ( )
        {
            _propertyLastCircle           = serializedObject.FindProperty ( "LastZoneCircle" );
            _propertyFirstCircle          = serializedObject.FindProperty ( "FirstZoneCircle" );
            _propertyZoneCirclesSizeCurve = serializedObject.FindProperty ( "ZoneCirclesSizeCurve" );
            _propertyEvents               = serializedObject.FindProperty ( "Events" );
            _propertyZoneCircles          = serializedObject.FindProperty ( "ZoneCircles" );
        }

        // Method for drawing all properties from script like we want
        public override void OnInspectorGUI ( )
        {
            var zone = ( Zone ) target;

            EditorGUILayout.LabelField ( "Common Setup" , EditorStyles.centeredGreyMiniLabel );
            EditorGUILayout.Space ( );

            zone.StepsToEnd = EditorGUILayout.IntField ( "Steps Count" , zone.StepsToEnd );
            zone.ResetupZoneCirclesOnStart =
                EditorGUILayout.Toggle ( "Re-setup steps on start ?" , zone.ResetupZoneCirclesOnStart );
            zone.StartupTheZoneOnStart =
                EditorGUILayout.Toggle ( "Startup the zone on start ?" , zone.StartupTheZoneOnStart );

            serializedObject.Update ( );
            EditorGUILayout.PropertyField ( _propertyFirstCircle , new GUIContent ( "First Step Circle" ) , true );
            EditorGUILayout.PropertyField ( _propertyLastCircle ,  new GUIContent ( "Last Step Circle" ) ,  true );
            EditorGUILayout.PropertyField ( _propertyZoneCirclesSizeCurve , new GUIContent ( "Circles Size Curve" ) ,
                                            true );
            EditorGUILayout.Space ( );

            EditorGUILayout.LabelField ( "Events Setup" , EditorStyles.centeredGreyMiniLabel );
            EditorGUILayout.PropertyField ( _propertyEvents , new GUIContent ( "Events" ) , true );

            EditorGUILayout.LabelField ( "Detailed Steps Setup" , EditorStyles.centeredGreyMiniLabel );
            EditorGUILayout.PropertyField ( _propertyZoneCircles , new GUIContent ( "Steps" ) , true );

            serializedObject.ApplyModifiedProperties ( );
            EditorGUILayout.Space ( );
            if ( GUILayout.Button ( "Re-setup Circles" ) ) zone.SetupCircles ( );
            EditorGUILayout.Space ( );
            EditorGUILayout.LabelField ( "Debug Circles" , EditorStyles.centeredGreyMiniLabel );
            zone.ShowStepsCircles = EditorGUILayout.Toggle ( "Show Circles Of Steps ?" , zone.ShowStepsCircles );
            zone.ShowCurrentStepCircle =
                EditorGUILayout.Toggle ( "Show Circle Of Current Step ?" , zone.ShowCurrentStepCircle );
            zone.ShowNextStepCircle = EditorGUILayout.Toggle ( "Show Circle Of Next Step ?" , zone.ShowNextStepCircle );

        }

        // Method for drawing zone circles handles like we want
        private void OnSceneGUI ( )
        {
            var zone = ( Zone ) target;

            if ( zone.ShowStepsCircles && zone.ZoneCircles.Count != 0 ) // If we want show all steps/circles
            {
                foreach ( var circle in zone.ZoneCircles ) // Show them in cycle
                {
                    // Draw the wire disc with position and radius of circle
                    Handles.color = Color.white;
                    Handles.DrawWireDisc ( circle.PositionAndRadius.Position , new Vector3 ( 0 , 1 , 0 ) ,
                                           circle.PositionAndRadius.Radius );
                    // Draw the solid disc with position and radius of circle
                    Handles.color = new Color ( 1 , 0 , 0 , 0.1f );
                    Handles.DrawSolidDisc ( circle.PositionAndRadius.Position , new Vector3 ( 0 , 1 , 0 ) ,
                                            circle.PositionAndRadius.Radius );
                    // Draw the cylinder handle cap with position and radius of circle
                    Handles.color = new Color ( 1 , 0 , 0 , 0.1f );
                    Handles.CylinderHandleCap ( 0 ,
                                                circle.PositionAndRadius.Position +
                                                Vector3.up * circle.PositionAndRadius.Radius ,
                                                Quaternion.identity *
                                                Quaternion.LookRotation ( Vector3.up ) ,
                                                circle.PositionAndRadius.Radius * 2 , EventType.Repaint );
                }
            }

            if ( zone.ShowCurrentStepCircle ) // If we want show current safe circle
            {
                // Draw the wire disc with position and radius of current safe zone
                Handles.color = Color.white;
                Handles.DrawWireDisc ( zone.CurrentSafeZone.Position , new Vector3 ( 0 , 1 , 0 ) ,
                                       zone.CurrentSafeZone.Radius );
                // Draw the solid disc with position and radius of current safe zone
                Handles.color = new Color ( 0 , 1 , 0 , 0.1f );
                Handles.DrawSolidDisc ( zone.CurrentSafeZone.Position , new Vector3 ( 0 , 1 , 0 ) ,
                                        zone.CurrentSafeZone.Radius );
                // Draw the cylinder handle cap with position and radius of current safe zone
                Handles.color = new Color ( 0 , 1 , 0 , 0.1f );
                Handles.CylinderHandleCap ( 0 ,
                                            zone.CurrentSafeZone.Position + Vector3.up * zone.CurrentSafeZone.Radius ,
                                            Quaternion.identity         * Quaternion.LookRotation ( Vector3.up ) ,
                                            zone.CurrentSafeZone.Radius * 2 , EventType.Repaint );
            }

            if ( zone.ShowNextStepCircle ) // If we want show next safe circle
            {
                // Draw the wire disc with position and radius of next safe zone
                Handles.color = Color.white;
                Handles.DrawWireDisc ( zone.NextSafeZone.Position , new Vector3 ( 0 , 1 , 0 ) ,
                                       zone.NextSafeZone.Radius );
                // Draw the solid disc with position and radius of next safe zone
                Handles.color = new Color ( 1 , 1 , 1 , 0.1f );
                Handles.DrawSolidDisc ( zone.NextSafeZone.Position , new Vector3 ( 0 , 1 , 0 ) ,
                                        zone.NextSafeZone.Radius );
                // Draw the cylinder handle cap with position and radius of next safe zone
                Handles.color = new Color ( 1 , 1 , 1 , 0.1f );
                Handles.CylinderHandleCap ( 0 , zone.NextSafeZone.Position + Vector3.up * zone.NextSafeZone.Radius ,
                                            Quaternion.identity      * Quaternion.LookRotation ( Vector3.up ) ,
                                            zone.NextSafeZone.Radius * 2 , EventType.Repaint );
            }
        }
    }
}
