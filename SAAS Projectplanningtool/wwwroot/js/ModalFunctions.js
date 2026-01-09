//window.showAddSubSectionModal = function (parentId) {
//    document.getElementById('parentSubSectionId').value = parentId;
//    new bootstrap.Modal(document.getElementById('addSubSectionModal')).show();
//}
//function showAddSectionModal() {
//         new bootstrap.Modal(document.getElementById('addSectionModal')).show();
//     }
//function showAddTaskModal(sectionId = null) {
//     //Section-Dropdown im Modal auf gewünschten Abschnitt setzen
//     if (sectionId){
//     const sel = document.getElementById('taskSectionSelect');
//     if (sel) sel.value = sectionId;
//     }

//     //Modal anzeigen
//     new bootstrap.Modal(document.getElementById('addTaskModal')).show();
//     }
//window.editTask = function (taskId, taskName, startDate, endDate) {
//    document.getElementById('editTaskId').value = taskId;
//    document.getElementById('editTaskName').value = taskName;
//    document.getElementById('editTaskStartDate').value = startDate;
//    document.getElementById('editTaskEndDate').value = endDate;

//    new bootstrap.Modal(document.getElementById('editTaskModal')).show();
//}

//window.deleteItem = function (itemId, itemType, itemName) {
//    document.getElementById('deleteConfirmText').textContent =
//        `Sind Sie sicher, dass Sie "${itemName}" löschen möchten?`;

//    const jsonData = {
//        Type: itemType === 'task' ? 'ProjectTask' : 'ProjectSection',
//        Data: { itemId, itemName }
//    };

//    document.getElementById('deleteObjectJson').value = JSON.stringify(jsonData);
//    new bootstrap.Modal(document.getElementById('deleteModal')).show();
//}

//window.showAddItemModal = function (parentId, parentType) {
//    addItemParentId = parentId;
//    addItemParentType = parentType;

//    const subBtn = document.getElementById('chooseSubBtn');
//    subBtn.disabled = (parentType !== 'section');

//    new bootstrap.Modal(document.getElementById('addItemModal')).show();
//}
///* ------------------------------
//Globale Hilfs-Variablen
//--------------------------------*/
//let addItemParentId = null;
//let addItemParentType = null;

///* Klick-Handler erst registrieren, wenn DOM fertig */
//document.addEventListener('DOMContentLoaded', () => {
//    document.getElementById('chooseTaskBtn').addEventListener('click', () => {
//        bootstrap.Modal.getInstance(document.getElementById('addItemModal')).hide();
//        showAddTaskModal(addItemParentId);        // existiert bereits
//    });

//    document.getElementById('chooseSubBtn').addEventListener('click', () => {
//        bootstrap.Modal.getInstance(document.getElementById('addItemModal')).hide();
//        showAddSubSectionModal(addItemParentId);  // existiert bereits
//    });
//});
//console.log("ModalFunctions.js loaded successfully");


// ModalFunctions.js -- safe to include multiple times
(function () {
    if (window.__ModalFunctionsInstalled__) {
        // already initialized -> nothing to do
        return;
    }
    window.__ModalFunctionsInstalled__ = true;

    // ensure namespace object exists
    window.ModalFunctions = window.ModalFunctions || {};

    (function (ns) {
        // private state (closure-scoped; not global)
        var _addItemParentId = null;
        var _addItemParentType = null;
        var _handlersRegistered = false;

        function el(id) { return document.getElementById(id); }

        ns.showAddSubSectionModal = function (parentId) {
            var p = el('parentSubSectionId');
            if (p) p.value = parentId || '';
            var m = el('addSubSectionModal');
            if (m) new bootstrap.Modal(m).show();
        };

        ns.showAddSectionModal = function () {
            var m = el('addSectionModal');
            if (m) new bootstrap.Modal(m).show();
        };

        ns.showAddTaskModal = function (sectionId) {
            if (sectionId) {
                var sel = el('taskSectionSelect');
                if (sel) sel.value = sectionId;
            }
            var m = el('addTaskModal');
            if (m) new bootstrap.Modal(m).show();
        };

        ns.showAddMileStoneModal = function () {
            //if (sectionId) {
            //    var sel = el('MilestoneSectionSelect');
            //    if (sel) sel.value = sectionId;
            //}
            var m = el('addMilestoneModal');
            if (m) new bootstrap.Modal(m).show();
        }

        ns.showAddTaskCatalogModal = function (projectId) {
            if (projectId) {
                var sel = el('projectId');
                if (sel) sel.value = projectId;
            }
            var m = el('addTaskCatalogModal');
            if (m) new bootstrap.Modal(m).show();
        };
        ns.editTask = function (taskId, taskName, startDate, endDate) {
            var idEl = el('editTaskId'); if (idEl) idEl.value = taskId || '';
            var nameEl = el('editTaskName'); if (nameEl) nameEl.value = taskName || '';
            var sEl = el('editTaskStartDate'); if (sEl) sEl.value = startDate || '';
            var eEl = el('editTaskEndDate'); if (eEl) eEl.value = endDate || '';
            //var tcEl = el('editTaskIsTaskCatalogEntry'); if (tcEl) tcEl.value = isTaskCatalogEntry || '';
            //var scEl = el('editTaskIsScheduleEntry'); if (scEl) scEl.value = isScheduleEntry || '';


            var m = el('editTaskModal'); if (m) new bootstrap.Modal(m).show();
        };

        ns.deleteItem = function (itemId, itemType, itemName) {
            var t = el('deleteConfirmText');
            if (t) t.textContent = 'Sind Sie sicher, dass Sie "' + (itemName || '') + '" löschen möchten?';
            var json = {
                Type: itemType === 'task' ? 'ProjectTask' : itemType === 'taskCatalog' ? 'TaskCatalogTask' : itemType === 'milestone' ? 'ProjectSectionMilestone' : 'ProjectSection',
                Data: { itemId: itemId, itemName: itemName }
            };
            var j = el('deleteObjectJson'); if (j) j.value = JSON.stringify(json);
            var m = el('deleteModal'); if (m) new bootstrap.Modal(m).show();
        };

        ns.showAddItemModal = function (parentId, parentType) {
            _addItemParentId = parentId;
            _addItemParentType = parentType;
            var subBtn = el('chooseSubBtn');
            if (subBtn) subBtn.disabled = (parentType !== 'section');
            var m = el('addItemModal'); if (m) new bootstrap.Modal(m).show();
        };

        function registerHandlers() {
            if (_handlersRegistered) return;
            _handlersRegistered = true;

            document.addEventListener('DOMContentLoaded', function () {
                var chooseTaskBtn = el('chooseTaskBtn');
                if (chooseTaskBtn) {
                    chooseTaskBtn.addEventListener('click', function () {
                        var m = el('addItemModal');
                        if (m) {
                            var inst = bootstrap.Modal.getInstance(m);
                            if (inst && inst.hide) inst.hide();
                        }
                        ns.showAddTaskModal(_addItemParentId);
                    });
                }

                var chooseSubBtn = el('chooseSubBtn');
                if (chooseSubBtn) {
                    chooseSubBtn.addEventListener('click', function () {
                        var m = el('addItemModal');
                        if (m) {
                            var inst = bootstrap.Modal.getInstance(m);
                            if (inst && inst.hide) inst.hide();
                        }
                        ns.showAddSubSectionModal(_addItemParentId);
                    });
                }
            });
        }

        registerHandlers();

        // Backwards compatibility: only create global aliases if they don't exist yet
        try {
            if (typeof window.showAddTaskModal === 'undefined') window.showAddTaskModal = ns.showAddTaskModal;
            if (typeof window.showAddSectionModal === 'undefined') window.showAddSectionModal = ns.showAddSectionModal;
            if (typeof window.showAddSubSectionModal === 'undefined') window.showAddSubSectionModal = ns.showAddSubSectionModal;
            if (typeof window.editTask === 'undefined') window.editTask = ns.editTask;
            if (typeof window.deleteItem === 'undefined') window.deleteItem = ns.deleteItem;
            if (typeof window.showAddItemModal === 'undefined') window.showAddItemModal = ns.showAddItemModal;
            if (typeof window.showAddTaskCatalogModal === 'undefined') window.showAddTaskCatalogModal = ns.showAddTaskCatalogModal;
            if (typeof window.showAddMileStoneModal === 'undefined') window.showAddMileStoneModal = ns.showAddMileStoneModal;
        } catch (e) { /* ignore */ }

        // optional debug helper
        ns._debug = function () { return { addItemParentId: _addItemParentId, addItemParentType: _addItemParentType }; };

    })(window.ModalFunctions);

    console.log('ModalFunctions initialized');
})();
