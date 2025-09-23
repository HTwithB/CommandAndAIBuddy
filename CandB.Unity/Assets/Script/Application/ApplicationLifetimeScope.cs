using System.Net.Http.Headers;
using CandB.Script.Controller;
using CandB.Script.Core;
using CandB.Script.Gateway;
using CandB.Script.Gateway.Impl;
using KanKikuchi.AudioManager;
using LLMUnity;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CandB.Script.Application
{
    public class ApplicationLifeTimeScope : LifetimeScope
    {
#nullable disable
        [SerializeField] private LLM llm;
        [SerializeField] private LLMCharacter llmCharacter;
#nullable enable

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.LogFormat("ApplicationLifeTimeScope Configure");

            DontDestroyOnLoad(gameObject);

            RegisterGateway(builder);
            RegisterCore(builder);
            RegisterView(builder);
            RegisterController(builder);

            builder.RegisterEntryPoint<ApplicationEntryPoint>();
        }

        private static void RegisterController(IContainerBuilder builder)
        {
            builder.Register<ApplicationInitializer>(Lifetime.Singleton);
        }

        private void RegisterGateway(IContainerBuilder builder)
        {
            builder.Register<ILlmGateway, LlmGateway>(Lifetime.Singleton)
                .WithParameter(nameof(llm), llm)
                .WithParameter(nameof(llmCharacter), llmCharacter);

            builder.Register<IAudioGateway, AudioGateway>(Lifetime.Singleton)
                .WithParameter("bgmManager", BGMManager.Instance)
                .WithParameter("seManager", SEManager.Instance);
        }

        private static void RegisterCore(IContainerBuilder builder)
        {
            builder.Register<SampleCoreService>(Lifetime.Singleton);

            // service
            builder.Register<StageService>(Lifetime.Singleton);
            builder.Register<EnvironmentService>(Lifetime.Singleton);
            builder.Register<BuddyActionService>(Lifetime.Singleton);
            builder.Register<UserPromptService>(Lifetime.Singleton);
        }

        private static void RegisterView(IContainerBuilder builder)
        {
        }
    }
}