using UnityEngine;
using static OptionsManager;

public class OptionsApplier : MonoBehaviour
{

    [SerializeField]
    Vector2Int[] resolutions;

    [SerializeField]
    int[] fps;

    void Start ( )
    {
        OnOptionsChanged += ApplyOptions;

    }
    void ApplyOptions ( OptionsData options )
    {
        var width = resolutions[options.ScreenResIndex].x;
        var heigth = resolutions[options.ScreenResIndex].y;

        int fps = this.fps[options.FpsIndex];

        QualitySettings.SetQualityLevel (options.QualityIndex);

        //Screen.SetResolution (width, heigth, options.Fullscreen);
        Application.targetFrameRate = fps;
        QualitySettings.vSyncCount = options.VSync ? 1 : 0;

        Debug.Log ($"Resolution: {Screen.currentResolution}");
        Debug.Log ($"FPS: {Application.targetFrameRate}");
        Debug.Log ($"Vsync: {QualitySettings.vSyncCount}");
    }
}
