using FilterExpressionCreator.Mvc.Attributes;
using System;

namespace FilterExpressionCreator.Demo.Models
{
    /// <summary>
    /// Project.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Project title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the project.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Identifier of the owning <see cref="Freelancer"/>.
        /// </summary>
        [Filter(Visible = false)]
        public Guid FreelancerId { get; set; }
    }
}
