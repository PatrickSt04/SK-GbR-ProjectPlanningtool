using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.IndependentTables;
using System.ComponentModel;
using Microsoft.VisualBasic;

public static class DbInitializer
{
    private static State openState = new State { StateName = "Offen", Color = "#007BFF" };
    private static State progressState = new State { StateName = "In Bearbeitung", Color = "#FFC107" };
    private static State doneState = new State { StateName = "Abgeschlossen", Color = "#28A745" };
    private static State waitingState = new State { StateName = "In Verzug", Color = "#DC3545" };
    public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (context.Company.Any())
        {
            return;
        }

        context.Add(openState);
        context.Add(progressState);
        context.Add(doneState);
        context.Add(waitingState);

        // Rollen erstellen 
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("Viewer"))
            await roleManager.CreateAsync(new IdentityRole("Viewer"));

        if (!await roleManager.RoleExistsAsync("Planner"))
            await roleManager.CreateAsync(new IdentityRole("Planner"));

        var adminUser = new IdentityUser { UserName = "admin@company.com", Email = "admin@company.com", EmailConfirmed = true };
        var result = await userManager.CreateAsync(adminUser, "Password123!");
        if (!result.Succeeded)
            throw new Exception($"Admin creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        await userManager.AddToRoleAsync(adminUser, "Admin");

        var viewerUser = new IdentityUser { UserName = "viewer@company.com", Email = "viewer@company.com", EmailConfirmed = true };
        result = await userManager.CreateAsync(viewerUser, "Password123!");
        if (!result.Succeeded)
            throw new Exception($"Viewer creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        await userManager.AddToRoleAsync(viewerUser, "Viewer");

        var company = new Company { CompanyName = "Unternehmen" };
        await context.Company.AddAsync(company);
        await context.SaveChangesAsync();

        var hourlyRateGroup = new HourlyRateGroup { Company = company, HourlyRate = 35, HourlyRateGroupName = "Kaufmännisch" };
        await context.HourlyRateGroup.AddAsync(hourlyRateGroup);
        await context.SaveChangesAsync();
        var hourlyRateGroupGWB = new HourlyRateGroup { Company = company, HourlyRate = 35, HourlyRateGroupName = "Gewerblich" };
        await context.HourlyRateGroup.AddAsync(hourlyRateGroupGWB);
        await context.SaveChangesAsync();

        var employee = new Employee { IdentityUser = adminUser, Company = company, HourlyRateGroup = hourlyRateGroup, EmployeeDisplayName = "Admin" };
        await context.Employee.AddAsync(employee);
        await context.SaveChangesAsync();
        employee = new Employee { IdentityUser = viewerUser, Company = company, HourlyRateGroup = hourlyRateGroupGWB, EmployeeDisplayName = "Viewer", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now };
        await context.Employee.AddAsync(employee);
        await context.SaveChangesAsync();

        var addresses = new[] {
        new Address { City = "Julbach", Company = company, PostalCode = "84387", Country = "Deutschland", HouseNumber = "1", Street = "Mooswinkl", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now }
        };
        await context.Address.AddRangeAsync(addresses);
        await context.SaveChangesAsync();
        var Licences = new[]
        {
            new LicenseModel{ LicenseName = "Basic" },
            new LicenseModel{ LicenseName = "Premium" }
        };
        await context.LicenseModel.AddRangeAsync(Licences);
        await context.SaveChangesAsync();

        var sectors = new[]
        {
             new IndustrySector{ SectorName = "Gebäudebau" },
             new IndustrySector{ SectorName = "Tiefbau" },
             new IndustrySector{ SectorName = "Ingenierbau" },
             new IndustrySector{ SectorName = "Sanierung" },
             new IndustrySector{ SectorName = "Elektrik" }
         };


        company.Address = addresses[0];
        company.Sector = sectors[0];
        company.License = Licences[1];
        context.Company.Update(company);
        await context.SaveChangesAsync();

        // ── Standard-Einheiten (Baubranche) ──
        var units = new[]
        {
            // Mengeneinheiten
            new Unit { Name = "Stück",              ShortName = "Stk" },
            new Unit { Name = "Paar",               ShortName = "Pr" },
            new Unit { Name = "Satz",               ShortName = "Stz" },
            new Unit { Name = "Pauschal",           ShortName = "psch" },

            // Längenmaße
            new Unit { Name = "Millimeter",         ShortName = "mm" },
            new Unit { Name = "Zentimeter",         ShortName = "cm" },
            new Unit { Name = "Meter",              ShortName = "m" },
            new Unit { Name = "Laufmeter",          ShortName = "lfm" },
            new Unit { Name = "Kilometer",          ShortName = "km" },

            // Flächenmaße
            new Unit { Name = "Quadratmeter",       ShortName = "m²" },
            new Unit { Name = "Quadratzentimeter",  ShortName = "cm²" },

            // Raummaße
            new Unit { Name = "Kubikmeter",         ShortName = "m³" },
            new Unit { Name = "Kubikzentimeter",    ShortName = "cm³" },
            new Unit { Name = "Liter",              ShortName = "l" },

            // Gewichtseinheiten
            new Unit { Name = "Gramm",              ShortName = "g" },
            new Unit { Name = "Kilogramm",          ShortName = "kg" },
            new Unit { Name = "Tonne",              ShortName = "t" },

            // Zeiteinheiten
            new Unit { Name = "Stunde",             ShortName = "Std" },
            new Unit { Name = "Tag",                ShortName = "Tg" },
            new Unit { Name = "Monat",              ShortName = "Mon" },

            // Verpackungs- / Gebindeeinheiten
            new Unit { Name = "Packung",            ShortName = "Pkg" },
            new Unit { Name = "Karton",             ShortName = "Krt" },
            new Unit { Name = "Sack",               ShortName = "Sk" },
            new Unit { Name = "Eimer",              ShortName = "Ei" },
            new Unit { Name = "Dose",               ShortName = "Ds" },
            new Unit { Name = "Rolle",              ShortName = "Rl" },
            new Unit { Name = "Bund",               ShortName = "Bd" },
            new Unit { Name = "Palette",            ShortName = "Pal" },
            new Unit { Name = "Tafel",              ShortName = "Tf" },
            new Unit { Name = "Platte",             ShortName = "Pl" },

            // Transport / Logistik
            new Unit { Name = "Fuhre",              ShortName = "Fu" },
            new Unit { Name = "Ladung",             ShortName = "Ldg" },

            // Sonstige
            new Unit { Name = "Prozent",            ShortName = "%" },
        };

        await context.Unit.AddRangeAsync(units);
        await context.SaveChangesAsync();

        // Alle Einheiten für das Demo-Unternehmen aktivieren
        var companyUnits = units.Select(u => new CompanyUnit
        {
            CompanyId = company.CompanyId,
            UnitId = u.UnitId
        });

        await context.CompanyUnit.AddRangeAsync(companyUnits);
        await context.SaveChangesAsync();
    }
}
//public static class DbInitializer
//{
//    private static State openState = new State { StateName = "Offen", Color = "#007BFF" };
//    private static State progressState = new State { StateName = "In Bearbeitung", Color = "#FFC107" };
//    private static State doneState = new State { StateName = "Abgeschlossen", Color = "#28A745" };
//    private static State waitingState = new State { StateName = "In Verzug", Color = "#DC3545" };
//    public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
//    {
//        if (context.Company.Any())
//        {
//            return;
//        }

//        context.Add(openState);
//        context.Add(progressState);
//        context.Add(doneState);
//        context.Add(waitingState);

//        // Rollen erstellen
//        if (!await roleManager.RoleExistsAsync("Admin"))
//            await roleManager.CreateAsync(new IdentityRole("Admin"));

//        if (!await roleManager.RoleExistsAsync("Viewer"))
//            await roleManager.CreateAsync(new IdentityRole("Viewer"));

//        if (!await roleManager.RoleExistsAsync("Planner"))
//            await roleManager.CreateAsync(new IdentityRole("Planner"));

//        var adminUser = new IdentityUser { UserName = "admin@company.com", Email = "admin@company.com", EmailConfirmed = true };
//        var result = await userManager.CreateAsync(adminUser, "Password123!");
//        if (!result.Succeeded)
//            throw new Exception($"Admin creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//        await userManager.AddToRoleAsync(adminUser, "Admin");

//        var plannerUser = new IdentityUser { UserName = "planner@company.com", Email = "planner@company.com", EmailConfirmed = true };
//        result = await userManager.CreateAsync(plannerUser, "Password123!");
//        if (!result.Succeeded)
//            throw new Exception($"Planner creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//        await userManager.AddToRoleAsync(plannerUser, "Planner");

//        var viewerUser = new IdentityUser { UserName = "viewer@company.com", Email = "viewer@company.com", EmailConfirmed = true };
//        result = await userManager.CreateAsync(viewerUser, "Password123!");
//        if (!result.Succeeded)
//            throw new Exception($"Viewer creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//        await userManager.AddToRoleAsync(viewerUser, "Viewer");

//        var company = new Company { CompanyName = "Unternehmen" };
//        await context.Company.AddAsync(company);
//        await context.SaveChangesAsync();

//        var hourlyRateGroup = new HourlyRateGroup { Company = company, HourlyRate = 35, HourlyRateGroupName = "Kaufmännisch" };
//        await context.HourlyRateGroup.AddAsync(hourlyRateGroup);
//        await context.SaveChangesAsync();
//        var hourlyRateGroupGWB = new HourlyRateGroup { Company = company, HourlyRate = 35, HourlyRateGroupName = "Gewerblich" };
//        await context.HourlyRateGroup.AddAsync(hourlyRateGroup);
//        await context.SaveChangesAsync();

//        var employee = new Employee { IdentityUser = adminUser, Company = company, HourlyRateGroup = hourlyRateGroup, EmployeeDisplayName = "Admin" };
//        await context.Employee.AddAsync(employee);
//        await context.SaveChangesAsync();
//        employee = new Employee { IdentityUser = plannerUser, Company = company, HourlyRateGroup = hourlyRateGroup, EmployeeDisplayName = "Planner", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now };
//        await context.Employee.AddAsync(employee);
//        await context.SaveChangesAsync();

//        employee = new Employee { IdentityUser = viewerUser, Company = company, HourlyRateGroup = hourlyRateGroupGWB, EmployeeDisplayName = "Viewer", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now };
//        await context.Employee.AddAsync(employee);
//        await context.SaveChangesAsync();

//        var addresses = new[] {
//        new Address { City = "Julbach", Company = company, PostalCode = "84387", Country = "Deutschland", HouseNumber = "1", Street = "Mooswinkl", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now },
//        new Address { City = "Frankfurt",  Company = company, PostalCode = "84387", Country = "Deutschland", HouseNumber = "1a", Street = "Fliegenweg", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now },
//         new Address { City = "München", Company = company, PostalCode = "84387", Country = "Deutschland", HouseNumber = "13b", Street = "Blumenstraße", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now },
//        new Address { City = "Straubing", Company = company, PostalCode = "84387", Country = "Deutschland", HouseNumber = "2a", Street = "Vorstraße", CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now }
//        };
//        await context.Address.AddRangeAsync(addresses);
//        await context.SaveChangesAsync();
//        var Licences = new[]
//        {
//            new LicenseModel{ LicenseName = "Basic" },
//            new LicenseModel{ LicenseName = "Premium" }
//        };
//        await context.LicenseModel.AddRangeAsync(Licences);
//        await context.SaveChangesAsync();

//        var customers = new[]
//       {
//             new Customer { CustomerName = "Schmidt Bau AG", Company = company, Address = addresses[1], CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now },
//             new Customer { CustomerName = "Mayer Immobilien GmbH", Company = company, Address = addresses[2], CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now },
//             new Customer { CustomerName = "Kraus Bauunternehmen", Company = company, Address = addresses[3], CreatedByEmployee = employee, CreatedTimestamp = DateTime.Now }
//         };
//        await context.Customer.AddRangeAsync(customers);
//        await context.SaveChangesAsync();

//        var sectors = new[]
//        {
//             new IndustrySector{ SectorName = "Gebäudebau" },
//             new IndustrySector{ SectorName = "Tiefbau" },
//             new IndustrySector{ SectorName = "Ingenierbau" },
//             new IndustrySector{ SectorName = "Sanierung" },
//             new IndustrySector{ SectorName = "Elektrik" }
//         };


//        company.Address = addresses[0];
//        company.Sector = sectors[0];
//        company.License = Licences[1];
//        context.Company.Update(company);
//        await context.SaveChangesAsync();

//        // // Projekte und Abschnitte erstellen
//        var projects = new List<Project>(6);

//        foreach (var customer in customers)
//        {
//            var index = new Random().Next(3, 6);
//            for (int i = 1; i <= index; i++)
//            {
//                var projectName = GetRandomProjectName() + " - " + customer.CustomerName;
//                var startDateTime = DateTime.Now.AddDays(i * 10);
//                var startDate = DateOnly.FromDateTime(startDateTime);
//                var dueDate = startDate.AddMonths(6); // Projekte dauern etwa 6 Monate
//                var projectBudget = new ProjectBudget
//                {

//                    CreatedByEmployee = employee,
//                    CreatedTimestamp = DateTime.Now,
//                    Company = company,
//                    LatestModifier = employee,
//                    InitialAdditionalCosts = new List<ProjectBudget.InitialAdditionalCost>
//                     {
//                         new ProjectBudget.InitialAdditionalCost { AdditionalCostName = "Genehmigungen", AdditionalCostAmount = 2000 },
//                         new ProjectBudget.InitialAdditionalCost { AdditionalCostName = "Versicherungen", AdditionalCostAmount = 1500 }
//                     },
//                    InitialBudget = 50000,
//                    InitialHRGPlannings = new List<ProjectBudget.InitialHRGPlanning>
//                     {
//                         new ProjectBudget.InitialHRGPlanning { HourlyRateGroupId = hourlyRateGroup.HourlyRateGroupId, HourlyRateGroupName = hourlyRateGroup.HourlyRateGroupName, HourlyRate = (decimal) hourlyRateGroup.HourlyRate, Amount = 2, EstimatedHours = 120 },
//                         new ProjectBudget.InitialHRGPlanning { HourlyRateGroupId = hourlyRateGroup.HourlyRateGroupId, HourlyRateGroupName = hourlyRateGroup.HourlyRateGroupName, HourlyRate = (decimal) hourlyRateGroup.HourlyRate, Amount = 1, EstimatedHours = 80 }
//                     },
//                    LatestModificationTimestamp = DateTime.Now,
//                    LatestModificationText = "Projektbudget erstellt",
//                };
//                context.Add(projectBudget);
//                await context.SaveChangesAsync();
//                var project = new Project
//                {
//                    ProjectBudget = projectBudget,
//                    ProjectName = projectName,
//                    ResponsiblePerson = employee,
//                    ProjectDescription = $"Projekt für {customer.CustomerName} mit dem Schwerpunkt auf {GetProjectFocus(projectName)}.",
//                    Customer = customer,
//                    Company = company,
//                    StartDate = startDate,
//                    EndDate = dueDate,
//                    CreatedByEmployee = employee,
//                    CreatedTimestamp = DateTime.Now
//                };
//                projects.Add(project);
//                context.Set<Project>().Add(project);
//                context.SaveChanges();

//                // Erstelle Abschnitte (z.B. Etagen eines Gebäudes)
//                for (int j = 1; j <= 3; j++)
//                {
//                    string sectionName;
//                    if (j == 1)
//                    {
//                        sectionName = "Erdgeschoss";
//                    }
//                    else
//                    {
//                        sectionName = $"{j - 1}. Obergeschoss";
//                    }

//                    // Start- und Enddaten für Abschnitte festlegen
//                    var sectionStartDate = startDate.AddMonths(j - 1);
//                    var sectionDueDate = sectionStartDate.AddMonths(1); // Jede Etage dauert ca. 1 Monat

//                    var section = new ProjectSection
//                    {
//                        ProjectSectionName = sectionName,
//                        Project = project,
//                        Company = company,
//                        CreatedByEmployee = employee,
//                        CreatedTimestamp = DateTime.Now
//                    };
//                    context.Set<ProjectSection>().Add(section);
//                    context.SaveChanges();

//                    // Erstelle Aufgaben (z.B. Bauaufgaben)
//                    var tasks = new List<string> { "Fundament gießen", "Wände hochziehen", "Estrich legen", "Dach decken", "Innenausbau" };

//                    foreach (var task in tasks)
//                    {
//                        // Start- und Enddaten für Aufgaben festlegen
//                        var taskStartDate = sectionStartDate.AddDays(new Random().Next(1, 5)); // Tasks starten innerhalb weniger Tage nach dem Beginn des Abschnitts
//                        var taskDueDate = taskStartDate.AddDays(new Random().Next(5, 15)); // Jede Aufgabe dauert 5-15 Tage

//                        var projectTask = new ProjectTask
//                        {
//                            ProjectTaskName = task,
//                            ProjectSection = section,
//                            Company = company,
//                            State = GetRandomState(),
//                            StartDate = taskStartDate,
//                            EndDate = taskDueDate,
//                            CreatedByEmployee = employee,
//                            CreatedTimestamp = DateTime.Now

//                        };
//                        context.Set<ProjectTask>().Add(projectTask);
//                        context.SaveChanges();
//                    }
//                    //TASK CATALOG TASKS
//                    // Erstelle Aufgaben (z.B. Bauaufgaben)
//                    if (j > 1)
//                    { // Nur für die 1. Section
//                        continue;
//                    }
//                    var tasksTC = new List<string> { "Verwaltungsbesuch", "Planungsaufgabe", "Abnahme" };

//                    foreach (var task in tasksTC)
//                    {
//                        // Start- und Enddaten für Aufgaben festlegen
//                        var taskStartDate = sectionStartDate.AddDays(new Random().Next(1, 5)); // Tasks starten innerhalb weniger Tage nach dem Beginn des Abschnitts
//                        var taskDueDate = taskStartDate.AddDays(new Random().Next(5, 15)); // Jede Aufgabe dauert 5-15 Tage

//                        var projectTask = new ProjectTaskCatalogTask
//                        {
//                            TaskName = task,
//                            Company = company,
//                            State = GetRandomState(),
//                            CreatedByEmployee = employee,
//                            CreatedTimestamp = DateTime.Now

//                        };
//                        context.Set<ProjectTaskCatalogTask>().Add(projectTask);
//                        context.SaveChanges();
//                    }
//                }
//            }
//        }

//    }

//    private static string GetRandomProjectName()
//    {
//        var projectNames = new List<string> { "Trockenbau", "Neubau", "Elektroinstallation", "Lüftungssysteme installieren", "Wärmedämmung", "Altbausanierung" };
//        return projectNames[new Random().Next(0, projectNames.Count)];
//    }
//    private static string GetProjectFocus(string projectName)
//    {
//        return projectName switch
//        {
//            "Trockenbau" => "Trockenbauarbeiten",
//            "Neubau" => "Neubau eines modernen Gebäudes",
//            "Elektroinstallation" => "Elektroinstallationen und Verkabelungen",
//            "Lüftungssysteme installieren" => "Installation von Lüftungssystemen",
//            "Wärmedämmung" => "Wärmedämmung für Energieeffizienz",
//            "Altbausanierung" => "Sanierung und Modernisierung von Altbauten",
//            _ => "allgemeine Bauarbeiten"
//        };
//    }

//    private static State GetRandomState()
//    {
//        var states = new List<State> { openState, doneState };
//        //var states = new List<State> { openState, progressState, doneState, waitingState };
//        return states[new Random().Next(0, states.Count)];
//    }
//}

