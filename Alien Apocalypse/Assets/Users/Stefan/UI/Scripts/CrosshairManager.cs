using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField]
    Color defaultColor, canGrappleColor;

    Color targetCrosshairColor;
    [SerializeField]
    float colorSmoothTime;

    float currentColorTime;



    [SerializeField]
    Image crosshairImage;

    [SerializeField]
    float animationTime;

    [SerializeField]
    AnimationCurve sizeCurve;

    bool active;

    float currentAnimationTime;

    [SerializeField]
    float bloom;

    [SerializeField]
    float power;

    [SerializeField]
    float crosshairSize;

    [SerializeField]
    UIImageSelector crosshair;

    Vector3 startPolygonAngles, targetPolygonAngles;



    float Progress
    {
        get
        {
            return Mathf.Clamp01 (Mathf.InverseLerp (0, animationTime, currentAnimationTime));
        }
    }

    [SerializeField]
    Grappling grappling;
    public bool CanGrapple
    {
        get
        {
            if ( grappling == null )
                grappling = FindObjectOfType<Grappling> ( );
            if ( grappling == null )
                return false;

            return grappling.canGrapple && grappling.inRange;
        }
    }

    private void Start ( )
    {
        SetCrosshair (crosshair.CurrentChoiseIndex, 1);
    }

    private void OnEnable ( )
    {
        OptionsManager.onOptionsChanged += SetCrosshair;
    }

    private void OnDisable ( )
    {
        OptionsManager.onOptionsChanged -= SetCrosshair;
    }

    public void SetCrosshair (OptionsManager.OptionsData options )
    {
        SetCrosshair (options.CrosshairIndex, options.CrosshairSize);
    }

    void SetCrosshair(int index, float size )
    {
        crosshairImage.sprite = crosshair.choises[index];
        crosshairSize = size;

        crosshairImage.transform.localScale = Vector3.one * crosshairSize;
    }

    private void Update ( )
    {
        //Progress Time
        if ( active && currentAnimationTime < animationTime )
        {
            UpdateShootAnimation ( );
            currentAnimationTime += Time.deltaTime;
        }
        else
        {
            active = false;
            ResetShootAnimation ( );
        }


        //Handle crosshair color depending if you can grapple
        if ( CanGrapple && targetCrosshairColor != canGrappleColor )
        {
            targetCrosshairColor = canGrappleColor;
            currentColorTime = 0;
        }
        else if ( CanGrapple == false && targetCrosshairColor != defaultColor )
        {
            targetCrosshairColor = defaultColor;
            currentColorTime = 0;
        }

        Color color = Color.Lerp (crosshairImage.color, targetCrosshairColor, Mathf.InverseLerp (0, colorSmoothTime, currentColorTime));

        crosshairImage.color = color;

        currentColorTime += Time.deltaTime;
    }

    void UpdateShootAnimation ( )
    {
        float sin = Mathf.Sin (Progress * 3);

        sin = Mathf.Clamp01 (sin);

        crosshairImage.transform.localScale = Vector3.one * crosshairSize + Vector3.one * sizeCurve.Evaluate (sin);

    }

    void ResetShootAnimation ( )
    {
        crosshairImage.transform.localScale = Vector3.one * crosshairSize;
    }

    public void SetBloomAndPower ( float bloom, float power )
    {
        this.bloom = bloom;
        this.power = power;
    }

    public void Shoot ( )
    {
        if ( OptionsManager.Options.CrosshairEffects == false )
            return;



        active = true;
        currentAnimationTime = 0;

    }
}
