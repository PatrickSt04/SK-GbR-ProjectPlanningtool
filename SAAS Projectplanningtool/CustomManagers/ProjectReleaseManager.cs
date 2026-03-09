using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using System.Security.Claims;

namespace SAAS_Projectplanningtool.CustomManagers
{
    public class ProjectReleaseManager
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomUserManager _customUserManager;
        public ProjectReleaseManager(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _customUserManager = new CustomUserManager(_context, _userManager);
        }
        ///<summary>
        /// Returns all released Projects for a Viewer Employee
        ///</summary>
        public async Task<IList<string>> GetReleasedProjectIDsForViewer(Employee employee)
        {
            // Projekt-IDs können in der Entität nullable sein (string?).
            // Filtere explizit NULL-Werte heraus und verwende den Nicht-Null-Operator,
            // damit die Methode weiterhin IList<string> zurückgibt und der Compilerfehler CS8619 verschwindet.
            return await _context.ProjectEmployeeViewerShare
                .Where(pvs => pvs.CompanyId == employee.CompanyId)
                .Where(pvs => pvs.EmployeeId == employee.EmployeeId)
                .Where(pvs => pvs.ProjectId != null)
                .Select(pvs => pvs.ProjectId!).ToListAsync();
            //return await _context.Project.Select(p => p.ProjectId).ToListAsync();

        }

        public async Task<Boolean> ReleaseProjectForViewer(string projectId, Employee viewer)
        {
            var project = await _context.Project.FirstOrDefaultAsync(p => p.ProjectId == projectId);
            if (project == null || project.CompanyId != viewer.CompanyId)
            {
                return false; // Projekt existiert nicht oder gehört nicht zur Firma des Viewers
            }
            var share = new ProjectEmployeeViewerShare
            {
                ProjectId = projectId,
                EmployeeId = viewer.EmployeeId,
                CompanyId = viewer.CompanyId
            };
            await _context.ProjectEmployeeViewerShare.AddAsync(share);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
