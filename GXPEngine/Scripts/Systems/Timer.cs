using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
public class Timer : GameObject
{
    readonly private Action timedOut;
    public int currentTime = 0, limit = 0;
    readonly private bool repeat = false;

    public Timer(Action timeMethod, int time) {
        timedOut += timeMethod;
        limit = time;
        SceneManager.rootObj.LateAddChild(this);
    }

    public Timer(Action timeMethod, int time, bool repeatMode) {
        timedOut += timeMethod;
        limit = time;
        repeat = repeatMode;
        SceneManager.rootObj.LateAddChild(this);
    }

    void Update() {
        if (currentTime < limit)
            currentTime++;
        else {
            timedOut?.Invoke();
            if (repeat) { currentTime = 0; }
            else { Destroy(); }
        }
    }
}