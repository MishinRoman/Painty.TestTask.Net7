using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

using Npgsql.Internal.TypeHandlers.FullTextSearchHandlers;

using Painty.TestTask.Net7.Data.Models;


namespace Painty.TestTask.Net7.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MyImageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MyImageController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }
        //TODO: add action for return filesCollection my and frends

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFileAsync(string fileName)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Name == User.Identity.Name);
            if (currentUser == null) 
            {
                return ValidationProblem(); 
            }

            var image = currentUser.Images.FirstOrDefault(i => i.Name == fileName);
            if (image == null) return NotFound("File not found");

            return File(image.CreateReadStream(), "application/jpg");



        }

        
        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationBinder.GetValue<string>(_configuration, "FilePath"),formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            
            return Ok(new { count = files.Count, size });
        }
    }
}
