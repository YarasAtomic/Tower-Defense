using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelLogic))]
public class MatrixEditor : Editor
{
    SerializedProperty waveList;
    SerializedProperty splitterMethods;
    void OnEnable()
    {
        waveList = serializedObject.FindProperty("waveList");
        splitterMethods = serializedObject.FindProperty("splitterMethods");
    }
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
        if (waveList == null)
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

                        waveList.GetArrayElementAtIndex(index).intValue
                            = EditorGUILayout.IntField($"", waveList.GetArrayElementAtIndex(index).intValue);
                    }else{
                        if(j == 0) EditorGUILayout.LabelField("");
                        EditorGUILayout.LabelField("Enemy " + j);
                    }
                }
                if(i > -1) {
                    string[] enumNamesList = System.Enum.GetNames (typeof(EnemySpawner.Splitter));
                    splitterMethods.GetArrayElementAtIndex(i).intValue 
                        = EditorGUILayout.Popup(splitterMethods.GetArrayElementAtIndex(i).intValue,enumNamesList);
                }
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Add a new column
            if(GUILayout.Button("Add wave")){
                for(int i = 0; i < enemyTypeCount;i++){
                    waveList.InsertArrayElementAtIndex(waveList.arraySize);
                    // levelLogic.waveList.Add(0);
                    splitterMethods.InsertArrayElementAtIndex(splitterMethods.arraySize);
                }
                serializedObject.ApplyModifiedProperties();
            }

            // Remove column
            if(GUILayout.Button("Remove wave")){
                if(levelLogic.waveList.Count > 0) {
                    for(int i = 0; i < enemyTypeCount;i++){
                        waveList.DeleteArrayElementAtIndex(waveList.arraySize-1);
                    }
                    // levelLogic.waveList.RemoveRange(enemyTypeCount*totalWaves-enemyTypeCount,enemyTypeCount);
                    waveList.DeleteArrayElementAtIndex(waveList.arraySize-1);
                    // levelLogic.splitterMethods.RemoveAt(levelLogic.splitterMethods.Count-1);
                    splitterMethods.DeleteArrayElementAtIndex(splitterMethods.arraySize-1);
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
#endif
