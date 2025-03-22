using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomizeTextBox
{
    public class TextBox : UserControl
    {
        #region 字段 / Fields
        private string _text = string.Empty; // 存储文本框内容
        private int _alpha; // 用于下划线动画的透明度
        private const int AnimationSpeed = 40; // 动画速度
        private Color _underlineColor = Color.FromArgb(33, 150, 243); // 下划线颜色
        private readonly System.Windows.Forms.Timer _animationTimer; // 动画计时器
        private bool _mouseHover; // 鼠标是否悬停
        private Bitmap? _backgroundCache; // 父控件背景缓存
        private bool _backgroundCacheDirty = true; // 是否需要更新缓存
        #endregion

        #region 构造函数 / Constructor
        public TextBox()
        {
            InitializeAppearance(); // 初始化外观
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 初始化计时器
            SetupAnimation(); // 设置动画
            SetupEventHandlers(); // 绑定事件处理程序
            DoubleBuffered = true; // 启用双缓冲，减少闪烁
            ImeMode = ImeMode.On; // 启用输入法
        }
        #endregion

        #region 初始化 / Initialization
        private void InitializeAppearance()
        {
            // 设置控件样式
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent; // 背景透明
            ForeColor = Color.Black; // 前景色为黑色
            Size = new Size(200, 36); // 默认大小
            Padding = new Padding(8); // 内边距
        }

        private void SetupAnimation()
        {
            _animationTimer.Tick += AnimationHandler; // 绑定计时器事件
        }

        private void SetupEventHandlers()
        {
            // 鼠标事件
            MouseEnter += (s, e) => { _mouseHover = true; _animationTimer.Start(); }; // 鼠标进入
            MouseLeave += (s, e) => { _mouseHover = false; _animationTimer.Start(); }; // 鼠标离开

            // 焦点事件
            GotFocus += (s, e) => Invalidate(); // 获取焦点时重绘
            LostFocus += (s, e) => Invalidate(); // 失去焦点时重绘

            // 键盘事件
            KeyPress += OnKeyPress; // 处理按键输入
            KeyDown += OnKeyDown; // 处理退格键

            // 父控件背景变化时更新缓存
            if (Parent != null)
            {
                Parent.BackgroundImageChanged += (s, e) => _backgroundCacheDirty = true;
                Parent.BackColorChanged += (s, e) => _backgroundCacheDirty = true;
            }
        }

        private void OnKeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return; // 忽略控制字符
            _text += e.KeyChar.ToString(); // 添加到文本
            Invalidate(); // 重绘控件
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && _text.Length > 0) // 处理退格键
            {
                _text = _text.Substring(0, _text.Length - 1); // 删除最后一个字符
                Invalidate(); // 重绘控件
            }
        }
        #endregion

        #region 绘制 / Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 更新背景缓存
            if (_backgroundCacheDirty)
            {
                CacheParentBackground();
                _backgroundCacheDirty = false;
            }

            // 绘制缓存的父控件背景
            if (_backgroundCache != null)
            {
                e.Graphics.DrawImage(_backgroundCache, ClientRectangle);
            }

            // 绘制圆角背景
            using (var path = CreateRoundPath(ClientRectangle, 10))
            using (var brush = BackgroundImage != null ? new TextureBrush(BackgroundImage, WrapMode.Tile) : new SolidBrush(base.BackColor) as Brush)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // 抗锯齿
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality; // 高质量绘制
                e.Graphics.FillPath(brush, path); // 填充路径
            }

            // 绘制下划线
            using (var underlineBrush = new LinearGradientBrush(new Point(0, Height - 2), new Point(Width, Height - 2), Color.Transparent, Color.FromArgb(_alpha, _underlineColor)))
            {
                e.Graphics.FillRectangle(underlineBrush, 0, Height - 2, Width, 2);
            }

            // 绘制文本
            using (var textBrush = new SolidBrush(ForeColor))
            {
                var textRect = new Rectangle(4, Height - Padding.Bottom + 6 - Font.Height, Width - 8, Font.Height);
                e.Graphics.DrawString(_text, Font, textBrush, textRect, StringFormat.GenericTypographic);
            }
        }

        private void CacheParentBackground()
        {
            if (Parent == null) return;
            _backgroundCache?.Dispose(); // 释放旧的缓存
            _backgroundCache = new Bitmap(Width, Height); // 创建新的缓存
            using (var g = Graphics.FromImage(_backgroundCache))
            {
                var rect = new Rectangle(Left, Top, Width, Height);
                using (var bmp = new Bitmap(Parent.Width, Parent.Height))
                {
                    Parent.DrawToBitmap(bmp, Parent.ClientRectangle); // 绘制父控件背景
                    g.DrawImage(bmp, ClientRectangle, rect, GraphicsUnit.Pixel); // 绘制到缓存
                }
            }
        }
        #endregion

        #region 动画 / Animation
        private void AnimationHandler(object? sender, EventArgs e)
        {
            // 更新透明度动画
            _alpha = Math.Clamp(_alpha + (_mouseHover ? AnimationSpeed : -AnimationSpeed), 0, 255);
            Invalidate(new Rectangle(0, Height - 2, Width, 2)); // 仅重绘下划线区域
        }
        #endregion

        #region 几何辅助 / Geometry Helpers
        private GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath(); // 创建圆角路径
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion

        #region 属性 / Properties
        [Category("Appearance"), Description("下划线高亮颜色")]
        public Color UnderlineColor
        {
            get => _underlineColor;
            set { _underlineColor = value; Invalidate(); } // 设置值并重绘
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "Transparent")]
        public new Color BackColor
        {
            get => base.BackColor;
            set { base.BackColor = value; Invalidate(); } // 设置值并重绘
        }

        [Category("Appearance")]
        public new Image? BackgroundImage
        {
            get => base.BackgroundImage;
            set { base.BackgroundImage = value; Invalidate(); } // 设置值并重绘
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => _text;
            set { _text = value; Invalidate(); } // 设置值并重绘
        }

        [Category("Appearance")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set { base.ForeColor = value; Invalidate(); } // 设置值并重绘
        }
        #endregion

        #region 布局 / Layout
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _backgroundCacheDirty = true; // 标记缓存需要更新
            Invalidate(); // 重绘控件
        }
        #endregion

        #region 焦点处理 / Focus Handling
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate(); // 获取焦点时重绘
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate(); // 失去焦点时重绘
        }
        #endregion

        #region 右键菜单和快捷键支持 / Right-Click Menu and Shortcuts
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Right) // 右键点击
            {
                ShowContextMenu(); // 显示上下文菜单
            }
        }

        private void ShowContextMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("复制", null, (s, e) => CopyText()); // 复制
            menu.Items.Add("粘贴", null, (s, e) => PasteText()); // 粘贴
            menu.Items.Add("剪切", null, (s, e) => CutText()); // 剪切
            menu.Show(this, PointToClient(Cursor.Position)); // 显示菜单
        }

        private void CopyText()
        {
            if (!string.IsNullOrEmpty(_text))
            {
                try
                {
                    Clipboard.SetText(_text); // 复制文本到剪贴板
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"复制失败: {ex.Message}"); // 处理异常
                }
            }
        }

        private void PasteText()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    _text += Clipboard.GetText(); // 从剪贴板粘贴文本
                    Invalidate(); // 重绘控件
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"粘贴失败: {ex.Message}"); // 处理异常
            }
        }

        private void CutText()
        {
            if (!string.IsNullOrEmpty(_text))
            {
                try
                {
                    Clipboard.SetText(_text); // 剪切文本到剪贴板
                    _text = string.Empty; // 清空文本
                    Invalidate(); // 重绘控件
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"剪切失败: {ex.Message}"); // 处理异常
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C)) // 处理 Ctrl+C
            {
                CopyText();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.V)) // 处理 Ctrl+V
            {
                PasteText();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.X)) // 处理 Ctrl+X
            {
                CutText();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData); // 默认处理
        }
        #endregion
    }
}
