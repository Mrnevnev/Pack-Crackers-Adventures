using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class MoveComponentToNewObject : MonoBehaviour
{
    [MenuItem("CONTEXT/Component/Move Component to Parent Object")]
    private static void MoveToParentObject(MenuCommand command)
    {
        // Step 1: Reference to the component to move
        var componentToMove = command.context as Component;
        if (componentToMove == null)
        {
            Debug.LogError("Component is null.");
            return;
        }

        // If htere is no parent, return
        if (componentToMove.transform.parent == null)
        {
            Debug.LogError("Component has no parent.");
            return;
        }

        var newParentObject = componentToMove.transform.parent.gameObject;

        // Step 3: Copy the component to the new parent object
        var newComponent = newParentObject.AddComponent(componentToMove.GetType());
        if (newComponent != null)
        {
            ComponentUtility.CopyComponent(componentToMove);
            ComponentUtility.PasteComponentValues(newComponent);
        }

        // Step 4: Remove the component from the original parent object
        DestroyImmediate(componentToMove);

        // Step 5: Select the new object and highlight it for renaming
        Selection.activeGameObject = newParentObject;
        EditorGUIUtility.PingObject(newParentObject);
    }

    [MenuItem("CONTEXT/Component/Move Component to New Object")]
    private static void MoveToNewObject(MenuCommand command)
    {
        // Step 1: Reference to the component to move
        var componentToMove = command.context as Component;
        if (componentToMove == null)
        {
            Debug.LogError("Component is null.");
            return;
        }

        // Step 2: Create a new child GameObject
        var parentObject = componentToMove.gameObject;
        var newChildObject = new GameObject($"{componentToMove.GetType().Name}");
        newChildObject.transform.SetParent(parentObject.transform.parent);
        newChildObject.transform.localPosition = Vector3.zero; // Position it at the parent's location

        // Step 3: Copy the component to the new child object
        var newComponent = newChildObject.AddComponent(componentToMove.GetType());
        if (newComponent != null)
        {
            ComponentUtility.CopyComponent(componentToMove);
            ComponentUtility.PasteComponentValues(newComponent);
        }

        // Step 4: Remove the component from the original parent object
        DestroyImmediate(componentToMove);

        // Step 5: Select the new object and highlight it for renaming
        Selection.activeGameObject = newChildObject;
        EditorGUIUtility.PingObject(newChildObject);
    }

    [MenuItem("CONTEXT/Component/Move Component to New Child Object")]
    private static void MoveToNewChildObject(MenuCommand command)
    {
        // Step 1: Reference to the component to move
        var componentToMove = command.context as Component;
        if (componentToMove == null)
        {
            Debug.LogError("Component is null.");
            return;
        }

        // Step 2: Create a new child GameObject
        var parentObject = componentToMove.gameObject;
        var newChildObject = new GameObject($"{componentToMove.GetType().Name}");
        newChildObject.transform.SetParent(parentObject.transform);
        newChildObject.transform.localPosition = Vector3.zero; // Position it at the parent's location

        // Step 3: Copy the component to the new child object
        var newComponent = newChildObject.AddComponent(componentToMove.GetType());
        if (newComponent != null)
        {
            ComponentUtility.CopyComponent(componentToMove);
            ComponentUtility.PasteComponentValues(newComponent);
        }

        // Step 4: Remove the component from the original parent object
        DestroyImmediate(componentToMove);

        // Step 5: Select the new object and highlight it for renaming
        Selection.activeGameObject = newChildObject;
        EditorGUIUtility.PingObject(newChildObject);
    }
}