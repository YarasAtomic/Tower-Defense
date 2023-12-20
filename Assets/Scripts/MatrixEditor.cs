using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelLogic))]
public class MatrixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelLogic levelLogic = (LevelLogic)target;

 
        GUIStyle lilCell = new GUIStyle();
        lilCell.fixedWidth = 50;
        
        GUIStyle box = new GUIStyle("box");
        box.padding = new RectOffset (10, 10, 10, 10);
        box.margin.right = 10;


        // Draw the default inspector
        DrawDefaultInspector();

        // Add a custom section for the matrix
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Matrix Editor", EditorStyles.boldLabel);

            

        // Ensure the matrix has been initialized
        if (levelLogic.waves == null)
        {
            EditorGUILayout.HelpBox("Matrix not initialized. Please set the dimensions and click 'Apply'.", MessageType.Warning);
        }
        else
        {
            
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // Draw input fields for each element of the matrix
            for (int i = 0; i < levelLogic.waves.GetLength(0); i++)
            {
                EditorGUILayout.BeginVertical(lilCell);
                for (int j = 0; j < levelLogic.waves.GetLength(1); j++)
                {
                    levelLogic.waves[i,j] = EditorGUILayout.IntField($"", levelLogic.waves[i,j]);
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndHorizontal();
            


            // Button to apply changes
            if (GUILayout.Button("Apply Changes"))
            {
                // You can add additional logic here if needed
                Debug.Log("Matrix values applied!");
            }
        }

    

        // Apply any changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
