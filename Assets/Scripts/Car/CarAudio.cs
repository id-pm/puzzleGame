using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [SerializeField] private GameObject audioSourcePrefab;
    private AudioSource audioSourceEngine;
    private AudioSource audioSourceEffects;
    private GameObject audioSourceObject;
    private CarSettings carSettings;

    private void Start()
    {
        audioSourceObject = Instantiate(audioSourcePrefab, transform);
        audioSourceEngine = audioSourceObject.transform.GetChild(0).GetComponent<AudioSource>();
        audioSourceEffects = audioSourceObject.transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void Initialize(CarSettings carSettings)
    {
        this.carSettings = carSettings;
    }
    public void StartEngine()
    {
        audioSourceObject.transform.position = Camera.main.transform.position;
        audioSourceEngine.Play();
    }
    public void Drift() => audioSourceEffects.Play();
    public void Crash() {
        audioSourceEngine.Stop();
        audioSourceEffects.PlayOneShot(carSettings.CrashClip);
    }
    public void StopEngine() => audioSourceEngine.Stop();
}
