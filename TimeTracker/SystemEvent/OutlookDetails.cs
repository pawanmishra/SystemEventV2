using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Win32;

namespace SystemEvent
{
    public class OutlookDetails
    {
        /// <summary>
        /// For more : http://adriandev.blogspot.in/2008/01/listening-to-calendar-events-with.html
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(typeof(OutlookDetails));
        private Outlook.NameSpace nameSpace;
        private Outlook.Application outlookApplication;
        private Outlook.Folder fldCalander;
        private Outlook.Folder fldDeletedItems;
        private Outlook.Items items;
        private Dictionary<string, MeetingDetails> meetings; 

        public OutlookDetails()
        {
            meetings = new Dictionary<string, MeetingDetails>();
            InitializeMeetingDuration();
        }

        /// <summary>
        /// Calculate the total minutes spent in meeting.
        /// The if clause checks, if Outlook process is running or not. If not, it kick-starts it once again.
        /// This is required for triggring the item_add, item_change events.
        /// </summary>
        public double OutlookMeetingMinutes
        {
            get
            {
                if (Process.GetProcessesByName("OUTLOOK").Any() && outlookApplication != null)
                {
                    log.Info("OUTLOOK process running & application initialized");
                    return meetings.Sum(item => item.Value.Duration);
                }
                else if (Process.GetProcessesByName("OUTLOOK").Any() && outlookApplication == null)
                {
                    log.Info("OUTLOOK process running & application not initialized");
                    InitializeMeetingDuration();
                    return meetings.Sum(item => item.Value.Duration);
                }

                log.Info("OUTLOOK process not running");
                return meetings.Sum(item => item.Value.Duration);
            }
        }

        public Dictionary<string, MeetingDetails> Meetings
        {
            get { return meetings; }
        }

        /// <summary>
        /// Initializes the OutLook application and retrieves the meetings scheduled for current day.
        /// For logging into OutLook, we need the profile name. Default profile name is extracted from registry settings.
        /// </summary>
        private void InitializeMeetingDuration()
        {
            try
            {
                if (!Process.GetProcessesByName("OUTLOOK").Any())
                    return;
                outlookApplication = Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;
                nameSpace = outlookApplication.GetNamespace("mapi");
                var profile = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles", "DefaultProfile", null);
                nameSpace.Logon(profile.ToString(), Missing.Value, true, true);
                fldCalander =
                    nameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar) as Outlook.Folder;
                fldDeletedItems = nameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderDeletedItems) as Outlook.Folder;
                items = GetAppointmentsInRange(fldCalander);

                fldCalander.BeforeItemMove += fldCalander_BeforeItemMove;
                items.ItemAdd += Items_ItemAdd;
                items.ItemChange += Items_ItemChange;

                if (items != null)
                {
                    foreach (Outlook.AppointmentItem oAppt in items)
                    {
                        meetings.Add(oAppt.GlobalAppointmentID,
                            new MeetingDetails(oAppt.Start, oAppt.End, oAppt.Duration));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Event handler for meeting deleted event
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="MoveTo"></param>
        /// <param name="Cancel"></param>
        void fldCalander_BeforeItemMove(object Item, Outlook.MAPIFolder MoveTo, ref bool Cancel)
        {
            log.Info("Meeting deleting");
            Outlook.AppointmentItem item = (Outlook.AppointmentItem)Item;
            if (MoveTo.EntryID == fldDeletedItems.EntryID)
            {
                if (meetings.ContainsKey(item.GlobalAppointmentID))
                {
                    meetings.Remove(item.GlobalAppointmentID);
                }
            }
        }

        /// <summary>
        /// Event handler of item changed event. It is fired everytime a change is made in OutLook calander.
        /// </summary>
        /// <param name="Item"></param>
        void Items_ItemChange(object Item)
        {
            Outlook.AppointmentItem item = (Outlook.AppointmentItem)Item;
            if (meetings.ContainsKey(item.GlobalAppointmentID))
            {
                meetings[item.GlobalAppointmentID].Start = item.Start;
                meetings[item.GlobalAppointmentID].End = item.End;
                meetings[item.GlobalAppointmentID].Duration = item.Duration;
            }
        }

        /// <summary>
        /// Event handler for item add event, fired when ever a meeting gets added.
        /// </summary>
        /// <param name="Item"></param>
        void Items_ItemAdd(object Item)
        {
            Outlook.AppointmentItem item = (Outlook.AppointmentItem)Item;
            log.Info("Meeting Added");
            meetings.Add(item.GlobalAppointmentID, new MeetingDetails(item.Start, item.End, item.Duration));
        }


        /// <summary>
        /// Fetch amount of time spent in minutes for todays date
        /// </summary>
        /// <returns></returns>
        public static double GetMeetingDuration()
        {
            Outlook.NameSpace nameSpace;
            Outlook.Application outlookApplication;
            Outlook.Folder fldCalander;
            double meetingMinutes = 0;

            try
            {
                outlookApplication = new Outlook.Application();
                nameSpace = outlookApplication.GetNamespace("mapi");
                var profile = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Windows Messaging Subsystem\Profiles", "DefaultProfile", null);
                nameSpace.Logon(profile, Missing.Value, true, true);
                fldCalander =
                    nameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar) as Outlook.Folder;
                Outlook.Items items = GetAppointmentsInRange(fldCalander);
                
                if (items != null)
                {
                    foreach (Outlook.AppointmentItem oAppt in items)
                    {
                        meetingMinutes += (oAppt.End - oAppt.Start).TotalMinutes;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                fldCalander = null;
                nameSpace = null;
            }

            return meetingMinutes;
        }

        /// <summary>
        /// Get recurring appointments in date range.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>Outlook.Items</returns>
        private static Outlook.Items GetAppointmentsInRange(Outlook.Folder folder)
        {
            DateTime today = DateTime.Now;
            var startTime = new DateTime(today.Year, today.Month, today.Day, Int32.Parse(ConfigurationManager.AppSettings["StartOfTheDayHour"]), 0, 0);
            var endTime = new DateTime(today.Year, today.Month, today.Day, Int32.Parse(ConfigurationManager.AppSettings["EndOfTheDayHour"]), 0, 0);

            string filter = "[Start] >= '"
                + startTime.ToString("g")
                + "' AND [End] <= '"
                + endTime.ToString("g") + "'";
            Debug.WriteLine(filter);
            try
            {
                Outlook.Items calItems = folder.Items;
                calItems.IncludeRecurrences = true;
                calItems.Sort("[Start]", Type.Missing);
                Outlook.Items restrictItems = calItems.Restrict(filter);
                if (restrictItems.Count > 0)
                {
                    return restrictItems;
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }
    }

    public class MeetingDetails
    {
        public MeetingDetails(DateTime start, DateTime end, double duration)
        {
            this.Start = start;
            this.End = end;
            this.Duration = duration;
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double Duration { get; set; }
    }
}
