window.showAddSubSectionModal = function (parentId) {
    document.getElementById('parentSubSectionId').value = parentId;
    new bootstrap.Modal(document.getElementById('addSubSectionModal')).show();
}
function showAddSectionModal() {
         new bootstrap.Modal(document.getElementById('addSectionModal')).show();
     }
function showAddTaskModal(sectionId = null) {
     //Section-Dropdown im Modal auf gewünschten Abschnitt setzen
     if (sectionId){
     const sel = document.getElementById('taskSectionSelect');
     if (sel) sel.value = sectionId;
     }

     //Modal anzeigen
     new bootstrap.Modal(document.getElementById('addTaskModal')).show();
     }
window.editTask = function (taskId, taskName, startDate, endDate) {
    document.getElementById('editTaskId').value = taskId;
    document.getElementById('editTaskName').value = taskName;
    document.getElementById('editTaskStartDate').value = startDate;
    document.getElementById('editTaskEndDate').value = endDate;

    new bootstrap.Modal(document.getElementById('editTaskModal')).show();
}

window.deleteItem = function (itemId, itemType, itemName) {
    document.getElementById('deleteConfirmText').textContent =
        `Sind Sie sicher, dass Sie "${itemName}" löschen möchten?`;

    const jsonData = {
        Type: itemType === 'task' ? 'ProjectTask' : 'ProjectSection',
        Data: { itemId, itemName }
    };

    document.getElementById('deleteObjectJson').value = JSON.stringify(jsonData);
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

window.showAddItemModal = function (parentId, parentType) {
    addItemParentId = parentId;
    addItemParentType = parentType;

    const subBtn = document.getElementById('chooseSubBtn');
    subBtn.disabled = (parentType !== 'section');

    new bootstrap.Modal(document.getElementById('addItemModal')).show();
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
console.log("ModalFunctions.js loaded successfully"); 