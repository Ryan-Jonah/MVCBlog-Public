using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCBlog.Data;
using MVCBlog.Enums;
using MVCBlog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Services
{
    /// <summary>
    /// <para>Seed roles into the system</para>
    /// <para>Seed users into the system</para>
    /// <para></para>
    /// </summary>
    public class DataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;

        public DataService(
            ApplicationDbContext context, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<BlogUser> userManager, 
            IImageService imageService, 
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _imageService = imageService;
            _configuration = configuration;
        }

        /// <summary>
        /// Wrapper for DataService methods
        /// </summary>
        /// <returns></returns>
        public async Task ManageDataAsync()
        {
            //Migrate/Create database if needed - Equivalent to running update-database in EF Core
            await _context.Database.MigrateAsync();

            //Seed Data to database
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //If roles exist, do not seed any roles
            if (_context.Roles.Any()) 
                return;

            //If no roles exist, seed data into the database
            foreach (var role in Enum.GetNames(typeof(BlogRoll)))
            {
                //Use the Role Manager to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            //If users exist, do not seed any users
            if (_context.Users.Any())
            {

            }
            else
            {
                #region Administrator
                //Create instance of BlogUser for Administrator role and apply values
                var adminUser = new BlogUser()
                {
                    Email = "admin@example.com",
                    UserName = "admin@example.com",
                    FirstName = "That",
                    LastName = "Guy",
                    PhoneNumber = "5555555555",
                    ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                    ContentType = Path.GetExtension(_configuration["DefaultUserImage"]),
                    EmailConfirmed = true
                };

                //Use the user manager to create a user asynchronously
                await _userManager.CreateAsync(adminUser, "Password1!");

                //Add the new user to the Administraor role
                await _userManager.AddToRoleAsync(adminUser, BlogRoll.Administrator.ToString());
                #endregion

                #region Moderator
                //Create instance of BlogUser for Moderator role apply values
                var modUser = new BlogUser()
                {
                    Email = "admin@example.ca",
                    UserName = "admin@example.ca",
                    FirstName = "That",
                    LastName = "Guy",
                    PhoneNumber = "5555555555",
                    ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                    ContentType = Path.GetExtension(_configuration["DefaultUserImage"]),
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(modUser, "Password1!");

                await _userManager.AddToRoleAsync(modUser, BlogRoll.Moderator.ToString());
                #endregion
            }


        }
    }
}