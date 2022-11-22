using System;
using System.Collections.Generic;
using Kamaria.Player.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI
{
    public sealed class UIFirstLogin : MonoBehaviour
    {
        #region REFERENCE_VARIABLE

        [SerializeField] private PlayerDataSO playerData;

        [Space] 
        [SerializeField] private Slider ageSlider;
        [SerializeField] private TextMeshProUGUI age;

        [Space] 
        [SerializeField] private Button submit;
        [SerializeField] private List<ButtonGender> buttonGenders;

        #endregion

        private void OnEnable()
        {
            Initialized();
            
            ageSlider.onValueChanged.AddListener(ChangeAgeValue);
            
            foreach (ButtonGender buttonGender in buttonGenders)
            {
                buttonGender.Button.onClick.AddListener(delegate { GetGender(buttonGender); });
            }
        }

        private void OnDisable()
        {
            ageSlider.onValueChanged.RemoveListener(ChangeAgeValue);
           
            foreach (ButtonGender buttonGender in buttonGenders)
            {
                buttonGender.Button.onClick.RemoveAllListeners();
            }
        }
        
        private void ChangeAgeValue(float callbackValue)
        {
            GetAge(callbackValue);
        }
        
        private void Initialized()
        {
            ageSlider.value = ageSlider.minValue;
            
            GetAge(ageSlider.value);
            
            submit.interactable = false;
            
            foreach (ButtonGender buttonGender in buttonGenders)
            {
                buttonGender.Button.interactable = true;
            }
        }
        
        private void GetAge(float age)
        {
            int temp = (int)age;
            this.age.text = temp.ToString();
            playerData.Info.Authentication.Age = temp;
        }

        private void GetGender(ButtonGender button)
        {
            playerData.Info.Authentication.Gender = button.Gender;

            submit.interactable = true;
            
            foreach (ButtonGender buttonGender in buttonGenders)
            {
                if (buttonGender.Gender == button.Gender)
                {
                    buttonGender.Button.interactable = false;
                    continue;
                }
                
                buttonGender.Button.interactable = true;
            }
        }
    }

    [Serializable]
    public sealed class ButtonGender
    {
        [SerializeField] private PlayerGenders gender;
        [SerializeField] private Button button;

        public PlayerGenders Gender => gender;
        public Button Button => button;
    }
}