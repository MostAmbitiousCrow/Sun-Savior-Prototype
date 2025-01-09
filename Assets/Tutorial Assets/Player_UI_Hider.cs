using TMPro;
using UnityEngine;

public class Player_UI_Hider : MonoBehaviour
{
    [SerializeField] RectTransform UI_Pivot;
    [SerializeField] TextMeshProUGUI hideButtonText;
    [SerializeField] bool UI_Hidden;

    [SerializeField] Vector3 pivotStartPos;
    [SerializeField] Vector3 pivotEndPos;

    void Start()
    {
        pivotStartPos = UI_Pivot.anchoredPosition;
    }

    // Update is called once per frame
    public void HideUI()
    {
        if (!UI_Hidden) 
        {
            hideButtonText.text = "Show";
            UI_Pivot.anchoredPosition = pivotEndPos;
            UI_Hidden = true;
        }
        else
        {
            hideButtonText.text = "Hide";
            UI_Pivot.anchoredPosition = pivotStartPos;
            UI_Hidden = false;
        }
    }
}
