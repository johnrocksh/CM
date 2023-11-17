namespace CastManager.Extentions
{
    using System.IO;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.Windows;
    using CastManager.Logger;

    static class FrameworkElementExtension
    {
        public static byte[] ToPngImage(this FrameworkElement surface)
        {
            using var stream = new MemoryStream();
            ElementToPngImage(surface, stream);
            return stream.ToArray();
        }

        static void ElementToPngImage(FrameworkElement surface, Stream outputStream)
        {
            try
            {
                // Save current canvas transform
                Transform transform = surface.LayoutTransform;
                Logger.WriteLine($"transform   - {transform}");
                // reset current transform (in case it is scaled or rotated)
                surface.LayoutTransform = null;

                // Get the size of canvas
                // Measure and arrange the surface
                // VERY IMPORTANT

                surface.InvalidateMeasure();
                surface.InvalidateArrange();
                surface.UpdateLayout();

                surface.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size size = surface.DesiredSize;

                Logger.WriteLine($"size   - {size}");

                surface.Arrange(new Rect(size));

                // Create a render bitmap and push the surface to it
                RenderTargetBitmap renderBitmap =
                    new RenderTargetBitmap(
                        (int)size.Width,
                        (int)size.Height,
                        96d,
                        96d,
                        PixelFormats.Pbgra32);
                renderBitmap.Render(surface);

                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outputStream);
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
        }
    }
}
