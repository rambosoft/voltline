using System.Collections.Generic;
using System.Linq;

namespace Voltline.Data
{
    public sealed class ConfigValidationResult
    {
        private readonly List<string> errors = new();

        public bool IsValid => errors.Count == 0;

        public IReadOnlyList<string> Errors => errors;

        public void Add(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            errors.Add(message);
        }

        public void AddRange(IEnumerable<string> messages)
        {
            if (messages == null)
            {
                return;
            }

            foreach (string message in messages.Where(static message => !string.IsNullOrWhiteSpace(message)))
            {
                errors.Add(message);
            }
        }

        public override string ToString()
        {
            return string.Join("\n", errors);
        }
    }
}