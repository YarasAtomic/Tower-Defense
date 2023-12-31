using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Save : ScriptableObject
{
    [SerializeField] List<SaveFile> saveFiles;
    [SerializeField] int current;

    public void SetCurrentFile(int i){
        if(i < saveFiles.Count) current = i;
        Debug.Log("Select saveFile "+current);
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
}
