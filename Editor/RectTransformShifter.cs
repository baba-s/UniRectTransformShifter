using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
	[InitializeOnLoad]
	internal static class RectTransformShifter
	{
		static RectTransformShifter()
		{
			SceneView.duringSceneGui -= OnDuringSceneGui;
			SceneView.duringSceneGui += OnDuringSceneGui;
		}

		private static void OnDuringSceneGui( SceneView sceneView )
		{
			var currentEvent   = Event.current;
			var modifiers      = currentEvent.modifiers;
			var isKey          = currentEvent.isKey;
			var isKeyDown      = currentEvent.type == EventType.KeyDown;
			var isFunctionKey  = modifiers == EventModifiers.None || modifiers == EventModifiers.FunctionKey;
			var isOrthographic = sceneView.camera.orthographic;
			var keyCode        = currentEvent.keyCode;

			var isArrowKey =
					keyCode == KeyCode.LeftArrow ||
					keyCode == KeyCode.RightArrow ||
					keyCode == KeyCode.DownArrow ||
					keyCode == KeyCode.UpArrow
				;

			if ( !isKey || !isKeyDown || !isFunctionKey || !isOrthographic || !isArrowKey ) return;

			var rectTransforms = Selection.gameObjects
					.Select( x => x.GetComponent<RectTransform>() )
					.Where( x => x != null )
					.ToArray()
				;

			if ( rectTransforms.Length <= 0 ) return;

			var objectsToUndo = rectTransforms
					.OfType<Object>()
					.ToArray()
				;

			Undo.RecordObjects( objectsToUndo, "Shift" );

			var offset = ToOffset( keyCode );

			foreach ( var rectTransform in rectTransforms )
			{
				var position = rectTransform.anchoredPosition;
				position                       += offset;
				position.x                     =  Mathf.Round( position.x );
				position.y                     =  Mathf.Round( position.y );
				rectTransform.anchoredPosition =  position;
			}

			currentEvent.Use();
		}

		private static Vector2 ToOffset( KeyCode keyCode )
		{
			switch ( keyCode )
			{
				case KeyCode.RightArrow: return Vector2.right;
				case KeyCode.LeftArrow:  return Vector2.left;
				case KeyCode.UpArrow:    return Vector2.up;
				case KeyCode.DownArrow:  return Vector2.down;
			}

			return Vector2.zero;
		}
	}
}