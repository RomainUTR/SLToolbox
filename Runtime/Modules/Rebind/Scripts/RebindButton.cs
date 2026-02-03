using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    [Header("Input Configuration")]
    [SerializeField] private InputActionReference actionReference;

    [Tooltip("Index of the binding in the action (0 for single, 1+ for composites like WASD)")]
    [SerializeField] private int bindingIndex;

    [Header("Configuration UI")]
    [SerializeField] private TMP_Text actionLabel;
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private GameObject listeningOverlay;

    private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

    private void Start()
    {
        if (actionReference != null)
            RebindStorage.Load(actionReference.action.actionMap.asset);

        UpdateUI();
    }

    public void StartRebinding()
    {
        if (actionReference == null) return;

        actionReference.action.Disable();
        if (listeningOverlay) listeningOverlay.SetActive(true);
        if (bindingText) bindingText.text = "...";

        if (bindingIndex >= actionReference.action.bindings.Count) bindingIndex = 0;

        _rebindingOperation = actionReference.action.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op => FinishRebinding())
            .OnCancel(op => CancelRebinding());

        if (actionReference.action.bindings[bindingIndex].groups.Contains("Keyboard"))
            _rebindingOperation.WithControlsExcluding("Mouse");

        _rebindingOperation.Start();
    }

    void FinishRebinding()
    {
        CleanUp();
        actionReference.action.Enable();
        RebindStorage.Save(actionReference.action.actionMap.asset);
        UpdateUI();
    }

    void CancelRebinding()
    {
        CleanUp();
        actionReference.action.Enable();
        UpdateUI();
    }

    private void CleanUp()
    {
        if (listeningOverlay) listeningOverlay.SetActive(false);
        _rebindingOperation?.Dispose();
        _rebindingOperation = null;
    }

    void UpdateUI()
    {
        if (actionReference == null) return;

        if (actionLabel)
        {
            string actionName = actionReference.action.name;
            var bindings = actionReference.action.bindings;

            if (bindingIndex < bindings.Count && bindings[bindingIndex].isPartOfComposite)
                actionName += $" ({bindings[bindingIndex].name})";

            actionLabel.text = actionName;
        }

        if (bindingText)
        {
            string displayString = actionReference.action.GetBindingDisplayString(bindingIndex,
                InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
            bindingText.text = displayString.ToUpper();
        }
    }
}