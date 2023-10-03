using System.Linq;
using UnityEngine;
using Editors.SketchEdit;
using Geometry;
using Rendering;

namespace Editors.PartEdit
{
    public enum EditModeT
    {
        Main,
        Sketch,
        Extrude
    }

    public class PartEditor : MonoBehaviour
    {
        [SerializeField] private string saveFileName;
        [SerializeField] private string saveFilesFolderPath = Application.dataPath + "/SaveFiles/";
        [SerializeField] private bool useJsonFile;
        [SerializeField] private TextAsset jsonFile;
        [SerializeField] private MeshDrawer meshDrawer;
        [SerializeField] private MeshManager meshManager;
        [SerializeField] private GameObject sketchEditorGameObject;
        [SerializeField] private GameObject mainEditorGameObject;
        public Part Part { get; private set; }
        private SketchEditor _sketchEditor;
        private EditModeT _editMode = EditModeT.Main;

        private void Awake()
        {
            _sketchEditor = sketchEditorGameObject.GetComponent<SketchEditor>();
        }
        
        private void Start()
        {
            if (!useJsonFile || jsonFile == null)
            {
                Part = new Part("EditorTest", "Made VR Editor", "Nico Zuber");
            }
            else
            {
                Part = JsonHandler.JsonLoad(jsonFile);
            }
            if (Part.Features.Count < 1)
            {
                // create Sketch if there is none
                if (Part.Sketches.Count < 1)
                {
                    Part.AddSketch(new Sketch(0, "Default"));
                    _sketchEditor.StartEditSketch(Part.Sketches.First());
                }
                // if sketch incomplete, edit it.
                if (Part.Sketches.Count == 1 && !Part.Sketches.First().IsClosed())
                {
                    _editMode = EditModeT.Sketch;
                    sketchEditorGameObject.SetActive(true);
                    mainEditorGameObject.SetActive(false);
                    return;
                }
            }
            // Features or closed sketch found, show Mesh
            _editMode = EditModeT.Main;
            RebuildPart();
            sketchEditorGameObject.SetActive(false);
            mainEditorGameObject.SetActive(true);
        }

        public void RebuildPart()
        {
            meshManager.RebuildMesh();
        }
        
        public void OnAccept()
        {
            switch (_editMode)
            {
                case EditModeT.Main: break;
                case EditModeT.Sketch:
                {
                    if (_sketchEditor.SketchIsClosed())
                    {
                        _editMode = EditModeT.Main;
                        meshManager.AddSketchAsSurface(_sketchEditor.Sketch);
                        sketchEditorGameObject.SetActive(false);
                        mainEditorGameObject.SetActive(true);
                    }
                } break;
                case EditModeT.Extrude: break;
            }
        }
        
        public void OnCancel()
        {
            switch (_editMode)
            {
                case EditModeT.Main: break;
                case EditModeT.Sketch: break;
                case EditModeT.Extrude: break;
            }

            meshManager.RebuildMesh();
        }

        public void OnSave()
        {
            Part.UpdateLastChanged();
            Part.Save(saveFilesFolderPath + saveFileName + ".json");
        }

        public void OnDelete()
        {
            Part = new Part("EditorTest", "Made VR Editor", "Nico Zuber");
            Part.AddSketch(new Sketch(0, "Default"));
            _sketchEditor.StartEditSketch(Part.Sketches.First());

            _editMode = EditModeT.Sketch;
            sketchEditorGameObject.SetActive(true);
            mainEditorGameObject.SetActive(false);

            meshManager.RebuildMesh();
        }
    }
}