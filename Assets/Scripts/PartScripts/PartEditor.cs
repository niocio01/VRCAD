using System.Linq;
using UnityEngine;

public enum EditMode_t
{
    Main,
    Sketch,
    Extrude
}

public class PartEditor : MonoBehaviour
{
    [SerializeField] private TextAsset jsonFile;
    [SerializeField] private bool useJsonFile;
    [SerializeField] private MeshDrawer meshDrawer;
    [SerializeField] private GameObject sketchEditorGameObject;
    [SerializeField] private GameObject mainEditorGameObject;
    public Part Part { get; private set; }
    private SketchEditor _sketchEditor;
    private EditMode_t _editMode = EditMode_t.Main;

    private void Awake()
    {
        _sketchEditor = sketchEditorGameObject.GetComponent<SketchEditor>();
    }

    private void Start()
    {
        if (!useJsonFile || jsonFile == null)
        {
            Part = new Part("EditorTest", "Made VR Editor", "Nico Zuber");
            Part.AddSketch(new Sketch(0, "Default"));
            _sketchEditor.StartEditSketch(Part.Sketches.First());

            _editMode = EditMode_t.Sketch;
            sketchEditorGameObject.SetActive(true);
            mainEditorGameObject.SetActive(false);
        }
        else
        {
            Part = JsonHandler.JsonLoad(jsonFile);

            if (_sketchEditor.StartEditSketch(Part.Sketches.First()))
            {
                _editMode = EditMode_t.Sketch;
                sketchEditorGameObject.SetActive(true);
                mainEditorGameObject.SetActive(false);
            }
        }
    }
    public void AcceptPressed()
    {
        switch (_editMode)
        {
            case EditMode_t.Main: break;
            case EditMode_t.Sketch:
                {
                    if (_sketchEditor.AcceptPressed())
                    {
                        _editMode = EditMode_t.Main;
                        meshDrawer.UpdateMesh();
                        sketchEditorGameObject.SetActive(false);
                        mainEditorGameObject.SetActive(true);
                    }
                } break;
            case EditMode_t.Extrude: break;
        }
    }
    public void CancelPressed()
    {
        switch (_editMode)
        {
            case EditMode_t.Main: break;
            case EditMode_t.Sketch: break;
            case EditMode_t.Extrude: break;
        }
    }
    public void PrintJson()
    {
        JsonHandler.JsonSave(Part);
    }
}
