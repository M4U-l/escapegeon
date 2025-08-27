using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showTime = 2f;

    private Coroutine currentRoutine;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void ShowNotification(string itemName, Sprite icon = null)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        panel.SetActive(true);
        messageText.text = $"{itemName} added to inventory";

        currentRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showTime);
        panel.SetActive(false);
    }
}
