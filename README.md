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

![Wordģ��ʾ��](./ZEQP.Print.Service/TestImage/Template.png)


# ��ӡ
## GET�����ӡ
![Wordģ��ʾ��](./ZEQP.Print.Service/TestImage/API_GET.png)

## POST�����ӡ
![Wordģ��ʾ��](./ZEQP.Print.Service/TestImage/API.png)

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