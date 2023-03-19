using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EditMode
{
    None,
    Sketch,
    Feature
}

public class PartEditor : MonoBehaviour
{
    [SerializeField] TextAsset JsonFile;
    [SerializeField] GameObject SketchEditorObject;
    // [SerializeField] GameObject FeatureEditor;

    public Part Part { get; private set; }
    private EditMode editMode = EditMode.None;

    private GameObject sketchEditor;
    public Feature CurrentFeature { get; private set; }

    private void Start()
    {
        if (JsonFile == null)
        {
            Part = new Part("EditorTest", "Made VR Editor", "Nico Zuber");
            Part.AddSketch(new Sketch(0, "Default"));
            StartEditSketch(Part.Sketches.First());
        }
        else
        {
            Part = JsonHandler.JsonLoad(JsonFile);
            StartEditSketch(Part.Sketches.First());
        }
    }

    private void StartEditSketch(Sketch sketch)
    {
        if (sketch == null) return;
        if (editMode != EditMode.None) return;

        editMode = EditMode.Sketch;

        SketchEditor editor = SketchEditorObject.GetComponent<SketchEditor>();
        editor.SetSketch(sketch);

        SketchEditorObject.SetActive(true);
    }

    public void PrintJson()
    {
        JsonHandler.JsonSave(Part);
    }
}
