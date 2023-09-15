using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField]
    Color defaultColor, canGrappleColor;

    Color targetCrosshairColor;
    [SerializeField]
    float colorSmoothTime;

    float currentColorTime;



    [SerializeField]
    Image crosshairDot;

    [SerializeField]
    Image crosshairPolygon;

    [SerializeField]
    Image leftSide,rightSide,upSide,downSide;

    [SerializeField]
    Vector3 leftOffset,rightOffset,upOffset,downOffset;

    [SerializeField]
    float animationTime;

    [SerializeField]
    AnimationCurve sidesOffset;

    [SerializeField]
    float sideOffsetMultiplier;

    [SerializeField]
    AnimationCurve polygonSize;

    [SerializeField]
    float polygonRotationAmount;

    [SerializeField]
    AnimationCurve dotSize;

    bool active;

    float currentAnimationTime;

    [SerializeField]
    float bloom;

    [SerializeField]
    float power;


    Vector3 startPolygonAngles, targetPolygonAngles;

    

    float Progress
    {
        get
        {
            return Mathf.Clamp01(Mathf.InverseLerp(0, animationTime, currentAnimationTime));
        }
    }

    [SerializeField]
    Grappling grappling;
    public bool CanGrapple
    {
        get
        {
            if (grappling == null) grappling = FindObjectOfType<Grappling>();
            if(grappling == null) return false;

            return grappling.canGrapple && grappling.inRange;
        }
    }

    private void Start()
    {
        leftOffset = leftSide.transform.position;
        rightOffset = rightSide.transform.position;
        upOffset = upSide.transform.position;
        downOffset = downSide.transform.position;
    }

    private void Update()
    {
        //Progress Time
        if (active && currentAnimationTime < animationTime)
        {
            UpdateShootAnimation();
            currentAnimationTime += Time.deltaTime;
        }
        else
        {
            active = false;
        }


        //Handle crosshair color depending if you can grapple
        if (CanGrapple && targetCrosshairColor != canGrappleColor)
        {
            targetCrosshairColor = canGrappleColor;
            currentColorTime = 0;
        }
        else if (CanGrapple == false && targetCrosshairColor != defaultColor)
        {
            targetCrosshairColor = defaultColor;
            currentColorTime = 0;
        }

        Color color = Color.Lerp(crosshairPolygon.color, targetCrosshairColor, Mathf.InverseLerp(0, colorSmoothTime, currentColorTime));

        crosshairPolygon.color = color;
        crosshairDot.color = color;

        currentColorTime += Time.deltaTime;
    }

    void UpdateShootAnimation()
    {
        float sin = Mathf.Sin(Progress * 3);

        Vector3 dotScale = Vector3.one * dotSize.Evaluate(power) + Vector3.one * sin;

        crosshairDot.transform.localScale = dotScale;

        Vector3 polygonSize = Vector3.one * this.polygonSize.Evaluate(power) + Vector3.one * sin;

        crosshairPolygon.transform.localScale = polygonSize;

        crosshairPolygon.transform.localEulerAngles = Vector3.Lerp(startPolygonAngles, targetPolygonAngles, Progress);

        //Handle Sides

        // Up side
        Vector3 upPos = upOffset;
        upPos.y += sidesOffset.Evaluate(bloom) * sideOffsetMultiplier * sin;

        upSide.transform.position = upPos;

        //Down Side

        Vector3 downPos = downOffset;
        downPos.y -= sidesOffset.Evaluate(bloom) * sideOffsetMultiplier * sin;

        downSide.transform.position = downPos;

        //Left side

        Vector3 leftPos = leftOffset;
        leftPos.x -= sidesOffset.Evaluate(bloom) * sideOffsetMultiplier * sin;

        leftSide.transform.position = leftPos;

        //Right side

        Vector3 rightPos = rightOffset;
        rightPos.x += sidesOffset.Evaluate(bloom) * sideOffsetMultiplier * sin;

        rightSide.transform.position = rightPos;

    }

    void SetSides(Transform target, Vector3 offset, float bloom ,float sin)
    {
        Vector3 pos = offset;
        if (pos.x != 0) pos.x += sidesOffset.Evaluate(bloom) * sin;
        if (pos.y != 0) pos.y += sidesOffset.Evaluate(bloom) * sin;

        target.transform.position = pos;
    }

    public void SetBloomAndPower(float bloom, float power)
    {
        this.bloom = bloom;
        this.power = power;
    }

    public void Shoot()
    {
        active = true;
        currentAnimationTime = 0;
        
        startPolygonAngles = Vector3.zero;

        targetPolygonAngles = startPolygonAngles;
        targetPolygonAngles.z += polygonRotationAmount;


    }
}
