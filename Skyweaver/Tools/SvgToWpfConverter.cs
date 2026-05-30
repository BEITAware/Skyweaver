using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Skyweaver.Tools
{
    /// <summary>
    /// 轻量级、高性能的 SVG 到 WPF DrawingImage 的原生转换器。
    /// 能够将 SVG 格式的 path、rect、circle、ellipse、polygon、polyline、line 转换为 WPF 对应的几何形状，
    /// 并支持渐变色（linearGradient/radialGradient）填充和线段描边。
    /// </summary>
    public static class SvgToWpfConverter
    {
        public static DrawingImage? Convert(string svgContent)
        {
            if (string.IsNullOrWhiteSpace(svgContent)) return null;

            try
            {
                var doc = XDocument.Parse(svgContent.Trim());
                var root = doc.Root;
                if (root == null || !string.Equals(root.Name.LocalName, "svg", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var rootGroup = new DrawingGroup();
                var brushes = new Dictionary<string, Brush>(StringComparer.OrdinalIgnoreCase);

                // 递归转换所有 SVG 子节点
                foreach (var element in root.Elements())
                {
                    ConvertElement(element, rootGroup, brushes);
                }

                return new DrawingImage(rootGroup);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting SVG natively: {ex}");
                return null;
            }
        }

        private static void ConvertElement(XElement element, DrawingGroup parentGroup, Dictionary<string, Brush> brushes)
        {
            var localName = element.Name.LocalName;

            // 如果是 defs，先解析其中的渐变
            if (string.Equals(localName, "defs", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var child in element.Elements())
                {
                    ParseGradient(child, brushes);
                }
                return;
            }

            // 支持在外侧直接定义的渐变
            if (string.Equals(localName, "linearGradient", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(localName, "radialGradient", StringComparison.OrdinalIgnoreCase))
            {
                ParseGradient(element, brushes);
                return;
            }

            // 如果是 g 元素（Group），转换为嵌套的 DrawingGroup
            if (string.Equals(localName, "g", StringComparison.OrdinalIgnoreCase))
            {
                var group = new DrawingGroup();
                var opacityStr = GetAttributeOrStyle(element, "opacity");
                if (double.TryParse(opacityStr, out var opacity))
                {
                    group.Opacity = opacity;
                }

                foreach (var child in element.Elements())
                {
                    ConvertElement(child, group, brushes);
                }

                if (group.Children.Count > 0)
                {
                    parentGroup.Children.Add(group);
                }
                return;
            }

            // 几何形状转换
            Geometry? geometry = null;
            if (string.Equals(localName, "path", StringComparison.OrdinalIgnoreCase))
            {
                var d = element.Attribute("d")?.Value;
                if (!string.IsNullOrWhiteSpace(d))
                {
                    try
                    {
                        // 借助 WPF 强大的内置 Geometry.Parse 语法分析器支持所有标准 SVG Path 路径指令
                        geometry = Geometry.Parse(d);
                    }
                    catch {}
                }
            }
            else if (string.Equals(localName, "rect", StringComparison.OrdinalIgnoreCase))
            {
                double x = ParseDouble(element.Attribute("x")?.Value, 0);
                double y = ParseDouble(element.Attribute("y")?.Value, 0);
                double width = ParseDouble(element.Attribute("width")?.Value, 0);
                double height = ParseDouble(element.Attribute("height")?.Value, 0);
                double rx = ParseDouble(element.Attribute("rx")?.Value, 0);
                double ry = ParseDouble(element.Attribute("ry")?.Value, 0);

                if (width > 0 && height > 0)
                {
                    geometry = new RectangleGeometry(new Rect(x, y, width, height), rx, ry);
                }
            }
            else if (string.Equals(localName, "circle", StringComparison.OrdinalIgnoreCase))
            {
                double cx = ParseDouble(element.Attribute("cx")?.Value, 0);
                double cy = ParseDouble(element.Attribute("cy")?.Value, 0);
                double r = ParseDouble(element.Attribute("r")?.Value, 0);

                if (r > 0)
                {
                    geometry = new EllipseGeometry(new Point(cx, cy), r, r);
                }
            }
            else if (string.Equals(localName, "ellipse", StringComparison.OrdinalIgnoreCase))
            {
                double cx = ParseDouble(element.Attribute("cx")?.Value, 0);
                double cy = ParseDouble(element.Attribute("cy")?.Value, 0);
                double rx = ParseDouble(element.Attribute("rx")?.Value, 0);
                double ry = ParseDouble(element.Attribute("ry")?.Value, 0);

                if (rx > 0 && ry > 0)
                {
                    geometry = new EllipseGeometry(new Point(cx, cy), rx, ry);
                }
            }
            else if (string.Equals(localName, "polygon", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(localName, "polyline", StringComparison.OrdinalIgnoreCase))
            {
                var pointsStr = element.Attribute("points")?.Value;
                if (!string.IsNullOrWhiteSpace(pointsStr))
                {
                    var points = ParsePoints(pointsStr);
                    if (points.Count > 0)
                    {
                        var geom = new PathGeometry();
                        var figure = new PathFigure
                        {
                            StartPoint = points[0],
                            IsClosed = string.Equals(localName, "polygon", StringComparison.OrdinalIgnoreCase)
                        };
                        for (int i = 1; i < points.Count; i++)
                        {
                            figure.Segments.Add(new LineSegment(points[i], true));
                        }
                        geom.Figures.Add(figure);
                        geometry = geom;
                    }
                }
            }
            else if (string.Equals(localName, "line", StringComparison.OrdinalIgnoreCase))
            {
                double x1 = ParseDouble(element.Attribute("x1")?.Value, 0);
                double y1 = ParseDouble(element.Attribute("y1")?.Value, 0);
                double x2 = ParseDouble(element.Attribute("x2")?.Value, 0);
                double y2 = ParseDouble(element.Attribute("y2")?.Value, 0);

                geometry = new LineGeometry(new Point(x1, y1), new Point(x2, y2));
            }

            if (geometry != null)
            {
                // 解析 Fill Brush
                Brush? fillBrush = null;
                var fillStr = GetAttributeOrStyle(element, "fill");
                if (fillStr != null && !string.Equals(fillStr, "none", StringComparison.OrdinalIgnoreCase))
                {
                    if (fillStr.StartsWith("url(#") && fillStr.EndsWith(")"))
                    {
                        var gradId = fillStr.Substring(5, fillStr.Length - 6);
                        brushes.TryGetValue(gradId, out fillBrush);
                    }
                    else
                    {
                        try
                        {
                            fillBrush = (Brush?)new BrushConverter().ConvertFromString(fillStr);
                        }
                        catch {}
                    }
                }
                else if (fillStr == null)
                {
                    // SVG 规范：缺省 fill 时默认为 Black 填充
                    fillBrush = Brushes.Black;
                }

                // 处理 fill-opacity
                var fillOpacityStr = GetAttributeOrStyle(element, "fill-opacity");
                if (fillBrush != null && double.TryParse(fillOpacityStr, out var fillOpacity))
                {
                    if (fillBrush.IsFrozen)
                    {
                        fillBrush = fillBrush.Clone();
                    }
                    fillBrush.Opacity *= fillOpacity;
                }

                // 解析 Stroke Pen
                Pen? strokePen = null;
                var strokeStr = GetAttributeOrStyle(element, "stroke");
                if (strokeStr != null && !string.Equals(strokeStr, "none", StringComparison.OrdinalIgnoreCase))
                {
                    Brush? strokeBrush = null;
                    if (strokeStr.StartsWith("url(#") && strokeStr.EndsWith(")"))
                    {
                        var gradId = strokeStr.Substring(5, strokeStr.Length - 6);
                        brushes.TryGetValue(gradId, out strokeBrush);
                    }
                    else
                    {
                        try
                        {
                            strokeBrush = (Brush?)new BrushConverter().ConvertFromString(strokeStr);
                        }
                        catch {}
                    }

                    if (strokeBrush != null)
                    {
                        var strokeOpacityStr = GetAttributeOrStyle(element, "stroke-opacity");
                        if (double.TryParse(strokeOpacityStr, out var strokeOpacity))
                        {
                            if (strokeBrush.IsFrozen)
                            {
                                strokeBrush = strokeBrush.Clone();
                            }
                            strokeBrush.Opacity *= strokeOpacity;
                        }

                        var strokeWidthStr = GetAttributeOrStyle(element, "stroke-width");
                        var strokeWidth = ParseDouble(strokeWidthStr, 1);

                        strokePen = new Pen(strokeBrush, strokeWidth);

                        var strokeLineCap = GetAttributeOrStyle(element, "stroke-linecap");
                        if (strokeLineCap != null)
                        {
                            if (string.Equals(strokeLineCap, "round", StringComparison.OrdinalIgnoreCase)) strokePen.StartLineCap = strokePen.EndLineCap = PenLineCap.Round;
                            else if (string.Equals(strokeLineCap, "square", StringComparison.OrdinalIgnoreCase)) strokePen.StartLineCap = strokePen.EndLineCap = PenLineCap.Square;
                        }

                        var strokeLineJoin = GetAttributeOrStyle(element, "stroke-linejoin");
                        if (strokeLineJoin != null)
                        {
                            if (string.Equals(strokeLineJoin, "round", StringComparison.OrdinalIgnoreCase)) strokePen.LineJoin = PenLineJoin.Round;
                            else if (string.Equals(strokeLineJoin, "bevel", StringComparison.OrdinalIgnoreCase)) strokePen.LineJoin = PenLineJoin.Bevel;
                        }
                    }
                }

                var drawing = new GeometryDrawing(fillBrush, strokePen, geometry);

                // 统一处理元素级别的 opacity 属性
                var elemOpacityStr = GetAttributeOrStyle(element, "opacity");
                if (double.TryParse(elemOpacityStr, out var elemOpacity) && elemOpacity < 1.0)
                {
                    var itemGroup = new DrawingGroup { Opacity = elemOpacity };
                    itemGroup.Children.Add(drawing);
                    parentGroup.Children.Add(itemGroup);
                }
                else
                {
                    parentGroup.Children.Add(drawing);
                }
            }
        }

        private static Brush? ParseGradient(XElement element, Dictionary<string, Brush> brushes)
        {
            var id = element.Attribute("id")?.Value;
            if (string.IsNullOrEmpty(id)) return null;

            var stops = new GradientStopCollection();
            foreach (var stopElement in element.Elements())
            {
                if (string.Equals(stopElement.Name.LocalName, "stop", StringComparison.OrdinalIgnoreCase))
                {
                    var offsetStr = GetAttributeOrStyle(stopElement, "offset");
                    var offset = ParsePercentOrDouble(offsetStr);
                    var colorStr = GetAttributeOrStyle(stopElement, "stop-color") ?? "#000000";
                    var opacityStr = GetAttributeOrStyle(stopElement, "stop-opacity");

                    var color = Colors.Black;
                    try
                    {
                        if (new BrushConverter().ConvertFromString(colorStr) is SolidColorBrush tempBrush)
                        {
                            color = tempBrush.Color;
                        }
                    }
                    catch {}

                    if (double.TryParse(opacityStr, out var opacity))
                    {
                        color = Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B);
                    }
                    stops.Add(new GradientStop(color, offset));
                }
            }

            if (string.Equals(element.Name.LocalName, "linearGradient", StringComparison.OrdinalIgnoreCase))
            {
                var x1 = ParsePercentOrDouble(element.Attribute("x1")?.Value, 0);
                var y1 = ParsePercentOrDouble(element.Attribute("y1")?.Value, 0);
                var x2 = ParsePercentOrDouble(element.Attribute("x2")?.Value, 1);
                var y2 = ParsePercentOrDouble(element.Attribute("y2")?.Value, 0);

                var brush = new LinearGradientBrush(stops, new Point(x1, y1), new Point(x2, y2));
                brushes[id] = brush;
                return brush;
            }
            else if (string.Equals(element.Name.LocalName, "radialGradient", StringComparison.OrdinalIgnoreCase))
            {
                var cx = ParsePercentOrDouble(element.Attribute("cx")?.Value, 0.5);
                var cy = ParsePercentOrDouble(element.Attribute("cy")?.Value, 0.5);
                var r = ParsePercentOrDouble(element.Attribute("r")?.Value, 0.5);
                var fx = ParsePercentOrDouble(element.Attribute("fx")?.Value, cx);
                var fy = ParsePercentOrDouble(element.Attribute("fy")?.Value, cy);

                var brush = new RadialGradientBrush(stops)
                {
                    Center = new Point(cx, cy),
                    RadiusX = r,
                    RadiusY = r,
                    GradientOrigin = new Point(fx, fy)
                };
                brushes[id] = brush;
                return brush;
            }
            return null;
        }

        private static string? GetAttributeOrStyle(XElement element, string name)
        {
            var attr = element.Attribute(name)?.Value;
            if (attr != null) return attr;

            var style = element.Attribute("style")?.Value;
            if (string.IsNullOrWhiteSpace(style)) return null;

            var parts = style.Split(';');
            foreach (var part in parts)
            {
                var idx = part.IndexOf(':');
                if (idx > 0)
                {
                    var key = part.Substring(0, idx).Trim();
                    var val = part.Substring(idx + 1).Trim();
                    if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return val;
                    }
                }
            }
            return null;
        }

        private static double ParsePercentOrDouble(string? val, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(val)) return defaultValue;
            val = val.Trim();
            if (val.EndsWith("%"))
            {
                if (double.TryParse(val.Substring(0, val.Length - 1), out var percent))
                {
                    return percent / 100.0;
                }
            }
            if (double.TryParse(val, out var res))
            {
                return res;
            }
            return defaultValue;
        }

        private static double ParseDouble(string? val, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(val)) return defaultValue;
            if (double.TryParse(val.Trim(), out var res)) return res;
            return defaultValue;
        }

        private static List<Point> ParsePoints(string pointsStr)
        {
            var list = new List<Point>();
            if (string.IsNullOrWhiteSpace(pointsStr)) return list;

            var parts = pointsStr.Split(new[] { ' ', ',', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length - 1; i += 2)
            {
                if (double.TryParse(parts[i], out var x) && double.TryParse(parts[i + 1], out var y))
                {
                    list.Add(new Point(x, y));
                }
            }
            return list;
        }
    }
}
