using System;

namespace server.core.Domain.Tasks.Helpers
{
    /// <summary>
    ///     Helper class to perform EF many-to-many relations mapping
    /// </summary>
    public class QuizTask
    {
        public Guid QuizId { get; set; }

        public Quiz Quiz { get; set; }

        public Guid TaskId { get; set; }

        public VariantTask Task { get; set; }
    }
}
