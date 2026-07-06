using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_BLL.ViewModels.SessionViewModels
{
    public class SessionViewModel
        {
            public int Id { get; set; }
            public string CategoryName { get; set; } = null!;
            public string Description { get; set; } = null!;
            public string TrainerName { get; set; } = null!;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Capacity { get; set; }
            public int AvailableSlots { get; set; }

            #region Computed - Properties
            public string DateDisplay => $"{StartDate:MMM dd, yyyy}";
            public string TimeRangeDisplay => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";
            public TimeSpan Duration => EndDate - StartDate;

            // Fix: show days when duration is more than 24 hours
public string DurationDisplay
{
    get
    {
        var d = Duration;
        if (d.TotalDays >= 1)
            return $"{(int)d.TotalDays} day(s) {d.Hours} hr {d.Minutes} min";
        else if (d.TotalHours >= 1)
            return $"{d.Hours} hr {d.Minutes} min";
        else
            return $"{d.Minutes} min";
    }
}

            public string Status
            {
                get
                {
                if (StartDate > DateTime.Now)
                {
                    return "Upcoming";
                }
                else if (StartDate <= DateTime.Now && EndDate > DateTime.Now)
                {
                    return "Ongoing";
                }
                else
                {
                    return "Completed";
                }
                }
            }
            #endregion
        }
}
