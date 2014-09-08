using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {
    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    private float fps;
    void Start() {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }
    void OnGUI()
    {
        Rect r = camera.pixelRect;
        Vector2 centre = r.center;
        Rect renderRect = new Rect(centre.x, centre.y, 100, 50);

        GUI.Label(renderRect, "" + fps.ToString("f2"));
    }
    void Update() {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval) {
            fps = (float)frames / (timeNow - (float)lastInterval);
            frames = 0;
            lastInterval = (double)timeNow;
        }
    }
}
