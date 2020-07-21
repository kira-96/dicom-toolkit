// original source https://github.com/canton7/Stylet/blob/master/Samples/Stylet.Samples.ModelValidation/FluentModelValidator.cs

using FluentValidation;
using Stylet;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDICOMToolkit.Validators
{
    public class FluentModelValidator<T> : IModelValidator<T>
    {
        private readonly IValidator<T> validator;
        private T subject;

        public FluentModelValidator(IValidator<T> validator)
        {
            this.validator = validator;
        }

        public void Initialize(object subject)
        {
            this.subject = (T)subject;
        }

        public async Task<IEnumerable<string>> ValidatePropertyAsync(string propertyName)
        {
            // If someone's calling us synchronously, and ValidationAsync does not complete synchronously,
            // we'll deadlock unless we continue on another thread.
            return (await this.validator.ValidateAsync(this.subject, CancellationToken.None, propertyName).ConfigureAwait(false))
                .Errors.Select(x => x.ErrorMessage);
        }

        public async Task<Dictionary<string, IEnumerable<string>>> ValidateAllPropertiesAsync()
        {
            // If someone's calling us synchronously, and ValidationAsync does not complete synchronously,
            // we'll deadlock unless we continue on another thread.
            return (await this.validator.ValidateAsync(this.subject).ConfigureAwait(false))
                .Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(failure => failure.ErrorMessage));
        }
    }
}
