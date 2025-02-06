using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Trees;

namespace TreeMage.Core.Drawing
{
    internal class PngDrawer : ITreeDrawer
    {
        private readonly PositionManager positionManager = new PositionManager();

        PositionManager ITreeDrawer.PositionManager => positionManager;

        void ITreeDrawer.InitDocument()
        {
        }

        /// <inheritdoc/>
        public void BeginTree(TMSize size, Tree tree)
        {
        }

        /// <inheritdoc/>
        public void DrawBranchDecoration(Clade target, BranchDecorationStyle style)
        {
        }

        public void DrawBranchValue(string value, TMPoint point, TMColor fill, int fontSize)
        {
        }

        public void DrawCladeLabel(string cladeName, TMPoint lineBegin, TMPoint lineEnd, TMPoint testPoint, int lineThickness, int fontSize)
        {
        }

        public void DrawCladeShade(TMRect area, TMColor fill)
        {
        }

        public void DrawCollapsedTriangle(TMPoint left, TMPoint rightTop, TMPoint rightBottom, TMColor stroke, int lineThickness)
        {
        }

        public void DrawHorizontalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness)
        {
        }

        public void DrawLeafLabel(string taxon, TMPoint point, TMColor fill, int fontSize)
        {
        }

        public void DrawNodeValue(string value, TMPoint point, TMColor fill, int fontSize)
        {
        }

        public void DrawScalebar(double value, TMPoint offset, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int fontSize, int lineThickness)
        {
        }

        public void DrawVerticalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness)
        {
        }

        void ITreeDrawer.FinishTree()
        {
        }
    }
}
