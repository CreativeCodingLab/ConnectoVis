﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class RangeSlider : MonoBehaviour
    {
        [Header("SETTINGS")]
        [Range(0,3)] public int DecimalPlaces = 0;
        public float minValue = 0;
        public float maxValue = 1;
        public bool useWholeNumbers = false;
        public bool showLabels = true;

        [Header("MIN SLIDER")]
        public RangeMinSlider minSlider;
        public TextMeshProUGUI minSliderLabel;

        [Header("MAX SLIDER")]
        public RangeMaxSlider maxSlider;
        public TextMeshProUGUI maxSliderLabel;

        private GameObject[] NetworkParents;
        private bool isFirstUpdate = true;
        // Properties
        public float CurrentLowerValue
        {
            get { return minSlider.value; }
        }
        public float CurrentUpperValue
        {
            get { return maxSlider.realValue; }
        }

        void Awake()
        {
            // Define if we use indicators:
            if (showLabels)
            {
                minSlider.label = minSliderLabel;
                minSlider.numberFormat = "n" + DecimalPlaces;
                maxSlider.label = maxSliderLabel;
                maxSlider.numberFormat = "n" + DecimalPlaces;
            }
            else
            {
                minSliderLabel.gameObject.SetActive(false);
                maxSliderLabel.gameObject.SetActive(false);
            }

            // Adjust Max/Min values for both sliders
            minSlider.minValue = minValue;
            minSlider.maxValue = maxValue;
            minSlider.wholeNumbers = useWholeNumbers;

            maxSlider.minValue = minValue;
            maxSlider.maxValue = maxValue;
            maxSlider.wholeNumbers = useWholeNumbers;
        }
        private void Update()
        {
            if (isFirstUpdate)
            {
                isFirstUpdate = false;
                NetworkParents = GameObject.FindGameObjectsWithTag("SingleConnectome");
            }
            if (this.name == "PosRange")
            {
                foreach (GameObject network in NetworkParents)
                {
                    network.GetComponent<SingleNetwork>().posThresholdMax = CurrentUpperValue;
                    network.GetComponent<SingleNetwork>().posThresholdMin = CurrentLowerValue;
                }
            }
            else if(this.name == "NegRange")
            {
                foreach (GameObject network in NetworkParents)
                {
                    network.GetComponent<SingleNetwork>().negThresholdMax = CurrentUpperValue;
                    network.GetComponent<SingleNetwork>().negThresholdMin = CurrentLowerValue;
                }
            }
            
        }
    }
}