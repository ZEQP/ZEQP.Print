#��װ
�Թ���Ա��ʽ����CMD
����ZEQP.Print.Service.exe����Ŀ¼
Ȼ������
ZEQP.Print.Service.exe install

#����
�Թ���Ա��ʽ����CMD
����ZEQP.Print.Service.exe����Ŀ¼
Ȼ������
ZEQP.Print.Service.exe start

Ҳ���Ե����������壬�ҵ�ZEQPPrintService����
�Ҽ�ѡ��������

#ֹͣ
�Թ���Ա��ʽ����CMD
����ZEQP.Print.Service.exe����Ŀ¼
Ȼ������
ZEQP.Print.Service.exe stop

Ҳ���Ե����������壬�ҵ�ZEQPPrintService����
�Ҽ�ѡ��ֹͣ��

#ж��
�Թ���Ա��ʽ����CMD
����ZEQP.Print.Service.exe����Ŀ¼
Ȼ������
ZEQP.Print.Service.exe uninstall


�����������ʱ��HttpListener����
�Թ���Ա��ʽ����CMD��������������
netsh http add urlacl url=http://+:8101/api/print/ sddl="D:(A;;GX;;;WD)" user=\Everyone
ע�⣺+Ϊ���õ�Host  8101Ϊ���õ�Port