using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProgressBarToolkit
{
    [ExecuteInEditMode]
    public class ProgressBar2D : MonoBehaviour
    {

        [Space(10)]
        [Header("Script tracking")]
        [Tooltip("This lets the bar track some value in another script automatically. The bar must be able to access that value. If you do not select a script and value here, you can change the bar values manually.")]
        public Component scriptToTrack;
        public string trackedVarName;
        public string trackedVarMax;
        public string trackedVarMin;

        [Space(10)]
        [Header("Graphics")]
        public Image barBackground;
        public Color barBackgroundColor;
        public Image barFill;
        public Color barFillColor;
        [Tooltip("This offset helps you align the bar fill with the other elements.")]
        public Vector2 barFillOffset;
        [Range(0.0f, 1.0f)]
        public float borderWidth;
        [Tooltip("This is the border that will be drawn around the bar's fill area, but inside the background area.")]
        public Image barBorder;
        public Color barBorderColor;


        [Space(10)]
        [Header("Text settings")]
        public bool showValue;
        [Tooltip("Configure the text style in a Text element and link it here. It will be automatically positioned and updated.")]
        public Text textElement;
        public Color textColor;
        public TextStyle textStyle;

        protected float _minValue;
        protected float _maxValue;
        public float _currentValue;

        public float minValue
        {
            get { return _minValue; }
            set { _minValue = Mathf.Min(_maxValue, value); }
        }

        public float maxValue
        {
            get { return _maxValue; }
            set { _maxValue = Mathf.Max(_minValue, value); }
        }

        public float currentValue
        {
            get { return _currentValue; }
            set { _currentValue = Mathf.Min(_maxValue, Mathf.Max(_minValue, value)); }
        }

        private float barValue;

        private RectTransform rectTransform;

        //Sets some initial values and validates input from inspector.
        public void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            minValue = 0.0f;
            maxValue = 100.0f;
            barValue = _currentValue;
            ValidateVars();
        }

        //Checks if required elements have been provided and informs the developer.
        private bool ValidateVars()
        {
            bool ok = true;

            if (scriptToTrack == null || trackedVarName == null || trackedVarName.Length < 1)
            {
                Debug.LogWarning(gameObject.name + " - ProgressBar2D - Invalid script or variable to track provided. Bar will not update automatically.", gameObject);
            }

            if (barBackground == null)
            {
                Debug.LogError(gameObject.name + " - ProgressBar2D - No background sprite provided. Add a background sprite in the Inspector view.", gameObject);
                ok = false;
            }

            if (barFill == null)
            {
                Debug.LogError(gameObject.name + " - ProgressBar2D - No fill sprite provided. Add a fill sprite in the Inspector view.", gameObject);
                ok = false;
            }

            if (textElement == null && showValue)
            {
                Debug.LogError(gameObject.name + " - ProgressBar2D - No Text element provided but value is set to be shown. Add a Text element in the Inspector view.", gameObject);
            }

            return ok;
        }

        //Smoothly transitions between values and tries tracking a value from another script, if it's provided.
        //currentValue is always exact, only the displayed progress of the bar transitions smoothly.
        public void Update()
        {
            if (Mathf.Abs(barValue - _currentValue) > 0.01f)
            {
                barValue = Mathf.Lerp(barValue, _currentValue, 0.05f);
            }

            //Update the value if script set to be tracked
            if (scriptToTrack != null)
            {
                try
                {
                    float newcurrent = (float)scriptToTrack.GetType().GetProperty(trackedVarName).GetValue(scriptToTrack, null);
                    float newmax = (float)scriptToTrack.GetType().GetProperty(trackedVarMax).GetValue(scriptToTrack, null);
                    float newmin = (float)scriptToTrack.GetType().GetProperty(trackedVarMin).GetValue(scriptToTrack, null);

                    _currentValue = newcurrent;
                    _maxValue = newmax;
                    _minValue = newmin;
                }
                catch (System.NullReferenceException)
                {
                    Debug.LogError("Script tracking error: One of the values does not exist - clearing script tracking");
                    scriptToTrack = null;
                }
            }
        }

        //Draws the elements of the bar and arranges them on the screen
        public void OnGUI()
        {
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            //Draw the background
            barBackground.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            barBackground.rectTransform.anchorMin = Vector2.zero;
            barBackground.rectTransform.offsetMin = Vector2.zero;
            barBackground.rectTransform.offsetMax = Vector2.zero;
            barBackground.color = barBackgroundColor;

            //Draw the border if it exists
            if (barBorder != null)
            {
                barBorder.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                barBorder.rectTransform.anchorMin = Vector2.zero;
                barBorder.rectTransform.offsetMin = Vector2.zero;
                barBorder.rectTransform.offsetMax = Vector2.zero;
                barBorder.color = barBorderColor;
            }

            //Draw the fill area
            barFill.rectTransform.anchorMin = new Vector2(0.0f, borderWidth);

            float scale = (1 - 2 * barFill.rectTransform.anchorMin.x) / (_maxValue - _minValue);
            barFill.rectTransform.anchorMax = new Vector2(barValue * scale + barFill.rectTransform.anchorMin.x, 1.0f - borderWidth);

            barFill.rectTransform.offsetMin = barFillOffset;
            barFill.rectTransform.offsetMax = barFillOffset;
            barFill.color = barFillColor;

            //Draw text
            if (showValue)
            {
                textElement.rectTransform.anchorMin = Vector2.zero;
                textElement.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                textElement.rectTransform.offsetMax = Vector2.zero;
                textElement.rectTransform.offsetMin = Vector2.zero;
                textElement.color = textColor;

                if (textStyle == TextStyle.percent)
                {
                    textElement.text = Mathf.RoundToInt((_currentValue / _maxValue) * 100f) + "%";
                }
                else if (textStyle == TextStyle.currentValue)
                {
                    textElement.text = "" + Mathf.RoundToInt(_currentValue);
                }
                else if (textStyle == TextStyle.currentSlashMax)
                {
                    textElement.text = Mathf.RoundToInt(_currentValue) + "/" + _maxValue;
                }
            }
            else
            {
                textElement.gameObject.SetActive(false);
            }
        }
    }
}