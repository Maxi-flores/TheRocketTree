using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheRocketTree.Intelligence;

namespace TheRocketTree.UI
{
    /// <summary>
    /// Displays a reflective Insight to the user.
    /// This panel is passive: it renders InsightDTO only.
    /// </summary>
    public sealed class InsightPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text dateRangeText;

        /// <summary>
        /// Render the given InsightDTO.
        /// </summary>
        public void SetInsight(InsightDTO insight)
        {
            if (insight == null)
            {
                Clear();
                return;
            }

            if (titleText != null)
                titleText.text = string.IsNullOrEmpty(insight.title)
                    ? "Reflection"
                    : insight.title;

            if (bodyText != null)
                bodyText.text = insight.text ?? string.Empty;

            if (dateRangeText != null)
                dateRangeText.text = FormatRange(insight.fromUtc, insight.toUtc);
        }

        /// <summary>
        /// Clears the panel UI.
        /// </summary>
        public void Clear()
        {
            if (titleText != null) titleText.text = string.Empty;
            if (bodyText != null) bodyText.text = string.Empty;
            if (dateRangeText != null) dateRangeText.text = string.Empty;
        }

        private string FormatRange(string fromUtc, string toUtc)
        {
            if (string.IsNullOrEmpty(fromUtc) || string.IsNullOrEmpty(toUtc))
                return string.Empty;

            return $"{fromUtc.Substring(0, 10)} → {toUtc.Substring(0, 10)}";
        }
    }
}
