using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelLogic))]
public class MatrixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        LevelLogic levelLogic = (LevelLogic)target;

        GUIStyle lilCell = new GUIStyle();
        lilCell.fixedWidth = 50;

        GUIStyle rowtitle = new GUIStyle();
        rowtitle.fixedWidth = 60;
        
        GUIStyle box = new GUIStyle("box");
        box.padding = new RectOffset (10, 10, 10, 10);
        box.margin.right = 10;


        // Draw the default inspector
        DrawDefaultInspector();

        // Add a custom section for the matrix
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Waves Editor", EditorStyles.boldLabel);

            

        // Ensure the matrix has been initialized
        if (levelLogic.waveList == null)
        {
            EditorGUILayout.HelpBox("Matrix not initialized. Please set the dimensions and click 'Apply'.", MessageType.Warning);
        }
        else
        {
            int enemyTypeCount = levelLogic.GetEnemyTypeCount();
            int totalWaves = levelLogic.GetTotalWaves();

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // Draw input fields for each element of the matrix
            for (int i = -1; i < totalWaves; i++)
            {
                EditorGUILayout.BeginVertical(i == -1 ? rowtitle : lilCell);
                for (int j = 0; j < enemyTypeCount; j++)
                {
                    int index = i*enemyTypeCount+j;
                    if(i >=0){
                        if(j == 0) EditorGUILayout.LabelField("Wave " + i);

                        levelLogic.waveList[index] = EditorGUILayout.IntField($"", levelLogic.waveList[index]);
                    }else{
                        if(j == 0) EditorGUILayout.LabelField("");
                        EditorGUILayout.LabelField("Enemy " + j);
                    }
                }
                if(i > -1) levelLogic.splitterMethods[i] = (EnemySpawner.Splitter) EditorGUILayout.EnumPopup(levelLogic.splitterMethods[i]);
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Add a new column
            if(GUILayout.Button("Add wave")){
                for(int i = 0; i < enemyTypeCount;i++){
                    levelLogic.waveList.Add(0);
                    levelLogic.splitterMethods.Add(EnemySpawner.Splitter.OneByOne);
                }
                serializedObject.ApplyModifiedProperties();
            }

            // Remove column
            if(GUILayout.Button("Remove wave")){
                if(levelLogic.waveList.Count > 0) {
                    levelLogic.waveList.RemoveRange(enemyTypeCount*totalWaves-enemyTypeCount,enemyTypeCount);
                    levelLogic.splitterMethods.RemoveAt(levelLogic.splitterMethods.Count-1);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            // Start the next wave
            if (GUILayout.Button("Next wave"))
            {
                levelLogic.StartWave();
                serializedObject.ApplyModifiedProperties();
            }

            // Detect changes
            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }
        }

    

        
    }
}
