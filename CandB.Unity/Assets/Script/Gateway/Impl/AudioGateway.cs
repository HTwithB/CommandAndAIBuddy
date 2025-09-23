using CandB.Script.Core;
using KanKikuchi.AudioManager;

namespace CandB.Script.Gateway.Impl
{
    public class AudioGateway : IAudioGateway
    {
        // dependency
        private readonly BGMManager _bgmManager;
        private readonly SEManager _seManager;

        // constructor
        public AudioGateway(
            BGMManager bgmManager,
            SEManager seManager
        )
        {
            _bgmManager = bgmManager;
            _seManager = seManager;
        }


        public void Play(SoundEffectId id)
        {
            var path = GetAudioPath(id);
            _seManager.Play(path);
        }

        public void Stop(SoundEffectId id)
        {
            var path = GetAudioPath(id);
            _seManager.Stop(path);
        }

        public void PlayBGM(BgmId bgmId)
        {
            var path = GetAudioPath(bgmId);
            _bgmManager.Play(path);
        }

        public void StopBGM()
        {
            _bgmManager.Stop();
        }

        private static string GetAudioPath(SoundEffectId id) => id switch
        {
            SoundEffectId.Unspecified => "",
            SoundEffectId.Ok => SEPath.OK,
            SoundEffectId.Error => SEPath.ERROR,
            SoundEffectId.Success => SEPath.SUCCESS,
            SoundEffectId.UnlockKey => SEPath.UNLOCKKEY,
            SoundEffectId.OpenCarDoor => SEPath.OPENCAR,
            SoundEffectId.CloseCarDoor => SEPath.CLOSECAR,
            SoundEffectId.NotOpenDoor => SEPath.NOTOPENDOOR,
            SoundEffectId.KnockDoor => SEPath.KNOCKDOOR,
            SoundEffectId.PickUpItem => SEPath.PICKUP,
            SoundEffectId.PutDownItem => SEPath.PUTDOWN,
            SoundEffectId.Paper => SEPath.PAPER,
            SoundEffectId.Text => SEPath.TEXT,
            SoundEffectId.OpenShutter => "open_shutter",
            _ => "",
        };

        private static string GetAudioPath(BgmId id) => id switch
        {
            BgmId.Unspecified => "",
            BgmId.Default => BGMPath.BGM01,
            BgmId.Secondary => BGMPath.BGM02,
            _ => "",
        };
    }
}