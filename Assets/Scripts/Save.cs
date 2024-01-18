using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class Save : ScriptableObject
{
    [SerializeField] List<SaveFile> saveFiles;
    [SerializeField] int current;
    [SerializeField] int currentLevel = -1;
    bool loaded = false;
    public void SetCurrentFile(int i){
        if(i < saveFiles.Count && i >= 0){
            if(saveFiles[i].IsEmpty()) saveFiles[i].InitSave();
            current = i;
        } 
        Debug.Log("Select saveFile "+current);
    }

    public SaveFile GetSaveFileFromIndex(int i){
        if(i < saveFiles.Count && i >= 0) return saveFiles[i];
        return null;
    }

    public SaveFile GetSaveFile(){
        return (current < saveFiles.Count) ? saveFiles[current] : null;
    }

    public void AddSaveFile(){
        saveFiles.Add(new SaveFile());
    }

    public int GetTotalSaves(){
        return saveFiles.Count;
    }

    public void SetCurrentLevel(int level){
        currentLevel = level;
    }

    public int GetCurrentLevel(){
        return currentLevel;
    }

    public void SaveData(){
        string json = JsonUtility.ToJson(this);
        string dataPath = Application.persistentDataPath + "/save.json";
        System.IO.File.WriteAllText(dataPath, json);
        Debug.Log("Data saved at: \""+dataPath+"\"");
    }

    private void OnEnable(){
        if(!loaded){
            string dataPath = Application.persistentDataPath + "/save.json";
            try{
                string json = System.IO.File.ReadAllText(dataPath);
                Debug.Log("json data: " + json);
                if(json!=null){
                    JsonUtility.FromJsonOverwrite(json, this);
                    loaded = true;
                    Debug.Log("Data loaded");
                }
            }catch(Exception e){
                Debug.Log(e.StackTrace);
                Debug.Log("Couldn't load file");
            }
        }
    }
}
