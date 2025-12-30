using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheRocketTree.Intelligence;

namespace TheRocketTree.UI
{
    /// <summary>
    /// UI panel for submitting user reflections.
    /// Reflections are signals of awareness, not performance.
    /// </summary>
    public sealed class ReflectionPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField reflectionInput;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text statusText;

        private ReflectionService _reflectionService;

        /// <summary>
        /// Inject required services.
        /// </summary>
        public void Initialize(ReflectionService reflectionService)
        {
            _reflectionService = reflectionService;
        }

        private void Awake()
        {
            if (submitButton != null)
                submitButton.onClick.AddListener(OnSubmitClicked);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(Clear);
        }

        private async void OnSubmitClicked()
        {
            if (_reflectionService == null)
            {
                Debug.LogWarning("[ReflectionPanel] ReflectionService not set.");
                return;
            }

            var text = reflectionInput != null ? reflectionInput.text : null;

            if (string.IsNullOrWhiteSpace(text))
            {
                SetStatus("Nothing to submit.");
                return;
            }

            SetStatus("Saving…");

            var success = await _reflectionService.SubmitReflectionAsync(
                text: text,
                relatedTaskIds: null,
                tags: null
            );

            if (success)
            {
                SetStatus("Saved.");
                ClearInput();
            }
            else
            {
                SetStatus("Could not save reflection.");
            }
        }

        private void Clear()
        {
            ClearInput();
            SetStatus(string.Empty);
        }

        private void ClearInput()
        {
            if (reflectionInput != null)
                reflectionInput.text = string.Empty;
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
    }
}
