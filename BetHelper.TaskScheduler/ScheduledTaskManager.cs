using BetHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskScheduler
{
    /// <summary>
    /// The TaskManager for time-based operations
    /// </summary>
    /// 
    
    public class ScheduledTaskManager : IDisposable
    {
        /// <summary>
        /// Fires at every hour at given minute
        /// </summary>
        /// <param name="executeOnTime"></param>
        /// <param name="absoluteMinute"></param>
        public void RegisterAbsoluteTask(Action executeOnTime, int absoluteMinute, bool isAsync = false)
        {
            Tasks.Add(new ScheduledTask { Body = executeOnTime, AbsoluteMinute = absoluteMinute, IsAsync = isAsync });
        }

        /// <summary>
        /// Firest every day at given hour and minute
        /// </summary>
        /// <param name="executeOnTime"></param>
        /// <param name="absoluteMinute"></param>
        /// <param name="absoluteHour"></param>
        public ScheduledTask RegisterAbsoluteTask(Action executeOnTime, int absoluteHour, int absoluteMinute, bool isAsync = false,int absoluteSeconds=0)
        {
            var currentTask = new ScheduledTask { Body = executeOnTime, AbsoluteMinute = absoluteMinute, AbsoluteHour = absoluteHour, IsAsync = isAsync ,AbsoluteSeconds=absoluteSeconds};
            Tasks.Add(currentTask);
            return currentTask;
        }    

        /// <summary>
        /// Fires after given minutes from last execution
        /// </summary>
        /// <param name="executeOnTime"></param>
        /// <param name="absoluteMinute"></param>
        public ScheduledTask RegisterSlidingTask(Action executeOnTime, int slidingMinute, bool isAsync = false,int startHour=0,int startMinute=0)
        {
           
            var curtask = new ScheduledTask { Body = executeOnTime, SlidingMinute = slidingMinute, IsAsync = isAsync, StartingSlideTime 
                = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,startHour,startMinute,DateTime.Now.Second)};
            Tasks.Add(curtask);
            return curtask;
        }

        /// <summary>
        /// Fires after given hours + minutes from last execution
        /// </summary>
        /// <param name="executeOnTime"></param>
        /// <param name="absoluteMinute"></param>
        /// <param name="absoluteHour"></param>     
        public void DeleteTask(ScheduledTask task)
        {
            Tasks.Remove(task);
        }
        public List< ScheduledTask> Tasks = new List< ScheduledTask>();       
        public class ScheduledTask
        {
            public Action Body { get; set; }
            
             public int? AbsoluteMinute { get; set; }
            public int? AbsoluteHour { get; set; }
            public int? AbsoluteSeconds { get; set; }
            public int? SlidingMinute { get; set; }
            public int? SlidingHour { get; set; }
            public bool IsAsync { get; set; }
            public DateTime StartingSlideTime { get; set; }
            private bool IsAbsolute { get { return AbsoluteMinute.HasValue; } }
            private bool IsSliding { get { return SlidingMinute.HasValue; } }          
            private Func<DateTime> Watch;

            public ScheduledTask(Func<DateTime> currentTimeWatch = null)
            {
                Watch = currentTimeWatch ?? new Func<DateTime>(() => DateTime.Now);
            }

            private DateTime last;
            
            public bool IsTime()
            {
                var now = Normalize(Watch());
                if (last == default(DateTime))
                    last = now;

                bool risp = false;

                if (IsAbsolute)
                {
                    var absoluteTime = new DateTime(now.Year, now.Month, now.Day, AbsoluteHour.HasValue ? AbsoluteHour.Value : now.Hour, AbsoluteMinute.Value,AbsoluteSeconds.Value);
                    risp = last < absoluteTime && absoluteTime <= now;
                }
                else if (IsSliding)
                {
                    //i will change it for seconds
                    //that is original code
                   // var minutes = (SlidingHour.HasValue ? SlidingHour.Value * 60 : 0) + SlidingMinute.Value;
                    //.                   risp = (now - last).TotalMinutes >= minutes;
                   // if (StartingSlideTime >= DateTime.Now)
                   // {
                        var seconds = SlidingMinute.Value;
                       
                        risp = (now - last).TotalSeconds >= seconds;
                   // }
                }

                if (risp)
                    last = now;

                return risp;
            }

            private DateTime Normalize(DateTime arg)
            {
                return new DateTime(arg.Year, arg.Month, arg.Day, arg.Hour, arg.Minute, arg.Second);
            }
        }
           
        private System.Timers.Timer Timer;
        private Func<DateTime> Watch;

        public ScheduledTaskManager(Func<DateTime> currentTimeWatch = null)
        {
            Watch = currentTimeWatch ?? new Func<DateTime>(() => DateTime.Now);
            Timer = new System.Timers.Timer(1000);
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            Timer.Start();
        }

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var t in Tasks)              
                if (t.IsTime())
                {                   
                    Console.WriteLine("it is time");
                        if (t.IsAsync)
                        { (t as ScheduledTask).Body.BeginInvoke(null, null); }
                        else
                        { (t as ScheduledTask).Body.Invoke(); }     
                }
        }

        public void Dispose()
        {
            Timer.Stop();
            Timer.Dispose();
        }
    }
}


