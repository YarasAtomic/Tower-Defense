using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Save : ScriptableObject
{
    [SerializeField] List<SaveFile> saveFiles;
    [SerializeField] int current;
    [SerializeField] int currentLevel = -1;
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
}
