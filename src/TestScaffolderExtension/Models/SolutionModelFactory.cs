namespace TestScaffolderExtension.Models
{
    using System;
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using TestScaffolderExtension.Models.Solution;

    public static class SolutionModelFactory
    {
        private const string ProjectKind = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        private const string SolutionFolderKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
        private const string FileKind = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        private const string ProjectFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        private const string SubProjectKind = "{66A26722-8FB5-11D2-AA7E-00C04F688DDE}"; // what makes this different from ProjectKind????
        private const string MiscellaneousFilesProjectKind = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}"; // "created" automatically when non-solution files are opened in the solution context. Can be ignored.

        public static async Task<SolutionModelBase> BuildHierarchyPathUpAsync(object item)
        {
            if (item is Project project)
            {
                return await BuildHierarchyPathUpAsync(project);
            }
            else if (item is ProjectItem projectItem)
            {
                return await BuildHierarchyPathUpAsync(projectItem);
            }
            else
            {
                throw new ArgumentException($"Unable to build hierarchy path for item {item}.");
            }
        }

        public static async Task<SolutionModelBase> BuildHierarchyTreeDownAsync(SolutionModelBase parent, Project project)
        {
            var current = await BuildCurrentNodeAsync(parent, project);
            await current.IterateChildrenAsync();
            return current;
        }

        public static async Task<SolutionModelBase> BuildHierarchyTreeDownAsync(SolutionModelBase parent, ProjectItem projectItem)
        {
            var current = await BuildCurrentNodeAsync(parent, projectItem);
            await current.IterateChildrenAsync();
            return current;
        }

        private static async Task<SolutionModelBase> BuildHierarchyPathUpAsync(Project project)
        {
            SolutionModelBase parent = null;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var parentProject = project.ParentProjectItem;

            if (parentProject != null)
            {
                if (parentProject.Kind == SubProjectKind)
                {
                    parent = await BuildHierarchyPathUpAsync(parentProject.Collection.Parent);
                }
                else
                {
                    parent = await BuildHierarchyPathUpAsync(parentProject);
                }
            }

            return await BuildCurrentNodeAsync(parent, project);
        }

        private static async Task<SolutionModelBase> BuildHierarchyPathUpAsync(ProjectItem projectItem)
        {
            SolutionModelBase parent = null;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var parentItem = projectItem.Collection.Parent;
            if (parentItem != null)
            {
                parent = await BuildHierarchyPathUpAsync(parentItem);
            }

            return await BuildCurrentNodeAsync(parent, projectItem);
        }

        private static async Task<SolutionModelBase> BuildCurrentNodeAsync(SolutionModelBase parent, Project project)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            switch (project.Kind)
            {
                case ProjectKind:
                    return new ProjectModel(parent, project);
                case SolutionFolderKind:
                    return new SolutionFolderModel(parent, project);
                case MiscellaneousFilesProjectKind:
                    return null;
                default:
                    throw new ArgumentException($"Project kind {project.Kind} is not supported for project {project.Name}.");
            }
        }

        private static async Task<SolutionModelBase> BuildCurrentNodeAsync(SolutionModelBase parent, ProjectItem projectItem)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            switch (projectItem.Kind)
            {
                case FileKind:
                    return new FileModel(parent, projectItem);
                case ProjectFolder:
                    return new ProjectFolderModel(parent, projectItem);
                case SubProjectKind:
                    if (projectItem.SubProject != null)
                    {
                        if (projectItem.SubProject.CodeModel != null)
                        {
                            return new ProjectModel(parent, projectItem.SubProject);
                        }

                        return new SolutionFolderModel(parent, projectItem.SubProject);
                    }

                    return new FileModel(parent, projectItem);
                case MiscellaneousFilesProjectKind:
                    return null;
                default:
                    throw new ArgumentException($"Project item kind {projectItem.Kind} is not supported for item {projectItem.Name}.");
            }
        }
    }
}