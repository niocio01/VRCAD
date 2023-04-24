using System.Linq;
using Editors.PartEdit;
using UnityEngine;

namespace Editors.FeatureEdit
{
    public class FeatureEditor : MonoBehaviour
    {
        [SerializeField] private PartEditor partEditor;

        public void AddExtrusion()
        {
            if (partEditor.Part.Features.Count < 1)
            {
                if (partEditor.Part.Sketches.Count == 1)
                {
                    partEditor.Part.AddFeature(new Extrude(partEditor.Part.Sketches.First(), 0.1f, partEditor.Part.FeatureIdCounter));
                    partEditor.RebuildPart();
                }
            }
        }
    }
}