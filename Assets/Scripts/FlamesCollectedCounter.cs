using UnityEngine;
using TMPro;

public class FlamesCollectedCounter : MonoBehaviour
{
    private void OnEnable()
    {
        Player.OnFlameCountUpdated += HandleFlameCollected;
    }

    private void OnDisable()
    {
        Player.OnFlameCountUpdated -= HandleFlameCollected;
    }

    private void HandleFlameCollected()
    {
        GetComponent<TextMeshProUGUI>().text = Player.Instance.flamesCollected.ToString();
    }
}
