using UnityEngine;

public class SampleAnimation : MonoBehaviour
{
    [SerializeField] private Animation _tableAnimation;
    [SerializeField] private Animation _movablePlant;

    private bool _isOpen;


    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _isOpen = !_isOpen;
            Debug.Log($"Tablet is {(_isOpen ? "Open" : "Closed")}");
            if (_isOpen)
            {
                Debug.Log("Play TabletIn animation");
                _tableAnimation.Play("TabletIn");
            }
            else
            {
                Debug.Log("Play TabletOut animation");
                _tableAnimation.Play("TabletOut");
            }
        }
    }
}