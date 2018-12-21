namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ConstructorInformation
    {
        private readonly string className;

        private readonly ConstructorType constructorType;

        public ConstructorInformation(string className, ConstructorType constructorType)
        {
            this.className = className;
            this.constructorType = constructorType;
            this.Parameters = new List<ParameterInformation>();
        }

        public ConstructorInformation(string className, ConstructorType constructorType, List<ParameterInformation> parameters)
        {
            this.className = className;
            this.constructorType = constructorType;
            this.Parameters = parameters;
        }

        public List<ParameterInformation> Parameters { get; }

        public string GetConstructorString(Func<ParameterInformation, string> parameterNameFunc)
        {
            if (this.constructorType == ConstructorType.Default && !this.Parameters.Any())
            {
                return $"default({this.className})";
            }

            return $"new {this.className}({string.Join(", ", this.Parameters.Select(parameterNameFunc))})";
        }

        public string GetConstructorStringWithMockParameters(Func<ParameterInformation, string> parameterNameFunc)
        {
            if (this.constructorType == ConstructorType.Default && !this.Parameters.Any())
            {
                return $"default({this.className})";
            }

            return $"new {this.className}({string.Join(", ", this.Parameters.Select(p => $"{parameterNameFunc(p)}.Object"))})";
        }
    }
}