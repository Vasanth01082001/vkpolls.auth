using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace vkpolls.auth.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> ValidationErrors { get; set; }
        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, ValidationResult validationResult) : base(message)
        {
            ValidationErrors = validationResult.ToDictionary();
        }
    }
}
