using System;
using System.Linq;
using CandB.Script.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CandB.Script.View
{
    public class CandBComponent : MonoBehaviour
    {
        private const string ApplicationLifetimeScopeGameObjectName = "Application";

        private EnvironmentService? _environmentService;

        private IObjectResolver? _resolver;

        public EnvironmentService EnvironmentService
        {
            get
            {
                if (_environmentService == null) _environmentService = Resolve<EnvironmentService>();
                return _environmentService;
            }
        }

        private void Awake()
        {
            Debug.LogFormat("DiContainer Awake");
            SetupResolver();
        }

        protected T Resolve<T>()
        {
            if (_resolver != null) return _resolver.Resolve<T>();

            Debug.LogError("DiContainer: objectResolver is null");
            SetupResolver();

            if (_resolver == null) throw new Exception("DiContainer: objectResolver is still null after setup");

            return _resolver.Resolve<T>();
        }

        private void SetupResolver()
        {
            var lifetimeScope = FindObjectsByType<LifetimeScope>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (lifetimeScope == null) return;

            var applicationLifetimeScope =
                lifetimeScope.FirstOrDefault(item => item.gameObject.name == ApplicationLifetimeScopeGameObjectName);
            if (applicationLifetimeScope == null) return;

            _resolver = applicationLifetimeScope.Container;
            Debug.LogFormat("DiContainer: resolver successfully setup");
        }
    }
}