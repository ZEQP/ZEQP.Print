# ��װ
�Թ���Ա��ʽ����CMD

����ZEQP.Print.Service.exe����Ŀ¼

Ȼ������

```CMD
ZEQP.Print.Service.exe install
```

# ����
�Թ���Ա��ʽ����CMD

����ZEQP.Print.Service.exe����Ŀ¼

Ȼ������

```CMD
ZEQP.Print.Service.exe start
```

Ҳ���Ե����������壬�ҵ�ZEQPPrintService����

�Ҽ�ѡ��������

# ģ��༭
ʹ��Word�༭Ҫ��ӡ��ģ�壬ʹ�á�����Ϊռλ��

![Wordģ��ʾ��](./ZEQP.Print.Service/TestImage/Template.png?raw=true)

## ͼƬ
������ʽ"Image:ImageKey"
## ���
���ʼ��������ʽ"TableStart:TableKey"

��������������ʽ"TableEnd:TableKey"


# ��ӡ
## GET�����ӡ
![GET�����ӡ](./ZEQP.Print.Service/TestImage/API_GET.png?raw=true)

## POST�����ӡ
![POST�����ӡ](./ZEQP.Print.Service/TestImage/API.png?raw=true)

## ��ӡЧ��
![��ӡЧ��](./ZEQP.Print.Service/TestImage/View.png?raw=true)

# API˵��
URL������ֻ����URL�����Query������

PrintName����ӡ������

Copies����ӡ����

Template��ģ�����ƣ���TemplateĿ¼�����word�ļ����ƣ�

Action��ִ�ж���(Print:��ӡ��File:ֻ������ļ����أ�PrintAndFile:��ӡ�������ļ�����)

ģ�������ݲ���

������POST��JSON�������аѶ�Ӧ���������ü�ֵ�Եķ�ʽ����

Ҳ������GET�����URL��Query�����д���

## ͼƬ
������Ϊ��Image:ImageKey����ֵΪJSON�ַ�������ʽΪ��

```JSON
{"Type":"ͼƬ����","Value":"ͼƬ���Ͷ�Ӧֵ","Width":���,"Height":�߶�}
```

ͼƬ���ͣ�Local������ͼƬ����Network������ͼƬ����BarCode�������룩��QRCode����ά�룩

## ���
������Ϊ��Table:TableKey��,ֵΪJSON���飬��ʽ���ա�GET�����ӡ����POST�����ӡ��

# ֹͣ
�Թ���Ա��ʽ����CMD

����ZEQP.Print.Service.exe����Ŀ¼

Ȼ������

```CMD
ZEQP.Print.Service.exe stop
```

Ҳ���Ե����������壬�ҵ�ZEQPPrintService����

�Ҽ�ѡ��ֹͣ��

# ж��
�Թ���Ա��ʽ����CMD

����ZEQP.Print.Service.exe����Ŀ¼

Ȼ������

```CMD
ZEQP.Print.Service.exe uninstall
```


�����������ʱ��HttpListener����

�Թ���Ա��ʽ����CMD��������������

```CMD
netsh http add urlacl url=http://+:8101/api/print/ sddl="D:(A;;GX;;;WD)" user=\Everyone
```

ע�⣺+Ϊ���õ�Host  8101Ϊ���õ�Port