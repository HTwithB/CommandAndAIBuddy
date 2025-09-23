using DG.Tweening;
using UnityEngine;

public class SampleLight : MonoBehaviour
{
    [SerializeField] private Light[] _lights;

    private bool _isOn;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (_isOn)
            {
                foreach (var light in _lights) light.DOIntensity(0.5f, 1f);
                _isOn = false;
            }
            else
            {
                foreach (var light in _lights) light.DOIntensity(4f, 1f);
                _isOn = true;
            }

            Debug.Log("Toggle Lights");
        }
    }
}