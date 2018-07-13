using EnvDTE;
using System;
using TestScaffolderExtension.Models.Solution;

namespace TestScaffolderExtension.Models
{
    public static class SolutionModelFactory
    {
        private const string ProjectKind = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        private const string SolutionFolderKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
        private const string FileKind = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        private const string ProjectFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        private const string SubProjectKind = "{66A26722-8FB5-11D2-AA7E-00C04F688DDE}"; // what makes this different from ProjectKind????
        private const string MiscellaneousFilesProjectKind = "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}"; // "created" automatically when non-solution files are opened in the solution context. Can be ignored.

        public static SolutionModelBase BuildHierarchyPathUp(object item)
        {
            if (item is Project project)
            {
                return BuildHierarchyPathUp(project);
            }
            else if (item is ProjectItem projectItem)
            {
                return BuildHierarchyPathUp(projectItem);
            }
            else
            {
                throw new ArgumentException($"Unable to build hierarchy path for item {item}.");
            }
        }

        public static SolutionModelBase BuildHierarchyTreeDown(SolutionModelBase parent, Project project)
        {
            var current = BuildCurrentNode(parent, project);
            current.IterateChildren();
            return current;
        }

        public static SolutionModelBase BuildHierarchyTreeDown(SolutionModelBase parent, ProjectItem projectItem)
        {
            var current = BuildCurrentNode(parent, projectItem);
            current.IterateChildren();
            return current;
        }

        private static SolutionModelBase BuildHierarchyPathUp(Project project)
        {
            SolutionModelBase parent = null;

            var parentProject = project.ParentProjectItem;

            if (parentProject != null)
            {
                if (parentProject.Kind == SubProjectKind)
                {
                    parent = BuildHierarchyPathUp(parentProject.Collection.Parent);
                }
                else
                {
                    parent = BuildHierarchyPathUp(parentProject);
                }
            }

            return BuildCurrentNode(parent, project);
        }

        private static SolutionModelBase BuildHierarchyPathUp(ProjectItem projectItem)
        {
            SolutionModelBase parent = null;

            var parentItem = projectItem.Collection.Parent;
            if (parentItem != null)
            {
                parent = BuildHierarchyPathUp(parentItem);
            }

            return BuildCurrentNode(parent, projectItem);
        }

        private static SolutionModelBase BuildCurrentNode(SolutionModelBase parent, Project project)
        {
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

        private static SolutionModelBase BuildCurrentNode(SolutionModelBase parent, ProjectItem projectItem)
        {
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