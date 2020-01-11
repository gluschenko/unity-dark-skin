using System;
using System.Collections.Generic;
using System.Threading;

// TODO: Migrate to Tasks

namespace UnityDarkSkin.App.Core
{
    // Singleton thread manager
    public static class ThreadHelper
    {
        private static Thread _thread;
        private static readonly Queue<Action> Actions = new Queue<Action>();

        private static void Init()
        {
            if (_thread == null)
            {
                _thread = new Thread(ThreadProc) {IsBackground = true};
                _thread.Start();
            }
        }

        private static void ThreadProc()
        {
            while (true)
            {
                if (Actions.Count > 0)
                {
                    Actions.Dequeue()?.Invoke();
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
                Actions.Enqueue(action);
            }
        }
    }
}