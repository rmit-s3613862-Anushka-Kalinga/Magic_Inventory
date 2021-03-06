﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magic_Inventory.Models;

namespace Magic_Inventory.Data
{
    public class SeedData
    {
       
        public static async Task Initialize(IServiceProvider serviceProvider)
        {    
            
            var roleManger = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { RoleConstants.OwnerRole, RoleConstants.FranchiseHolderRole};

            foreach (var role in roles)
            {
                if (!await roleManger.RoleExistsAsync(role))
                {
                    await roleManger.CreateAsync(new IdentityRole { Name = role });
                }
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            await EnsureUserHasRole(userManager,"ownermagic18@gmail.com", RoleConstants.OwnerRole);
            await EnsureUserHasRole(userManager, "franchisemagic18@gmail.com", RoleConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "franchisemagicEAST@gmail.com", RoleConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "franchisemagicNorth@gmail.com", RoleConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "franchisemagicWEST@gmail.com", RoleConstants.FranchiseHolderRole);
            await EnsureUserHasRole(userManager, "franchisemagicSOUTH@gmail.com", RoleConstants.FranchiseHolderRole);

        }      

        private static async Task EnsureUserHasRole(UserManager<ApplicationUser> userManager,string userName, string role)
        {            
            var user = await userManager.FindByNameAsync(userName);
            if (user != null && !await userManager.IsInRoleAsync(user, role))
            {

                await userManager.AddToRoleAsync(user, role);
            }
        }

    }
}
