using PrimitiveFactory.ScriptableObjectSuite;
using UnityEditor;

public class PromptWindow : ScriptableObjectEditorWindow<Prompt>
{
	// Name of the object (Display purposes)
	protected override string c_ObjectName { get { return "Prompt"; } }
	// Relative path from Project Root
	protected override string c_ObjectFullPath { get { return "Assets/Scripts/Scriptable Objects/MenuPrompts/Data"; } }

	[MenuItem("RCCL/MenuPrompts/Menus")]
	public static void ShowWindow()
	{
		PromptWindow window = GetWindow<PromptWindow>("Prompt Editor");
		window.Show();
	}
}