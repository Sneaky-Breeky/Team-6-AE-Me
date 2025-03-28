﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAMBackend.Data;
using DAMBackend.Models;
using NuGet.Protocol;
using DAMBackend.blob;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Xabe.FFmpeg;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using DAMBackend.Models;
using ImageMagick;
using System.Diagnostics;
using File = System.IO.File;
using ImageSharpExif = SixLabors.ImageSharp.Metadata.Profiles.Exif;
using ImageSharpExifTag = SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag;


namespace DAMBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AzureBlobService _azureBlobService;
        private readonly SQLDbContext _context;

        public FilesController(SQLDbContext context, AzureBlobService azureBlobService)
        {
            _context = context;
            _azureBlobService = azureBlobService;
        }

        // GET: api/Files
        [HttpGet("{userId}/palette")]
        public async Task<ActionResult<IEnumerable<FileModel>>> GetFiles(int userId)
        {
            var files = await _context.Files
                              .Where(f => f.UserId == userId && f.Palette)
                              .Include(f => f.bTags)
                              .ToListAsync();
            return Ok(files);
        }
         // GET: api/Files
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileModel>>> GetFilesFromPalette()
        {
            var files = await _context.Files.ToListAsync();
            return Ok(files);
        }

        // GET: api/Files/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FileModel>> GetFile(int id)
        {
            var @file = await _context.Files.FindAsync(id);

            if (@file == null)
            {
                return NotFound();
            }

            return Ok(@file);
        }

        // PUT: api/Files/{id}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFile(int id, FileModel @file)
        {
            if (!id.Equals(@file.Id))
            {
                return BadRequest();
            }

            _context.Entry(@file).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Files
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        // Call function in submission engine
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<List<FileModel>>> UploadFiles(List<IFormFile> files)
        {
            // Check if the number of files exceeds 100
            if (files.Count > 100)
            {
                return BadRequest("You can upload a maximum of 100 files at once.");
            }
            Console.WriteLine("the length of files is: ", files.Count);
            List<string> filesLinks = new List<string> { };

            // Validate and process each file
            foreach (var file in files)
            {
                // Validate file extension (e.g., allow only images and videos)
                var allowedExtensionsphoto = new[] { ".jpg", ".jpeg", ".png", ".raw", ".arw" };
                var allowedExtensionsvideo = new[] { ".mp4" };
                // to be supported: .tiff, .jpg, .gif, .mov 
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensionsphoto.Contains(fileExtension) && !allowedExtensionsvideo.Contains(fileExtension))
                {
                    return BadRequest($"File {file.FileName} has an unsupported file type.");
                }

                // Validate file size (e.g., 500MB limit)
                if (file.Length > 500 * 1024 * 1024) // 100MB
                {
                    return BadRequest($"File {file.FileName} exceeds the maximum allowed size.");
                }
                var id = Guid.NewGuid();

                var fileName = string.Concat("Original_", id.ToString(), fileExtension);
                // Save the file to the upload directory
                using var stream = file.OpenReadStream();
                string fileUrl = await _azureBlobService.UploadAsync(file, fileName, ContainerType.Palette);
                filesLinks.Add(fileUrl);
            }
            return Ok(filesLinks);
        }

        [HttpPost]
        public async Task<ActionResult<List<FileModel>>> AddFiles(List<FileDTO> files)
        {
            // Check if the number of files exceeds 100
            if (files.Count > 100)
            {
                return BadRequest("You can upload a maximum of 100 files at once.");
            }

            //Delete any old file before inserting
            var filesToDelete = await _context.Files
                .Where(f => f.UserId == files[0].userId && f.ProjectId == files[0].projectId)
                .ToListAsync();

            if (filesToDelete.Any())
            {
                _context.Files.RemoveRange(filesToDelete);
                await _context.SaveChangesAsync();
            }

            var savedFiles = new List<FileModel> { };
            var tagsExists = new List<String>{};
            var tagsDoNotExists = new List<String>{};
            var existingTags = new List<TagBasicModel>{};
            foreach (var file in files)
            {

                var project = _context.Projects.Find(file.projectId);
                if (project == null)
                {
                    return BadRequest(string.Concat("Project with id = ", file.projectId, " not found"));
                }
                var user = _context.Users.Find(file.userId);
                if (user == null)
                {
                    return BadRequest(string.Concat("User with id = ", file.userId, " not found"));
                }


                var updatedPath = file.filePath;
                if (!file.palette)
                {
                    updatedPath = await _azureBlobService.MoveBlobWithinContainerAsync("palettes", Path.GetFileName(new Uri(file.filePath).LocalPath), "projects");
                }
                var dimensions = FileEngine.GetDimensions(file.filePath);
                
                foreach(var _file in files){
                    foreach(var tag in _file.metadata){
                        bool exists = await TagExistsAsync(tag);
                        if(exists){
                            tagsExists.Add(tag);
                        }else{
                            tagsDoNotExists.Add(tag);
                        }
                    }
                }
                
                existingTags = await _context.BasicTags
                .Where(t => tagsExists.Contains(t.Value))
                .ToListAsync();
                
                var newTags = tagsDoNotExists
                .Where(t => !existingTags.Any(et => et.Value == t))
                .Select(t => new TagBasicModel { Value = t })
                .ToHashSet();

                FileModel fileModel = new FileModel
                {
                    Name = Path.GetFileName(new Uri(file.filePath).LocalPath),
                    Extension = Path.GetExtension(new Uri(file.filePath).LocalPath),
                    Description = "",
                    ThumbnailPath = updatedPath,
                    ViewPath = updatedPath,
                    OriginalPath = updatedPath,
                    DateTimeOriginal = file.date,
//                    User = user,
                    UserId = file.userId,
//                    Project = project,
                    Palette = file.palette,
                    ProjectId = file.projectId,
                    PixelHeight = dimensions.HasValue ? dimensions.Value.Height : 0,
                    PixelWidth = dimensions.HasValue ? dimensions.Value.Width : 0,
                    bTags = newTags,
                };
                //call exif function and update fileModel object before saving
                //addExifData(fileModel)
                // ADDING EXIF
//                using (var stream = imageFile.OpenReadStream())
//                            using (var image = Image.Load(stream))
//                            {
//                                fileModel.PixelWidth = image.Width;
//                                fileModel.PixelHeight = image.Height;
//                                fileModel.Palette = true;
//
//                                var exifProfile = image.Metadata.ExifProfile;
//                                if (exifProfile != null)
//                                {
//                                    ExtractExifMetadata(exifProfile, fileModel);
//                                }
//                            }
                //
                _context.Files.Add(fileModel);
                savedFiles.Add(fileModel);
            }
            await _context.SaveChangesAsync();
            
            foreach(var file in savedFiles){
               foreach (var tag in existingTags)
                {
                    _context.FileTags.Add(new FileTag
                    {
                        FileId = file.Id,
                        TagId = tag.Value
                    });
                }
            }
            
            await _context.SaveChangesAsync();
            var savedFileIds = savedFiles.Select(f => f.Id).ToList();

            var filesWithTags = await _context.Files
                .Where(f => savedFileIds.Contains(f.Id))
                .Include(f => f.bTags) // Ensure tags are loaded
                .ToListAsync();

            return Ok(filesWithTags);
        }
        public async Task<bool> TagExistsAsync(string tagValue)
        {
            return await _context.BasicTags.AnyAsync(t => t.Value == tagValue);
        }



        // DELETE: api/Files/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var @file = await _context.Files.FindAsync(id);
            if (@file == null)
            {
                return NotFound();
            }

            _context.Files.Remove(@file);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.Id == id);
        }
        private void ExtractExifMetadata(ImageSharpExif.ExifProfile exifProfile, FileModel fileModel)
                {
                    object? latRef = null;
                    object? lonRef = null;

                    foreach (var tag in exifProfile.Values)
                    {
                        if (tag.Tag == ImageSharpExifTag.GPSLatitudeRef)
                        {
                            latRef = tag.GetValue();
                        }
                        else if (tag.Tag == ImageSharpExifTag.GPSLongitudeRef)
                        {
                            lonRef = tag.GetValue();
                        }
                        else if (tag.Tag == ImageSharpExifTag.GPSLatitude)
                        {
                            fileModel.GPSLat = ConvertDMSToDecimal(tag.GetValue(), latRef?.ToString());
                        }
                        else if (tag.Tag == ImageSharpExifTag.GPSLongitude)
                        {
                            fileModel.GPSLon = ConvertDMSToDecimal(tag.GetValue(), lonRef?.ToString());
                        }
                        else if (tag.Tag == ImageSharpExifTag.GPSAltitude && tag.GetValue() is SixLabors.ImageSharp.Rational altitudeRational)
                        {
                            fileModel.GPSAlt = (decimal)altitudeRational.ToDouble();
                        }
                        else if (tag.Tag == ImageSharpExifTag.Make)
                        {
                            fileModel.Make = tag.GetValue()?.ToString();
                        }
                        else if (tag.Tag == ImageSharpExifTag.Model)
                        {
                            fileModel.Model = tag.GetValue()?.ToString();
                        }
                        else if (tag.Tag == ImageSharpExifTag.Copyright)
                        {
                            fileModel.Copyright = tag.GetValue()?.ToString();
                        }
                        else if (tag.Tag == ImageSharpExifTag.FocalLength)
                        {
                            if (tag.GetValue() is SixLabors.ImageSharp.Rational rational)
                            {
                                fileModel.FocalLength = (int)rational.ToDouble();
                            }
                        }
                        else if (tag.Tag == ImageSharpExifTag.FNumber)
                        {
                            if (tag.GetValue() is SixLabors.ImageSharp.Rational rational)
                            {
                                fileModel.Aperture = (float)rational.ToDouble();
                            }
                        }
                    }
                }

                private decimal? ConvertDMSToDecimal(object dmsValue, string? reference)
                {
                    if (dmsValue is SixLabors.ImageSharp.Rational[] dmsArray && dmsArray.Length == 3)
                    {
                        decimal degrees = (decimal)dmsArray[0].ToDouble();
                        decimal minutes = (decimal)dmsArray[1].ToDouble();
                        decimal seconds = (decimal)dmsArray[2].ToDouble();

                        decimal decimalDegrees = degrees + (minutes / 60) + (seconds / 3600);

                        if (reference == "S" || reference == "W")
                        {
                            decimalDegrees = -decimalDegrees;
                        }

                        return decimalDegrees;
                    }

                    return null;
                }
            }
    }
