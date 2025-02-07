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
        private int __elementCount;

        PositionManager ITreeDrawer.PositionManager => positionManager;

#pragma warning disable ASP0006 // Do not use non-literal sequence numbers

        void ITreeDrawer.InitDocument()
        {
            __elementCount = 1;
        }

        void ITreeDrawer.BeginTree(TMSize size, Tree tree)
        {
            builder.OpenElement(++__elementCount, "svg");
            builder.AddAttribute(++__elementCount, "width", size.Width);
            builder.AddAttribute(++__elementCount, "height", size.Height);

            builder.OpenElement(++__elementCount, "style");
            builder.AddAttribute(++__elementCount, "type", "text/css");
            builder.AddMarkupContent(++__elementCount,
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

                     .collapsed-rectangle {{
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

            builder.OpenElement(++__elementCount, "rect");
            builder.AddAttribute(++__elementCount, "width", "100%");
            builder.AddAttribute(++__elementCount, "height", "100%");
            builder.AddAttribute(++__elementCount, "style", "fill: transparent; stroke: none");
            builder.AddAttribute(++__elementCount, "onclick", ViewModel.UnfocusAllCommand.ToDelegate());
            builder.CloseElement(); // rect

            // tree area
            builder.OpenElement(++__elementCount, "g");
            builder.AddAttribute(++__elementCount, "transform", "translate(50, 50)");
        }

        void ITreeDrawer.DrawCladeShade(TMRect area, TMColor fill, Clade target)
        {
            builder.OpenElement(++__elementCount, "rect");
            builder.AddAttribute(++__elementCount, "x", area.X);
            builder.AddAttribute(++__elementCount, "y", area.Y);
            builder.AddAttribute(++__elementCount, "width", area.Width);
            builder.AddAttribute(++__elementCount, "height", area.Height);
            builder.AddAttribute(++__elementCount, "fill", fill.Value);
            builder.CloseElement();
        }

        void ITreeDrawer.DrawCollapsedTriangle(TMPoint left, TMPoint rightTop, TMPoint rightBottom, TMColor stroke, int lineThickness, Clade target)
        {
            builder.OpenElement(++__elementCount, "path");
            builder.AddAttribute(++__elementCount, "class", "collapsed-rectangle");
            builder.AddAttribute(++__elementCount, "d",
                $@"""M {left.X} {left.Y}
                     L {rightTop.X} {rightTop.Y}
                     L {rightBottom.X} {rightBottom.Y}
                     L {left.X} {left.Y}
                  """);
            builder.AddAttribute(++__elementCount, "stroke", stroke.Value);
            builder.CloseElement();
        }

        void ITreeDrawer.DrawLeafLabel(string taxon, TMRect area, TMColor fill, int fontSize, Clade target)
        {
            builder.OpenElement(++__elementCount, "text");
            builder.AddAttribute(++__elementCount, "class", "leaf");
            builder.AddAttribute(++__elementCount, "x", area.X);
            builder.AddAttribute(++__elementCount, "y", area.Y);
            builder.AddAttribute(++__elementCount, "fill", fill.Value);
            builder.AddContent(++__elementCount, taxon);
            builder.CloseElement();

            // クリックエリア
            if (ViewModel.EditMode.Value == TreeEditMode.Select)
            {
                CladeId id = target.GetId(CladeIdSuffix.Leaf);
                string fillColor = ViewModel.FocusedSvgElementIdList.Contains(id) ? "#A0D8EFC4" : "transparent";
                const int margin = 3;
                builder.OpenElement(++__elementCount, "rect");
                builder.AddAttribute(++__elementCount, "id", id);
                builder.AddAttribute(++__elementCount, "class", "clickable-leaf");
                builder.AddAttribute(++__elementCount, "x", area.X - margin);
                builder.AddAttribute(++__elementCount, "y", area.Y - area.Height - margin);
                builder.AddAttribute(++__elementCount, "width", area.Width + margin * 2);
                builder.AddAttribute(++__elementCount, "height", area.Height + margin * 2);
                builder.AddAttribute(++__elementCount, "fill", fillColor);
                builder.AddAttribute(++__elementCount, "onclick", ViewModel.SvgElementClickedCommand.ToDelegate(id));
                builder.CloseElement();
            }
        }

        void ITreeDrawer.DrawNodeValue(string value, TMPoint point, TMColor fill, int fontSize, Clade target)
        {
            builder.OpenElement(++__elementCount, "text");
            builder.AddAttribute(++__elementCount, "class", "node-value");
            builder.AddAttribute(++__elementCount, "x", point.X);
            builder.AddAttribute(++__elementCount, "y", point.Y);
            builder.AddAttribute(++__elementCount, "fill", fill.Value);
            builder.AddContent(++__elementCount, value);
            builder.CloseElement();
        }

        void ITreeDrawer.DrawBranchValue(string value, TMPoint point, TMColor fill, int fontSize, Clade target)
        {
            builder.OpenElement(++__elementCount, "text");
            builder.AddAttribute(++__elementCount, "class", "branch-value");
            builder.AddAttribute(++__elementCount, "x", point.X);
            builder.AddAttribute(++__elementCount, "y", point.Y);
            builder.AddAttribute(++__elementCount, "fill", fill.Value);
            builder.AddContent(++__elementCount, value);
            builder.CloseElement();
        }

        void ITreeDrawer.DrawCladeLabel(string cladeName, TMPoint lineBegin, TMPoint lineEnd, TMPoint testPoint, int lineThickness, int fontSize, Clade target)
        {
            builder.OpenElement(++__elementCount, "line");
            builder.AddAttribute(++__elementCount, "x1", lineBegin.X);
            builder.AddAttribute(++__elementCount, "x2", lineEnd.X);
            builder.AddAttribute(++__elementCount, "y1", lineBegin.Y);
            builder.AddAttribute(++__elementCount, "y2", lineEnd.Y);
            builder.AddAttribute(++__elementCount, "stroke", "black");
            builder.AddAttribute(++__elementCount, "stroke-width", lineThickness);
            builder.CloseElement();

            builder.OpenElement(++__elementCount, "text");
            builder.AddAttribute(++__elementCount, "class", "clade-label");
            builder.AddAttribute(++__elementCount, "x", testPoint.X);
            builder.AddAttribute(++__elementCount, "y", testPoint.Y);
            builder.AddContent(++__elementCount, cladeName);
            builder.CloseElement();
        }

        void ITreeDrawer.DrawHorizontalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness, Clade target)
        {
            builder.OpenElement(++__elementCount, "line");
            builder.AddAttribute(++__elementCount, "class", "branch");
            builder.AddAttribute(++__elementCount, "x1", parentPoint.X);
            builder.AddAttribute(++__elementCount, "y1", parentPoint.Y);
            builder.AddAttribute(++__elementCount, "x2", childPoint.X);
            builder.AddAttribute(++__elementCount, "y2", childPoint.Y);
            builder.AddAttribute(++__elementCount, "stroke", stroke.Value);
            builder.CloseElement();
        }

        void ITreeDrawer.DrawVerticalBranch(TMPoint parentPoint, TMPoint childPoint, TMColor stroke, int thickness, Clade target)
        {
            builder.OpenElement(++__elementCount, "line");
            builder.AddAttribute(++__elementCount, "class", "branch");
            builder.AddAttribute(++__elementCount, "x1", parentPoint.X);
            builder.AddAttribute(++__elementCount, "y1", parentPoint.Y);
            builder.AddAttribute(++__elementCount, "x2", childPoint.X);
            builder.AddAttribute(++__elementCount, "y2", childPoint.Y);
            builder.AddAttribute(++__elementCount, "stroke", stroke.Value);
            builder.CloseElement();
        }

        void ITreeDrawer.DrawBranchDecoration(Clade target, BranchDecorationStyle style)
        {
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
                        builder.OpenElement(++__elementCount, "circle");
                        builder.AddAttribute(++__elementCount, "cx", center.X);
                        builder.AddAttribute(++__elementCount, "cy", center.Y);
                        builder.AddAttribute(++__elementCount, "r", radius);
                        builder.AddAttribute(++__elementCount, "stroke", stroke);
                        builder.AddAttribute(++__elementCount, "fill", fill);
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
                        builder.OpenElement(++__elementCount, "rect");
                        builder.AddAttribute(++__elementCount, "x", x);
                        builder.AddAttribute(++__elementCount, "y", y);
                        builder.AddAttribute(++__elementCount, "width", width);
                        builder.AddAttribute(++__elementCount, "height", height);
                        builder.AddAttribute(++__elementCount, "stroke", stroke);
                        builder.AddAttribute(++__elementCount, "fill", fill);
                        builder.CloseElement();
                    }
                    break;
            }
        }

        void ITreeDrawer.OnCladeDrawn(Clade target)
        {
            Tree? tree = ViewModel.TargetTree.Value;
            Debug.Assert(tree is not null);

            // Reroot用クリックエリア
            if (ViewModel.EditMode.Value is TreeEditMode.Reroot && !target.IsRoot)
            {
                if (!target.IsLeaf && (tree.IsUnrooted || !target.Parent.IsRoot))
                {
                    const double size = 5;
                    double x = (positionManager.CalcX1(target) + positionManager.CalcX2(target)) / 2 - size;
                    double y = positionManager.CalcY1(target) - size;
                    builder.OpenElement(++__elementCount, "rect");
                    builder.AddAttribute(++__elementCount, "x", x);
                    builder.AddAttribute(++__elementCount, "y", y);
                    builder.AddAttribute(++__elementCount, "width", size * 2);
                    builder.AddAttribute(++__elementCount, "height", size * 2);
                    builder.AddAttribute(++__elementCount, "style", "stroke: black; stroke-width: 2px; fill: white");
                    builder.AddAttribute(++__elementCount, "onclick", ViewModel.RerootCommand.ToDelegate((target, true)));
                    builder.CloseElement();
                }

                if (!target.IsLeaf)
                {
                    CladeId id = target.GetId(CladeIdSuffix.Node);
                    (double x, double y, double width, double height) = positionManager.CalcNodeDecorationRectangleArea(target);
                    builder.OpenElement(++__elementCount, "rect");
                    builder.AddAttribute(++__elementCount, "id", id);
                    builder.AddAttribute(++__elementCount, "x", x);
                    builder.AddAttribute(++__elementCount, "y", y);
                    builder.AddAttribute(++__elementCount, "width", width);
                    builder.AddAttribute(++__elementCount, "height", height);
                    builder.AddAttribute(++__elementCount, "style", "fill: black; stroke: transparent; stroke-width: 2px");
                    builder.AddAttribute(++__elementCount, "onclick", ViewModel.RerootCommand.ToDelegate((target, false)));
                    builder.CloseElement();
                }
            }

            // Swap用クリックエリア
            if (ViewModel.EditMode.Value == TreeEditMode.Swap && !target.IsRoot)
            {
                CladeId id = target.GetId(CladeIdSuffix.Node);
                int index = target.Parent.Children.IndexOf(target);
                if (index < target.Parent.Children.Count - 1)
                {
                    Clade sister = target.Parent.Children[index + 1];
                    double x = positionManager.CalcX1(target) - 5;
                    double y = (positionManager.CalcY1(target) + positionManager.CalcY1(sister)) / 2 - 5;
                    builder.OpenElement(++__elementCount, "rect");
                    builder.AddAttribute(++__elementCount, "id", id);
                    builder.AddAttribute(++__elementCount, "x", x);
                    builder.AddAttribute(++__elementCount, "y", y);
                    builder.AddAttribute(++__elementCount, "width", "10");
                    builder.AddAttribute(++__elementCount, "height", "10");
                    builder.AddAttribute(++__elementCount, "style", "fill: black; stroke: transparent; stroke-width: 2px");
                    builder.AddAttribute(++__elementCount, "onclick", ViewModel.SwapSisterCommand.ToDelegate((target, sister)));
                    builder.CloseElement();
                }
            }

            // Subtree用クリックエリア
            if (ViewModel.EditMode.Value is TreeEditMode.Subtree && !target.IsRoot && !target.IsLeaf)
            {
                const double size = 5;
                double x = (positionManager.CalcX1(target) + positionManager.CalcX2(target)) / 2 - size;
                double y = positionManager.CalcY1(target) - size;
                builder.OpenElement(++__elementCount, "rect");
                builder.AddAttribute(++__elementCount, "x", x);
                builder.AddAttribute(++__elementCount, "y", y);
                builder.AddAttribute(++__elementCount, "width", size * 2);
                builder.AddAttribute(++__elementCount, "height", size * 2);
                builder.AddAttribute(++__elementCount, "style", "stroke: transparent; stroke-width: 2px; fill: black");
                builder.AddAttribute(++__elementCount, "onclick", ViewModel.ExtractSubtreeCommand.ToDelegate(target));
                builder.CloseElement();
            }

            // 枝クリックエリア
            if (ViewModel.EditMode.Value == TreeEditMode.Select)
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

                    builder.OpenElement(++__elementCount, "path");
                    builder.AddAttribute(++__elementCount, "id", id);
                    builder.AddAttribute(++__elementCount, "class", "clickable-branch");
                    builder.AddAttribute(++__elementCount, "d", path);
                    builder.AddAttribute(++__elementCount, "stroke", strokeColor);
                    builder.AddAttribute(++__elementCount, "style", $"stroke-width: {tree.Style.BranchThickness + 4}px");
                    builder.AddAttribute(++__elementCount, "onclick", ViewModel.SvgElementClickedCommand.ToDelegate(id));
                    builder.CloseElement();
                }
            }
        }

        void ITreeDrawer.DrawScalebar(double value, TMPoint offset, TMPoint lineBegin, TMPoint lineEnd, TMPoint textPoint, int fontSize, int lineThickness)
        {
            builder.CloseElement(); // tree area

            builder.OpenElement(++__elementCount, "g");
            builder.AddAttribute(++__elementCount, "transform", $"translate({offset.X}, {offset.Y})");

            builder.OpenElement(++__elementCount, "text");
            builder.AddAttribute(++__elementCount, "class", "scalebar-text");
            builder.AddAttribute(++__elementCount, "x", textPoint.X);
            builder.AddAttribute(++__elementCount, "y", textPoint.Y);
            builder.AddAttribute(++__elementCount, "text-anchor", "middle");
            builder.AddContent(++__elementCount, value);
            builder.CloseElement(); // text

            builder.OpenElement(++__elementCount, "line");
            builder.AddAttribute(++__elementCount, "class", "scalebar-line");
            builder.AddAttribute(++__elementCount, "x1", lineBegin.X);
            builder.AddAttribute(++__elementCount, "y1", lineBegin.Y);
            builder.AddAttribute(++__elementCount, "x2", lineEnd.X);
            builder.AddAttribute(++__elementCount, "y2", lineEnd.Y);
            builder.CloseElement(); // line
        }

        void ITreeDrawer.FinishTree()
        {
            builder.CloseElement(); // g
            builder.CloseElement(); // svg
        }
    }
}
