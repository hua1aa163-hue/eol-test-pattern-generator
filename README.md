# EOL 图卡生成器

这是一个基于 C#、WinForms、.NET 8 和 OpenCvSharp4 的图卡生成工具。程序的生成、预览、批量处理和 LightTools 文本输出均由 C# 完成，不需要 MATLAB，也不会调用 `Lt_Source_Creator.exe`。

所有窗体、按钮、文本框、数值框和下拉框均在对应的 `.Designer.cs` 中定义，可以继续使用 Visual Studio WinForms 设计器调整；业务 `.cs` 不会在运行时临时创建这些界面控件。

## 功能窗口

### 主窗口

- 画布宽高、图案位置和矩形区域尺寸可调，支持一键居中和越界裁剪。
- 九点图和畸变点阵可调整圆点半径、行列数及首末圆心跨度；圆点不使用抗锯齿。
- 支持外框、九点图、畸变点阵、上下校正、白色矩形、全黑、全白、全红、全绿、全蓝共十张基础图卡的批量导出。
- “白框”是独立叠加层，可以添加到任意生成图或导入的外部图片上，位置、尺寸和线宽均可调。
- 可导入 PNG、JPEG、BMP、TIFF、WebP 作为底图，并使用最近邻方式适配目标画布。

### 串扰图：RGB 八步相移

- RGB 八步二值斜相移位于独立窗口。
- 支持 `RGB`、`RBG`、`GRB`、`GBR`、`BRG`、`BGR` 六种子像素排列。
- 条纹区域、相位、画布、白框和输出格式均可调，可保存当前相位或批量导出 1–8 相位。

### 非整数融合与 LightTools（纯 C#）

从“串扰图”窗口进入，共有三个页签：

1. **非整数连续融合**
   - 复现 MATLAB 脚本的连续覆盖率融合：有效周期 `T = spixtol × m`，每行位移为 `3 × tan(theta)`，`picnum` 按非整数超周期循环。
   - 支持全白/全黑、固定 1 子像素、可调点亮比例 Duty、红/蓝、自定义左右图片五种图源。
   - Duty 表示左眼半周期内部的点亮比例；默认 `0.25` 对应整个周期宽度的 `12.5%`。
   - 可交换左右图源；自定义图源尺寸必须一致并与画布相同，不会静默拉伸或裁剪。
   - 文件夹批量模式识别同目录下成对的 `<名称>_L`、`<名称>_R` PNG/TIFF。
   - 正常模式保持 RGB 颜色；另提供“兼容旧 MATLAB（交换 R/B）”选项，用于复现旧脚本文件夹分支的通道行为。
   - 可选择同时保存实际参与融合的左右 TIFF 图源，以及输出 LightTools MESH TXT。

2. **离散光源图**
   - 纯 C# 复现 `Lt_Source_Creator` 的分辨率、倾角、子像素周期、点亮颗数、组号、分区宽度、策略角、平移量和方向参数。
   - 支持白黑、黑白、红蓝及两张自定义图源。
   - `组 = -1` 时生成 `0` 到 `周期-1` 的全部组；默认命名格式与原工具一致，例如 `5.00_8_4_0WB.bmp`。
   - 可导出单个 LightTools 光源 TXT，或拆分为 `_R.txt`、`_G.txt`、`_B.txt`。
   - 写入 TXT 时先完成同目录临时文件再原子替换。切换单文件/三文件模式不会自动删除另一模式的旧文件，建议使用空输出目录或在确认后自行整理。

3. **图片转 LightTools**
   - 读取 JPEG、BMP、PNG 或 TIFF，按 RGB 子像素展开为 MESH。
   - 可输出单文件，或分别输出 R/G/B 三个光源文件。

连续融合页中的 `pixL` 是横向子像素宽度（毫米），`pixH` 是纵向像素高度（毫米）；它们只用于 MESH 物理范围，不改变融合几何。离散页和图片转换页使用完整像素的 X/Y 尺寸（微米）。

### 1号屏三张图卡

独立窗口可调画布分辨率，以及左、右矩形各自的 X、Y、宽度和高度。默认画布为 `3200 × 2000`，对应：

| 文件名 | 默认布局 |
| --- | --- |
| `1.B_W` | 白底、左侧黑色矩形 |
| `2.W_B` | 白底、右侧黑色矩形 |
| `3.B` | 白底、左右两个黑色矩形 |

默认四周白边为 `50 px`，左右区域各 `1500 × 1900`，中间白缝为 `100 px`。可保存当前图卡或批量导出三张。

## 预览与输出

- 鼠标滚轮以指针位置为锚点缩放，鼠标左键拖动；画布外区域使用界面背景色。
- 可显示图片中心十字虚线和鼠标所在的零基像素坐标，两项可分别关闭。
- 预览辅助线和坐标只绘制在界面层，不会写入导出的图片。
- 支持 PNG、JPEG/JPG、BMP、TIFF、WebP；JPEG/WebP 可设置质量。
- 要求通道严格为 `0/255` 的纯色图、1号屏图和离散光源图，只允许使用 PNG、BMP 或 TIFF 无损导出。
- 单方向最大 `16384 px`，多数生成入口同时限制画布总像素不超过 4000 万，以避免内存耗尽。

## 保存上次输入

主窗口和所有子窗口会在关闭时保存全部可编辑输入，包括：

- 各图卡的画布、位置、尺寸、圆点、行列、线宽、白框和导入路径；
- RGB 相移、1号屏、连续融合、离散光源及图片转换参数；
- 输出格式、质量、目录、文件前缀、图源路径和预览辅助显示开关。

设置文件位于：

```text
%LocalAppData%\EolTestPatternGenerator\settings.json
```

配置采用版本化 JSON 和同目录临时文件原子替换。文件缺失或损坏时程序会回退为默认值。预览缩放和平移属于临时视图状态，不会保存。

## 打开、设计与构建

使用 Visual Studio 打开解决方案，而不是仅打开文件夹或单个 `.cs`：

```text
EolTestPatternGenerator.sln
```

项目文件已显式声明所有 Form/UserControl 的 `SubType` 和 `.Designer.cs` 的 `DependentUpon`，并将运行时 OpenCV 初始化移到带设计时保护的 `OnLoad`，避免 WinForms Designer 加载原生库。

若 Visual Studio 仍显示旧的 `hierarchy` 缓存错误，请关闭 Visual Studio，清理项目的 `.vs`、`bin`、`obj` 后重新打开解决方案，并从主 `.cs` 文件选择“查看设计器”，不要直接打开 `.Designer.cs`。

命令行构建：

```powershell
dotnet restore .\EolTestPatternGenerator.sln --configfile .\NuGet.Config
dotnet build .\EolTestPatternGenerator.sln -c Release --property:Platform=x64
```

输出程序位于：

```text
src\EolTestPatternGenerator\bin\x64\Release\net8.0-windows\win-x64\EolTestPatternGenerator.exe
```

## 验证

```powershell
dotnet .\src\EolTestPatternGenerator\bin\x64\Release\net8.0-windows\win-x64\EolTestPatternGenerator.dll --self-test
dotnet .\src\EolTestPatternGenerator\bin\x64\Release\net8.0-windows\win-x64\EolTestPatternGenerator.dll --ui-smoke-test
dotnet .\src\EolTestPatternGenerator\bin\x64\Release\net8.0-windows\win-x64\EolTestPatternGenerator.dll --verify "C:\Users\admin\Desktop\90K图片\EOL图卡png"
```

`--self-test` 会进行像素级验证，包括纯色图、1号屏、六种 RGB 排列、导入图白框、预览坐标换算、非整数覆盖率、离散串扰 4×2 参考样例、LightTools 文本和用户配置持久化。`--ui-smoke-test` 会构造并加载四个窗体及预览控件，检查 Designer 事件和 OpenCV 运行时初始化。

## OpenCvSharp 本地依赖

项目使用用户提供的核心包：

```text
D:\历史资料\2026.7\opencvsharp4.4.13.0.20260627.nupkg
```

并显式引用同版本 `OpenCvSharp4.runtime.win 4.13.0.20260627`。复制到其他电脑时还需要 .NET 8 Desktop Runtime x64；`NuGet.Config` 中的本机离线包路径也要在新电脑上调整。

## GitHub 参考

实现未直接复制第三方源码，界面组织和 OpenCV 用法主要参考：

- [fryderykhuang/TestPatternGenerator](https://github.com/fryderykhuang/TestPatternGenerator)（MIT）
- [shimat/opencvsharp](https://github.com/shimat/opencvsharp)（Apache-2.0）
- [shimat/opencvsharp_samples](https://github.com/shimat/opencvsharp_samples)（Apache-2.0）
- [elerac/structuredlight](https://github.com/elerac/structuredlight)（MIT）
- [nzhagen/fpp_tools](https://github.com/nzhagen/fpp_tools)（MIT）
- [opencv/opencv_contrib](https://github.com/opencv/opencv_contrib)（Apache-2.0）

RGB 八步相移、非整数连续融合和离散 LightTools 图样均按用户样图及提供程序的实际规则在本项目中重新实现。
