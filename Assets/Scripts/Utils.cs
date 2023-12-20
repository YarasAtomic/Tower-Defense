using System;
using UnityEngine;

public class Utils{
    static public bool Raycast(Vector3 pos, Vector3 dir, float length,LayerMask layer,out RaycastHit hit){
        if(Physics.Raycast(pos,dir,out hit,length,layer)){
            Debug.DrawRay(pos, dir*length, Color.red);
            return true;
        }else{
            Debug.DrawRay(pos, dir*length, Color.green);
            return false;
        }
    }

    static public void DrawLocator(Vector3 pos){
        Debug.DrawRay(pos,Vector3.left);
        Debug.DrawRay(pos,Vector3.right);
        Debug.DrawRay(pos,Vector3.forward);
        Debug.DrawRay(pos,Vector3.back);
        Debug.DrawRay(pos,Vector3.up);
        Debug.DrawRay(pos,Vector3.down);
    }

    static public double Rad2Degree = (180.0 / Math.PI);
}