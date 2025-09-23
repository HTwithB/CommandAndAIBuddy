using CandB.Script.Controller;
using UnityEngine;
using VContainer.Unity;

namespace CandB.Script.Application
{
    public class ApplicationEntryPoint : IStartable
    {
        private readonly ApplicationInitializer _applicationInitializer;

        public ApplicationEntryPoint(
            ApplicationInitializer applicationInitializer
        )
        {
            _applicationInitializer = applicationInitializer;
        }

        public void Start()
        {
            Debug.LogFormat("ApplicationEntryPoint");
            _applicationInitializer.Execute();
        }
    }
}