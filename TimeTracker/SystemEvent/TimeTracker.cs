using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using Microsoft.WindowsAPICodePack.Net;

namespace SystemEvent
{
    public class TimeTracker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TimeTracker));
        private static DateTime _startTime;
        private static int _totalActiveMinutes = 0;
        private static string _connectionString;
        private static ManualResetEventSlim _screenSaverOn = new ManualResetEventSlim();
        private static ManualResetEventSlim _screenSaverOff = new ManualResetEventSlim();
        private static OutlookDetails _outlookDetails = new OutlookDetails();
        private static List<int> _activeMinutes = new List<int>();

        static TimeTracker()
        {
            log.Debug("Time Tracker Initialized");
            _startTime = DateTime.Now;
            _connectionString = ConfigurationManager.ConnectionStrings["time_tracker"].ConnectionString;

            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnded += SystemEvents_SessionEnded;
            PowerManager.IsMonitorOnChanged += PowerManager_IsMonitorOnChanged;
            PowerManager.PowerSourceChanged += PowerManager_PowerSourceChanged;

            Thread threadOn = new Thread(CheckIfScreenSaverIsOn) {IsBackground = true};
            Thread threadOff = new Thread(CheckIfScreenSaverIsOff) {IsBackground = true};

            threadOn.Start();
            threadOff.Start();
        }

        static void PowerManager_PowerSourceChanged(object sender, EventArgs e)
        {
            if (PowerManager.PowerSource == PowerSource.Battery || PowerManager.PowerSource == PowerSource.Ups)
            {
                log.Info("PowerSource Changed to either Battery or Ups");
                CalculateElapsedTime();
            }

            else if (PowerManager.PowerSource == PowerSource.AC)
            {
                log.Info("PowerSource changed to AC");
                ResetTimer();
            }
        }

        /// <summary>
        /// Event handler for event which is fired when Monitor gets off/on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PowerManager_IsMonitorOnChanged(object sender, EventArgs e)
        {
            if (!PowerManager.IsMonitorOn)
            {
                log.Info("Monitor turned Off");
                if (!NativeMethods.IsScreensaverRunning())
                {
                    CalculateElapsedTime();
                }
            }

            if (PowerManager.IsMonitorOn)
            {
                log.Info("Monitor turned On");
                ResetTimer();
            }
        }

        /// <summary>
        /// Event handler for system shutting down event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            log.InfoFormat("Session Ending {0}", e.Reason);
            CalculateElapsedTime();
            if (_activeMinutes.Count > 0)
            {
                log.InfoFormat("Date : {0}, Total Active Minutes for this session {1}", DateTime.Now.Date,
                    _activeMinutes.Sum());
            }
        }

        /// <summary>
        /// Event handler for event related to power. Fired when power settings change like suspended or resume.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                log.InfoFormat("Power Mode {0}", e.Mode);
                CalculateElapsedTime();
            }

            if (e.Mode == PowerModes.Resume)
            {
                log.InfoFormat("Power Mode {0}", e.Mode);
                ResetTimer();
            }
        }

        /// <summary>
        /// Event handler fired when process is shutting down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            log.Info("Process Shutting Down");
            CalculateElapsedTime();
            if (_activeMinutes.Count > 0)
            {
                log.InfoFormat("Date : {0}, Total Active Minutes for this session {1}", DateTime.Now.Date,
                    _activeMinutes.Sum());
            }
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
            PowerManager.IsMonitorOnChanged -= PowerManager_IsMonitorOnChanged;
        }

        /// <summary>
        /// Event handler fired when system is locked or unlocked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock || e.Reason == SessionSwitchReason.SessionLogoff)
            {
                log.InfoFormat("Session {0}", e.Reason);
                CalculateElapsedTime();
            }

            else if (e.Reason == SessionSwitchReason.SessionUnlock || e.Reason == SessionSwitchReason.SessionLogon)
            {
                log.InfoFormat("Session {0}", e.Reason);
                ResetTimer();
            }
        }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        private static void ResetTimer()
        {
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// Method to calculate active minutes and update it in database
        /// </summary>
        public static void CalculateElapsedTime()
        {
            _totalActiveMinutes = Convert.ToInt32((DateTime.Now - _startTime).TotalMinutes);
            _activeMinutes.Add(_totalActiveMinutes);
            log.InfoFormat("Active Minutes {0}", _activeMinutes.Sum());

            //if (NetworkInterface.GetAllNetworkInterfaces().Any(x => x.Name.Contains(@"devid")))
            //{
                _totalActiveMinutes = _activeMinutes.Sum();
                UpdateDatabase();
                _activeMinutes = new List<int>();
            //}
        }

        private static void CheckIfScreenSaverIsOn()
        {
            log.Info("Thread1 - started running");
            while (true)
            {
                if (NativeMethods.IsScreensaverRunning())
                {
                    CalculateElapsedTime();
                    log.Info("Thread1 - Screen Saver Started");
                    _screenSaverOff.Set();
                    _screenSaverOn.Wait();
                    _screenSaverOn.Reset();
                }

                Thread.Sleep(2000);
            }
        }

        private static void CheckIfScreenSaverIsOff()
        {
            log.Info("Thread2 - Started Running");
            _screenSaverOff.Wait();

            while (true)
            {
                if (!NativeMethods.IsScreensaverRunning())
                {
                    log.Info("Thread2 - Screen Saver Stopped");
                    ResetTimer();
                    _screenSaverOn.Set();
                    _screenSaverOff.Reset();
                    _screenSaverOff.Wait();
                }

                Thread.Sleep(2000);
            }
        }

        private static void UpdateDatabase()
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                string templateSql = string.Empty;
                int meetingMinutes = Convert.ToInt32(_outlookDetails.OutlookMeetingMinutes);
                var todaysDate = DateTime.Now.Date.ToShortDateString();
                var isWorkingDay = (DateTime.Now.DayOfWeek == DayOfWeek.Saturday ||
                                    DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    ? 0
                    : 1;
                builder.Append(
                    @"Insert into @temp_TimeTracker (UserName, Date, MeetingMinutes, ActiveMinutes, IsWorkingDay) 
                        values ('" + Environment.UserName + "','" + todaysDate + "'," + meetingMinutes + 
                                   "," + _totalActiveMinutes + "," + isWorkingDay + ") \n");

                using (
                    var stream =
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("SystemEvent.Sql.TimeTrackerUpdate.sql"))
                using (var reader = new StreamReader(stream))
                {
                    templateSql = reader.ReadToEnd();
                    templateSql = templateSql.Replace(@"--__USER_PERMISISON_STATEMENTS__", builder.ToString());
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(templateSql, connection))
                {
                    connection.Open();
                    int recordsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.ToString());
            }
            finally
            {
                ResetTimer();
            }
        }
    }
}
