using EnvDTE;
using System;
using TestScaffolderExtension.Extensions;

namespace TestScaffolderExtension.Models
{
    public class CurrentSelection
    {
        public CurrentSelection(TextSelection selectedText)
        {
            SelectedMethod = selectedText?.ActivePoint.CodeElement[vsCMElement.vsCMElementFunction] as CodeFunction;
            SelectedClass = selectedText?.ActivePoint.CodeElement[vsCMElement.vsCMElementClass] as CodeClass;
        }

        public CodeFunction SelectedMethod { get; }
        public CodeClass SelectedClass { get; }
    }
}