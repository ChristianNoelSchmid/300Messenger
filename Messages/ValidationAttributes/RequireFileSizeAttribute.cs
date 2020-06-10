using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Messages.ValidationAttributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequireFileSizeAttribute : ValidationAttribute
    {
        // The size limit, in KB
        public int Size { get; }
        public RequireFileSizeAttribute(int size)
        {
            Size = size;
        }
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if(value is IFormFile file)
            {
                if(file.Length > Size) 
                {
                    return new ValidationResult($"File size cannot be more than {Size/1024.0f}MB");
                }
            }
            if(value is List<IFormFile> files)
            {
                foreach(var currentFile in files)
                {
                    // IFormFile Length is measured in bytes. Divide by
                    // 1024 to ensure KB to KB comparison
                    if(currentFile.Length / 1024 > Size)
                    {
                        string name = Path.GetFileName(currentFile.FileName);
                        return new ValidationResult($"File size cannot be more than {Size/1024.0f}MB (file {name})");
                    }
                }
            } 
            else
            {
                return new ValidationResult("Validation Error");
            }

            return ValidationResult.Success;
        }
    }
}