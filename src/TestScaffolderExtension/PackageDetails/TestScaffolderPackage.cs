﻿namespace TestScaffolderExtension.PackageDetails
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using TestScaffolderExtension.Commands;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(EditorWindowContextGuid, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideUIContextRule(
        EditorWindowContextGuid,
        name: "Test Scaffolder Package Load",
        expression: "CSharpEditorWindow | SingleSolutionItemSelected",
        termNames: new[] { "CSharpEditorWindow", "SingleSolutionItemSelected" },
        termValues: new[] { "ActiveEditorContentType:CSharp", "ActiveProjectFlavor:FAE04EC0-301F-11D3-BF4B-00C04F79EFBC" })]
    public sealed class TestScaffolderPackage : AsyncPackage
    {
        /// <summary>
        /// TestScaffolderPackage GUID string.
        /// </summary>
        private const string PackageGuidString = "6b0cf9bc-35f3-4637-9b8c-34f63c6b00ef";
        private const string EditorWindowContextGuid = "DCAB817C-68D8-49E0-92DA-435D12F840D0";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await CreateUnitTestsForMethodCommand.InitializeAsync(this);
            await CreateUIAutomationTestsCommand.InitializeAsync(this);
        }
    }
}