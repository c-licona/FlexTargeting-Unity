namespace Cyclic.FlexTargeting.Highlight
{
    /// <summary>
    /// Highlighting a target is a common thing to do with Flex Targeting. This and the <see cref="IHighlightNode"/>
    /// interface are nice-to-haves for getting started putting together highlighting functionality for targets.
    ///
    /// Implementations handle what to do when they have been requested to highlight a target.
    /// </summary>
    public interface IHighlighter
    {
        /// <summary>
        /// Whether this highlighter can currently highlight the target.
        /// </summary>
        bool CanHighlight { get; }

        /// <summary>
        /// Highlight the target for the requested amount of time.
        /// </summary>
        /// <param name="requestedSeconds">The requested amount of time to highlight the target for.</param>
        void Highlight(float requestedSeconds = 0.0f);
    }

    /// <summary>
    /// Highlighting a target is a common thing to do with Flex Targeting. This and the <see cref="IHighlighter"/>
    /// interface are nice-to-haves for getting started putting together highlighting functionality for targets.
    ///
    /// Provides a easy fire and forget method for attempting to highlight a target. Nodes can be strung together to
    /// pass along a request. For example, you may have multiple targets on an entity, that all link together to a
    /// single hub that handles highlighting functionality for the entity as a whole.
    /// </summary>
    public interface IHighlightNode
    {
        /// <summary>
        /// Try to highlight the target. An optional amount of time can be passed along as a request for highlighting
        /// the target for that amount of time.
        /// </summary>
        /// <param name="requestedSeconds">The requested amount of time to highlight the target for.</param>
        void TryHighlight(float requestedSeconds = 0.0f);
    }
}
