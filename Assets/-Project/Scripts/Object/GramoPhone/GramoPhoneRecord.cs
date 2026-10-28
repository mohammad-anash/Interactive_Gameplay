using UnityEngine;

[System.Serializable]
public class GramophoneRecord
{
    [Header("Record Settings")]

    [SerializeField] private string _recordName = "Unnamed Record";
    [SerializeField, Min(0f)] private float _spinSpeed = 30f;
    [SerializeField] private Color _lightColor = Color.blue;
    [SerializeField] private AudioClip _audioClip;


    // ---- Public Properties -----------------------
    public string RecordName => _recordName;
    public float SpinSpeed => _spinSpeed;
    public Color LightColor => _lightColor;
    public AudioClip AudioClip => _audioClip;
}
