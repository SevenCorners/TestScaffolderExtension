namespace TestScaffolderExtension.Common.Command
{
    using System;

    [Serializable]
    internal class CommandException : Exception
    {
        public CommandException(string title, string message)
            : base(message)
        {
            this.Title = title;
        }

        public CommandException(string title, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Title = title;
        }

        public string Title { get; set; }
    }
}