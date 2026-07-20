using System.ComponentModel.DataAnnotations;

namespace Financas.Domain.Validations 
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        private string PropertyName { get; }
        private object DesiredValue { get; }

        /// <summary>
        /// Torna o campo obrigatório se outra propriedade tiver o valor esperado.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade dependente (ex: "isSubscription")</param>
        /// <param name="desiredValue">Valor que ativa a obrigatoriedade (padrão é true)</param>
        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredValue;
        }

        /// <summary>
        /// Atalho simples para propriedades booleanas onde o valor esperado é true.
        /// </summary>
        public RequiredIfAttribute(string propertyName)
            : this(propertyName, true)
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();
            var property = type.GetProperty(PropertyName);

            if (property == null)
            {
                return new ValidationResult($"Propriedade desconhecida: {PropertyName}");
            }

            var propertyValue = property.GetValue(instance, null);

            bool isRequired = Equals(propertyValue, DesiredValue);

            if (isRequired)
            {
                if (value == null || IsDefaultValue(value))
                {
                    return new ValidationResult(ErrorMessage ?? $"O campo {validationContext.DisplayName} é obrigatório.");
                }
            }

            return ValidationResult.Success;
        }

        private static bool IsDefaultValue(object value)
        {
            if (value is string str)
                return string.IsNullOrWhiteSpace(str);

            var valueType = value.GetType();
            if (valueType.IsValueType)
            {
                var defaultValue = Activator.CreateInstance(valueType);
                return value.Equals(defaultValue);
            }

            return false;
        }
    }
}