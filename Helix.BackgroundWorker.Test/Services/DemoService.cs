using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helix.BackgroundWorker.Test.Services
{
    class DemoService
    {
        public static bool Done { get; private set; }

        public async ValueTask MethodThatTakesTimeToExecute(int delay)
        {
            Done = false;
            await Task.Delay(delay);
            Done = true;
        }
    }
}
