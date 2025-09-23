using CandB.Script.Core;

namespace CandB.Script.Gateway
{
    public interface IAudioGateway
    {
        public void Play(SoundEffectId soundId);
        public void Stop(SoundEffectId soundId);
        public void PlayBGM(BgmId bgmId);
        public void StopBGM();
    }
}