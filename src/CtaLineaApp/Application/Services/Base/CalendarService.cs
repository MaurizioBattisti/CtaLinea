using CtaLinea.Model.Base;

namespace CtaLineaApp.Application.Services.Base
{
    public class CalendarService 
        : ICalendarService
    {
        private readonly IList<Calendar> _list;

        public CalendarService ()
        {
            _list = new List<Calendar>()
            {
                new Calendar() {CalendarId = 1, CalendarName = "Annuale" },
                new Calendar() {CalendarId = 2, CalendarName = "Annuale feriale" },
                new Calendar() {CalendarId = 3, CalendarName = "Annuale festivo" },
                new Calendar() {CalendarId = 4, CalendarName = "Invernale" },
                new Calendar() {CalendarId = 5, CalendarName = "Estivo" },
                new Calendar() {CalendarId = 6, CalendarName = "Invernale festivo" },
                new Calendar() {CalendarId = 7, CalendarName = "Invernale feriale" },
                new Calendar() {CalendarId = 8, CalendarName = "Scolastico" },
                new Calendar() {CalendarId = 9, CalendarName = "Non scolastico" },
                new Calendar() {CalendarId = 10, CalendarName = "pre-festivo" },
                new Calendar() {CalendarId = 11, CalendarName = "post-festivo" },
                new Calendar() {CalendarId = 12, CalendarName = "Estivo feriale" },
                new Calendar() {CalendarId = 13, CalendarName = "Estivo Festivo" }
            };
        }

        public async Task<IList<Calendar>> GetCalendarListAsync()
        {
            await Task.CompletedTask;

            return _list;
        }
    }
}

