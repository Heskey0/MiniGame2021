using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class GlobalStateMachineWindow : EditorWindow
{
    [MenuItem("CustomTools/GlobalStateMachineWindow")]
    public static void ShowExample()
    {
        GlobalStateMachineWindow wnd = GetWindow<GlobalStateMachineWindow>();
        wnd.titleContent = new GUIContent("GlobalStateMachineWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.


        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/1.Scripts/1.Core/StateMachine/Editor/GlobalStateMachineWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/1.Scripts/1.Core/StateMachine/Editor/GlobalStateMachineWindow.uss");

        
    }
}