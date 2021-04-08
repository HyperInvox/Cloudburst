using UnityEngine;

namespace RoR2
{
	public class TemporaryVisualEffect : MonoBehaviour
	{
		public enum VisualState
		{
			Enter = 0,
			Exit = 1,
		}

		public float radius;
		public Transform parentTransform;
		public Transform visualTransform;
		public MonoBehaviour[] enterComponents;
		public MonoBehaviour[] exitComponents;
		public VisualState visualState;
		public HealthComponent healthComponent;
	}
}
