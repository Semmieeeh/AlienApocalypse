using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    private static MiniMap _instance;

    public static MiniMap Instance
    {
        get
        {
            if(_instance == null )
            {
                _instance = FindObjectOfType<MiniMap> ( );
            }
            return _instance;
        }
    }
    [SerializeField]
    MapTrackable followTarget;

    Dictionary<MapTrackable,MiniMapElement> activeTrackables = new ( );

    [SerializeField]
    Vector2 minMap,maxMap;

    [SerializeField]
    Vector2 minMapResult,maxMapResult;

    [SerializeField]
    Image mapImage;

    [SerializeField]
    GameObject elementPrefab;

    [SerializeField]
    Transform parent;

private void Update ( )
    {
        if ( followTarget != null)
        {

        }

        MapTrackablesToMap ( );
    }

    void MapTrackablesToMap ( )
    {
        foreach ( var item in activeTrackables )
        {
            Vector2 mappedPosition = MapPosition (item.Key);

            Vector3 angles = Vector3.zero;

            if ( item.Key.UseRotation )
            {
                angles.z = item.Key.EulerAngles.y;
            }

            item.Value.SetPositionAndRotation (mappedPosition, angles);
        }
    }

    public Vector2 MapPosition (MapTrackable trackable)
    {
        float xl = Mathf.InverseLerp (minMap.x, maxMap.x, trackable.Position.x);
        float yl = Mathf.InverseLerp (minMap.y, maxMap.y, trackable.Position.z);

        float x = Mathf.Lerp (minMapResult.x, maxMapResult.x, xl);
        float y = Mathf.Lerp (minMapResult.y, maxMapResult.y, yl);

        return new Vector2 (x, y);
    }

    public void AddTrackable(MapTrackable trackable )
    {
        if ( activeTrackables.ContainsKey (trackable) )
            return;

        var element = Instantiate(elementPrefab,parent).GetComponent<MiniMapElement>();

        element.Initialize (trackable);


        activeTrackables.Add (trackable,element);
    }


    public void RemoveTrackable ( MapTrackable trackable )
    {
        activeTrackables[trackable].Remove ( );

        activeTrackables.Remove (trackable);
    }
}
