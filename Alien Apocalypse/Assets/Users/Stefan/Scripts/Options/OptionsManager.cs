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
    public delegate void OptionsEvent ( OptionsData options );

    private static OptionsManager m_instance;

    public static OptionsManager Instance
    {
        get
        {
            if ( m_instance == null )
                m_instance = FindObjectOfType<OptionsManager> (true);
            return m_instance;
        }
        set
        {
            m_instance = value;
        }
    }

    /// <summary>
    /// Event called whenever the user changed the options, containting all the data
    /// </summary>
    public static OptionsEvent OnOptionsChanged{ get; set; }

    public static OptionsData Options
    {
        get
        {
            return OptionsData.Options;
        }
    }

    private List<IOption<bool>> boolOptions = new ( );

    private List<IOption<float>> floatOptions = new ( );

    private List<IOption<int>> intOptions = new ( );

    private void Start ( )
    {
        Instance = this;

        FetchOptions ( );
        SetElementsOptionData ( );
    }

    public void OnEnable ( )
    {
        SetElementsOptionData ( );
    }

    public void OnDisable ( )
    {
        SaveElementsOptionData ( );
    }

    private void OnOptionChanged ( )
    {
        OnOptionsChanged?.Invoke (OptionsData.Options);
        Debug.Log ("If you see this, the optionschanged delegate has been called!");
        Debug.Log (OptionsData.Options.ToString ( ));
    }

    #region Saving Methods

    private void SaveElementsOptionData ( )
    {
        SaveValuesOfElements (boolOptions);
        SaveValuesOfElements (floatOptions);
        SaveValuesOfElements (intOptions);
    }

    private void SaveValuesOfElements<T> ( IList<IOption<T>> list )
    {
        foreach ( var element in list )
        {
            var index = element.OptionIndex;
            var value = element.GetValue ( );

            OptionsData.Options[index] = value;
            Debug.Log ($"Index ({index}) has a value: {value}");
        }

        OptionsData.Options.Save ( );
    }

    #endregion Saving Methods

    #region Setting Methods

    private void SetElementsOptionData ( )
    {
        SetValuesOfElements (boolOptions);
        SetValuesOfElements (floatOptions);
        SetValuesOfElements (intOptions);
    }

    private void SetValuesOfElements<T> ( IList<IOption<T>> list )
    {
        foreach ( var element in list )
        {
            try
            {
                element.SetValue ((T) OptionsData.Options[element.OptionIndex]);
                Debug.Log ($"Set value of element index ({element.OptionIndex}) to {OptionsData.Options[element.OptionIndex]}");
            }
            catch ( Exception )
            {
                Debug.Log ($"Failed to set value at index {element.OptionIndex}, data: {OptionsData.Options[element.OptionIndex]}");
            }
        }
    }

    private void FetchOptions ( )
    {
        boolOptions = GetOptionInstances<bool> ( ).ToList ( );
        floatOptions = GetOptionInstances<float> ( ).ToList ( );
        intOptions = GetOptionInstances<int> ( ).ToList ( );
    }

    #endregion Setting Methods

    // Get all instances of IOption<T> attached to GameObjects with UISelectable component
    private static IEnumerable<IOption<T>> GetOptionInstances<T> ( )
    {
        var optionType = typeof (IOption<T>);

        // Find all GameObjects with UISelectable component

        var canvas = GameObject.FindObjectOfType<Canvas> ( );

        Debug.Log (canvas.name);
        var selectableObjects = canvas.transform.GetComponentsInHierarchy<UISelectable> ( );

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
    /// Container that holds all the data for settings that can be changed. Call the Save() function on an Instance of this class to save it.
    /// The static OptionsData.Options property reads the last saved Instance of OptionsData and returns it
    /// </summary>
    public class OptionsData
    {
        #region Saving & Loading

        private const string saveFileName = "Options.json";

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

            m_Options = this;

            Instance.OnOptionChanged ( );
        }

        #endregion Saving & Loading

        // ===== Controls =====

        /// <summary>
        /// If crosshair efefcts should be enabled
        /// </summary>

        [SerializeField]
        private bool crosshairEffects;

        /// <summary>
        /// The selected crosshair index
        /// </summary>
        [SerializeField]
        private int crosshairIndex;

        /// <summary>
        /// The size of the crosshair
        /// </summary>
        [SerializeField]
        private float crosshairSize;

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
        /// The index of the screen resolution resolutionSelector;
        /// </summary>

        [SerializeField]
        private int screenResIndex;

        /// <summary>
        /// The index of the fps resolutionSelector
        /// </summary>

        [SerializeField]
        private int fpsIndex;

        /// <summary>
        /// The index of the quality resolutionSelector
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
                try
                {
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
                        12 => UIStrength,
                        13 => crosshairIndex,
                        14 => crosshairSize,
                        _ => throw new IndexOutOfRangeException ( ),
                    };
                }
                catch ( Exception )
                {
                    Debug.Log ($"failed to get {index}");
                    throw;
                }
            }

            set
            {
                try
                {
                    switch ( index )
                    {
                        case 0:
                            crosshairEffects = (bool) value;
                            break;

                        case 1:
                            fov = (float) value;
                            break;

                        case 2:
                            horizontalSens = (float) value;
                            break;

                        case 3:
                            verticalSens = (float) value;
                            break;

                        case 4:
                            screenResIndex = (int) value;
                            break;

                        case 5:
                            fpsIndex = (int) value;
                            break;

                        case 6:
                            qualityIndex = (int) value;
                            break;

                        case 7:
                            fullscreen = (bool) value;
                            break;

                        case 8:
                            vSync = (bool) value;
                            break;

                        case 9:
                            mainAudioStrength = (float) value;
                            break;

                        case 10:
                            soundsStrength = (float) value;
                            break;

                        case 11:
                            musicStrength = (float) value;
                            break;

                        case 12:
                            ambienceStrength = (float) value;

                            break;

                        case 13:
                            crosshairIndex = (int) value;
                            break;

                        case 14:
                            crosshairSize = (float) value;
                            break;

                        default:
                            throw new IndexOutOfRangeException ( );
                    }
                }
                catch ( Exception )
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
                return 15;
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

        public int CrosshairIndex
        {
            get => crosshairIndex;
            set
            {
                crosshairIndex = value;
                Save ( );
            }
        }

        public float CrosshairSize
        {
            get => crosshairSize;
            set
            {
                crosshairSize = value;
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

        public float UIStrength
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

            Debug.LogWarning ("Options save file overridden by new default settings!");

            Save ( );
        }
    }
}