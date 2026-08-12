# EOL 图卡生成器

这是一个使用 C#、WinForms 和 OpenCvSharp4 编写的 EOL 图卡生成工具。界面控件全部定义在 `MainForm.Designer.cs`，可以在 Visual Studio 的 WinForms 设计器中继续拖放和修改。

## 已实现功能

- 可修改输出画布宽度、高度，默认 `1920 × 1080`
- 可修改矩形/图案的 X、Y、宽度和高度
- 一键将图案居中，超出画布的部分会安全裁剪
- 可修改圆点半径；默认半径 `4 px`，实际直径 `2r+1 = 9 px`
- 可修改点阵行数、列数和首末圆心跨度
- 可修改外框与校正十字的线宽
- 实时预览、保存当前图卡、批量导出全部 14 张图卡
- 支持 PNG、JPEG/JPG、BMP、TIFF、WebP
- JPEG 和 WebP 可设置压缩质量

界面允许每个方向最多 `8192 px`，并限制总画布不超过 4000 万像素，以避免超大预览和编码耗尽内存；预览会自动缩小显示，实际保存仍使用所设的完整像素尺寸。

支持的图卡包括：

- `0`：白色外框
- `1–8`：RGB 八步二值相移斜条纹
- 九点图
- `27 × 7` 畸变点阵
- 上下校正十字
- 白色矩形
- 全黑图

> 检测和标定图建议优先导出 PNG、BMP 或 TIFF。JPEG 是有损格式，会改变边缘和精确像素值。

## 打开与运行

使用 Visual Studio 打开：

```text
EolTestPatternGenerator.sln
```

主窗体文件：

```text
src/EolTestPatternGenerator/MainForm.cs
src/EolTestPatternGenerator/MainForm.Designer.cs
```

命令行构建：

```powershell
dotnet restore .\EolTestPatternGenerator.sln --configfile .\NuGet.Config
dotnet build .\EolTestPatternGenerator.sln --configuration Release --property:Platform=x64
```

本机已经生成的程序位于：

```text
src\EolTestPatternGenerator\bin\x64\Release\net8.0-windows\win-x64\EolTestPatternGenerator.exe
```

## 参数含义

矩形类图卡（外框、相移、白色矩形）：

- X/Y：矩形左上角
- 宽度/高度：矩形区域的像素尺寸

点阵类图卡（九点、畸变）：

- X/Y：左上第一个圆点的圆心
- X/Y 跨度：第一个圆心到最后一个圆心的距离
- 圆点半径：整数像素半径
- 行/列：点阵数量

圆点按 `dx² + dy² <= radius²` 生成，不使用抗锯齿。默认半径 4 的圆点恰好是 9×9 边界框、49 个白色像素。

## 批量导出

程序会分别记住矩形、九点、畸变点阵和校正十字的参数；切换图卡类型不会丢失已修改的值。“批量导出全部 14 张”会使用：

- 当前画布宽高
- 各图卡已保存的位置、区域/跨度、圆点半径、行列数和线宽
- 输出格式与质量

也可以使用命令行批量导出：

```powershell
EolTestPatternGenerator.exe --export-defaults 输出目录 [png|jpg|bmp|tiff|webp] [宽度] [高度] [质量]
```

例如：

```powershell
EolTestPatternGenerator.exe --export-defaults D:\Cards png 1920 1080 95
```

## OpenCvSharp 本地依赖

项目使用用户提供的核心包：

```text
D:\历史资料\2026.7\opencvsharp4.4.13.0.20260627.nupkg
```

该包只有托管 `OpenCvSharp.dll`，不包含 Windows 原生运行库。项目同时显式引用同版本：

```text
OpenCvSharp4.runtime.win 4.13.0.20260627
```

本机 NuGet 缓存已经存在该运行库，因此当前项目可以离线构建和运行。`NuGet.Config` 已配置本机的三个离线来源。复制到其他电脑时，需要同时准备：

- `OpenCvSharp4 4.13.0.20260627`
- `OpenCvSharp4.runtime.win 4.13.0.20260627`
- `System.Memory 4.6.3`
- .NET 8 Desktop Runtime x64

## 样图验证

内置验证命令：

```powershell
EolTestPatternGenerator.exe --verify "C:\Users\admin\Desktop\90K图片\EOL图卡png"
```

当前验证结果：

- `0.png`、`1–8.png`、`白.png`、`黑.png`：逐像素差异为 0
- 九点图、`畸变.png`、`上下校正图.png`：对参考图做灰度阈值后，几何差异为 0

后三张参考图带有轻微的 `0–3` / `248–255` 灰度伪影，程序输出的是更适合检测的纯 `0/255` 图像。

## GitHub 参考

实现没有直接复制第三方源码，结构和 API 用法主要参考：

- [fryderykhuang/TestPatternGenerator](https://github.com/fryderykhuang/TestPatternGenerator)（MIT）：WinForms Designer、参数界面和图案分类
- [shimat/opencvsharp](https://github.com/shimat/opencvsharp)（Apache-2.0）：`Mat`、图形绘制和图像编码
- [shimat/opencvsharp_samples](https://github.com/shimat/opencvsharp_samples)（Apache-2.0）：WinForms 预览和图像生命周期
- [elerac/structuredlight](https://github.com/elerac/structuredlight)（MIT）：结构光图案与批量导出思路
- [nzhagen/fpp_tools](https://github.com/nzhagen/fpp_tools)（MIT）：N 步相移图案接口
- [opencv/opencv_contrib](https://github.com/opencv/opencv_contrib)（Apache-2.0）：Structured Light 模块

项目中的 RGB 相移条纹按用户样图反推的离散公式实现，并非直接使用上述项目的正弦相移公式。
