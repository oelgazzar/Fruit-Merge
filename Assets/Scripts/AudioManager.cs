using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip _dropSfx;
    [SerializeField] AudioClip _mergeSfx;

    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void FruitController_OnFruitDropped(Fruit _)
    {
        _audioSource.PlayOneShot(_dropSfx);
    }

    private void OnFruitMerge(Fruit _, Fruit __, Fruit ___)
    {
        _audioSource.PlayOneShot(_mergeSfx);
    }

    private void OnEnable()
    {
        FruitController.OnFruitDrop += FruitController_OnFruitDropped;
        MergeManager.OnFruitMerge += OnFruitMerge;
    }


    private void OnDisable()
    {
        FruitController.OnFruitDrop -= FruitController_OnFruitDropped;
        MergeManager.OnFruitMerge -= OnFruitMerge;
    }    
}
