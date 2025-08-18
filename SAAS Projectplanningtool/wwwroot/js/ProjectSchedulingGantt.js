
document.addEventListener('click', e => {
    if (e.target.classList.contains('caret')) {
        e.stopPropagation();                    // keine Zeilen-Clicks auslösen
        toggleCollapse(e.target.dataset.id);
    }
});

const collapsed = new Map();

const projectData = {
    project: @Html.Raw(System.Text.Json.JsonSerializer.Serialize(new {
        Id = Model.Project?.ProjectId,
        Name = Model.Project?.ProjectName,
        StartDate = Model.Project?.StartDate?.ToString("yyyy-MM-dd"),
        EndDate = Model.Project?.EndDate?.ToString("yyyy-MM-dd"),
        Sections = Model.Project?.ProjectSections?.Where(s => s.ParentSectionId == null).Select(s => new {
            Id = s.ProjectSectionId,
            Name = s.ProjectSectionName,
            Type = "section",
            SubSections = s.SubSections?.Select(sub => new {
                Id = sub.ProjectSectionId,
                Name = sub.ProjectSectionName,
                Type = "subsection",
                Tasks = sub.ProjectTasks?.Select(t => new {
                    Id = t.ProjectTaskId,
                    Name = t.ProjectTaskName,
                    StartDate = t.StartDate?.ToString("yyyy-MM-dd"),
                    EndDate = t.EndDate?.ToString("yyyy-MM-dd"),
                    Type = "task"
                })
            }),
            Tasks = s.ProjectTasks?.Select(t => new {
                Id = t.ProjectTaskId,
                Name = t.ProjectTaskName,
                StartDate = t.StartDate?.ToString("yyyy-MM-dd"),
                EndDate = t.EndDate?.ToString("yyyy-MM-dd"),
                Type = "task"
            })
        })
    }))
};

let currentTimeScale = 'week';
let ganttStartDate = new Date();
let ganttEndDate = new Date();

document.addEventListener('DOMContentLoaded', function () {
    initializeGanttChart();
});

function preloadCollapse() {
    function walk(node) {
        if (node.Type !== 'task') {                  // nur Section/Subsection
            collapsed.set(node.Id, true);         // ← als eingeklappt merken
            if (node.SubSections) node.SubSections.forEach(walk);
        }
    }
    projectData.project.Sections?.forEach(walk);
}

function initializeGanttChart() {
    calculateDateRange();
    preloadCollapse();
    renderTaskList();
    renderTimeline();
}

function calculateDateRange() {
    const today = new Date();
    ganttStartDate = new Date(today.getFullYear(), today.getMonth(), 1);
    ganttEndDate = new Date(today.getFullYear(), today.getMonth() + 6, 0);

    // Adjust based on project dates if available
    if (projectData.project.StartDate) {
        const projectStart = new Date(projectData.project.StartDate);
        if (projectStart < ganttStartDate) {
            ganttStartDate = new Date(projectStart.getFullYear(), projectStart.getMonth(), 1);
        }
    }

    if (projectData.project.EndDate) {
        const projectEnd = new Date(projectData.project.EndDate);
        if (projectEnd > ganttEndDate) {
            ganttEndDate = new Date(projectEnd.getFullYear(), projectEnd.getMonth() + 1, 0);
        }
    }
}

function isCollapsed(id) {
    return collapsed.has(id) && collapsed.get(id);   // true | false
}


function toggleCollapse(id) {
    collapsed.set(id, !isCollapsed(id));
    renderTaskList();
    renderTimeline();
}

/* Ermittelt frühesten Start / spätestes Ende unterhalb eines Knotens */
function getDateRange(node) {
    let min = node.StartDate ? new Date(node.StartDate) : null;
    let max = node.EndDate ? new Date(node.EndDate) : null;

    if (node.SubSections) {
        node.SubSections.forEach(sub => {
            const [s, e] = getDateRange(sub);
            if (s && (!min || s < min)) min = s;
            if (e && (!max || e > max)) max = e;
        });
    }
    if (node.Tasks) {
        node.Tasks.forEach(t => {
            if (t.StartDate) {
                const d = new Date(t.StartDate);
                if (!min || d < min) min = d;
            }
            if (t.EndDate) {
                const d = new Date(t.EndDate);
                if (!max || d > max) max = d;
            }
        });
    }
    return [min, max];
}

function renderTaskList() {
    // const body = document.getElementById('task-list-body');
    // body.innerHTML = '';

    // if (!projectData.project.Sections) return;

    // projectData.project.Sections.forEach(sec => walk(sec, 0));

    const body = document.getElementById('task-list-body');
    body.innerHTML = '';

    if (!projectData.project.Sections) return;
    projectData.project.Sections.forEach(sec => walk(sec, 0));

    /* Footer-Adder ganz unten */
    const footer = document.createElement('div');
    footer.className = 'task-row adder-row';
    footer.dataset.level = 0;
    footer.innerHTML = `
             <div class="task-name">
                <button class="btn btn-sm btn-outline-primary"
                        onclick="showAddSectionModal()">
                   <i class="fas fa-plus"></i> Neuer Abschnitt
                </button>
             </div>`;
    body.appendChild(footer);

    // ---------- rekursive Hilfsfunktion ----------
    function walk(node, depth) {
        renderTaskRow(node, body, depth);

        if (node.Type === 'task' || isCollapsed(node.Id)) return;

        // Inline-Adder hinter jedem Nicht-Task
        if (node.Type !== 'task') {
            const adder = document.createElement('div');
            adder.className = 'task-row adder-row';
            adder.style.setProperty('--lvl', depth);      // ⬅︎  ebenfalls!
            adder.innerHTML = `
                   <div class="task-name">
                      <button class="btn btn-sm btn-outline-primary"
                              onclick="showAddItemModal('${node.Id}','${node.Type}')">
                          <i class="fas fa-plus"></i>
                      </button>
                   </div>`;
            body.appendChild(adder);
        }

        // ---------- rekursiv weiterlaufen ----------
        if (node.SubSections) {
            node.SubSections.forEach(sub => walk(sub, depth + 1));
        }
        if (node.Tasks) {
            node.Tasks.forEach(t => walk(t, depth + 1));
        }
    }
}

function renderTaskRow(item, container, depth) {
    /* Grundgerüst */
    const row = document.createElement('div');
    row.className = `task-row ${item.Type}`;
    row.style.setProperty('--lvl', depth);
    row.dataset.id = item.Id;
    row.dataset.type = item.Type;

    /* ⬇︎  NEU: Datenbereich ermitteln  --------------------------- */
    let start = item.StartDate;
    let end = item.EndDate;

    if (item.Type !== 'task') {                   // Section / Subsection
        const [min, max] = getDateRange(item);  // rekursive Suche
        if (min && max) {
            start = min.toISOString().slice(0, 10);   // "yyyy-mm-dd"
            end = max.toISOString().slice(0, 10);
        }
    }

    const duration = (start && end) ? calculateDuration(start, end) : '-';
    const startStr = start ? formatDate(start) : '-';
    const endStr = end ? formatDate(end) : '-';

    /* Pfeil & Aktionen bleiben wie zuvor ------------------------ */
    const hasChildren =
        (item.SubSections && item.SubSections.length) ||
        (item.Tasks && item.Tasks.length);

    const folded = isCollapsed(item.Id);
    const caret = hasChildren
        ? `<i class="fas fa-caret-down caret ${folded ? 'collapsed' : ''}"
                  data-id="${item.Id}"></i>`
        : '';

    const isTask = item.Type === 'task';

    row.innerHTML = `
            <div class="task-name">${caret}${item.Name}</div>
            <div class="task-duration">${duration}</div>
            <div class="task-start">${startStr}</div>
            <div class="task-end">${endStr}</div>
            <div class="task-actions">
                ${isTask ? `
                    <button class="btn btn-sm btn-outline-primary me-1"
                            onclick="editTask('${item.Id}', '${item.Name}',
                                              '${item.StartDate || ''}', '${item.EndDate || ''}')">
                        <i class="fas fa-edit"></i>
                    </button>` : ''}
                <button class="btn btn-sm btn-outline-danger"
                        onclick="deleteItem('${item.Id}', '${item.Type}', '${item.Name}')">
                    <i class="fas fa-trash"></i>
                </button>
            </div>`;
    container.appendChild(row);
}

function renderTimeline() {
    const timelineHeader = document.getElementById('timeline-header');
    const timelineBody = document.getElementById('timeline-body');

    timelineHeader.innerHTML = '';
    timelineBody.innerHTML = '';

    //renderTimelineHeader(timelineHeader);
    //renderTimelineBars(timelineBody);
    renderTimelineHeader(timelineHeader);
    renderVerticalGridLines(
        timelineBody,
        Math.ceil((ganttEndDate - ganttStartDate) / 86400000),
        40                       // gleicher Wert wie dayWidth
    );
    renderTimelineBars(timelineBody);
}

function renderTimelineHeader(container) {
    const totalDays = Math.ceil((ganttEndDate - ganttStartDate) / (1000 * 60 * 60 * 24));
    const dayWidth = 48; // pixels per day

    for (let i = 0; i <= totalDays; i += (currentTimeScale === 'day' ? 1 : currentTimeScale === 'week' ? 7 : 30)) {
        const date = new Date(ganttStartDate);
        date.setDate(date.getDate() + i);

        const headerDiv = document.createElement('div');
        headerDiv.className = 'timeline-header-day';
        headerDiv.style.left = (i * dayWidth) + 'px';
        headerDiv.style.width = (dayWidth * (currentTimeScale === 'day' ? 1 : currentTimeScale === 'week' ? 7 : 30)) + 'px';
        headerDiv.textContent = formatHeaderDate(date);

        container.appendChild(headerDiv);
    }

    container.style.width = (totalDays * dayWidth) + 'px';
}

function renderTimelineBars(container) {
    const totalDays = Math.ceil((ganttEndDate - ganttStartDate) / 86400000);
    const dayWidth = 48;
    let row = 0;
    container.style.width = (totalDays * dayWidth) + 'px';
    container.style.height = (calculateTotalRows() * 40) + 'px';

    if (!projectData.project.Sections) return;
    projectData.project.Sections.forEach(sec => walk(sec, 0));

    /* ---------- rekursiv ---------- */
    function walk(node, depth) {
        // const isCollapsed=collapsed.get(node.Id);
        const fold = isCollapsed(node.Id)

        if (node.Type !== 'task' && fold) {
            const [min, max] = getDateRange(node);
            if (min && max) drawBar(node, min, max, row, dayWidth, 'fold');
            row++;                              // eigene Zeile
            return;                             // Kinder überspringen
        }

        if (node.StartDate && node.EndDate)
            drawBar(node, new Date(node.StartDate), new Date(node.EndDate), row, dayWidth, node.Type);
        row++;                                  // eigene Zeile

        if (node.Type !== 'task') {
            /* Adder-Row überspringen => +1 */
            row++;
            if (node.SubSections) node.SubSections.forEach(sub => walk(sub, depth + 1));
            if (node.Tasks) node.Tasks.forEach(t => walk(t, depth + 1));
        }
    }

    function drawBar(item, startDate, endDate, r, dayWidth, extraClass = '') {
        const startOff = Math.max(0, Math.ceil((startDate - ganttStartDate) / 86400000));
        const dur = Math.ceil((endDate - startDate) / 86400000) + 1;

        const bar = document.createElement('div');
        bar.className = `timeline-bar ${item.Type} ${extraClass}`;
        bar.style.left = (startOff * dayWidth) + 'px';
        bar.style.width = (dur * dayWidth - 4) + 'px';
        bar.style.top = (r * 40 + 10) + 'px';
        bar.textContent = item.Name;
        container.appendChild(bar);
    }
}


function renderTimelineBar(item, container, rowIndex, dayWidth) {
    if (!item.StartDate || !item.EndDate) return;

    const startDate = new Date(item.StartDate);
    const endDate = new Date(item.EndDate);

    const startOffset = Math.max(0, Math.ceil((startDate - ganttStartDate) / (1000 * 60 * 60 * 24)));
    const duration = Math.ceil((endDate - startDate) / (1000 * 60 * 60 * 24)) + 1;

    const bar = document.createElement('div');
    bar.className = `timeline-bar ${item.Type}`;
    bar.style.left = (startOffset * dayWidth) + 'px';
    bar.style.width = (duration * dayWidth - 4) + 'px';
    bar.style.top = (rowIndex * 40 + 10) + 'px'; // 10px offset to center in 40px row
    bar.textContent = item.Name;
    bar.title = `${item.Name} (${formatDate(item.StartDate)} - ${formatDate(item.EndDate)})`;

    container.appendChild(bar);
}
function renderVerticalGridLines(container, totalDays, dayWidth) {
    // Alte Linien entfernen (beim Neurendern)
    container.querySelectorAll('.timeline-grid-line').forEach(el => el.remove());

    // Schrittweite wie im Header
    const step = currentTimeScale === 'day' ? 1 :
        currentTimeScale === 'week' ? 7 : 30;

    for (let i = 0; i <= totalDays; i += step) {
        const line = document.createElement('div');
        line.className = 'timeline-grid-line';
        line.style.left = (i * dayWidth) + 'px';
        container.appendChild(line);
    }
}


function calculateTotalRows() {
    let rows = 1;                                // Footer
    if (!projectData.project.Sections) return rows;

    function walk(node) {
        rows++;                                // eigene Zeile
        if (node.Type === 'task') return;         // fertig

        if (collapsed.get(node.Id)) return;     // Kinder versteckt

        rows++;                                // Adder
        if (node.SubSections) node.SubSections.forEach(walk);
        if (node.Tasks) node.Tasks.forEach(walk);
    }
    projectData.project.Sections.forEach(walk);
    return rows;
}


function calculateDuration(startDate, endDate) {
    if (!startDate || !endDate) return '-';

    const start = new Date(startDate);
    const end = new Date(endDate);
    const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24)) + 1;

    return days + 'd';
}

function formatDate(dateString) {
    if (!dateString) return '-';

    const date = new Date(dateString);
    return date.toLocaleDateString('de-DE');
}

function formatHeaderDate(date) {
    switch (currentTimeScale) {
        case 'day':
            return date.toLocaleDateString('de-DE', { day: '2-digit', month: '2-digit' });
        case 'week':
            return `KW ${getWeekNumber(date)}`;
        case 'month':
            return date.toLocaleDateString('de-DE', { month: 'short', year: '2-digit' });
        default:
            return date.toLocaleDateString('de-DE');
    }
}

function getWeekNumber(date) {
    const firstDayOfYear = new Date(date.getFullYear(), 0, 1);
    const pastDaysOfYear = (date - firstDayOfYear) / 86400000;
    return Math.ceil((pastDaysOfYear + firstDayOfYear.getDay() + 1) / 7);
}

function changeTimeScale(scale) {
    currentTimeScale = scale;

    // Update active button
    document.querySelectorAll('.btn-group button').forEach(btn => {
        btn.classList.remove('active');
    });
    event.target.classList.add('active');

    renderTimeline();
}

function toggleViewMode() {
    // Toggle between different view modes (could be expanded)
    alert('Ansichtsmodus-Funktion kann hier implementiert werden');
}

function zoomToFit() {
    calculateDateRange();
    renderTimeline();
}

/* ------------------------------
Globale Hilfs-Variablen
--------------------------------*/
let addItemParentId = null;
let addItemParentType = null;

/* Klick-Handler erst registrieren, wenn DOM fertig */
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('chooseTaskBtn').addEventListener('click', () => {
        bootstrap.Modal.getInstance(document.getElementById('addItemModal')).hide();
        showAddTaskModal(addItemParentId);        // existiert bereits
    });

    document.getElementById('chooseSubBtn').addEventListener('click', () => {
        bootstrap.Modal.getInstance(document.getElementById('addItemModal')).hide();
        showAddSubSectionModal(addItemParentId);  // existiert bereits
    });
});