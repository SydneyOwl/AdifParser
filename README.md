# AdifParser

.NET 6 高性能 ADIF（Amateur Data Interchange Format）解析库，基于原 ADIFLib 重构优化。

## 安装

```bash
dotnet add package AdifParser
```

## 快速开始

### 解析 ADIF 文件

```csharp
using AdifParser;

// 从文件读取
var adif = new ADIF("sample.adi");
Console.WriteLine($"QSO 数量: {adif.QSOCount}");

foreach (ADIFQSO qso in adif.TheQSOs)
{
    // 按标签名获取 Token
    foreach (Token token in qso)
    {
        Console.WriteLine($"{token.Name}: {token.Data}");
    }
}
```

### 解析 ADIF 字符串

```csharp
var adif = new ADIF();
adif.ReadFromString("MY ADIF HEADER<eoh>\n<CALL:4>NV9U<BAND:3>80M<eor>");
```

### 解析单个 QSO 或 Header

```csharp
var qso = new ADIFQSO("<CALL:4>NV9U<BAND:3>80M");
var header = new ADIFHeader("<PROGRAMID:9>AdifParser<eoh>");
```

### 构建 ADIF 并保存

```csharp
var adif = new ADIF();
adif.AddHeader("<PROGRAMID:9>AdifParser<eoh>");

var qso = new ADIFQSO();
qso.AddToken("CALL", "NV9U");
qso.AddToken("BAND", "80M");
adif.AddQSO(qso);

adif.SaveToFile("output.adi");
```

### 程序化构建 Header

```csharp
var header = new ADIFHeader();
header.Add(new TokenNameData("PROGRAMID", "AdifParser"));
header.Add(new TokenNameData("PROGRAMVERSION", "2.0"));
```

## ADIF 格式说明

ADIF (Amateur Data Interchange Format) 是业余无线电日志交换标准格式。每条记录由 `<标签名:长度>数据` 形式的 Token 组成。

- Header 以 `<EOH>` 结束
- 每条 QSO 记录以 `<EOR>` 结束
- 示例：`<CALL:4>NV9U<BAND:3>80M<eor>`

## 性能优化

相比原 ADIFLib，本库做了以下优化：

- 使用 `ReadOnlySpan<char>` 进行零分配解析
- 延迟 `Length` 计算，仅在序列化时更新
- `StringBuilder` 预分配容量
- 避免不必要的 `ToUpper()` 和 `Split()` 分配

## 许可

MIT