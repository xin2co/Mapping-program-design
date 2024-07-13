using System.Drawing;
using System.Windows.Forms;

namespace 导线网平差及精度评定程序
{
    public class DrawingHelper
    {
        public static void DrawEllipseOnPictureBox(PictureBox pictureBox, Pen pen, Ellipse ellipse)
        {
            Graphics graphics = pictureBox.CreateGraphics();

            // 计算椭圆的边界矩形
            float width = (float)(2 * ellipse.E);
            float height = (float)(2 * ellipse.F);
            float x = (float)(ellipse.center.x - ellipse.E);
            float y = (float)(ellipse.center.y - ellipse.F);
            RectangleF rect = new RectangleF(x, y, width, height);

            graphics.TranslateTransform((float)ellipse.center.x, (float)ellipse.center.y);
            graphics.RotateTransform((float)ellipse.orientation.Degrees);

            // 绘制椭圆
            graphics.DrawEllipse(pen, -width / 2, -height / 2, width, height);

            // 恢复绘图对象的旋转和位移
            graphics.ResetTransform();
        }
    }
}
