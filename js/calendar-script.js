$(document).ready(function () {
    var date = new Date();
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();
    $(‘#calendar’).fullCalendar({
        editable: true,
        firstDay: 1,
        year: y,
        month: m,
        date: d,
        header: {
            left: ‘title’,
            right: ‘prev, next today, agendaDay, agendaWeek, month’
        },
        defaultView: ‘agendaWeek’,
        columnFormat: {
            month: ‘ddd’,
            week: ‘ddd d/ M’,
        day: ‘dddd d/ M’
},
    minTime: 7,
    maxTime: 22,
    editable: true,
    droppable: false,
    slotMinutes: 15,
    disableResizing: false,
    events: [
    {
        “title”: “Calendar Test”,
    “start”: “2014 - 06 - 01 18: 30: 00”,
    “end”: “2014 - 06 - 01 19: 00: 00”,
    “allDay”: false
},
    {
        “title”: “Calendar Test 1”,
    “start”: “2014 - 06 - 02 18: 30: 00”,
    “end”: “2014 - 06 - 02 19: 00: 00”,
    “allDay”: false
},
    {
        “title”: “Calendar Test 2”,
    “start”: “2014 - 06 - 03 18: 30: 00”,
    “end”: “2014 - 06 - 03 19: 00: 00”,
    “allDay”: false
}
] ,
});
});