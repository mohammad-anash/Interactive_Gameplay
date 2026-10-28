using UnityEngine;

[CreateAssetMenu(fileName = "MansionSafeCode", menuName = "Mansion/Safe Code Data")]
public class SafeCodeData : ScriptableObject
{
    [SerializeField] private string _code = "4816";

    public string Code => _code;

    // First half of the code, for example "48" from "4816"
    public string FirstHalf
    {
        get
        {
            string code = Code ?? string.Empty;
            return code.Substring(0, code.Length / 2);
        }
    }

    // Second half of the code, for example "16" from "4816"
    public string SecondHalf
    {
        get
        {
            string code = Code ?? string.Empty;
            return code.Substring(code.Length / 2);
        }
    }

    private void OnValidate()
    {
        if (_code.Length != 4 ||
            !System.Text.RegularExpressions.Regex.IsMatch(_code, @"^\d{4}$"))
        {
            Debug.LogWarning("Safe code must contain exactly 4 digits.", this);
        }
    }
}
