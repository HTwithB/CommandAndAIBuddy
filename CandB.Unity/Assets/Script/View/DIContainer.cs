using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CandB.Script.View
{
    public class DiContainer : MonoBehaviour
    {
#nullable disable
        [SerializeField] private MonoBehaviour lifetimeScope;
#nullable enable

        private IObjectResolver? _objectResolver;

        private void Awake()
        {
            Debug.LogFormat("DiContainer Awake");
            SetupResolver();
        }

        public T Resolve<T>()
        {
            if (_objectResolver != null) return _objectResolver.Resolve<T>();

            Debug.LogError("DiContainer: objectResolver is null");
            SetupResolver();

            if (_objectResolver == null) throw new Exception("DiContainer: objectResolver is still null after setup");

            return _objectResolver.Resolve<T>();
        }

        private void SetupResolver()
        {
            if (lifetimeScope is not LifetimeScope scope) return;

            _objectResolver = scope.Container;
            Debug.LogFormat("DiContainer: resolver successfully setup");
        }
    }
}