# Codex 协作说明

## 项目入口

- 解决方案：`EolTestPatternGenerator.sln`
- 主项目：`src/EolTestPatternGenerator/EolTestPatternGenerator.csproj`
- 构建：`dotnet build EolTestPatternGenerator.sln -c Release`

## 修改约定

- `main` 保持可构建；日常修改默认使用 `codex/<任务名>` 分支。
- 提交前至少运行一次 Release 构建，并检查生成图卡的尺寸、颜色与文件名。
- 不提交 `bin`、`obj`、`.vs`、`GeneratedCards`、用户配置、密钥或发布产物。
- `NuGet.Config` 当前引用本机缓存；更换电脑时先确认 OpenCvSharp 包源可用。
- 修改图卡算法或预设时，同步更新 README 中的参数和验证方式。

