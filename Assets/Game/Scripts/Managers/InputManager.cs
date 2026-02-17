using UnityEngine;

namespace Game.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private LayerMask m_CardLayer;

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
                Raycast(Input.mousePosition);
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            Raycast(Input.GetTouch(0).position);
#endif
        }

        private void Raycast(Vector2 screenPos)
        {
            Ray ray = m_MainCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, m_CardLayer))
            {
                Card card = hit.collider.GetComponent<Card>();
                card?.Select();
            }
        }
    }
}