using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public struct NamedClip
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField]
    private List<NamedClip> sounds = new List<NamedClip>();

    private AudioSource _audioRef;
    private Dictionary<string, AudioClip> _soundDict;

    // Wrapper para usar desde Button.onClick (1 parámetro)
    public void PlaySound(string soundName)
    {
        Play(soundName, true);
    }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Asegurar AudioSource
        _audioRef = GetComponent<AudioSource>();
        if (_audioRef == null)
        {
            _audioRef = gameObject.AddComponent<AudioSource>();
            _audioRef.playOnAwake = false;
        }

        _audioRef.volume = .3f;

        ConstruirDiccionario();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Reconstruye el diccionario en editor para evitar errores en runtime
        if (Application.isPlaying) return;
        ConstruirDiccionario();
    }
#endif

    private void ConstruirDiccionario()
    {
        _soundDict = new Dictionary<string, AudioClip>();
        foreach (var sound in sounds)
        {
            if (!string.IsNullOrEmpty(sound.name) && sound.clip != null)
                _soundDict[sound.name] = sound.clip;
        }
    }

    public void Play(string soundName, bool interrupt = true)
    {
        if (_soundDict == null)
        {
            Debug.LogWarning("AudioManager: diccionario no inicializado.");
            return;
        }

        if (_soundDict.TryGetValue(soundName, out var clip))
        {
            if (interrupt)
            {
                if (_audioRef.isPlaying) _audioRef.Stop();
                _audioRef.clip = clip;
                _audioRef.Play();
            }
            else
            {
                _audioRef.PlayOneShot(clip);
            }
        }
        else
        {
            Debug.LogWarning($"AudioManager: sonido '{soundName}' no encontrado.");
        }
    }

    public bool IsPlaying(string soundName)
    {
        return _audioRef != null && _audioRef.isPlaying && _audioRef.clip != null && _audioRef.clip == GetClip(soundName);
    }

    public AudioClip GetClip(string soundName)
    {
        if (_soundDict != null && _soundDict.TryGetValue(soundName, out var clip))
            return clip;
        return null;
    }

    // Permite forzar creación si otra clase lo necesita antes de que exista.
    public static void EnsureInstance()
    {
        if (Instance != null) return;
        var go = new GameObject("AudioManager");
        go.AddComponent<AudioManager>();
    }
}