using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics;
using TreeMage.Core.Drawing;
using TreeMage.Core.Drawing.Styles;
using TreeMage.Core.Trees;
using TreeMage.Data;

namespace TreeMage.Components.Shared
{
    public partial class TreeView : ITreeDrawer
    {
        private readonly PositionManager positionManager = new();

        private RenderTreeBuilder builder = default!;

        PositionManager ITreeDrawer.PositionManager => positionManager;

        void ITreeDrawer.InitDocument()
        {
        }

        void ITreeDrawer.BeginTree(TMSize size, Tree tree)
        {
            builder.OpenElement(0, "svg");
            builder.AddAttribute(1, "width", size.Width);
            builder.AddAttribute(2, "height", size.Height);

            builder.OpenRegion(3);

            builder.OpenElement(0, "style");
            builder.AddAttribute(1, "type", "text/css");
            builder.AddMarkupContent(2,
                @$"""
                     text {{
                         font-family: {FontManager.DefaultFontFamily};
                     }}

                     .leaf {{
                         font-size: {tree.Style.LeafLabelsFontSize}px;
                     }}

                     .clade-label {{
                         font-size: {tree.Style.CladeLabelsFontSize}px;
                     }}

                     .node-value {{
                         font-size: {tree.Style.NodeValueFontSize}px;
                     }}

                     .branch-value {{
                         font-size: {tree.Style.BranchValueFontSize}px;
                     }}

                     .branch {{
                         fill: none;
                         stroke-width: {tree.Style.BranchThickness}px;
                     }}

                     .collapsed-triangle {{
                         fill: none;
                         stroke-width: {tree.Style.BranchThickness}px;
                     }}

                     .scalebar-line {{
                         fill: none;
                         stroke: black;
                         stroke-width: {tree.Style.ScaleBarThickness}px;
                     }}

                     .scalebar-text {{
                         font-size: {tree.Style.ScaleBarFontSize}px;
                     }}

                     .clickable-leaf {{
                         stroke: none;
                     }}

                     .clickable-branch {{
                         fill: none;
                     }}
                     """);
            builder.CloseElement(); // style

            builder.OpenElement(3, "rect");
            builder.AddAttribute(4, "width", "100%");
            builder.AddAttribute(5, "height", "100%");
            builder.AddAttribute(6, "style", "fill: transparent; stroke: none");
            builder.AddAttribute(7, "onclick", ViewModel.UnfocusAllCommand.ToDelegate());
            builder.CloseElement(); // rect

            builder.CloseRegion();

            // tree area
            builder.OpenElement(4, "g");
            builder.AddAttribute(5, "transform", "translate(50, 50)");
        }

        void ITreeDrawer.DrawCladeShade(TMRect area, TMColor fill, Clade target)
        {
            builder.OpenRegion(6);

            builder.OpenElement(0, "rect");
            builder.AddAttribute(1, "x", area.X);
            builder.AddAttribute(2, "y", area.Y);
            builder.AddAttribute(3, "width", area.Width);
            builder.AddAttribute(4, "height", area.Height);
            builder.AddAttribute(5, "fill", fill.Value);
            builder.CloseElement();

            builder.CloseRegion();
        }

        void ITreeDrawer.DrawCollapsedTriangle(TMPoint left, TMPoint rightTop, TMPoint rightBottom, TMColor stroke, int lineThickness, Clade target)
        {
            builder.OpenRegion(7);

            builder.OpenElement(8, "path");
            builder.AddAttribute(9, "class", "collapsed-triangle");
            builder.AddAttribute(10, "d", $"""
                                     M {left.X} {left.Y}
                                     L {rightTop.X} {rightTop.Y}
                                     L {rightBottom.X} {rightBottom.Y}
                                     L {left.X} {left.Y}
                """);
            builder.AddAttribute(11, "stroke", stroke.Value);
            builder.CloseElement();

            builder.CloseRegion();
        }

        void ITreeDrawer.DrawLeafLabel(string taxon, TMRect area, TMColor fill, int fontSize, Clade target)
        {
            builder.OpenRegion(8);

            builder.OpenElement(0, "text");
            builder.AddAttribute(1, "class", "leaf");
            builder.AddAttribute(2, "x", area.X);
            builder.AddAttribute(3, "y", area.Y);
            builder.AddAttribute(4, "fill", fill.Value);
            builder.AddContent(5, taxon);
            builder.CloseElement();

            // クリックエリア
            if (ViewModel.EditMode.Value is TreeEditMode.Select)
            {
                CladeId id = target.GetId(CladeIdSuffix.Leaf);
                string fillColor = ViewModel.FocusedSvgElementIdList.Contains(id) ? "#A0D8EFC4" : "transparent";
                const int margin = 3;
                builder.OpenElement(6, "rect");
                builder.AddAttribute(7, "id", id);
                builder.AddAttribute(8, "class", "clickable-leaf");
                builder.AddAttribute(9, "x", area.X - margin);
                builder.AddAttribute(10, "y", area.Y - area.Height - margin);
                builder.AddAttribute(11, "width", area.Width + margin * 2);
                builder.AddAttribute(12, "height", area.Height + margin * 2);
                builder.AddAttribute(13, "fill", fillColor);
                builder.AddAttribute(14, "onclick", ViewModel.SvgElementClickedCommand.ToDelegate(id));
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        void ITreeDrawer.DrawNodeValue(string value, TMPoint point, TMColor fill, int fontSize, Clade target)
        {
            builder.OpenRegion(9);

            builder.OpenElement(0, "text");
            builder.AddAttribute(1, "class", "node-value");
            builder.AddAttribute(2, "x", point.X);
            builder.AddAttribute(3, "y", point.Y);
            builder.AddAttribute(4, "fill", fill.Value);
            builder.AddContent(5, value);
            builder.CloseElement();

            builder.CloseRegion();
        }

        void ITreeDrawer.DrawBranchValue(string value, TMPoint point, TMColor fill, int fontSize, Clade target)
        {
            builder.OpenRegion(10);

            builder.OpenElement(0, "text");
            builder.AddAttribute(1, "class", "branch-value");
            builder.AddAttribute(2, "x", point.X);
            builder.AddAttribute(3, "y", point.Y);
            builder.AddAttribute(4, "fill", fill.Value);
            builder.AddContent(5, value);
            builder.CloseElement();

            builder.CloseRegion();
        }

        void ITreeDrawer.DrawCladeLabel(string cladeName, TMPoint lineBegin, TMPoint lineEnd, TMPoint testPoint, int lineThickness, int fontSize, Clade target)
        {
            builder.OpenRegion(11);

            builder.OpenElement(0, "line");
            builder.AddAttribute(1, "x1", lineBegin.X);
            builder.AddAttribute(2, "x2", lineEnd.X);
            builder.AddAttribute(3, "y1", lineBegin.Y);
            builder.AddAttribute(4, "y2", lineEnd.Y);
            builder.AddAttribute(5, "stroke", "black");
            builder.AddAttribute(6, "stroke-width", lineThickness);
            builder.CloseElement();

            builder.OpenElement(7, "text");
            builder.AddAttribute(8, "class", "clade-label");
            builder.AddAttribute(9, "x", testPoint.X);
            builder.AddAttribute(10, "y", testPoint.Y);
            builder.AddContent(11, cladeName);
            builder.CloseElement();

            builder.CloseRegion();
        }

        void ITreeDrawer.DrawHorizontalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness, Clade target)
        {
            builder.OpenRegion(12);

            builder.OpenElement(0, "line");
            builder.AddAttribute(1, "class", "branch");
            builder.AddAttribute(2, "x1", parentPoint.X);
            builder.AddAttribute(3, "y1", parentPoint.Y);
            builder.AddAttribute(4, "x2", childPoint.X);
            builder.AddAttribute(5, "y2", childPoint.Y);
            builder.AddAttribute(6, "stroke", stroke.Value);
            builder.CloseElement();

            builder.CloseRegion();
        }

        void ITreeDrawer.DrawVerticalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness, Clade target)
        {
            builder.OpenRegion(13);

            builder.OpenElement(0, "line");
            builder.AddAttribute(1, "class", "branch");
            builder.AddAttribute(2, "x1", parentPoint.X);
            builder.AddAttribute(3, "y1", parentPoint.Y);
            builder.AddAttribute(4, "x2", childPoint.X);
            builder.AddAttribute(5, "y2", childPoint.Y);
            builder.AddAttribute(6, "stroke", stroke.Value);
            builder.CloseElement();

            builder.CloseRegion();
        }

        void ITreeDrawer.DrawBranchDecoration(Clade target, BranchDecorationStyle style)
        {
            builder.OpenRegion(14);

            string color = style.ShapeColor;
            switch (style.DecorationType)
            {
                case BranchDecorationType.ClosedCircle:
                case BranchDecorationType.OpenCircle:
                    {
                        (TMPoint center, double radius) = positionManager.CalcBranchDecorationCircleArea(target, style);
                        string fill, stroke;
                        if (style.DecorationType == BranchDecorationType.ClosedCircle)
                        {
                            fill = color;
                            stroke = "none";
                        }
                        else
                        {
                            fill = "white";
                            stroke = color;
                        }
                        builder.OpenElement(0, "circle");
                        builder.AddAttribute(1, "cx", center.X);
                        builder.AddAttribute(2, "cy", center.Y);
                        builder.AddAttribute(3, "r", radius);
                        builder.AddAttribute(4, "stroke", stroke);
                        builder.AddAttribute(5, "fill", fill);
                        builder.CloseElement();
                    }
                    break;

                case BranchDecorationType.ClosedRectangle:
                case BranchDecorationType.OpenedRectangle:
                    {
                        (double x, double y, double width, double height) = positionManager.CalcBranchDecorationRectangleArea(target, style);
                        string fill, stroke;
                        if (style.DecorationType == BranchDecorationType.ClosedRectangle)
                        {
                            fill = color;
                            stroke = "none";
                        }
                        else
                        {
                            fill = "white";
                            stroke = color;
                        }
                        builder.OpenElement(0, "rect");
                        builder.AddAttribute(1, "x", x);
                        builder.AddAttribute(2, "y", y);
                        builder.AddAttribute(3, "width", width);
                        builder.AddAttribute(4, "height", height);
                        builder.AddAttribute(5, "stroke", stroke);
                        builder.AddAttribute(6, "fill", fill);
                        builder.CloseElement();
                    }
                    break;
            }

            builder.CloseRegion();
        }

        void ITreeDrawer.OnCladeDrawn(Clade target)
        {
            Tree? tree = ViewModel.TargetTree.Value;
            Debug.Assert(tree is not null);

            // Reroot用クリックエリア
            if (ViewModel.EditMode.Value is TreeEditMode.Reroot && !target.IsRoot)
            {
                builder.OpenRegion(15);

                // Rooted
                if (!target.IsLeaf && (tree.IsUnrooted || !target.Parent.IsRoot))
                {
                    const double size = 5;
                    double x = (positionManager.CalcX1(target) + positionManager.CalcX2(target)) / 2 - size;
                    double y = positionManager.CalcY1(target) - size;
                    builder.OpenElement(0, "rect");
                    builder.AddAttribute(1, "x", x);
                    builder.AddAttribute(2, "y", y);
                    builder.AddAttribute(3, "width", size * 2);
                    builder.AddAttribute(4, "height", size * 2);
                    builder.AddAttribute(5, "style", "stroke: black; stroke-width: 2px; fill: white");
                    builder.AddAttribute(6, "onclick", ViewModel.RerootCommand.ToDelegate((target, true)));
                    builder.CloseElement();
                }

                // Unrooted
                if (!target.GetIsExternal())
                {
                    CladeId id = target.GetId(CladeIdSuffix.Node);
                    (double x, double y, double width, double height) = positionManager.CalcNodeDecorationRectangleArea(target);
                    builder.OpenElement(7, "rect");
                    builder.AddAttribute(8, "id", id);
                    builder.AddAttribute(9, "x", x);
                    builder.AddAttribute(10, "y", y);
                    builder.AddAttribute(11, "width", width);
                    builder.AddAttribute(12, "height", height);
                    builder.AddAttribute(13, "style", "fill: black; stroke: transparent; stroke-width: 2px");
                    builder.AddAttribute(14, "onclick", ViewModel.RerootCommand.ToDelegate((target, false)));
                    builder.CloseElement();
                }

                builder.CloseRegion();
            }

            // Swap用クリックエリア
            if (ViewModel.EditMode.Value is TreeEditMode.Swap && !target.IsRoot)
            {
                CladeId id = target.GetId(CladeIdSuffix.Node);
                int index = target.Parent.Children.IndexOf(target);
                if (index < target.Parent.Children.Count - 1)
                {
                    Clade sister = target.Parent.Children[index + 1];
                    double x = positionManager.CalcX1(target) - 5;
                    double y = (positionManager.CalcY1(target) + positionManager.CalcY1(sister)) / 2 - 5;

                    builder.OpenRegion(16);

                    builder.OpenElement(0, "rect");
                    builder.AddAttribute(1, "id", id);
                    builder.AddAttribute(2, "x", x);
                    builder.AddAttribute(3, "y", y);
                    builder.AddAttribute(4, "width", "10");
                    builder.AddAttribute(5, "height", "10");
                    builder.AddAttribute(6, "style", "fill: black; stroke: transparent; stroke-width: 2px");
                    builder.AddAttribute(7, "onclick", ViewModel.SwapSisterCommand.ToDelegate((target, sister)));
                    builder.CloseElement();

                    builder.CloseRegion();
                }
            }

            // Subtree用クリックエリア
            if (ViewModel.EditMode.Value is TreeEditMode.Subtree && !target.IsRoot && !target.IsLeaf)
            {
                const double size = 5;
                double x = (positionManager.CalcX1(target) + positionManager.CalcX2(target)) / 2 - size;
                double y = positionManager.CalcY1(target) - size;

                builder.OpenRegion(17);

                builder.OpenElement(0, "rect");
                builder.AddAttribute(1, "x", x);
                builder.AddAttribute(2, "y", y);
                builder.AddAttribute(3, "width", size * 2);
                builder.AddAttribute(4, "height", size * 2);
                builder.AddAttribute(5, "style", "stroke: transparent; stroke-width: 2px; fill: black");
                builder.AddAttribute(6, "onclick", ViewModel.ExtractSubtreeCommand.ToDelegate(target));
                builder.CloseElement();

                builder.CloseRegion();
            }

            // 枝クリックエリア
            if (ViewModel.EditMode.Value is TreeEditMode.Select)
            {
                double x2 = positionManager.CalcX2(target);
                if (!double.IsNaN(x2))
                {
                    double x1 = positionManager.CalcX1(target);
                    double y1 = positionManager.CalcY1(target);
                    double y2 = positionManager.CalcY2(target);
                    string path = $"M {x2} {y1}";
                    if (x1 != x2) path += $"H {x1}";
                    if (y1 != y2) path += $"V {y2}";
                    CladeId id = target.GetId(CladeIdSuffix.Branch);
                    string strokeColor = ViewModel.FocusedSvgElementIdList.Contains(id) ? "#A0D8EFC4" : "transparent";

                    builder.OpenRegion(18);

                    builder.OpenElement(0, "path");
                    builder.AddAttribute(1, "id", id);
                    builder.AddAttribute(2, "class", "clickable-branch");
                    builder.AddAttribute(3, "d", path);
                    builder.AddAttribute(4, "stroke", strokeColor);
                    builder.AddAttribute(5, "style", $"stroke-width: {tree.Style.BranchThickness + 4}px");
                    builder.AddAttribute(6, "onclick", ViewModel.SvgElementClickedCommand.ToDelegate(id));
                    builder.CloseElement();

                    builder.CloseRegion();
                }
            }
        }

        void ITreeDrawer.DrawScalebar(double value, TMPoint offset, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int fontSize, int lineThickness)
        {
            builder.CloseElement(); // tree area

            builder.OpenRegion(19);

            builder.OpenElement(0, "g");
            builder.AddAttribute(1, "transform", $"translate({offset.X}, {offset.Y})");

            builder.OpenElement(2, "text");
            builder.AddAttribute(3, "class", "scalebar-text");
            builder.AddAttribute(4, "x", textPoint.X);
            builder.AddAttribute(5, "y", textPoint.Y);
            builder.AddAttribute(6, "text-anchor", "middle");
            builder.AddContent(7, value);
            builder.CloseElement(); // text

            builder.OpenElement(8, "line");
            builder.AddAttribute(9, "class", "scalebar-line");
            builder.AddAttribute(10, "x1", lineBegin.X);
            builder.AddAttribute(11, "y1", lineBegin.Y);
            builder.AddAttribute(12, "x2", lineEnd.X);
            builder.AddAttribute(13, "y2", lineEnd.Y);
            builder.CloseElement(); // line

            builder.CloseRegion();
        }

        void ITreeDrawer.FinishTree()
        {
            builder.CloseElement(); // g
            builder.CloseElement(); // svg
        }
    }
}
