using TMPro;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gemCountLabel;
    [SerializeField] private Vector3 _offset;

    private IAgent mAgent;
    private Camera mCamera;

    public void Initialize(IAgent agent, Camera camera)
    {
        mAgent = agent;
        mCamera = camera;
    }

    public void UpdateGemCount(int count)
    {
        _gemCountLabel.text = count.ToString();
    }

    private void Update()
    {
        if (mAgent == null || mCamera == null)
        {
            return;
        }

        var screenPosition = mCamera.WorldToScreenPoint(mAgent.Position + _offset);
        if (TryGetComponent<RectTransform>(out var rectTransform))
        {
            rectTransform.position = screenPosition;
        }
    }
}
