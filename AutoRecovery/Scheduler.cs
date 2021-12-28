using System;
using System.Threading.Tasks;

namespace Abraham.Threading
{
	public class Scheduler
	{
		#region ------------- Types and constants -------------------------------------------------
		public delegate Task ActionHandler();
		public delegate void ExceptionHandler(Exception ex);
		public delegate void SchedulerEndedHandler();
		#endregion



		#region ------------- Properties ----------------------------------------------------------
		public ActionHandler Action 
		{
			get { return _action; }
			set { if (value != null) _action = value; else _action = NullObject; }
		}
		private ActionHandler _action;

		public ExceptionHandler OnScheduleException
		{
			get { return _onScheduleException; }
			set { if (value != null) _onScheduleException = value; else _onScheduleException = ExceptionNullObject; }
		}
		private ExceptionHandler _onScheduleException;

		public SchedulerEndedHandler OnSchedulerEnded
		{
			get { return _onSchedulerEnded; }
			set { if (value != null) _onSchedulerEnded = value; else _onSchedulerEnded = SchedulerEndedNullObject; }
		}

		private SchedulerEndedHandler _onSchedulerEnded;

		public bool IsRunning => _running;
		#endregion



		#region ------------- Fields --------------------------------------------------------------
		private ThreadExtensions _thread;
		private uint             _timerPreset;
		private bool             _running;
		private bool _abortWait;
		#endregion



		#region ------------- Init ----------------------------------------------------------------
		public Scheduler()
		{
			Action = null;
			OnScheduleException = null;
			OnSchedulerEnded = null;
		}
		#endregion



		#region ------------- Methods -------------------------------------------------------------
		public void SetSimpleSchedule(uint seconds)
		{
			_timerPreset = seconds;
		}

		public void Start()
		{
			_running = false;
			_thread = new ThreadExtensions(ThreadProc, "InternetConnectionCheckerThread");
			_thread.thread.Start();
		}

		public void Stop()
        {
            if (_thread != null)
                _thread.SendStopSignalAndWait();
        }

        public void Restart()
        {
			_abortWait = true;
        }

        public void Wait(uint secondsToWait)
        {
			_abortWait = false;
			var endOfWaitTime = DateTime.Now.AddSeconds(secondsToWait);
            
			while (_thread.Run && !_abortWait && endOfWaitTime > DateTime.Now)
            {
				_thread.Sleep(100);
            }
        }
		#endregion



		#region ------------- Implementation ------------------------------------------------------
        private void ThreadProc()
        {
            try
            {
				_running = true;
                while (_thread.Run)
                {
					_action().GetAwaiter().GetResult();
					Wait(_timerPreset);
                }
            }
			catch (Exception ex)
			{
				_onScheduleException(ex);
			}
			finally
			{
				_running = false;
				_onSchedulerEnded();
			}
        }

		private async Task NullObject()
		{
		}

		private void ExceptionNullObject(Exception ex)
		{
		}

		private void SchedulerEndedNullObject()
		{
		}
		#endregion
	}
}
