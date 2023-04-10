using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//https://github.com/merpheus-dev/NodeBasedDialogueSystem/tree/master/com.subtegral.dialoguesystem/Editor/Graph
//https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e
namespace BTEditor
{
    public class BTGraph : EditorWindow
    {
        private BTGraphView _graphView;
        private Toolbar _toolbar;
        private string _filepath;
        private bool _hasSaveButton;

        [MenuItem("Behavior Tree/New Graph")]
        public static void Open()
        {
            var window = CreateWindow<BTGraph>();
            window.titleContent = new GUIContent("Behavior Tree Graph");
        }

        [MenuItem("Behavior Tree/Load Graph")]
        public static void Load()
        {
            var filepath = EditorUtility.OpenFilePanel("Load Behavior Tree", Application.dataPath, "json");
            if (string.IsNullOrEmpty(filepath))
                return;

            var window = CreateWindow<BTGraph>();
            window._filepath = filepath;
            window.AddSavebutton();
            window.titleContent = new GUIContent("Behavior Tree Graph");
            window._graphView.LoadFromFile(filepath);
        }

        private void OnEnable()
        {
            _graphView = new BTGraphView(this)
            {
                name = "Behavior Tree Graph"
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
            GenerateToolbar();
            GenerateMiniMap();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void GenerateToolbar()
        {
            _toolbar = new Toolbar();

            _toolbar.Add(new Button(SaveAs) {text = "Save As"});
            rootVisualElement.Add(_toolbar);
        }

        private void AddSavebutton()
        {
            if (_hasSaveButton)
                return;

            _toolbar.Add(new Button(Save) {text = "Save"});
            _hasSaveButton = true;
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap {anchored = true};
            var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }

        private void SaveAs()
        {
            _filepath = EditorUtility.SaveFilePanel("Save Behavior Tree", Application.dataPath, "New Behavior Tree", "json");
            if (string.IsNullOrEmpty(_filepath))
                return;
            AddSavebutton();
            BTIO.Export(_graphView, _filepath);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_filepath))
                return;

            BTIO.Export(_graphView, _filepath);
        }
    }
}