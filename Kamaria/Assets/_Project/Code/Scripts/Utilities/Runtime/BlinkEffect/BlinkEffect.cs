using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Utilities
{
    public sealed class BlinkEffect : MonoBehaviour
    {
        [SerializeField] private Color colorBlink;
        [SerializeField] private Color colorContinuousBlink;
        [SerializeField] private float timeBlink;
        [SerializeField] private float continuousBlinkingTime;
        [SerializeField] private float continuousBlinkingSpeed;

        [SerializeField] private List<Renderer> renderers = new List<Renderer>();
        [SerializeField] private List<Color> tempColors = new List<Color>();

        private readonly int mainColor = Shader.PropertyToID("_MainColor");
        private const string RT_SHADER = "Universal Render Pipeline/RealToon/Version 5/Default/Default";

        private void Awake()
        {
            if (renderers.Count == 0)
            {
                renderers.AddRange(GetComponentsInChildren<Renderer>());
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                tempColors.Add(renderers[i].material.shader.name == RT_SHADER
                    ? renderers[i].material.GetColor(mainColor)
                    : renderers[i].material.color);
            }
        }

        public void Blink()
        {
            StopCoroutine(nameof(BlinkTime));
            StartCoroutine(nameof(BlinkTime));
        }

        public void ContinuousBlinking()
        {
            StartCoroutine(nameof(ContinuousBlinkingTime));
            StartCoroutine(nameof(Cooldown));
        }

        private IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(continuousBlinkingTime);
            StopCoroutine(nameof(ContinuousBlinkingTime));
            
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == RT_SHADER)
                {
                    renderers[i].material.SetColor(mainColor, 
                        Color.Lerp(renderers[i].material.GetColor(mainColor), tempColors[i], 1));
                }
                else
                {
                    renderers[i].material.color =
                        Color.Lerp(renderers[i].material.color, tempColors[i], 1);
                }
            }
        }
        
        private IEnumerator ContinuousBlinkingTime()
        {
            while (true)
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    if (renderers[i].material.shader.name == RT_SHADER)
                    {
                        renderers[i].material.SetColor(mainColor,
                            Color.Lerp(renderers[i].material.GetColor(mainColor), colorContinuousBlink, 1));
                    }
                    else
                    {
                        renderers[i].material.color = Color.Lerp(renderers[i].material.color, colorContinuousBlink, 1);
                    }
                }

                yield return new WaitForSeconds(continuousBlinkingSpeed);
            
                for (int i = 0; i < renderers.Count; i++)
                {
                    if (renderers[i].material.shader.name == RT_SHADER)
                    {
                        renderers[i].material.SetColor(mainColor, 
                            Color.Lerp(renderers[i].material.GetColor(mainColor), tempColors[i], 1));
                    }
                    else
                    {
                        renderers[i].material.color =
                            Color.Lerp(renderers[i].material.color, tempColors[i], 1);
                    }
                }
                
                yield return new WaitForSeconds(continuousBlinkingSpeed);
            }
        }

        private IEnumerator BlinkTime()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == RT_SHADER)
                {
                    renderers[i].material.SetColor(mainColor,
                        Color.Lerp(renderers[i].material.GetColor(mainColor), colorBlink, 1));
                }
                else
                {
                    renderers[i].material.color = Color.Lerp(renderers[i].material.color, colorBlink, 1);
                }
            }

            yield return new WaitForSeconds(timeBlink);
            
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == RT_SHADER)
                {
                    renderers[i].material.SetColor(mainColor, 
                        Color.Lerp(renderers[i].material.GetColor(mainColor), tempColors[i], 1));
                }
                else
                {
                    renderers[i].material.color =
                        Color.Lerp(renderers[i].material.color, tempColors[i], 1);
                }
            }
        }
    }
}