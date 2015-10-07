using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProgressBarToolkit
{

    public class NonscalingProgressBar2D : MonoBehaviour
    {

       
        public Component scriptToTrack;
        public string trackedVarName;
        public string trackedVarMax;
        public string trackedVarMin;

        
        public Image barBackground;
        public Color barBackgroundColor;
        public Image barFill;
        public Color barFillColor;
        [Tooltip("This offset helps you align the bar fill with the other elements.")]
        public Vector2 barFillOffset;
        [Tooltip("This is the distance the bar's fill area will be separated by from the edges of the background. Range: 0-1")]
        public float borderWidth;
        [Tooltip("This is the border that will be drawn around the bar's fill area, but inside the background area.")]
        public Image barBorder;
        public Color barBorderColor;


        public bool showValue;
        [Tooltip("Configure the text style in a Text element and link it here. It will be automatically positioned and updated.")]
        public Text textElement;
        public Color textColor;
        public TextStyle textStyle;
        public Vector2 textOffset;
        [Range(0, 100)]
        protected float _currentValue;
        protected float _minValue;
        protected float _maxValue;


        public float minValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public float maxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
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
            
        }


        //Smoothly transitions between values and tries tracking a value from another script, if it's provided.
        //currentValue is always exact, only the displayed progress of the bar transitions smoothly.
        public void Update()
        {
			
			barValue = Mathf.Min(barValue + Time.deltaTime * 30, _maxValue);
			_currentValue = barValue;
        }

        //Draws the elements of the bar and arranges them on the screen
        public void OnGUI()
        {
		

            //Draw the background
            barBackground.rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
            barBackground.rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
            barBackground.rectTransform.offsetMin = Vector2.zero;
            barBackground.rectTransform.offsetMax = Vector2.zero;
            barBackground.rectTransform.pivot = new Vector2(0.0f, 0.5f);
            barBackground.SetNativeSize();
            barBackground.color = barBackgroundColor;

            //Draw the border if it exists
            Vector2 border = new Vector2(borderWidth, borderWidth);
            if (barBorder != null)
            {
                barBorder.rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
                barBorder.rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
                barBorder.rectTransform.offsetMin = Vector2.zero;
                barBorder.rectTransform.offsetMax = Vector2.zero;
                barBorder.rectTransform.pivot = new Vector2(0.0f, 0.5f);
                barBorder.SetNativeSize();
                barBorder.color = barBorderColor;
            }
            

            //Draw the fill area
            barFill.SetNativeSize();

            barFill.rectTransform.anchorMin = new Vector2(border.x / rectTransform.rect.width, -barFill.mainTexture.height / rectTransform.rect.height / 2 + 0.5f);//2+0.5f);

            float scale = (barBackground.mainTexture.width / rectTransform.rect.width - barFill.rectTransform.anchorMin.x) / (_maxValue - _minValue);

            barFill.rectTransform.anchorMax = new Vector2(barValue * scale + barFill.rectTransform.anchorMin.x - border.x / rectTransform.rect.width, barFill.mainTexture.height / rectTransform.rect.height / 2 + 0.5f);///2 + 0.5f);

            barFill.rectTransform.offsetMin = barFillOffset;
            barFill.rectTransform.offsetMax = barFillOffset;
            barFill.color = barFillColor;

            //Draw text
            if (showValue)
            {
                textElement.rectTransform.anchorMin = new Vector2(0, 0.5f);
                textElement.rectTransform.anchorMax = new Vector2(0, 0.5f);
                textElement.rectTransform.offsetMax = textOffset + new Vector2(barBackground.rectTransform.rect.xMax / 2f, 0.0f);
                textElement.rectTransform.offsetMin = textElement.rectTransform.offsetMax;

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
        }
    }
}