using System;
using System.Collections.Generic;
using System.Threading;

namespace UnityDarkSkin.App.Core
{
    // Singleton thread manager
    public static class ThreadHelper
    {
        private static Thread thread;
        private static readonly Queue<Action> actions = new Queue<Action>();

        private static void Init()
        {
            if (thread == null)
            {
                thread = new Thread(ThreadProc) { IsBackground = true };
                thread.Start();
            }
        }

        private static void ThreadProc()
        {
            while (true)
            {
                if (actions.Count > 0)
                {
                    actions.Dequeue()?.Invoke();
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public static void Invoke(Action action)
        {
            if (action != null)
            {
                Init();
                actions.Enqueue(action);
            }
        }
    }
}
