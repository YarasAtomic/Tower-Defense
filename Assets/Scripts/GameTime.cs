using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime
{
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------- CLASS ATTRIBUTES -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    static bool paused = false;

    static float gameSpeed = 1f;

    //-----------------------------------------------------------------//
    
    public static float GameSpeed {
        get {
            return paused ? 0 : gameSpeed;
        }
        set {
            gameSpeed = value > 0 ? value : gameSpeed;
        }
    }

    //-----------------------------------------------------------------//

    public static float DeltaTime {
        get {
            return paused ? 0 : Time.deltaTime * gameSpeed;
        }
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!------------------------ CLASS METHODS ------------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//

    public static void Pause() {
        paused = true;
    }

    //-----------------------------------------------------------------//

    public static void Resume() {
        paused = false;
    }

    //-----------------------------------------------------------------//

    public static bool IsPaused() {
        return GameSpeed == 0;
    }

    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
    //!----------------------- END OF GameTime -----------------------!//
    //!---------------------------------------------------------------!//
    //!---------------------------------------------------------------!//
}
