using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class Gramophone : MonoBehaviour, IInteractable
{
  [Header("References")]
  [SerializeField] private Transform _discPivot;
  [SerializeField] private Light _gramophoneLight;
  [SerializeField] private AudioSource _audioSource;

  [Header("Records")]
  [SerializeField] private List<GramophoneRecord> _records = new();

  private const int Off = -1;
  private int _currentRecordIndex = Off;

  private bool HasValidSetup =>
      _discPivot != null &&
      _gramophoneLight != null &&
      _records != null &&
      _records.Count > 0;

  public string InteractText
  {
    get
    {
      if (!HasValidSetup)
        return string.Empty;

      int nextIndex = GetNextIndex();

      if (nextIndex == Off)
        return "Stop the music";

      return $"Play {_records[nextIndex].RecordName}";
    }
  }

  private void Awake()
  {
    if (!HasValidSetup)
    {
      Debug.LogError(
          "Gramophone setup is invalid. Assign the disc pivot, light, and at least one record.",
          this
      );

      return;
    }

    _gramophoneLight.enabled = false;
  }

  private int GetNextIndex()
  {
    int nextIndex = _currentRecordIndex + 1;

    return nextIndex >= _records.Count ? Off : nextIndex;
  }

  public void Interact()
  {
    if (!HasValidSetup)
      return;

    int nextIndex = GetNextIndex();

    if (nextIndex == Off)
    {
      StopGramophone();
      return;
    }

    _currentRecordIndex = nextIndex;
    PlayCurrentRecord();
  }

  private void Update()
  {
    if (!HasValidSetup || _currentRecordIndex == Off)
      return;

    float spinSpeed = _records[_currentRecordIndex].SpinSpeed;
    _discPivot.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
  }

  private void PlayCurrentRecord()
  {
    GramophoneRecord record = _records[_currentRecordIndex];

    _gramophoneLight.enabled = true;
    _gramophoneLight.color = record.LightColor;

    if (record.AudioClip != null)
    {
      _audioSource.clip = record.AudioClip;
      _audioSource.Play();
    }
  }
  private void StopGramophone()
  {
    _currentRecordIndex = Off;
    _gramophoneLight.enabled = false;

    _audioSource.Stop();
    _audioSource.clip = null;
  }
}
