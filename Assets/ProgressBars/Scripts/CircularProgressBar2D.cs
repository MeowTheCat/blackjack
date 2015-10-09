using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProgressBarToolkit
{
    public enum TextStyle
    {
        percent, currentValue, currentSlashMax
    }


    public class CircularProgressBar2D : MonoBehaviour
    {
      
   
        public Image barBackground;
        public Color barBackgroundColor;
        public Image barFill;
        public Color barFillColor;

        public bool showValue;
   
        public Text textElement;
        public Color textColor;
        public TextStyle textStyle;
        public Vector2 textOffset;

		public float _minValue ;
		public float _maxValue ;
        public float _currentValue;
		public float barValue ;

 
        private RectTransform rectTransform;

        //Sets some initial values and validates input from inspector.
        public void Start()
        {
            rectTransform = GetComponent<RectTransform>();

        }

		public void Init(float time)
		{
			 _minValue = 0f;
			 _maxValue = time;
			 _currentValue = 0f;
			 barValue = 0f;
		}
       

        public void Update()
        {
			barValue = Mathf.Min(barValue + Time.deltaTime,_maxValue);
			_currentValue = barValue;   
        }

        //Draws the elements of the bar and arranges them on the screen
        public void OnGUI()
        {
			/*
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            */

            //Draw the background
			/*
            barBackground.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            barBackground.rectTransform.anchorMin = Vector2.zero;
            barBackground.rectTransform.offsetMin = Vector2.zero;
            barBackground.rectTransform.offsetMax = Vector2.zero;
            */
            barBackground.color = barBackgroundColor;
            barBackground.preserveAspect = true;

            //Draw the fill area
			/*
            barFill.rectTransform.anchorMin = Vector2.zero;
            barFill.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            barFill.rectTransform.offsetMin = Vector2.zero;
            barFill.rectTransform.offsetMax = Vector2.zero;
            */
            barFill.color = barFillColor;
            barFill.fillAmount = barValue / _maxValue;
            barFill.preserveAspect = true;


            //Draw text
            if (showValue)
            {
                textElement.rectTransform.anchorMin = Vector2.zero;
                textElement.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                textElement.rectTransform.offsetMax = textOffset;
                textElement.rectTransform.offsetMin = textOffset;
                textElement.color = textColor;

                if (textStyle == TextStyle.percent)
                {
                    textElement.text = Mathf.RoundToInt((_currentValue / _maxValue) * 100f) + "%";
                }
                else if (textStyle == TextStyle.currentValue)
                {
                    textElement.text = "" + Mathf.FloorToInt(_currentValue);
                }
                else if (textStyle == TextStyle.currentSlashMax)
                {
                    textElement.text = Mathf.RoundToInt(_currentValue) + "/" + _maxValue;
                }
            }
        }
    }
}
