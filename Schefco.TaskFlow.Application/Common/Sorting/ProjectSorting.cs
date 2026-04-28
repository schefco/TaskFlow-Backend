using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Common.Sorting
{
    public static class ProjectSorting
    {
        // Entry point for sorting logic.
        // The handler will call this method and pass in:
        // - the list of projects
        // - the field to sort by (SortBy)
        // - the direction ("asc" or "desc")

        public static List<Project> SortProjects(List<Project> projects, string? sortBy, string? direction)
        {
            // Normalize the values to lowercase for comparison.
            var key = sortBy?.ToLower();
            direction = direction?.ToLower();

            // If for whatever reason this is called with a null list
            if (projects == null || projects.Count == 0)
                return new List<Project>(); // Return an Empty List

            return key switch
            {
                "name" => SortByName(projects, direction),
                "duedate" => SortByDueDate(projects, direction),
                "createdat" => SortByCreatedAt(projects, direction),
                "updatedat" => SortByUpdatedAt(projects, direction),
                "priority" => SortByPriority(projects, direction),
                "status" => SortByStatus(projects, direction),

                // Default sort: newest projects first
                _ => SortByCreatedAt(projects, "desc")
            };
        }


        // Sort by project name (A-Z or Z-A)
        private static List<Project> SortByName(List<Project> projects, string? direction)
        {
            return direction == "desc" 
                ? projects.OrderByDescending(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList()
                : projects.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // Sort by due date
        // Null due dates are pushed to the bottom
        private static List<Project> SortByDueDate(List<Project> projects, string? direction)
        {
            return direction == "desc"
                ? projects.OrderByDescending(p => p.DueDate.HasValue).ThenByDescending(p => p.DueDate).ToList()
                : projects.OrderBy(p => p.DueDate.HasValue).ThenBy(p => p.DueDate).ToList();
        }

        // Sort by creation date
        private static List<Project> SortByCreatedAt(List<Project> projects, string? direction)
        {
            return direction == "asc"
                ? projects.OrderBy(p => p.CreatedAt).ToList()
                : projects.OrderByDescending(p => p.CreatedAt).ToList();
        }

        // Sort by updated timestamp
        private static List<Project> SortByUpdatedAt(List<Project> projects, string? direction)
        {
            return direction == "asc"
                ? projects.OrderBy(p => p.UpdatedAt).ToList()
                : projects.OrderByDescending(p => p.UpdatedAt).ToList();
        }

        // Sort by priority (enum or int)
        private static List<Project> SortByPriority(List<Project> projects, string? direction)
        {
            return direction == "asc"
                ? projects.OrderBy(p => p.Priority).ToList()
                : projects.OrderByDescending(p => p.Priority).ToList();
        }

        // Sort by status (enum or int)
        private static List<Project> SortByStatus(List<Project> projects, string? direction)
        {
            return direction == "asc"
                ? projects.OrderBy(p => p.Status).ToList()
                : projects.OrderByDescending(p => p.Status).ToList();
        }
    }
}

