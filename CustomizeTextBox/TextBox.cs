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
        private string _text = string.Empty;
        private int _alpha;
        private const int AnimationSpeed = 40;
        private Color _underlineColor = Color.FromArgb(33, 150, 243);
        private System.Windows.Forms.Timer _animationTimer;
        private bool _mouseHover;
        private Bitmap _backgroundCache;
        private bool _backgroundCacheDirty = true;
        #endregion

        #region 构造函数 / Constructor
        public TextBox()
        {
            InitializeAppearance();
            SetupAnimation();
            SetupEventHandlers();
            DoubleBuffered = true;
            ImeMode = ImeMode.On;
        }
        #endregion

        #region 初始化 / Initialization
        private void InitializeAppearance()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            ForeColor = Color.Black;
            Size = new Size(200, 36);
            Padding = new Padding(8);
        }

        private void SetupAnimation()
        {
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += AnimationHandler;
        }

        private void SetupEventHandlers()
        {
            MouseEnter += (s, e) => { _mouseHover = true; _animationTimer.Start(); };
            MouseLeave += (s, e) => { _mouseHover = false; _animationTimer.Start(); };

            GotFocus += (s, e) => Invalidate();
            LostFocus += (s, e) => Invalidate();

            KeyPress += OnKeyPress;
            KeyDown += OnKeyDown;

            if (Parent != null)
            {
                Parent.BackgroundImageChanged += (s, e) => _backgroundCacheDirty = true;
                Parent.BackColorChanged += (s, e) => _backgroundCacheDirty = true;
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            _text += e.KeyChar.ToString();
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && _text.Length > 0)
            {
                _text = _text.Substring(0, _text.Length - 1);
                Invalidate();
            }
        }
        #endregion

        #region 绘制 / Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_backgroundCacheDirty)
            {
                CacheParentBackground();
                _backgroundCacheDirty = false;
            }

            if (_backgroundCache != null)
            {
                e.Graphics.DrawImage(_backgroundCache, ClientRectangle);
            }

            using (var path = CreateRoundPath(ClientRectangle, 10))
            using (var brush = BackgroundImage != null ? new TextureBrush(BackgroundImage, WrapMode.Tile) : new SolidBrush(base.BackColor) as Brush)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.FillPath(brush, path);
            }

            using (var underlineBrush = new LinearGradientBrush(new Point(0, Height - 2), new Point(Width, Height - 2), Color.Transparent, Color.FromArgb(_alpha, _underlineColor)))
            {
                e.Graphics.FillRectangle(underlineBrush, 0, Height - 2, Width, 2);
            }

            using (var textBrush = new SolidBrush(ForeColor))
            {
                var textRect = new Rectangle(4, Height - Padding.Bottom + 6 - Font.Height, Width - 8, Font.Height);
                e.Graphics.DrawString(_text, Font, textBrush, textRect, StringFormat.GenericTypographic);
            }
        }

        private void CacheParentBackground()
        {
            if (Parent == null) return;
            _backgroundCache?.Dispose();
            _backgroundCache = new Bitmap(Width, Height);
            using (var g = Graphics.FromImage(_backgroundCache))
            {
                var rect = new Rectangle(Left, Top, Width, Height);
                using (var bmp = new Bitmap(Parent.Width, Parent.Height))
                {
                    Parent.DrawToBitmap(bmp, Parent.ClientRectangle);
                    g.DrawImage(bmp, ClientRectangle, rect, GraphicsUnit.Pixel);
                }
            }
        }
        #endregion

        #region 动画 / Animation
        private void AnimationHandler(object sender, EventArgs e)
        {
            _alpha = Math.Clamp(_alpha + (_mouseHover ? AnimationSpeed : -AnimationSpeed), 0, 255);
            Invalidate(new Rectangle(0, Height - 2, Width, 2));
        }
        #endregion

        #region 几何辅助 / Geometry Helpers
        private GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion

        #region 属性 / Properties
        [Category("Appearance"), Description("Underline highlight color")]
        public Color UnderlineColor
        {
            get => _underlineColor;
            set { _underlineColor = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "Transparent")]
        public new Color BackColor
        {
            get => base.BackColor;
            set { base.BackColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public new Image BackgroundImage
        {
            get => base.BackgroundImage;
            set { base.BackgroundImage = value; Invalidate(); }
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => _text;
            set { _text = value; Invalidate(); }
        }

        [Category("Appearance")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set { base.ForeColor = value; Invalidate(); }
        }
        #endregion

        #region 布局 / Layout
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _backgroundCacheDirty = true;
            Invalidate();
        }
        #endregion

        #region 焦点处理 / Focus Handling
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }
        #endregion
    }
}
