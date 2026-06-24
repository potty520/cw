-- 金蝶风格财务软件数据库初始化脚本
-- 数据库: MySQL

CREATE DATABASE IF NOT EXISTS FinanceDB DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE FinanceDB;

-- 1. 会计科目表
CREATE TABLE IF NOT EXISTS AccountSubjects (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    SubjectCode VARCHAR(20) NOT NULL UNIQUE COMMENT '科目编码',
    SubjectName VARCHAR(100) NOT NULL COMMENT '科目名称',
    SubjectType ENUM('资产', '负债', '权益', '成本', '损益') NOT NULL COMMENT '科目类型',
    ParentId INT NULL COMMENT '上级科目ID',
    IsDetail BIT DEFAULT 1 COMMENT '是否明细科目',
    BalanceDirection ENUM('借', '贷') DEFAULT '借' COMMENT '余额方向',
    IsEnabled BIT DEFAULT 1 COMMENT '是否启用',
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ParentId) REFERENCES AccountSubjects(Id)
);

-- 2. 凭证字表
CREATE TABLE IF NOT EXISTS VoucherWords (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    WordName VARCHAR(20) NOT NULL COMMENT '凭证字名称',
    PrintTitle VARCHAR(50) NULL COMMENT '打印标题',
    IsDefault BIT DEFAULT 0 COMMENT '是否默认',
    SortOrder INT DEFAULT 0,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 3. 凭证表
CREATE TABLE IF NOT EXISTS Vouchers (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    VoucherNumber VARCHAR(50) NOT NULL UNIQUE COMMENT '凭证号',
    VoucherWordId INT NOT NULL COMMENT '凭证字ID',
    VoucherDate DATE NOT NULL COMMENT '凭证日期',
    AccountingPeriod INT NOT NULL COMMENT '会计期间',
    AttachmentCount INT DEFAULT 0 COMMENT '附件数',
    AuditorId INT NULL COMMENT '审核人ID',
    PosterId INT NULL COMMENT '过账人ID',
    VoucherStatus ENUM('新建', '已审核', '已过账', '已结账') DEFAULT '新建',
    Memo VARCHAR(500) NULL COMMENT '备注',
    CreatorId INT NOT NULL COMMENT '制单人ID',
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (VoucherWordId) REFERENCES VoucherWords(Id),
    FOREIGN KEY (AccountingPeriod) REFERENCES AccountingPeriods(Id)
);

-- 4. 凭证分录表
CREATE TABLE IF NOT EXISTS VoucherEntries (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    VoucherId INT NOT NULL COMMENT '凭证ID',
    AccountSubjectId INT NOT NULL COMMENT '科目ID',
    EntryDate DATE NOT NULL COMMENT '分录日期',
    Summary VARCHAR(200) NULL COMMENT '摘要',
    DebitAmount DECIMAL(18, 2) DEFAULT 0 COMMENT '借方金额',
    CreditAmount DECIMAL(18, 2) DEFAULT 0 COMMENT '贷方金额',
    Quantity DECIMAL(18, 4) NULL COMMENT '数量',
    UnitPrice DECIMAL(18, 4) NULL COMMENT '单价',
    ForeignCurrencyAmount DECIMAL(18, 2) NULL COMMENT '外币金额',
    CurrencyCode VARCHAR(10) NULL COMMENT '币种',
    SettlementDate DATE NULL COMMENT '结算日期',
    BillNumber VARCHAR(50) NULL COMMENT '票据号',
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id) ON DELETE CASCADE,
    FOREIGN KEY (AccountSubjectId) REFERENCES AccountSubjects(Id)
);

-- 5. 会计期间表
CREATE TABLE IF NOT EXISTS AccountingPeriods (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    PeriodYear INT NOT NULL COMMENT '会计年度',
    PeriodMonth INT NOT NULL COMMENT '会计月份',
    StartDate DATE NOT NULL COMMENT '开始日期',
    EndDate DATE NOT NULL COMMENT '结束日期',
    IsClosed BIT DEFAULT 0 COMMENT '是否结账',
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uk_year_month (PeriodYear, PeriodMonth)
);

-- 6. 客户档案表
CREATE TABLE IF NOT EXISTS Customers (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    CustomerCode VARCHAR(50) NOT NULL UNIQUE COMMENT '客户编码',
    CustomerName VARCHAR(100) NOT NULL COMMENT '客户名称',
    ShortName VARCHAR(50) NULL COMMENT '简称',
    ContactPerson VARCHAR(50) NULL COMMENT '联系人',
    Phone VARCHAR(30) NULL COMMENT '电话',
    Mobile VARCHAR(30) NULL COMMENT '手机',
    Email VARCHAR(100) NULL COMMENT '邮箱',
    Address VARCHAR(200) NULL COMMENT '地址',
    TaxNumber VARCHAR(50) NULL COMMENT '税务登记号',
    BankName VARCHAR(100) NULL COMMENT '开户银行',
    BankAccount VARCHAR(50) NULL COMMENT '银行账号',
    InitialAmount DECIMAL(18, 2) DEFAULT 0 COMMENT '期初应收余额',
    CreditLimit DECIMAL(18, 2) DEFAULT 0 COMMENT '信用额度',
    IsEnabled BIT DEFAULT 1,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 7. 供应商档案表
CREATE TABLE IF NOT EXISTS Suppliers (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    SupplierCode VARCHAR(50) NOT NULL UNIQUE COMMENT '供应商编码',
    SupplierName VARCHAR(100) NOT NULL COMMENT '供应商名称',
    ShortName VARCHAR(50) NULL COMMENT '简称',
    ContactPerson VARCHAR(50) NULL COMMENT '联系人',
    Phone VARCHAR(30) NULL COMMENT '电话',
    Mobile VARCHAR(30) NULL COMMENT '手机',
    Email VARCHAR(100) NULL COMMENT '邮箱',
    Address VARCHAR(200) NULL COMMENT '地址',
    TaxNumber VARCHAR(50) NULL COMMENT '税务登记号',
    BankName VARCHAR(100) NULL COMMENT '开户银行',
    BankAccount VARCHAR(50) NULL COMMENT '银行账号',
    InitialAmount DECIMAL(18, 2) DEFAULT 0 COMMENT '期初应付余额',
    IsEnabled BIT DEFAULT 1,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 8. 固定资产类别表
CREATE TABLE IF NOT EXISTS FixedAssetCategories (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    CategoryCode VARCHAR(20) NOT NULL UNIQUE COMMENT '类别编码',
    CategoryName VARCHAR(100) NOT NULL COMMENT '类别名称',
    DepreciationMethod ENUM('平均年限法', '双倍余额递减法', '年数总和法', '工作量法') DEFAULT '平均年限法',
    DefaultUsefulLife INT DEFAULT 10 COMMENT '默认使用年限',
    DefaultNetSalvageRate DECIMAL(5, 2) DEFAULT 0.05 COMMENT '默认净残值率',
    DepreciationAccountId INT NULL COMMENT '折旧科目ID',
    AccumulatedDepreciationAccountId INT NULL COMMENT '累计折旧科目ID',
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (DepreciationAccountId) REFERENCES AccountSubjects(Id),
    FOREIGN KEY (AccumulatedDepreciationAccountId) REFERENCES AccountSubjects(Id)
);

-- 9. 固定资产卡片表
CREATE TABLE IF NOT EXISTS FixedAssets (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    AssetCode VARCHAR(50) NOT NULL UNIQUE COMMENT '资产编号',
    AssetName VARCHAR(100) NOT NULL COMMENT '资产名称',
    CategoryId INT NOT NULL COMMENT '资产类别ID',
    Specification VARCHAR(100) NULL COMMENT '规格型号',
    MeasurementUnit VARCHAR(10) NULL COMMENT '计量单位',
    Quantity INT DEFAULT 1 COMMENT '数量',
    OriginalValue DECIMAL(18, 2) NOT NULL COMMENT '原值',
    NetSalvageValue DECIMAL(18, 2) DEFAULT 0 COMMENT '净残值',
    UsefulLife INT NOT NULL COMMENT '使用年限',
    DepreciationMethod ENUM('平均年限法', '双倍余额递减法', '年数总和法', '工作量法') NOT NULL COMMENT '折旧方法',
    PurchaseDate DATE NOT NULL COMMENT '购置日期',
    StartUseDate DATE NOT NULL COMMENT '开始使用日期',
    AccumulatedDepreciation DECIMAL(18, 2) DEFAULT 0 COMMENT '累计折旧',
    NetValue DECIMAL(18, 2) NOT NULL COMMENT '净值',
    DepreciationAccountId INT NOT NULL COMMENT '折旧计入科目ID',
    FixedAssetAccountId INT NOT NULL COMMENT '固定资产科目ID',
    AccumulatedDepreciationAccountId INT NOT NULL COMMENT '累计折旧科目ID',
    DepreciationStatus ENUM('正常使用', '已提足折旧', '已处置') DEFAULT '正常使用',
    Memo VARCHAR(500) NULL,
    IsEnabled BIT DEFAULT 1,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoryId) REFERENCES FixedAssetCategories(Id),
    FOREIGN KEY (DepreciationAccountId) REFERENCES AccountSubjects(Id),
    FOREIGN KEY (FixedAssetAccountId) REFERENCES AccountSubjects(Id),
    FOREIGN KEY (AccumulatedDepreciationAccountId) REFERENCES AccountSubjects(Id)
);

-- 10. 固定资产折旧记录表
CREATE TABLE IF NOT EXISTS AssetDepreciationRecords (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    AssetId INT NOT NULL COMMENT '资产ID',
    DepreciationDate DATE NOT NULL COMMENT '折旧日期',
    DepreciationAmount DECIMAL(18, 2) NOT NULL COMMENT '折旧金额',
    AccumulatedDepreciation DECIMAL(18, 2) NOT NULL COMMENT '累计折旧',
    NetValue DECIMAL(18, 2) NOT NULL COMMENT '净值',
    VoucherId INT NULL COMMENT '对应凭证ID',
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (AssetId) REFERENCES FixedAssets(Id),
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id)
);

-- 11. 部门表
CREATE TABLE IF NOT EXISTS Departments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    DepartmentCode VARCHAR(20) NOT NULL UNIQUE COMMENT '部门编码',
    DepartmentName VARCHAR(100) NOT NULL COMMENT '部门名称',
    ParentId INT NULL COMMENT '上级部门ID',
    ManagerId INT NULL COMMENT '负责人ID',
    IsEnabled BIT DEFAULT 1,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ParentId) REFERENCES Departments(Id)
);

-- 12. 员工表
CREATE TABLE IF NOT EXISTS Employees (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    EmployeeCode VARCHAR(50) NOT NULL UNIQUE COMMENT '员工编码',
    EmployeeName VARCHAR(100) NOT NULL COMMENT '员工姓名',
    DepartmentId INT NULL COMMENT '所属部门ID',
    Position VARCHAR(50) NULL COMMENT '职位',
    IdCard VARCHAR(18) NULL COMMENT '身份证号',
    Phone VARCHAR(30) NULL COMMENT '电话',
    Email VARCHAR(100) NULL COMMENT '邮箱',
    BankName VARCHAR(100) NULL COMMENT '开户银行',
    BankAccount VARCHAR(50) NULL COMMENT '银行账号',
    EntryDate DATE NULL COMMENT '入职日期',
    LeaveDate DATE NULL COMMENT '离职日期',
    IsEnabled BIT DEFAULT 1,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);

-- 13. 工资项目表
CREATE TABLE IF NOT EXISTS SalaryItems (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    ItemCode VARCHAR(20) NOT NULL UNIQUE COMMENT '项目编码',
    ItemName VARCHAR(50) NOT NULL COMMENT '项目名称',
    ItemType ENUM('应发', '代扣', '实发') NOT NULL COMMENT '项目类型',
    IsTaxable BIT DEFAULT 1 COMMENT '是否计税',
    IsDefault BIT DEFAULT 0 COMMENT '是否默认项目',
    CalculationFormula VARCHAR(200) NULL COMMENT '计算公式',
    SortOrder INT DEFAULT 0,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 14. 工资发放表
CREATE TABLE IF NOT EXISTS SalaryPayments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    PaymentMonth VARCHAR(7) NOT NULL COMMENT '发放月份',
    DepartmentId INT NULL COMMENT '部门ID',
    EmployeeId INT NOT NULL COMMENT '员工ID',
    GrossSalary DECIMAL(18, 2) DEFAULT 0 COMMENT '应发工资',
    TotalDeductions DECIMAL(18, 2) DEFAULT 0 COMMENT '扣款合计',
    NetSalary DECIMAL(18, 2) DEFAULT 0 COMMENT '实发工资',
    TaxAmount DECIMAL(18, 2) DEFAULT 0 COMMENT '扣税金额',
    VoucherId INT NULL COMMENT '对应凭证ID',
    PaymentStatus ENUM('未审核', '已审核', '已发放') DEFAULT '未审核',
    Memo VARCHAR(500) NULL,
    CreatorId INT NOT NULL,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id)
);

-- 15. 工资项目明细表
CREATE TABLE IF NOT EXISTS SalaryPaymentDetails (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    SalaryPaymentId INT NOT NULL COMMENT '工资发放ID',
    SalaryItemId INT NOT NULL COMMENT '工资项目ID',
    Amount DECIMAL(18, 2) NOT NULL COMMENT '金额',
    FOREIGN KEY (SalaryPaymentId) REFERENCES SalaryPayments(Id) ON DELETE CASCADE,
    FOREIGN KEY (SalaryItemId) REFERENCES SalaryItems(Id)
);

-- 16. 应收票据表
CREATE TABLE IF NOT EXISTS AccountsReceivableBills (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    BillNumber VARCHAR(50) NOT NULL UNIQUE COMMENT '票据号',
    CustomerId INT NOT NULL COMMENT '客户ID',
    BillType ENUM('银行承兑汇票', '商业承兑汇票') NOT NULL COMMENT '票据类型',
    Drawer VARCHAR(100) NOT NULL COMMENT '出票人',
    Payee VARCHAR(100) NOT NULL COMMENT '收款人',
    Drawee VARCHAR(100) NOT NULL COMMENT '付款人',
    BillAmount DECIMAL(18, 2) NOT NULL COMMENT '票据金额',
    IssueDate DATE NOT NULL COMMENT '出票日期',
    DueDate DATE NOT NULL COMMENT '到期日期',
    AcceptanceBank VARCHAR(100) NULL COMMENT '承兑银行',
    BillStatus ENUM('应收', '已收', '已背书', '已贴现', '已到期', '已作废') DEFAULT '应收',
    VoucherId INT NULL COMMENT '对应凭证ID',
    Memo VARCHAR(500) NULL,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id)
);

-- 17. 应付票据表
CREATE TABLE IF NOT EXISTS AccountsPayableBills (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    BillNumber VARCHAR(50) NOT NULL UNIQUE COMMENT '票据号',
    SupplierId INT NOT NULL COMMENT '供应商ID',
    BillType ENUM('银行承兑汇票', '商业承兑汇票') NOT NULL COMMENT '票据类型',
    Drawer VARCHAR(100) NOT NULL COMMENT '出票人',
    Payee VARCHAR(100) NOT NULL COMMENT '收款人',
    Drawee VARCHAR(100) NOT NULL COMMENT '付款人',
    BillAmount DECIMAL(18, 2) NOT NULL COMMENT '票据金额',
    IssueDate DATE NOT NULL COMMENT '出票日期',
    DueDate DATE NOT NULL COMMENT '到期日期',
    AcceptanceBank VARCHAR(100) NULL COMMENT '承兑银行',
    BillStatus ENUM('应付', '已付', '已到期', '已作废') DEFAULT '应付',
    VoucherId INT NULL COMMENT '对应凭证ID',
    Memo VARCHAR(500) NULL,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id)
);

-- 18. 发票表
CREATE TABLE IF NOT EXISTS Invoices (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    InvoiceNumber VARCHAR(50) NOT NULL UNIQUE COMMENT '发票号',
    InvoiceType ENUM('增值税专用发票', '增值税普通发票', '收据', '其他') NOT NULL COMMENT '发票类型',
    CustomerId INT NULL COMMENT '客户ID（销项）',
    SupplierId INT NULL COMMENT '供应商ID（进项）',
    InvoiceDate DATE NOT NULL COMMENT '发票日期',
    InvoiceAmount DECIMAL(18, 2) NOT NULL COMMENT '发票金额',
    TaxAmount DECIMAL(18, 2) DEFAULT 0 COMMENT '税额',
    TaxRate DECIMAL(5, 2) DEFAULT 0 COMMENT '税率',
    VoucherId INT NULL COMMENT '对应凭证ID',
    InvoiceStatus ENUM('未核销', '部分核销', '已核销', '已作废') DEFAULT '未核销',
    Memo VARCHAR(500) NULL,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    FOREIGN KEY (VoucherId) REFERENCES Vouchers(Id)
);

-- 19. 系统用户表
CREATE TABLE IF NOT EXISTS Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UserCode VARCHAR(50) NOT NULL UNIQUE COMMENT '用户编码',
    UserName VARCHAR(100) NOT NULL COMMENT '用户姓名',
    Password VARCHAR(100) NOT NULL COMMENT '密码',
    UserRole ENUM('系统管理员', '账套管理员', '财务主管', '会计', '出纳', '审计') NOT NULL COMMENT '用户角色',
    DepartmentId INT NULL COMMENT '所属部门',
    IsEnabled BIT DEFAULT 1,
    LastLoginTime DATETIME NULL,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id)
);

-- 20. 操作日志表
CREATE TABLE IF NOT EXISTS OperationLogs (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL COMMENT '操作用户ID',
    OperationType VARCHAR(50) NOT NULL COMMENT '操作类型',
    ModuleName VARCHAR(50) NOT NULL COMMENT '模块名称',
    OperationDesc VARCHAR(500) NOT NULL COMMENT '操作描述',
    IpAddress VARCHAR(50) NULL COMMENT 'IP地址',
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- 21. 账套表
CREATE TABLE IF NOT EXISTS AccountSets (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    SetCode VARCHAR(50) NOT NULL UNIQUE COMMENT '账套编码',
    SetName VARCHAR(100) NOT NULL COMMENT '账套名称',
    CompanyName VARCHAR(200) NOT NULL COMMENT '公司名称',
    TaxNumber VARCHAR(50) NULL COMMENT '税务登记号',
    LegalPerson VARCHAR(100) NULL COMMENT '法人代表',
    Chief Accountant VARCHAR(100) NULL COMMENT '主管会计',
    Accountant VARCHAR(100) NULL COMMENT '会计',
    Cashier VARCHAR(100) NULL COMMENT '出纳',
    StartDate DATE NOT NULL COMMENT '建账日期',
    CurrencyCode VARCHAR(10) DEFAULT 'RMB' COMMENT '本位币代码',
    IsEnabled BIT DEFAULT 1,
    CreateTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdateTime DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 初始化默认数据
INSERT INTO AccountSubjects (SubjectCode, SubjectName, SubjectType, IsDetail, BalanceDirection) VALUES
('1001', '库存现金', '资产', 1, '借'),
('1002', '银行存款', '资产', 1, '借'),
('1122', '应收账款', '资产', 1, '借'),
('2202', '应付账款', '负债', 1, '贷'),
('1601', '固定资产', '资产', 1, '借'),
('1602', '累计折旧', '资产', 1, '贷'),
('5001', '生产成本', '成本', 1, '借'),
('6001', '主营业务收入', '损益', 1, '贷'),
('6401', '主营业务成本', '损益', 1, '借');

INSERT INTO VoucherWords (WordName, PrintTitle, IsDefault, SortOrder) VALUES
('记', '记账凭证', 1, 1),
('收', '收款凭证', 0, 2),
('付', '付款凭证', 0, 3),
('转', '转账凭证', 0, 4);

INSERT INTO Users (UserCode, UserName, Password, UserRole) VALUES
('admin', '系统管理员', '123456', '系统管理员');
