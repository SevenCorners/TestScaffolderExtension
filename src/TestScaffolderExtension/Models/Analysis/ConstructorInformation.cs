using System;
using System.Collections.Generic;
using System.Linq;

namespace TestScaffolderExtension.Models.Analysis
{
    public class ConstructorInformation
    {
        public ConstructorInformation(string className, ConstructorType constructorType)
        {
            _className = className;
            _constructorType = constructorType;
            Parameters = new List<ParameterInformation>();
        }

        public ConstructorInformation(string className, ConstructorType constructorType, List<ParameterInformation> parameters)
        {
            _className = className;
            _constructorType = constructorType;
            Parameters = parameters;
        }

        public string GetConstructorStringWithMockParameters(Func<ParameterInformation, string> parameterNameFunc)
        {
            if (_constructorType == ConstructorType.Default && !Parameters.Any())
            {
                return $"default({_className})";
            }

            return $"new {_className}({string.Join(", ", Parameters.Select(p => $"{parameterNameFunc(p)}.Object"))})";
        }

        public string GetConstructorString(Func<ParameterInformation, string> parameterNameFunc)
        {
            if (_constructorType == ConstructorType.Default && !Parameters.Any())
            {
                return $"default({_className})";
            }

            return $"new {_className}({string.Join(", ", Parameters.Select(parameterNameFunc))})";
        }

        private readonly ConstructorType _constructorType;
        private readonly string _className;
        public List<ParameterInformation> Parameters { get; }
    }
}