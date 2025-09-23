using CandB.Script.Core;

namespace CandB.Script.View
{
    public class SampleComponent : CandBComponent
    {
        private void Start()
        {
            var service = Resolve<SampleCoreService>();
            service.Setup();
        }
    }
}