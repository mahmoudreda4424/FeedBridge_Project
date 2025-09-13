using Feed_Bridge.Models.Entities;
using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // التأكد من وجود الـ Roles
        string[] roles = { "Admin", "Delivery", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin account
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "Admin@123");
        }
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            await userManager.AddToRoleAsync(adminUser, "Admin");

        // Delivery account
        var deliveryEmail = "delivery@example.com";
        var deliveryUser = await userManager.FindByEmailAsync(deliveryEmail);
        if (deliveryUser == null)
        {
            deliveryUser = new ApplicationUser
            {
                UserName = "delivery",
                Email = deliveryEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(deliveryUser, "Delivery@123");
        }
        if (!await userManager.IsInRoleAsync(deliveryUser, "Delivery"))
            await userManager.AddToRoleAsync(deliveryUser, "Delivery");
    }
}
