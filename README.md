# 安装
以管理员方式运行CMD

进入ZEQP.Print.Service.exe所在目录

然后运行

```CMD
ZEQP.Print.Service.exe install
```

# 启动
以管理员方式运行CMD

进入ZEQP.Print.Service.exe所在目录

然后运行

```CMD
ZEQP.Print.Service.exe start
```

也可以到服务管理面板，找到ZEQPPrintService服务

右键选择“启动”

# 模板编辑
使用Word编辑要打印的模板，使用“域”作为占位符

![Word模板示例](./ZEQP.Print.Service/TestImage/Template.png?raw=true)

## 图片
域名格式"Image:ImageKey"
## 表格
表格开始，域名格式"TableStart:TableKey"

表格结束，域名格式"TableEnd:TableKey"


# 打印
## GET请求打印
![GET请求打印](./ZEQP.Print.Service/TestImage/API_GET.png?raw=true)

## POST请求打印
![POST请求打印](./ZEQP.Print.Service/TestImage/API.png?raw=true)

## 打印效果
![打印效果](./ZEQP.Print.Service/TestImage/View.png?raw=true)

# API说明
URL参数（只能在URL后面的Query参数）

PrintName：打印机名称

Copies：打印份数

Template：模板名称（在Template目录下面的word文件名称）

Action：执行动作(Print:打印，File:只输出成文件下载，PrintAndFile:打印并生成文件下载)

模板域数据参数

可以在POST的JSON的数据中把对应的域数据用键值对的方式传入

也可以在GET请求的URL的Query参数中传入

## 图片
参数名为“Image:ImageKey”，值为JSON字符串，格式为：

```JSON
{"Type":"图片类型","Value":"图片类型对应值","Width":宽度,"Height":高度}
```

图片类型：Local（本地图片），Network（网络图片），BarCode（条形码），QRCode（二维码）

## 表格
参数名为“Table:TableKey”,值为JSON数组，格式参照“GET请求打印”“POST请求打印”

# 停止
以管理员方式运行CMD

进入ZEQP.Print.Service.exe所在目录

然后运行

```CMD
ZEQP.Print.Service.exe stop
```

也可以到服务管理面板，找到ZEQPPrintService服务

右键选择“停止”

# 卸载
以管理员方式运行CMD

进入ZEQP.Print.Service.exe所在目录

然后运行

```CMD
ZEQP.Print.Service.exe uninstall
```


如果在启动的时候报HttpListener错误

以管理员方式运行CMD，输入以下命令

```CMD
netsh http add urlacl url=http://+:8101/api/print/ sddl="D:(A;;GX;;;WD)" user=\Everyone
```

注意：+为配置的Host  8101为配置的Port