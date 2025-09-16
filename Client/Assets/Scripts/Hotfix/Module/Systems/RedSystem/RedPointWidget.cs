// RedPointWidget.cs

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CanvasGroup))]
public class RedPointWidget : MonoBehaviour
{
    
    [RedPointNodeSelector("current node")]
    [SerializeField] private string nodePath;
    [RedPointNodeSelector("parent node")]
    [SerializeField] private string parentPath;
    [SerializeField] private GameObject numberRedPoint;
    [SerializeField] private GameObject exclamationRedPoint;
    [SerializeField] private GameObject onlyRedPoint;
    [SerializeField] RedPointType displayType;
    
    private TextMeshProUGUI numberText;
    private RedPointNode targetNode;
    private CanvasGroup canvasGroup;
    private bool isVisible;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        numberText = numberRedPoint.transform.Find("count").GetComponent<TextMeshProUGUI>();
        targetNode = RedPointSystem.RegisterNode(nodePath, parentPath);
        targetNode.OnValueChanged += HandleStateChange;
    }

    private void Start()
    {
        numberRedPoint.SetActive(false);
        exclamationRedPoint.SetActive(false);
        onlyRedPoint.SetActive(false);
        UpdateVisibility();
    }

    private void Update()
    {
        bool newVisibility = canvasGroup.alpha > 0.01f && gameObject.activeInHierarchy;
        if (newVisibility != isVisible)
        {
            isVisible = newVisibility;
            UpdateVisibility();
        }
    }

    private void HandleStateChange(RedPointData data)
    {
        if (!isVisible) 
            return;

        switch (displayType)
        {
            case RedPointType.Number:
                numberRedPoint.SetActive(data.totalValue > 0);
                if (numberRedPoint.gameObject.activeSelf)
                    numberText.text = data.totalValue > 99 ? "99+" : data.totalValue.ToString();
                break;
            case RedPointType.Exclamation:
                //exclamationRedPoint.SetActive(data.isActive);
                onlyRedPoint.SetActive(data.totalValue > 0);
                break;
            case RedPointType.OnlyRed:
                onlyRedPoint.SetActive(data.totalValue > 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //numberRedPoint.gameObject.SetActive(data is { displayType: RedPointType.Number, totalValue: > 0 });
        //exclamationRedPoint.SetActive(data is { displayType: RedPointType.Exclamation, isActive: true });

      
    }

    private void UpdateVisibility()
    {
        if (isVisible)
        {
            HandleStateChange(targetNode.GetState());
        }
    }

    private void OnDestroy()
    {
        if (targetNode != null)
        {
            targetNode.OnValueChanged -= HandleStateChange;
            RedPointSystem.UnregisterNode(nodePath);
        }
    }
}