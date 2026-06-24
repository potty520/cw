# 财智星财务软件

一款功能完整的财务管理软件，包含总账、应收应付、固定资产、工资核算、报表分析等核心模块。

## 功能模块

- **总账管理**：凭证录入、审核、过账、账簿查询
- **应收应付**：客户/供应商管理、应收/应付账款跟踪
- **固定资产**：资产卡片、折旧计算、资产盘点
- **工资核算**：员工档案、工资项目、个税计算
- **报表分析**：资产负债表、利润表、现金流量表
- **凭证打印**：支持套打和自定义纸张设置

## 技术栈

- .NET 8.0 / WPF
- MySQL 数据库
- MySqlConnector 数据访问组件
- MVVM 架构模式

## 系统要求

- Windows 10/11
- .NET 8.0 SDK 或 Runtime
- MySQL 5.7+ 数据库

## 数据库配置

默认连接信息（在 `App.xaml.cs` 中配置）：
- 主机：`103.236.96.82`
- 端口：`13306`
- 数据库：`FinanceDB`
- 用户：`remote`
- 密码：`ToCAt69Aidc16I1KvkQo2YWMlUl01U0o`

修改连接字符串后重新编译即可。

## 项目结构

```
FinanceApp/
├── database/                    # 数据库脚本
│   └── init.sql                 # 初始化脚本
├── src/
│   ├── FinanceApp/              # 主程序（WPF 界面）
│   ├── FinanceApp.Models/       # 数据模型
│   ├── FinanceApp.Data/         # 数据访问层
│   ├── FinanceApp.Services/     # 业务逻辑层
│   └── FinanceApp.ViewModels/   # 视图模型层
└── README.md
```

## 默认登录账号

- 用户名：`admin`
- 密码：`123456`

## 快速开始

1. 克隆项目
   ```bash
   git clone https://github.com/potty520/cw.git
   ```

2. 执行数据库脚本
   ```bash
   mysql -h 103.236.96.82 -P 13306 -u remote -p < database/init.sql
   ```

3. 编译运行
   ```bash
   cd src/FinanceApp
   dotnet build
   dotnet run
   ```

## 凭证打印功能

支持以下打印特性：
- **套打模式**：基于预印凭证表单的精确打印，可调整偏移
- **自定义纸张**：支持自定义纸张尺寸、方向、边距
- **打印预览**：所见即所得的预览效果
- **多打印机支持**：可选择系统已安装的打印机

## 许可证

MIT License
