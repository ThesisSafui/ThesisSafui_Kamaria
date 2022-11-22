using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Utilities.FadeObject
{
    public sealed class FadingObject : MonoBehaviour, IEquatable<FadingObject>
    { 
        public List<Renderer> Renderers = new List<Renderer>();
        public Vector3 Position;
        public List<Material> Materials = new List<Material>();
        
        [HideInInspector] public float InitialAlpha;

        private readonly int opacity = Shader.PropertyToID("_Opacity");
        private const string RT_SHADER = "Universal Render Pipeline/RealToon/Version 5/Default/Default";

        private void Awake()
        {
            Position = transform.position;

            if (Renderers.Count == 0)
            {
                Renderers.AddRange(GetComponentsInChildren<Renderer>());
            }
            foreach(Renderer renderer in Renderers)
            {
                Materials.AddRange(renderer.materials);
            }

            InitialAlpha = Materials[0].shader.name == RT_SHADER ? Materials[0].GetFloat(opacity) : Materials[0].color.a;
        }

        public bool Equals(FadingObject other)
        {
            return Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}