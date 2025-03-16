using Microsoft.AspNetCore.Identity;
using SAAS_Projectplanningtool.Data;
using SAAS_Projectplanningtool.Models.Budgetplanning;
using SAAS_Projectplanningtool.Models;
using SAAS_Projectplanningtool.Models.IndependentTables;
using System.ComponentModel;
using Microsoft.VisualBasic;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Rollen erstellen
        if (!roleManager.RoleExistsAsync("Admin").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
        }
        if (!roleManager.RoleExistsAsync("Worker").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Worker")).Wait();
        }
        if (!roleManager.RoleExistsAsync("Planner").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Planner")).Wait();
        }
        if (!roleManager.RoleExistsAsync("Analyzer").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Analyzer")).Wait();
        }
        var adminUser = new IdentityUser { UserName = "admin@company.com", Email = "admin@company.com", EmailConfirmed = true };
        userManager.CreateAsync(adminUser, "Password123!").Wait();
        userManager.AddToRoleAsync(adminUser, "Admin").Wait();

        var company = new Company { CompanyName = "Innovatec UG" };
        await context.Company.AddAsync(company);
        await context.SaveChangesAsync();
        
        var hourlyRateGroup = new HourlyRateGroup { Company = company, HourlyRate = 35, HourlyRateGroupName = "Maler" };
        await context.HourlyRateGroup.AddAsync(hourlyRateGroup);
        await context.SaveChangesAsync();

        var employee = new Employee { IdentityUser = adminUser, Company = company, HourlyRateGroup = hourlyRateGroup, EmployeeDisplayName = "Admin", IdentityRole = await roleManager.FindByNameAsync("Admin") };
        await context.Employee.AddAsync(employee);
        await context.SaveChangesAsync();

        var addresses = new[] {
        new Address { City = "Julbach", PostalCode = "84387", Country = "Deutschland", HouseNumber = "1", Street = "Mooswinkl" },
        new Address { City = "Frankfurt", PostalCode = "84387", Country = "Deutschland", HouseNumber = "1a", Street = "Fliegenweg" },
         new Address { City = "München", PostalCode = "84387", Country = "Deutschland", HouseNumber = "13b", Street = "Blumenstraße" },
        new Address { City = "Straubing", PostalCode = "84387", Country = "Deutschland", HouseNumber = "2a", Street = "Vorstraße" }
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

        var customers = new[]
       {
            new Customer { CustomerName = "Schmidt Bau AG", Address = addresses[1] },
            new Customer { CustomerName = "Mayer Immobilien GmbH", Address = addresses[2] },
            new Customer { CustomerName = "Kraus Bauunternehmen", Address = addresses[3] }
        };
        await context.Customer.AddRangeAsync(customers);
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

        // Projekte und Abschnitte erstellen
        var projects = new List<Project>(6);

        foreach (var customer in customers)
        {
            var index = new Random().Next(3, 6);
            for (int i = 1; i <= index; i++)
            {
                var projectName = GetRandomProjectName() + " - " + customer.CustomerName;
                var startDateTime = DateTime.Now.AddDays(i * 10);
                var startDate = DateOnly.FromDateTime(startDateTime);
                var dueDate = startDate.AddMonths(6); // Projekte dauern etwa 6 Monate

                var project = new Project
                {
                    ProjectName = projectName,
                    ProjectDescription = $"Projekt für {customer.CustomerName} mit dem Schwerpunkt auf {GetProjectFocus(projectName)}.",
                    Customer = customer,
                    Company = company,
                    StartDate = startDate,
                    EndDate = dueDate,
                };
                projects.Add(project);
                context.Set<Project>().Add(project);
                context.SaveChanges();

                // Erstelle Abschnitte (z.B. Etagen eines Gebäudes)
                for (int j = 1; j <= 3; j++)
                {
                    string sectionName;
                    if (j == 1)
                    {
                        sectionName = "Erdgeschoss";
                    }
                    else
                    {
                        sectionName = $"{j - 1}. Obergeschoss";
                    }

                    var sectionStartDate = startDate.AddMonths(j - 1);
                    var sectionDueDate = sectionStartDate.AddMonths(1); // Jede Etage dauert ca. 1 Monat

                    var section = new ProjectSection
                    {
                        ProjectSectionName = sectionName,
                        Project = project,
                        Company = company,
                    };
                    context.Set<ProjectSection>().Add(section);
                    context.SaveChanges();

                    // Erstelle Aufgaben (z.B. Bauaufgaben)
                    var tasks = new List<string> { "Fundament gießen", "Wände hochziehen", "Estrich legen", "Dach decken", "Innenausbau" };

                    foreach (var task in tasks)
                    {
                        var taskStartDate = sectionStartDate.AddDays(new Random().Next(1, 5)); // Tasks starten innerhalb weniger Tage nach dem Beginn des Abschnitts
                        var taskDueDate = taskStartDate.AddDays(new Random().Next(5, 15)); // Jede Aufgabe dauert 5-15 Tage

                        var projectTask = new ProjectTask
                        {
                            ProjectTaskName = task,
                            ProjectSection = section,
                            Company = company,
                        };
                        context.Set<ProjectTask>().Add(projectTask);
                        context.SaveChanges();



                    }

                }
            }
        }

    }

    private static string GetRandomProjectName()
    {
        var projectNames = new List<string> { "Trockenbau", "Neubau", "Elektroinstallation", "Lüftungssysteme installieren", "Wärmedämmung", "Altbausanierung" };
        return projectNames[new Random().Next(0, projectNames.Count)];
    }
    private static string GetProjectFocus(string projectName)
    {
        return projectName switch
        {
            "Trockenbau" => "Trockenbauarbeiten",
            "Neubau" => "Neubau eines modernen Gebäudes",
            "Elektroinstallation" => "Elektroinstallationen und Verkabelungen",
            "Lüftungssysteme installieren" => "Installation von Lüftungssystemen",
            "Wärmedämmung" => "Wärmedämmung für Energieeffizienz",
            "Altbausanierung" => "Sanierung und Modernisierung von Altbauten",
            _ => "allgemeine Bauarbeiten"
        };
    }
}

