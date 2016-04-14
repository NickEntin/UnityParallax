using UnityEngine;
using System.Collections;

public class ParallaxController : MonoBehaviour {

	[System.Serializable]
	public class ParallaxLayer {
		public GameObject prefab;

		[Range(-1,1)]
		[Tooltip("Delta to scroll speed. Zero is regular scrolling. Positive value for slower scrolling. Negative value for faster scrolling.")]
		public float scrollDelta;

		public GameObject primary { get; private set; }
		public GameObject secondary { get; private set; }

		[Tooltip("Width of prefab element. Set element width to -1 to calculate from prefab's renderer.")]
		public float elementWidth = -1;

		public void GenerateElements() {
			if (prefab == null) {
				Debug.LogError("Parallax layer prefab cannot be null");
				return;
			}

			primary = Instantiate(prefab);
			primary.name = prefab.name + " (Parallax Primary)";

			secondary = Instantiate(prefab);
			secondary.name = prefab.name + " (Parallax Secondary)";

			if (elementWidth == -1f) {
				Renderer renderer = primary.GetComponent<Renderer>();
				if (renderer == null) {
					Debug.LogError("You must either provide an element width or use a prefab with a renderer.");
					return;
				}
				elementWidth = renderer.bounds.size.x;
			}
		}
	}

    [Tooltip("Camera used to calculate parallax. Defaults to main scene camera.")]
	public GameObject mainCamera;
	
	public ParallaxLayer[] layers;

	void Start() {
		// Default to the main scene camera
		if (mainCamera == null) {
			mainCamera = Camera.main.gameObject;
		}

		// Generate elements for each of the layers
		for (int i = 0 ; i < layers.Length ; i++) {
			layers[i].GenerateElements();
		}
	}

	private Vector3 previousCameraPosition = Vector3.zero;

	void LateUpdate() {
		// Calculate how much the camera moved this frame
		float xDelta = (mainCamera.transform.position - previousCameraPosition).x;
		previousCameraPosition = mainCamera.transform.position;

        // Exit early if camera has not moved
		if (xDelta == 0) {
			return;
		}

		for (int i = 0 ; i < layers.Length ; i++) {
			ParallaxLayer layer = layers[i];

			// Calculate primary position
			Vector3 position = layer.primary.transform.position;
			position.x += xDelta * layer.scrollDelta;

			// Check if elements can be shifted
			if (mainCamera.transform.position.x - position.x > layer.elementWidth) {
				position.x += layer.elementWidth;
			}

			// Set primary position
			layer.primary.transform.position = position;

			// Calculate and set secondary position
			position.x += layer.elementWidth;
			layer.secondary.transform.position = position;
		}
	}
}
