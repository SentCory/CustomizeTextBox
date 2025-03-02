# CustomizeTextBox

### **Appearance Design:**

- **Underline Animation:** When the mouse hovers over the control, the underline color gradually becomes visible (opacity increases from 0 to 255), and it fades out when the mouse leaves. This animation effect is implemented using the `_animationTimer`.
- **Background Image Caching:** When the parent container's background or background image changes, the control caches the parent container's background image (via the `CacheParentBackground` method). This allows the control to properly display a transparent background.

### **Custom Properties:**

- **`UnderlineColor`:** Defines the color of the underline.
- **`BackColor` and `BackgroundImage`:** Inherited from `Control`, but overridden here to adapt to custom drawing logic.
- **`Text`:** Inherited from `Control`, used to store and display user-entered text.
- **`ForeColor`:** Inherited from `Control`, defines the text color.

### **Performance Optimization:**

- **Double Buffering:** Reduces flickering during drawing by setting `DoubleBuffered = true`.
- **Background Caching:** The parent container's background image is cached using `_backgroundCache` to avoid regenerating it during each draw operation.

### **Appearance and Interaction:**

- **Dynamic Underline:** `CustomizeTextBox` features a dynamic underline effect on mouse hover, which is not present in `TextBox`.
- **Transparent Background:** `CustomizeTextBox` supports a transparent background, whereas `TextBox` typically does not.

### **Functionality:**

- **Parent Container Background Caching:** `CustomizeTextBox` supports caching the parent container's background image to achieve transparency effects.
- **Animation:** `CustomizeTextBox` includes animation effects (e.g., gradual display of the underline).
- **Text Handling:** `CustomizeTextBox` provides basic text input and deletion functionality, while `TextBox` offers more advanced text processing features (e.g., multi-line input, scrolling, text selection).

### **Functional Limitations:**

- **Text Handling Features:** `CustomizeTextBox` only supports single-line text input and lacks the advanced features of `TextBox` (e.g., auto-wrapping, scrollbars, text selection).
- **Input Method Support:** Although `CustomizeTextBox` sets `ImeMode = ImeMode.On`, its support for input methods is less comprehensive compared to `TextBox`.

# 中文:

### **外观设计：**

下划线动画：当鼠标悬停时，下划线颜色会逐渐显示（透明度从 0 增加到 255），鼠标离开时逐渐消失。这种动画效果通过 _animationTimer 实现.
背景图像缓存：当父容器的背景或背景图像发生变化时，控件会缓存父容器的背景图像（CacheParentBackground 方法）这使得控件可以正确显示透明背景.

### **自定义属性：**

UnderlineColor：定义下划线的颜色.
BackColor 和 BackgroundImage：继承自 Control，但在这里进行了重写以适应自定义绘制逻辑.
Text：继承自 Control，用于存储和显示用户输入的文本.
ForeColor：继承自 Control，定义文本颜色.

### **性能优化：**

双缓冲：通过 DoubleBuffered = true 减少绘制时的闪烁.
背景缓存：通过 _backgroundCache 缓存父容器的背景图像，避免每次绘制时重新生成.

### **外观与交互：**

动态下划线：CustomizeTextBox 有鼠标悬停时的动态下划线效果，TextBox 没有.
透明背景：CustomizeTextBox 支持透明背景，TextBox 通常不支持.

### **功能：**

父容器背景缓存：CustomizeTextBox 支持缓存父容器的背景图像，以实现透明效果.
动画：CustomizeTextBox 有动画效果（下划线的渐变显示）.
文本处理：CustomizeTextBox 对文本输入和删除的处理较为简单，TextBox 提供了更复杂的文本处理功能（如多行输入、滚动、文本选择等）.

### **功能限制：**

文本处理功能：CustomizeTextBox 仅支持单行文本输入，并且没有 TextBox 的高级功能（如自动换行、滚动条、文本选择等）.
输入法支持：虽然 CustomizeTextBox 设置了 ImeMode = ImeMode.On，但它对输入法的支持不如 TextBox 完善.
