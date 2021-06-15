using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [SerializeField] Text _retryText;
    

    private void Update()
    {
        UpdateRetries(GameManager.Instance.RewindsLeft);
    }

    public void UpdateRetries(int retries)
    {
        if(_retryText != null)
        {
            _retryText.text = "Retries: "+retries;
        }
    }
}
