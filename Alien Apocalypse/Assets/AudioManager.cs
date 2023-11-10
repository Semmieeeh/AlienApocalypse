using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer uiMixer, masterMixer, SFXMixer, musicMixer;

    // Start is called before the first frame update
    void Start ( )
    {
        OptionsManager.onOptionsChanged += UpdateAudioOptions;
    }

    void UpdateAudioOptions ( OptionsManager.OptionsData data )
    {
        uiMixer.SetFloat ("Volume", ToVolumeStrength(data.UIStrength));
        masterMixer.SetFloat ("Volume", ToVolumeStrength(data.MainAudioStrength));
        SFXMixer.SetFloat ("Volume", ToVolumeStrength(data.SoundsStrength));
        musicMixer.SetFloat ("Volume", ToVolumeStrength(data.MusicStrength));
    }

    public float ToVolumeStrength ( float inputVolume )
    {
        float mappedVolume = Mathf.Log10 (inputVolume) * 20;
        Debug.Log($"INPUT IS {inputVolume}, OUTPUT IS {mappedVolume}!");

        return mappedVolume;
    }
}
