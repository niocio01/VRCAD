using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EditMode_t
{
    None,
    Sketch,
    Feature
}

public class PartEditor : MonoBehaviour
{
    [SerializeField] TextAsset JsonFile;
    [SerializeField] private SketchEditor SketchEditor;
    [SerializeField] private GameObject MeshParent;
    [SerializeField] private MeshDrawer MeshDrawer;

    public Part Part { get; private set; }
    private EditMode_t EditMode = EditMode_t.None;

    
    public Feature CurrentFeature { get; private set; }

    private void Start()
    {
        if (JsonFile == null)
        {
            Part = new Part("EditorTest", "Made VR Editor", "Nico Zuber");
            Part.AddSketch(new Sketch(0, "Default"));
            SketchEditor.StartEditSketch(Part.Sketches.First());
        }
        else
        {
            Part = JsonHandler.JsonLoad(JsonFile);

            if (SketchEditor.StartEditSketch(Part.Sketches.First()))
            {
                EditMode = EditMode_t.Sketch;
            }
        }
    }

    public void AcceptPressed()
    {
        switch (EditMode)
        {
            case EditMode_t.None: break;
            case EditMode_t.Sketch:
                {
                    if (SketchEditor.AcceptPressed())
                    {
                        EditMode = EditMode_t.None;

                        MeshDrawer.UpdateMesh();
                    }
                } break;
            case EditMode_t.Feature: break;
        }
    }

    public void CancelPressed()
    {
        switch (EditMode)
        {
            case EditMode_t.None: break;
            case EditMode_t.Sketch: break;
            case EditMode_t.Feature: break;
        }
    }

    public void PrintJson()
    {
        JsonHandler.JsonSave(Part);
    }
}
