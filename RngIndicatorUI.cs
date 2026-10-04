using System.Collections;
using Modding;
using UnityEngine;
using UnityEngine.UI;

namespace AbsRadConfigurableAttacks {
    public class RngIndicatorUI : MonoBehaviour {
        private const float DisplaySeconds = 2.5f;

        private CanvasGroup canvasGroup;
        private Text textComponent;
        private Coroutine activeRoutine;

        private void Awake() {
            DontDestroyOnLoad(gameObject);

            GameObject canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
            canvas.transform.SetParent(transform);

            canvasGroup = canvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            GameObject textPanel = CanvasUtil.CreateTextPanel(
                canvas,
                "",
                48,
                TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(new Vector2(1600, 90), new Vector2(0, -270), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f))
            );
            textComponent = textPanel.GetComponent<Text>();
            textComponent.color = Color.red;
            // Prevent the box size from clipping/truncating longer messages.
            textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
        }

        public void Show(string message) {
            textComponent.text = message;
            if (activeRoutine != null) {
                StopCoroutine(activeRoutine);
            }
            activeRoutine = StartCoroutine(ShowRoutine());
        }

        public void ShowFirstPhaseIfAltered() {
            if (!AbsRadConfigurableAttacks.instance.FirstPhaseSettingsAreDefault()) {
                Show("First Phase: Modified Attack Weights");
            }
        }

        public void ShowPlatsPhaseIfAltered() {
            if (!AbsRadConfigurableAttacks.instance.PlatsSettingsAreDefault()) {
                Show("Platform Phase: Modified Attack Weights");
            }
        }

        private IEnumerator ShowRoutine() {
            yield return StartCoroutine(CanvasUtil.FadeInCanvasGroup(canvasGroup));
            yield return new WaitForSeconds(DisplaySeconds);
            yield return StartCoroutine(CanvasUtil.FadeOutCanvasGroup(canvasGroup));
            activeRoutine = null;
        }
    }
}
