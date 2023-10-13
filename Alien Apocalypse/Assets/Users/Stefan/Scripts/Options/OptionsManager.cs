using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that manages options
/// </summary>
public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    public delegate void OptionsEvent ( OptionsData options );

    /// <summary>
    /// Event called whenever the user changed the options, containting all the data
    /// </summary>
    public static OptionsEvent onOptionsChanged;

    public static OptionsData Options
    {
        get
        {
            return OptionsData.Options;
        }
    }

    private List<IOption<bool>> boolOptions = new();

    private List<IOption<float>> floatOptions = new ( );

    private List<IOption<int>> intOptions = new ( );

    bool needsUpdate;

    private void Start ( )
    {
        instance = this;

        FetchOptions ( );
        SetElementsOptionData ( );
    }

    private void Update ( )
    {
        if ( needsUpdate )
        {
            needsUpdate = false;
            onOptionsChanged?.Invoke(OptionsData.Options );
            Debug.Log ("If you see this, the optionschanged delegate has been called!");
        }
    }

    void FetchOptions ( )
    {
        boolOptions = GetOptionInstances<bool> ( ).ToList();
        floatOptions = GetOptionInstances<float> ( ).ToList ( );
        intOptions = GetOptionInstances<int> ( ).ToList ( );
    }

    public void OnEnable ( )
    {
        SetElementsOptionData ( );
    }

    public void OnDisable ( )
    {
        SaveOptions ( );
    }

    public void SaveOptions ( )
    {
        SaveElementsOptionData ( );
    }


    void SetElementsOptionData ( )
    {
        SetValuesOfElements (boolOptions);
        SetValuesOfElements (floatOptions);
        SetValuesOfElements (intOptions);
    }

    void SaveElementsOptionData ( )
    {
        SaveValuesOfElements (boolOptions);
        SaveValuesOfElements (intOptions);
        SaveValuesOfElements (floatOptions);
    }

    private void OnOptionChanged ( )
    {
        needsUpdate = true;
    }

    void SetValuesOfElements<T> ( IList<IOption<T>> list )
    {
        foreach ( var element in list )
        {
            try
            {
                element.SetValue ((T)OptionsData.Options[element.OptionIndex]);
            }
            catch(Exception e )
            {
                Debug.Log ($"Failed to set value at index {element.OptionIndex}, data: {OptionsData.Options[element.OptionIndex]}");
            }
        }
    }

    void SaveValuesOfElements<T>(IList<IOption<T>> list )
    {
        foreach ( var element in list )
        {
            OptionsData.Options[element.OptionIndex] = element.GetValue ( );
        }
    }

    // Get all instances of IOption<T> attached to GameObjects with UISelectable component
    private static IEnumerable<IOption<T>> GetOptionInstances<T> ( )
    {
        var optionType = typeof (IOption<T>);

        // Find all GameObjects with UISelectable component
        var selectableObjects = GameObject.FindObjectsOfType<UISelectable> (true );

        var optionInstances = new List<IOption<T>> ( );

        foreach ( var selectableObject in selectableObjects )
        {
            // Get all components attached to the GameObject that implement IOption<T>
            var optionComponents = selectableObject.GetComponents<Component> ( )
                .Where (component => optionType.IsInstanceOfType (component))
                .Select (component => (IOption<T>) component);

            optionInstances.AddRange (optionComponents);
        }

        return optionInstances;
    }


    /// <summary>
    /// Container that holds all the data for settings that can be changed. Call the Save() function on an instance of this class to save it.
    /// The static OptionsData.Options property reads the last saved instance of OptionsData and returns it
    /// </summary>
    public class OptionsData
    {
        #region Saving & Loading

        const string saveFileName = "Options.json";

        private static OptionsData m_Options;

        public static OptionsData Options
        {
            get
            {
                if ( m_Options == null )
                {
                    string path = Path.Combine (Application.persistentDataPath, saveFileName);

                    if ( File.Exists (path) )
                    {
                        string json = File.ReadAllText (path);
                        m_Options = JsonUtility.FromJson<OptionsData> (json);
                    }
                    else
                    {
                        m_Options = new ( );
                    }
                }
                return m_Options;
            }
        }


        public void Save ( )
        {
            string path = Path.Combine (Application.persistentDataPath, saveFileName);

            string json = JsonUtility.ToJson (this);

            File.WriteAllText (path, json);

            Debug.Log ($"Options saved to: {path}");

            instance.OnOptionChanged ( );
        }

        #endregion


        // ===== Controls =====

        /// <summary>
        /// If crosshair efefcts should be enabled
        /// </summary>

        [SerializeField]
        private bool crosshairEffects;

        /// <summary>
        /// The fov of the player
        /// </summary>

        [SerializeField]
        private float fov;

        /// <summary>
        /// The horizontal mouse speed multiplier
        /// </summary>

        [SerializeField]
        private float horizontalSens;

        /// <summary>
        /// The vertical mouse speed multiplier
        /// </summary>

        [SerializeField]
        private float verticalSens;




        // ==== Video Graphics =====

        /// <summary>
        /// The index of the screen resolution selector;
        /// </summary>

        [SerializeField]
        private int screenResIndex;

        /// <summary>
        /// The index of the fps selector
        /// </summary>

        [SerializeField]
        private int fpsIndex;

        /// <summary>
        /// The index of the quality selector
        /// </summary>

        [SerializeField]
        private int qualityIndex;

        /// <summary>
        /// should v-sync be enabled?
        /// </summary>

        [SerializeField]
        private bool vSync;

        /// <summary>
        /// Is the game windowed or in full-screen mode?
        /// </summary>

        [SerializeField]
        private bool fullscreen;



        // ===== Audio =====

        /// <summary>
        /// The Main strength of all audios
        /// </summary>

        [SerializeField]
        private float mainAudioStrength;

        /// <summary>
        /// The strength of all in-game sound effects
        /// </summary>

        [SerializeField]
        private float soundsStrength;

        /// <summary>
        /// The strength of all music
        /// </summary>

        [SerializeField]
        private float musicStrength;

        /// <summary>
        /// The strength of the in-game ambience sounds
        /// </summary>

        [SerializeField]
        private float ambienceStrength;

        public object this[int index]
        {
            get
            {
                try{
                return index switch
                {
                    0 => CrosshairEffects,
                    1 => Fov,
                    2 => HorizontalSens,
                    3 => VerticalSens,
                    4 => ScreenResIndex,
                    5 => FpsIndex,
                    6 => QualityIndex,
                    7 => Fullscreen,
                    8 => VSync,
                    9 => MainAudioStrength,
                    10 => SoundsStrength,
                    11 => MusicStrength,
                    12 => AmbienceStrength,
                    _ => throw new IndexOutOfRangeException ( ),
                }; 
                }
                catch(Exception e )
                {
                    Debug.Log ($"failed to get {index}");
                    throw;
                }
            }

            set
            {
                try{
                switch(index)
                {
                case 0:
                        CrosshairEffects = (bool)value;
                     break;
                case 1:
                        Fov = (float) value;
                    break;
                case 2:
                        HorizontalSens = (float)value;
                    break;
                case 3:
                        VerticalSens = (float)value;
                    break;
                case 4:
                        ScreenResIndex = (int) value;
                    break;
                case 5:
                        FpsIndex = (int) value;
                    break;
                case 6:
                        QualityIndex = (int)value;
                    break;
                case 7:
                        Fullscreen = (bool) value;
                    break;
                case 8:
                        VSync = (bool)value;
                    break;
                case 9:
                        MainAudioStrength = (float)value;
                    break;
                case 10:
                        SoundsStrength = (float)value;
                    break;
                case 11:
                        MusicStrength = (float) value;
                    break;
                case 12:
                        AmbienceStrength = (float) value;
                    break;


                default:
                    throw new IndexOutOfRangeException ( );

                } 
                }
                catch(Exception e )
                {
                    Debug.Log ($"Failed to set {index} : {value}");
                    throw;
                }
            }
        }

        public int Length
        {
            get
            {
                return 13;
            }
        }

        public bool CrosshairEffects
        {
            get => crosshairEffects;
            set
            {
                crosshairEffects = value;
                Save ( );
            }
        }
        public float Fov
        {
            get => fov;
            set
            {
                fov = value;
                Save ( );
            }
        }
        public float HorizontalSens
        {
            get => horizontalSens;
            set
            {
                horizontalSens = value;
                Save ( );
            }
        }
        public float VerticalSens
        {
            get => verticalSens;
            set
            {
                verticalSens = value;
                Save ( );
            }
        }
        public int ScreenResIndex
        {
            get => screenResIndex;
            set
            {
                screenResIndex = value;
                Save ( );
            }
        }
        public int FpsIndex
        {
            get => fpsIndex;
            set
            {
                fpsIndex = value;
                Save ( );
            }
        }
        public int QualityIndex
        {
            get => qualityIndex;
            set
            {
                qualityIndex = value;
                Save ( );
            }
        }
        public bool VSync
        {
            get => vSync;
            set
            {
                vSync = value;
                Save ( );
            }
        }
        public bool Fullscreen
        {
            get => fullscreen;
            set
            {
                fullscreen = value;
                Save ( );
            }
        }
        public float MainAudioStrength
        {
            get => mainAudioStrength;
            set
            {
                mainAudioStrength = value;
                Save ( );
            }
        }
        public float SoundsStrength
        {
            get => soundsStrength;
            set
            {
                soundsStrength = value;
                Save ( );
            }
        }
        public float MusicStrength
        {
            get => musicStrength;
            set
            {
                musicStrength = value;
                Save ( );
            }
        }
        public float AmbienceStrength
        {
            get => ambienceStrength;
            set
            {
                ambienceStrength = value;
                Save ( );
            }
        }

        /// <summary>
        /// Creates the basic options 
        /// </summary>
        private OptionsData ( )
        {
            crosshairEffects = true;
            fov = 80;
            horizontalSens = 50;
            verticalSens = 50;

            screenResIndex = 1;
            fpsIndex = 6;
            vSync = false;
            qualityIndex = 3;
            fullscreen = true;

            mainAudioStrength = 1;
            soundsStrength = 1;
            musicStrength = 1;
            ambienceStrength = 1;

            Save ( );
        }

    }

}

