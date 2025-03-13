
namespace MerckCuida.Web.ValueObjects
{
    public class FormResult
    {
        public bool HasMessage { get { return Validations.Any(x => x.Value.Any()); } }
        public Dictionary<string, List<string>> Validations { get; set; }

        public FormResult()
        {
            this.Validations = new Dictionary<string, List<string>>();
        }

        public ValidationField WithField(string key)
        {
            return new ValidationField(this, key);
        }


        public class ValidationField
        {
            private FormResult Result;
            public string Key { get; init; }

            public ValidationField(FormResult result, string key)
            {
                Result = result;
                Key = key;
            }

            public ValidationField Add(string validation)
            {
                if (!Result.Validations.TryGetValue(Key, out var values))
                {
                    values = new List<string>();
                    Result.Validations.Add(Key, values);
                }

                values.Add(validation);
                return this;
            }

            public bool Any()
            {
                if (!Result.Validations.TryGetValue(Key, out var values))
                    return false;

                return values.Any();
            }
        }
    }
}