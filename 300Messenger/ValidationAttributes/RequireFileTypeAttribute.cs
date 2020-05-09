using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace _300Messenger.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequireFileTypeAttribute : ValidationAttribute
    {
        public string Type { get; }
        public RequireFileTypeAttribute(string type)
        {
            Type = type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                string fileType = file.ContentType.Split('/')[0];
                if(fileType != Type)
                {
                    return new ValidationResult($"Cannot upload file of type {fileType}");
                }
            }
            if(value is List<IFormFile> files)
            {
                foreach(var currentFile in files)
                {
                    string fileType = currentFile.ContentType.Split('/')[0];
                    if(fileType != Type)
                    {
                        string fileName = Path.GetFileName(currentFile.FileName);
                        return new ValidationResult($"Cannot upload file of type {fileType} ({fileName})");
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