using CandB.Script.Core;
using CandB.Script.Gateway;
using UnityEngine;

namespace CandB.Script.Controller
{
    public class ApplicationInitializer
    {
        private readonly IAudioGateway _audioGateway;
        private readonly EnvironmentService _environmentService;
        private readonly StageService _stageService;
        private readonly UserPromptService _userPromptService;

        public ApplicationInitializer(
            IAudioGateway audioGateway,
            UserPromptService userPromptService,
            StageService stageService,
            EnvironmentService environmentService
        )
        {
            _audioGateway = audioGateway;
            _userPromptService = userPromptService;
            _stageService = stageService;
            _environmentService = environmentService;
        }

        public void Execute()
        {
            Debug.LogFormat("ApplicationInitializer:Execute");

            _userPromptService.Setup();
            _stageService.Setup();
            _environmentService.Setup();

            _audioGateway.PlayBGM(BgmId.Default);
        }
    }
}